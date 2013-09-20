// 
// Radegast Metaverse Client
// Copyright (c) 2009-2013, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
using System;
using System.Collections.Generic;
using System.Threading;
using OpenMetaverse;

namespace Radegast
{
    public abstract class ReloadableBase : IDisposable
    {
        protected RadegastInstance Instance;
        protected GridClient Client { get { return Instance.Client; } }

        public ReloadableBase(RadegastInstance instance)
        {
            this.Instance = instance;
            Instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
        }

        public virtual void Dispose()
        {
            Instance.ClientChanged -= new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(e.Client);
        }

        protected virtual void RegisterClientEvents(GridClient client)
        {
        }

        protected virtual void UnregisterClientEvents(GridClient client)
        {
        }

    }

    public class InventoryLoader : ReloadableBase
    {
        Thread LoadThread = null;
        Queue<InventoryFolder> FoldersToFetch = new Queue<InventoryFolder>();
        Queue<InventoryFolder> LibraryToFetch = new Queue<InventoryFolder>();
        OpenMetaverse.Inventory Inventory { get { return Client.Inventory.Store; } }
        public event EventHandler<EventArgs> Finished;
        int MaxRetries = 3;
        int MaxConcurrent = 4;

        public InventoryLoader(RadegastInstance instance)
            : base(instance)
        {
            RegisterClientEvents(Client);
        }

        protected override void RegisterClientEvents(GridClient client)
        {
            Client.Network.LoginProgress += new EventHandler<LoginProgressEventArgs>(Network_LoginProgress);
            base.RegisterClientEvents(client);
        }

        protected override void UnregisterClientEvents(GridClient client)
        {
            Client.Network.LoginProgress -= new EventHandler<LoginProgressEventArgs>(Network_LoginProgress);
            base.UnregisterClientEvents(client);
        }

        public override void Dispose()
        {
            AbortFetch();
            UnregisterClientEvents(Client);
            base.Dispose();
        }

        void Network_LoginProgress(object sender, LoginProgressEventArgs e)
        {
            if (e.Status == LoginStatus.Success)
            {
                StartFetch();
            }
        }

        void AbortFetch()
        {
            if (LoadThread != null)
            {
                LoadThread.Abort();
                LoadThread = null;
            }
        }

        void StartFetch()
        {
            AbortFetch();
            LoadThread = new Thread(new ThreadStart(() => LoadInventory()));
            LoadThread.IsBackground = true;
            LoadThread.Name = "Inventory Loading";
            LoadThread.Start();
        }

        void AddFoldersToUpdate(ref Queue<InventoryFolder> queue, InventoryNode node)
        {
            bool hasItems = false;
            foreach (var child in node.Nodes.Values)
            {
                if (child.Data is InventoryItem)
                {
                    hasItems = true;
                }
                else if (child.Data is InventoryFolder)
                {
                    AddFoldersToUpdate(ref queue, child);
                }

            }

            if (node.NeedsUpdate || !hasItems)
            {
                lock (queue) queue.Enqueue((InventoryFolder)node.Data);
            }
        }

        void FetchFoldersUDP(ref Queue<InventoryFolder> queue)
        {
            Parallel.ForEach<InventoryFolder>(Math.Min(queue.Count, MaxConcurrent), queue, (folder =>
            {
                AutoResetEvent gotFolderEvent = new AutoResetEvent(false);
                bool success = false;

                EventHandler<FolderUpdatedEventArgs> callback = (sender, e) =>
                {
                    if (folder.UUID == e.FolderID && e.Success)
                    {
                        success = true;
                        gotFolderEvent.Set();
                    }
                };

                Client.Inventory.FolderUpdated += callback;
                int retryNr = 0;

                while (!success && retryNr < MaxRetries)
                {
                    Client.Inventory.RequestFolderContents(folder.UUID, folder.OwnerID, true, true, InventorySortOrder.SystemFoldersToTop | InventorySortOrder.ByDate);
                    gotFolderEvent.WaitOne(30 * 1000, false);
                    retryNr++;
                }

                Client.Inventory.FolderUpdated -= callback;
            }));
        }

        void PerformFetch(List<InventoryFolder> batch, Uri cap)
        {
            AutoResetEvent gotFolderEvent = new AutoResetEvent(false);
            bool success = false;

            EventHandler<FolderUpdatedEventArgs> callback = (sender, e) =>
            {
                if (batch[0].UUID == e.FolderID && e.Success)
                {
                    success = true;
                    gotFolderEvent.Set();
                }
            };

            Client.Inventory.FolderUpdated += callback;
            int retryNr = 0;

            while (!success && retryNr < MaxRetries)
            {
                Client.Inventory.RequestFolderContentsCap(batch, cap, true, true, (InventorySortOrder.SystemFoldersToTop | InventorySortOrder.ByDate));
                gotFolderEvent.WaitOne(30 * 1000, false);
                MaxRetries++;
            }
        }

        void FetchFoldersCap(ref Queue<InventoryFolder> queue, string cap)
        {
            Uri url = null;
            if (Client.Network.CurrentSim.Caps == null ||
                null == (url = Client.Network.CurrentSim.Caps.CapabilityURI(cap)))
            {
                Logger.Log(cap + " capability not available in the current sim", Helpers.LogLevel.Warning, Client);
                return;
            }

            Queue<List<InventoryFolder>> batches = new Queue<List<InventoryFolder>>();
            int maxPerRequest = 10;
            lock (queue)
            {
                while (queue.Count > 0)
                {
                    List<InventoryFolder> batch = new List<InventoryFolder>();
                    while (batch.Count < maxPerRequest && queue.Count > 0)
                    {
                        batch.Add(queue.Dequeue());
                    }
                    if (batch.Count > 0)
                    {
                        batches.Enqueue(batch);
                    }
                }
            }

            Parallel.ForEach<List<InventoryFolder>>(Math.Min(MaxConcurrent, batches.Count), batches, (batch =>
            {
                PerformFetch(batch, url);
            }));
        }

        void LoadInventory()
        {
            lock (FoldersToFetch) FoldersToFetch.Clear();
            lock (LibraryToFetch) LibraryToFetch.Clear();

            if (!Client.Network.CurrentSim.Caps.IsEventQueueRunning)
            {
                AutoResetEvent EQRunning = new AutoResetEvent(false);
                EventHandler<EventQueueRunningEventArgs> handler = (sender, e) =>
                {
                    EQRunning.Set();
                };
                Client.Network.EventQueueRunning += handler;
                EQRunning.WaitOne(30 * 1000, false);
                Client.Network.EventQueueRunning -= handler;
                if (!Client.Network.CurrentSim.Caps.IsEventQueueRunning)
                {
                    Logger.Log("EventQueue not running, aborting inventory update", Helpers.LogLevel.Warning, Client);
                    return;
                }
            }

            Logger.Log("Reading inventory cache from " + Instance.InventoryCacheFileName, Helpers.LogLevel.Debug, Client);
            Inventory.RestoreFromDisk(Instance.InventoryCacheFileName);

            if (Finished != null)
            {
                Finished(this, EventArgs.Empty);
            }

            AddFoldersToUpdate(ref FoldersToFetch, Inventory.RootNode);
            AddFoldersToUpdate(ref LibraryToFetch, Inventory.LibraryRootNode);

            bool useCaps = Client.Settings.HTTP_INVENTORY && Client.Network.CurrentSim.Caps != null;
            useCaps = false;

            if (useCaps && (Client.Network.CurrentSim.Caps.CapabilityURI("FetchInventoryDescendents2") != null))
            {
                FetchFoldersCap(ref FoldersToFetch, "FetchInventoryDescendents2");
            }
            else
            {
                FetchFoldersUDP(ref FoldersToFetch);
            }

            if (useCaps && (Client.Network.CurrentSim.Caps.CapabilityURI("FetchLibDescendents2") != null))
            {
                FetchFoldersCap(ref LibraryToFetch, "FetchLibDescendents2");
            }
            else
            {
                FetchFoldersUDP(ref LibraryToFetch);
            }

            Logger.Log("Finished updating invenory folders, saving cache...", Helpers.LogLevel.Debug, Client);
            Inventory.SaveToDisk(Instance.InventoryCacheFileName);
            Instance.TabConsole.DisplayNotificationInChat("Inventory update completed.");
        }

        public bool IsRunning()
        {
            return LoadThread != null && LoadThread.IsAlive;
        }
    }
}
