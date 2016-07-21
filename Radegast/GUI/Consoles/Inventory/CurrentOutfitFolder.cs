// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
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
        bool AppearanceSent = false;
        bool COFReady = false;
        bool InitialUpdateDone = false;
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
            InitiCOF = false;
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
            bool partOfCOF = false;
            var links = ContentLinks();
            foreach (var cofItem in links)
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

            if (Content.Count == links.Count)
            {
                COFReady = true;
                if (AppearanceSent)
                {
                    InitialUpdate();
                }
                lock (Content)
                {
                    foreach (InventoryItem link in Content.Values)
                    {
                        if (link.InventoryType == InventoryType.Wearable)
                        {
                            InventoryWearable w = (InventoryWearable)link;
                            InventoryItem lk = links.Find(l => l.AssetUUID == w.UUID);
                            // Logger.DebugLog(string.Format("\nName: {0}\nDescription: {1}\nType: {2} - {3}", w.Name, lk == null ? "" : lk.Description, w.Flags.ToString(), w.WearableType.ToString())); ;
                        }
                    }

                }
            }
        }

        object FolderSync = new object();

        void Inventory_FolderUpdated(object sender, FolderUpdatedEventArgs e)
        {
            if (COF == null) return;

            if (e.FolderID == COF.UUID && e.Success)
            {
                COF = (InventoryFolder)Client.Inventory.Store[COF.UUID];
                lock (FolderSync)
                {
                    lock (Content) Content.Clear();


                    List<UUID> items = new List<UUID>();
                    List<UUID> owners = new List<UUID>();

                    foreach (var link in ContentLinks())
                    {
                        //if (Client.Inventory.Store.Contains(link.AssetUUID))
                        //{
                        //    continue;
                        //}
                        items.Add(link.AssetUUID);
                        owners.Add(Client.Self.AgentID);
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
            Client.Inventory.RequestFolderContents(folderID, Client.Self.AgentID, true, true, InventorySortOrder.ByDate);
        }

        void InitCOF()
        {
            List<InventoryBase> rootContent = Client.Inventory.Store.GetContents(Client.Inventory.Store.RootFolder.UUID);
            foreach (InventoryBase baseItem in rootContent)
            {
                if (baseItem is InventoryFolder && ((InventoryFolder)baseItem).PreferredType == FolderType.CurrentOutfit)
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
            UUID cofID = Client.Inventory.CreateFolder(Client.Inventory.Store.RootFolder.UUID, "Current Outfit", FolderType.CurrentOutfit);
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
        /// Get COF contents
        /// </summary>
        /// <returns>List if InventoryItems that can be part of appearance (attachments, wearables)</returns>
        public List<InventoryItem> ContentLinks()
        {
            List<InventoryItem> ret = new List<InventoryItem>();
            if (COF == null) return ret;

            Client.Inventory.Store.GetContents(COF)
                .FindAll(b => CanBeWorn(b) && ((InventoryItem)b).AssetType == AssetType.Link)
                .ForEach(item => ret.Add((InventoryItem)item));

            return ret;
        }

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
        /// Checks if inventory item of Wearable type is worn
        /// </summary>
        /// <param name="currentlyWorn">Current outfit</param>
        /// <param name="item">Item to check</param>
        /// <returns>True if the item is worn</returns>
        public static bool IsWorn(Dictionary<WearableType, AppearanceManager.WearableData> currentlyWorn, InventoryItem item)
        {
            foreach (var n in currentlyWorn.Values)
            {
                if (n.ItemID == item.UUID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Can this inventory type be worn
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>True if the inventory item can be worn</returns>
        public static bool CanBeWorn(InventoryBase item)
        {
            return item is InventoryWearable || item is InventoryAttachment || item is InventoryObject;
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
            AddLink(item);
        }

        /// <summary>
        /// Creates a new COF link
        /// </summary>
        /// <param name="item">Original item to be linked from COF</param>
        public void AddLink(InventoryItem item)
        {
            if (item.InventoryType == InventoryType.Wearable && !IsBodyPart(item))
            {
                InventoryWearable w = (InventoryWearable)item;
                int layer = 0;
                string desc = string.Format("@{0}{1:00}", (int)w.WearableType, layer);
                AddLink(item, desc);
            }
            else
            {
                AddLink(item, string.Empty);
            }
        }

        /// <summary>
        /// Creates a new COF link
        /// </summary>
        /// <param name="item">Original item to be linked from COF</param>
        /// <param name="newDescription">Description for the link</param>
        public void AddLink(InventoryItem item, string newDescription)
        {
            if (COF == null) return;

            bool linkExists = false;

            linkExists = null != ContentLinks().Find(itemLink => itemLink.AssetUUID == item.UUID);

            if (!linkExists)
            {
                Client.Inventory.CreateLink(COF.UUID, item.UUID, item.Name, newDescription, AssetType.Link, item.InventoryType, UUID.Random(), (success, newItem) =>
                {
                    if (success)
                    {
                        Client.Inventory.RequestFetchInventory(newItem.UUID, newItem.OwnerID);
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
            RemoveLink(new List<UUID>(1) { itemID });
        }

        /// <summary>
        /// Remove a link to specified inventory item
        /// </summary>
        /// <param name="itemIDs">List of IDs of the target inventory item for which we want link to be removed</param>
        public void RemoveLink(List<UUID> itemIDs)
        {
            if (COF == null) return;

            List<UUID> toRemove = new List<UUID>();

            foreach (UUID itemID in itemIDs)
            {
                var links = ContentLinks().FindAll(itemLink => itemLink.AssetUUID == itemID);
                links.ForEach(item => toRemove.Add(item.UUID));
            }

            Client.Inventory.Remove(toRemove, null);
        }

        /// <summary>
        /// Remove attachment
        /// </summary>
        /// <param name="item">>Inventory item to be detached</param>
        public void Detach(InventoryItem item)
        {
            var realItem = RealInventoryItem(item);
            if (Instance.RLV.AllowDetach(realItem))
            {
                Client.Appearance.Detach(item);
                RemoveLink(item.UUID);
            }
        }

        public List<InventoryItem> GetWornAt(WearableType type)
        {
            var ret = new List<InventoryItem>();
            ContentLinks().ForEach(link =>
            {
                var item = RealInventoryItem(link);
                if (item is InventoryWearable)
                {
                    var w = (InventoryWearable)item;
                    if (w.WearableType == type)
                    {
                        ret.Add(item);
                    }
                }
            });

            return ret;
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

        /// <summary>
        /// Replaces the current outfit and updates COF links accordingly
        /// </summary>
        /// <param name="outfit">List of new wearables and attachments that comprise the new outfit</param>
        public void ReplaceOutfit(List<InventoryItem> newOutfit)
        {
            // Resolve inventory links
            List<InventoryItem> outfit = new List<InventoryItem>();
            foreach (var item in newOutfit)
            {
                outfit.Add(RealInventoryItem(item));
            }

            // Remove links to all exiting items
            List<UUID> toRemove = new List<UUID>();
            ContentLinks().ForEach(item =>
            {
                if (IsBodyPart(item))
                {
                    WearableType linkType = ((InventoryWearable)RealInventoryItem(item)).WearableType;
                    bool hasBodyPart = false;

                    foreach (var newItemTmp in newOutfit)
                    {
                        var newItem = RealInventoryItem(newItemTmp);
                        if (IsBodyPart(newItem))
                        {
                            if (((InventoryWearable)newItem).WearableType == linkType)
                            {
                                hasBodyPart = true;
                                break;
                            }
                        }
                    }

                    if (hasBodyPart)
                    {
                        toRemove.Add(item.UUID);
                    }
                }
                else
                {
                    toRemove.Add(item.UUID);
                }
            });

            Client.Inventory.Remove(toRemove, null);

            // Add links to new items
            List<InventoryItem> newItems = outfit.FindAll(item => CanBeWorn(item));
            foreach (var item in newItems)
            {
                AddLink(item);
            }

            Client.Appearance.ReplaceOutfit(outfit, false);
            WorkPool.QueueUserWorkItem(sync =>
            {
                Thread.Sleep(2000);
                Client.Appearance.RequestSetAppearance(true);
            });
        }

        /// <summary>
        /// Add items to current outfit
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="replace">Should existing wearable of the same type be removed</param>
        public void AddToOutfit(InventoryItem item, bool replace)
        {
            AddToOutfit(new List<InventoryItem>(1) { item }, replace);
        }

        /// <summary>
        /// Add items to current outfit
        /// </summary>
        /// <param name="items">List of items to add</param>
        /// <param name="replace">Should existing wearable of the same type be removed</param>
        public void AddToOutfit(List<InventoryItem> items, bool replace)
        {
            List<InventoryItem> current = ContentLinks();
            List<UUID> toRemove = new List<UUID>();

            // Resolve inventory links and remove wearables of the same type from COF
            List<InventoryItem> outfit = new List<InventoryItem>();

            foreach (var item in items)
            {
                InventoryItem realItem = RealInventoryItem(item);
                if (realItem is InventoryWearable)
                {
                    foreach (var link in current)
                    {
                        var currentItem = RealInventoryItem(link);
                        if (link.AssetUUID == item.UUID)
                        {
                            toRemove.Add(link.UUID);
                        }
                        else if (currentItem is InventoryWearable)
                        {
                            var w = (InventoryWearable)currentItem;
                            if (w.WearableType == ((InventoryWearable)realItem).WearableType)
                            {
                                toRemove.Add(link.UUID);
                            }
                        }
                    }
                }

                outfit.Add(realItem);
            }
            Client.Inventory.Remove(toRemove, null);

            // Add links to new items
            List<InventoryItem> newItems = outfit.FindAll(item => CanBeWorn(item));
            foreach (var item in newItems)
            {
                AddLink(item);
            }

            Client.Appearance.AddToOutfit(outfit, replace);
            WorkPool.QueueUserWorkItem(sync =>
            {
                Thread.Sleep(2000);
                Client.Appearance.RequestSetAppearance(true);
            });
        }

        /// <summary>
        /// Remove an item from the current outfit
        /// </summary>
        /// <param name="items">Item to remove</param>
        public void RemoveFromOutfit(InventoryItem item)
        {
            RemoveFromOutfit(new List<InventoryItem>(1) { item });
        }

        /// <summary>
        /// Remove specified items from the current outfit
        /// </summary>
        /// <param name="items">List of items to remove</param>
        public void RemoveFromOutfit(List<InventoryItem> items)
        {
            // Resolve inventory links
            List<InventoryItem> outfit = new List<InventoryItem>();
            foreach (var item in items)
            {
                var realItem = RealInventoryItem(item);
                if (Instance.RLV.AllowDetach(realItem))
                {
                    outfit.Add(realItem);
                }
            }

            // Remove links to all items that were removed
            List<UUID> toRemove = new List<UUID>();
            foreach (InventoryItem item in outfit.FindAll(item => CanBeWorn(item) && !IsBodyPart(item)))
            {
                toRemove.Add(item.UUID);
            }
            RemoveLink(toRemove);

            Client.Appearance.RemoveFromOutfit(outfit);
        }

        public bool IsBodyPart(InventoryItem item)
        {
            var realItem = RealInventoryItem(item);
            if (realItem is InventoryWearable)
            {
                var w = (InventoryWearable)realItem;
                var t = w.WearableType;
                if (t == WearableType.Shape ||
                    t == WearableType.Skin ||
                    t == WearableType.Eyes ||
                    t == WearableType.Hair)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Force rebaking textures
        /// </summary>
        public void RebakeTextures()
        {
            Client.Appearance.RequestSetAppearance(true);
        }

        #endregion Public methods
    }
}
