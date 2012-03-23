// 
// Radegast Metaverse Client
// Copyright (c) 2009-2012, Radegast Development Team
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
// $Id$
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public class CurrentOutfitFolder : IDisposable
    {
        #region Fields
        GridClient Client;
        RadegastInstance Instance;
        bool InitiCOF = false;
        bool InvCAP = false;
        bool AppearanceSent = false;
        bool COFReady = false;
        bool InitialUpdateDone = false;
        public List<InventoryItem> ContentLinks = new List<InventoryItem>();
        public Dictionary<UUID, InventoryItem> Content = new Dictionary<UUID, InventoryItem>();
        public InventoryFolder COF;
        #endregion Fields

        #region Construction and disposal
        public CurrentOutfitFolder(RadegastInstance instance)
        {
            this.Instance = instance;
            this.Client = instance.Client;
            Instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
            RegisterClientEvents(Client);
        }

        public void Dispose()
        {
            UnregisterClientEvents(Client);
            Instance.ClientChanged -= new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
        }
        #endregion Construction and disposal

        #region Event handling
        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(Client);
            Client = e.Client;
            RegisterClientEvents(Client);
        }

        void RegisterClientEvents(GridClient client)
        {
            client.Network.EventQueueRunning += new EventHandler<EventQueueRunningEventArgs>(Network_EventQueueRunning);
            client.Inventory.FolderUpdated += new EventHandler<FolderUpdatedEventArgs>(Inventory_FolderUpdated);
            client.Inventory.ItemReceived += new EventHandler<ItemReceivedEventArgs>(Inventory_ItemReceived);
            client.Appearance.AppearanceSet += new EventHandler<AppearanceSetEventArgs>(Appearance_AppearanceSet);
            client.Objects.KillObject += new EventHandler<KillObjectEventArgs>(Objects_KillObject);
        }

        void UnregisterClientEvents(GridClient client)
        {
            client.Network.EventQueueRunning -= new EventHandler<EventQueueRunningEventArgs>(Network_EventQueueRunning);
            client.Inventory.FolderUpdated -= new EventHandler<FolderUpdatedEventArgs>(Inventory_FolderUpdated);
            client.Inventory.ItemReceived -= new EventHandler<ItemReceivedEventArgs>(Inventory_ItemReceived);
            client.Appearance.AppearanceSet -= new EventHandler<AppearanceSetEventArgs>(Appearance_AppearanceSet);
            client.Objects.KillObject -= new EventHandler<KillObjectEventArgs>(Objects_KillObject);
            lock (Content) Content.Clear();
            lock (ContentLinks) ContentLinks.Clear();
            InitiCOF = false;
            InvCAP = false;
            AppearanceSent = false;
            COFReady = false;
            InitialUpdateDone = false;
        }

        void Appearance_AppearanceSet(object sender, AppearanceSetEventArgs e)
        {
            AppearanceSent = true;
            if (COFReady)
            {
                InitialUpdate();
            }
        }

        void Inventory_ItemReceived(object sender, ItemReceivedEventArgs e)
        {
            lock (ContentLinks)
            {
                bool partOfCOF = false;
                foreach (var cofItem in ContentLinks)
                {
                    if (cofItem.AssetUUID == e.Item.UUID)
                    {
                        partOfCOF = true;
                        break;
                    }
                }

                if (partOfCOF)
                {
                    lock (Content)
                    {
                        Content[e.Item.UUID] = e.Item;
                    }
                }
            }

            if (Content.Count == ContentLinks.Count)
            {
                COFReady = true;
                if (AppearanceSent)
                {
                    InitialUpdate();
                }
            }
        }

        object FolderSync = new object();

        void Inventory_FolderUpdated(object sender, FolderUpdatedEventArgs e)
        {
            if (COF == null) return;

            if (e.FolderID == COF.UUID && e.Success)
            {
                lock (FolderSync)
                {
                    lock (Content) Content.Clear();
                    lock (ContentLinks) ContentLinks.Clear();

                    List<InventoryBase> content = Client.Inventory.Store.GetContents(COF);
                    foreach (var baseItem in content)
                    {
                        if (baseItem is InventoryItem)
                        {
                            InventoryItem item = (InventoryItem)baseItem;
                            if (item.AssetType == AssetType.Link)
                            {
                                ContentLinks.Add(item);
                            }
                        }
                    }

                    List<UUID> items = new List<UUID>();
                    List<UUID> owners = new List<UUID>();

                    lock (ContentLinks)
                    {
                        foreach (var link in ContentLinks)
                        {
                            items.Add(link.AssetUUID);
                            owners.Add(Client.Self.AgentID);
                        }
                    }

                    if (items.Count > 0)
                    {
                        Client.Inventory.RequestFetchInventory(items, owners);
                    }
                }
            }
        }

        void Objects_KillObject(object sender, KillObjectEventArgs e)
        {
            if (Client.Network.CurrentSim != e.Simulator) return;

            Primitive prim = null;
            if (Client.Network.CurrentSim.ObjectsPrimitives.TryGetValue(e.ObjectLocalID, out prim))
            {
                UUID invItem = GetAttachmentItem(prim);
                if (invItem != UUID.Zero)
                {
                    RemoveLink(invItem);
                }
            }
        }

        void Network_EventQueueRunning(object sender, EventQueueRunningEventArgs e)
        {
            if (e.Simulator == Client.Network.CurrentSim && !InitiCOF)
            {
                InitiCOF = true;
                InitCOF();
            }
        }
        #endregion Event handling

        #region Private methods
        void RequestDescendants(UUID folderID)
        {
            if (InvCAP)
            {
                Client.Inventory.RequestFolderContentsCap(folderID, Client.Self.AgentID, true, true, InventorySortOrder.ByDate);
            }
            else
            {
                Client.Inventory.RequestFolderContents(folderID, Client.Self.AgentID, true, true, InventorySortOrder.ByDate);
            }
        }

        void InitCOF()
        {
            Uri url = null;

            if (Client.Network.CurrentSim.Caps == null ||
                null == (url = Client.Network.CurrentSim.Caps.CapabilityURI("FetchInventoryDescendents2")))
            {
                InvCAP = false;
            }
            else
            {
                InvCAP = true;
            }

            List<InventoryBase> rootContent = Client.Inventory.Store.GetContents(Client.Inventory.Store.RootFolder.UUID);
            foreach (InventoryBase baseItem in rootContent)
            {
                if (baseItem is InventoryFolder && ((InventoryFolder)baseItem).PreferredType == AssetType.CurrentOutfitFolder)
                {
                    COF = (InventoryFolder)baseItem;
                    break;
                }
            }

            if (COF == null)
            {
                CreateCOF();
            }
            else
            {
                RequestDescendants(COF.UUID);
            }
        }

        void CreateCOF()
        {
            UUID cofID = Client.Inventory.CreateFolder(Client.Inventory.Store.RootFolder.UUID, "Current Look", AssetType.CurrentOutfitFolder);
            if (Client.Inventory.Store.Items.ContainsKey(cofID) && Client.Inventory.Store.Items[cofID].Data is InventoryFolder)
            {
                COF = (InventoryFolder)Client.Inventory.Store.Items[cofID].Data;
                COFReady = true;
                if (AppearanceSent)
                {
                    InitialUpdate();
                }
            }
        }

        void InitialUpdate()
        {
            if (InitialUpdateDone) return;
            InitialUpdateDone = true;
            lock (Content)
            {
                List<Primitive> myAtt = Client.Network.CurrentSim.ObjectsPrimitives.FindAll((Primitive p) => p.ParentID == Client.Self.LocalID);

                foreach (InventoryItem item in Content.Values)
                {
                    if (item is InventoryObject || item is InventoryAttachment)
                    {
                        if (!IsAttached(myAtt, item))
                        {
                            Client.Appearance.Attach(item, AttachmentPoint.Default, false);
                        }
                    }
                }
            }
        }
        #endregion Private methods

        #region Public methods
        /// <summary>
        /// Get inventory ID of a prim
        /// </summary>
        /// <param name="prim">Prim to check</param>
        /// <returns>Inventory ID of the object. UUID.Zero if not found</returns>
        public static UUID GetAttachmentItem(Primitive prim)
        {
            if (prim.NameValues == null) return UUID.Zero;

            for (int i = 0; i < prim.NameValues.Length; i++)
            {
                if (prim.NameValues[i].Name == "AttachItemID")
                {
                    return (UUID)prim.NameValues[i].Value.ToString();
                }
            }
            return UUID.Zero;
        }

        /// <summary>
        /// Is an inventory item currently attached
        /// </summary>
        /// <param name="attachments">List of root prims that are attached to our avatar</param>
        /// <param name="item">Inventory item to check</param>
        /// <returns>True if the inventory item is attached to avatar</returns>
        public static bool IsAttached(List<Primitive> attachments, InventoryItem item)
        {
            foreach (Primitive prim in attachments)
            {
                if (GetAttachmentItem(prim) == item.UUID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attach an inventory item
        /// </summary>
        /// <param name="item">Item to be attached</param>
        /// <param name="point">Attachment point</param>
        /// <param name="replace">Replace existing attachment at that point first?</param>
        public void Attach(InventoryItem item, AttachmentPoint point, bool replace)
        {
            Client.Appearance.Attach(item, point, replace);
            if (COF == null) return;

            bool linkExists = false;
            lock (ContentLinks)
            {
                linkExists = null != ContentLinks.Find(itemLink => itemLink.AssetUUID == item.UUID);
            }
            if (!linkExists)
            {
                Client.Inventory.CreateLink(COF.UUID, item, (success, newItem) =>
                {
                    if (success)
                    {
                        lock (ContentLinks)
                        {
                            ContentLinks.Add(newItem);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Remove a link to specified inventory item
        /// </summary>
        /// <param name="itemID">ID of the target inventory item for which we want link to be removed</param>
        public void RemoveLink(UUID itemID)
        {
            if (COF == null) return;

            lock (ContentLinks)
            {
                InventoryItem attachment = ContentLinks.Find(itemLink => itemLink.AssetUUID == itemID);
                if (attachment != null)
                {
                    Client.Inventory.RemoveItem(attachment.UUID);
                    ContentLinks.Remove(attachment);
                }
            }
        }

        /// <summary>
        /// Remove attachment
        /// </summary>
        /// <param name="item">>Inventory item to be detached</param>
        public void Detach(InventoryItem item)
        {
            Client.Appearance.Detach(item);
            RemoveLink(item.UUID);
        }

        /// <summary>
        /// Resolves inventory links and returns a real inventory item that
        /// the link is pointing to
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public InventoryItem RealInventoryItem(InventoryItem item)
        {
            if (item.IsLink() && Client.Inventory.Store.Contains(item.AssetUUID) && Client.Inventory.Store[item.AssetUUID] is InventoryItem)
            {
                return (InventoryItem)Client.Inventory.Store[item.AssetUUID];
            }

            return item;
        }

        #endregion Public methods
    }
}
