// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
﻿using System;
using System.Collections;
using System.Collections.Generic;
﻿using System.ComponentModel;
﻿using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using OpenMetaverse;

namespace Radegast
{
    public partial class InventoryConsole : UserControl
    {
        public class AttachmentInfo
        {
            public Primitive Prim;
            public InventoryItem Item;
            public UUID InventoryID;
            public UUID PrimID;
            public bool MarkedAttached = false;

            public AttachmentPoint Point
            {
                get
                {
                    if (Prim != null)
                    {
                        return Prim.PrimData.AttachmentPoint;
                    }
                    else
                    {
                        return AttachmentPoint.Default;
                    }
                }
            }
        }

        RadegastInstance instance;
        GridClient client { get { return instance.Client; } }
        Dictionary<UUID, TreeNode> FolderNodes = new Dictionary<UUID, TreeNode>();

        private InventoryManager Manager;
        private OpenMetaverse.Inventory Inventory;
        private TreeNode invRootNode;
        private string newItemName = string.Empty;
        private List<UUID> fetchedFolders = new List<UUID>();
        private System.Threading.Timer _EditTimer;
        private TreeNode _EditNode;
        private Dictionary<UUID, AttachmentInfo> attachments = new Dictionary<UUID, AttachmentInfo>();
        private System.Threading.Timer TreeUpdateTimer;
        private Queue<InventoryBase> ItemsToAdd = new Queue<InventoryBase>();
        private Queue<InventoryBase> ItemsToUpdate = new Queue<InventoryBase>();
        private bool TreeUpdateInProgress = false;
        private Dictionary<UUID, TreeNode> UUID2NodeCache = new Dictionary<UUID, TreeNode>();
        private int updateInterval = 1000;
        private Thread InventoryUpdate;
        private List<UUID> WornItems = new List<UUID>();
        private bool appearnceWasBusy;

        #region Construction and disposal
        public InventoryConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(InventoryConsole_Disposed);

            this.instance = instance;
            Manager = client.Inventory;
            Inventory = Manager.Store;
            Inventory.RootFolder.OwnerID = client.Self.AgentID;
            invTree.ImageList = frmMain.ResourceImages;
            invRootNode = AddDir(null, Inventory.RootFolder);
            Logger.Log("Reading inventory cache from " + instance.InventoryCacheFileName, Helpers.LogLevel.Debug, client);
            Inventory.RestoreFromDisk(instance.InventoryCacheFileName);
            AddFolderFromStore(invRootNode, Inventory.RootFolder);
            saveAllTToolStripMenuItem.Enabled = false;
            InventoryUpdate = new Thread(new ThreadStart(StartTraverseNodes));
            InventoryUpdate.Name = "InventoryUpdate";
            InventoryUpdate.IsBackground = true;
            InventoryUpdate.Start();

            invRootNode.Expand();

            invTree.TreeViewNodeSorter = new InvNodeSorter();
            invTree.AfterExpand += new TreeViewEventHandler(TreeView_AfterExpand);
            invTree.NodeMouseClick += new TreeNodeMouseClickEventHandler(invTree_MouseClick);
            invTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(invTree_NodeMouseDoubleClick);

            _EditTimer = new System.Threading.Timer(OnLabelEditTimer, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            // Callbacks
            Inventory.OnInventoryObjectAdded += new Inventory.InventoryObjectAdded(Store_OnInventoryObjectAdded);
            Inventory.OnInventoryObjectUpdated += new Inventory.InventoryObjectUpdated(Store_OnInventoryObjectUpdated);
            Inventory.OnInventoryObjectRemoved += new Inventory.InventoryObjectRemoved(Store_OnInventoryObjectRemoved);
            client.Objects.OnNewAttachment += new ObjectManager.NewAttachmentCallback(Objects_OnNewAttachment);
            client.Objects.OnObjectKilled += new ObjectManager.KillObjectCallback(Objects_OnObjectKilled);
            client.Appearance.OnAppearanceSet += new AppearanceManager.AppearanceSetCallback(Appearance_OnAppearanceSet);
        }

        void InventoryConsole_Disposed(object sender, EventArgs e)
        {
            if (TreeUpdateTimer != null)
            {
                TreeUpdateTimer.Dispose();
                TreeUpdateTimer = null;
            }
            if (InventoryUpdate != null)
            {
                if (InventoryUpdate.IsAlive)
                    InventoryUpdate.Abort();
                InventoryUpdate = null;
            }
            Inventory.OnInventoryObjectAdded -= new Inventory.InventoryObjectAdded(Store_OnInventoryObjectAdded);
            Inventory.OnInventoryObjectUpdated -= new Inventory.InventoryObjectUpdated(Store_OnInventoryObjectUpdated);
            Inventory.OnInventoryObjectRemoved -= new Inventory.InventoryObjectRemoved(Store_OnInventoryObjectRemoved);
            client.Objects.OnNewAttachment -= new ObjectManager.NewAttachmentCallback(Objects_OnNewAttachment);
            client.Objects.OnObjectKilled -= new ObjectManager.KillObjectCallback(Objects_OnObjectKilled);
            client.Appearance.OnAppearanceSet -= new AppearanceManager.AppearanceSetCallback(Appearance_OnAppearanceSet);
        }
        #endregion

        #region Network callbacks
        void Appearance_OnAppearanceSet(bool success)
        {
            UpdateWornLabels();
            if (appearnceWasBusy)
            {
                appearnceWasBusy = false;
                client.Appearance.RequestSetAppearance(true);
            }
        }

        void Objects_OnObjectKilled(Simulator simulator, uint objectID)
        {
            AttachmentInfo attachment = null;
            lock (attachments)
            {
                foreach (AttachmentInfo att in attachments.Values)
                {
                    if (att.Prim != null && att.Prim.LocalID == objectID)
                    {
                        attachment = att;
                        break;
                    }
                }

                if (attachment != null)
                {
                    attachments.Remove(attachment.InventoryID);
                    Store_OnInventoryObjectUpdated(attachment.Item, attachment.Item);
                }
            }
        }

        void Objects_OnNewAttachment(Simulator simulator, Primitive prim, ulong regionHandle, ushort timeDilation)
        {
            if (prim.ParentID != client.Self.LocalID) return;

            for (int i = 0; i < prim.NameValues.Length; i++)
            {
                if (prim.NameValues[i].Name == "AttachItemID")
                {
                    AttachmentInfo attachment = new AttachmentInfo();
                    attachment.Prim = prim;
                    attachment.InventoryID = new UUID(prim.NameValues[i].Value.ToString());
                    attachment.PrimID = prim.ID;

                    lock (attachments)
                    {
                        // Do we have attachmetns already on this spot?
                        AttachmentInfo oldAttachment = null;
                        UUID oldAttachmentUUID = UUID.Zero;
                        foreach (KeyValuePair<UUID, AttachmentInfo> att in attachments)
                        {
                            if (att.Value.Point == prim.PrimData.AttachmentPoint)
                            {
                                oldAttachment = att.Value;
                                oldAttachmentUUID = att.Key;
                                break;
                            }
                        }

                        if (oldAttachment != null && oldAttachment.InventoryID != attachment.InventoryID)
                        {
                            attachments.Remove(oldAttachmentUUID);
                            if (oldAttachment.Item != null)
                            {
                                attachment.MarkedAttached = false;
                                Store_OnInventoryObjectUpdated(oldAttachment.Item, oldAttachment.Item);
                            }
                        }

                        // Add new attachment info
                        if (!attachments.ContainsKey(attachment.InventoryID))
                        {
                            attachments.Add(attachment.InventoryID, attachment);

                        }
                        else
                        {
                            attachment = attachments[attachment.InventoryID];
                            if (attachment.Prim == null)
                            {
                                attachment.Prim = prim;
                            }
                        }

                        // Don't update the tree yet if we're still updating invetory tree from server
                        if (!TreeUpdateInProgress)
                        {
                            if (Inventory.Contains(attachment.InventoryID))
                            {
                                if (attachment.Item == null)
                                {
                                    InventoryItem item = (InventoryItem)Inventory[attachment.InventoryID];
                                    attachment.Item = item;
                                }
                                if (!attachment.MarkedAttached)
                                {
                                    attachment.MarkedAttached = true;
                                    Store_OnInventoryObjectUpdated(attachments[attachment.InventoryID].Item, attachments[attachment.InventoryID].Item);
                                }
                            }
                            else
                            {
                                client.Inventory.RequestFetchInventory(attachment.InventoryID, client.Self.AgentID);
                            }
                        }
                    }
                    break;
                }
            }
        }

        void Store_OnInventoryObjectAdded(InventoryBase obj)
        {
            if (TreeUpdateInProgress)
            {
                lock (ItemsToAdd)
                {
                    ItemsToAdd.Enqueue(obj);
                }
            }
            else
            {
                Exec_OnInventoryObjectAdded(obj);
            }
        }

        void Exec_OnInventoryObjectAdded(InventoryBase obj)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate()
                    {
                        Exec_OnInventoryObjectAdded(obj);
                    }
                ));
                return;
            }

            lock (attachments)
            {
                if (attachments.ContainsKey(obj.UUID))
                {
                    attachments[obj.UUID].Item = (InventoryItem)obj;
                }
            }

            TreeNode parent = findNodeForItem(obj.ParentUUID);

            if (parent != null)
            {
                TreeNode newNode = AddBase(parent, obj);
                if (obj.Name == newItemName)
                {
                    if (newNode.Parent.IsExpanded)
                    {
                        newNode.BeginEdit();
                    }
                    else
                    {
                        newNode.Parent.Expand();
                    }
                }
            }
            newItemName = string.Empty;
        }

        void Store_OnInventoryObjectRemoved(InventoryBase obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Store_OnInventoryObjectRemoved(obj);
                }
                ));
                return;
            }

            lock (attachments)
            {
                if (attachments.ContainsKey(obj.UUID))
                {
                    attachments.Remove(obj.UUID);
                }
            }

            TreeNode currentNode = findNodeForItem(obj.UUID);
            if (currentNode != null)
            {
                removeNode(currentNode);
            }
        }

        void Store_OnInventoryObjectUpdated(InventoryBase oldObject, InventoryBase newObject)
        {
            if (TreeUpdateInProgress)
            {
                lock (ItemsToUpdate)
                {
                    if (newObject is InventoryFolder)
                    {
                        TreeNode currentNode = findNodeForItem(newObject.UUID);
                        if (currentNode != null && currentNode.Text == newObject.Name) return;
                    }

                    if (!ItemsToUpdate.Contains(newObject))
                    {
                        ItemsToUpdate.Enqueue(newObject);
                    }
                }
            }
            else
            {
                Exec_OnInventoryObjectUpdated(oldObject, newObject);
            }
        }

        void Exec_OnInventoryObjectUpdated(InventoryBase oldObject, InventoryBase newObject)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Store_OnInventoryObjectUpdated(oldObject, newObject);
                }
                ));
                return;
            }

            if (attachments.ContainsKey(newObject.UUID))
            {
                attachments[newObject.UUID].Item = (InventoryItem)newObject;
            }

            // Find our current node in the tree
            TreeNode currentNode = findNodeForItem(newObject.UUID);

            // Find which node should be our parrent
            TreeNode parent = findNodeForItem(newObject.ParentUUID);

            if (parent == null) return;

            if (currentNode != null)
            {
                // Did we move to a different folder
                if (currentNode.Parent != parent)
                {
                    removeNode(currentNode);
                    AddBase(parent, newObject);
                }
                else // Update
                {
                    currentNode.Tag = newObject;
                    currentNode.Text = ItemLabel(newObject, false);
                    currentNode.Name = newObject.Name;
                }
            }
            else // We are not in the tree already, add
            {
                AddBase(parent, newObject);
            }
        }
        #endregion

        #region Node manipulation
        public static int GetDirImageIndex(string t)
        {
            int res = frmMain.ImageNames.IndexOf("inv_folder_" + t);
            if (res == -1)
            {
                switch (t)
                {
                    case "trashfolder":
                        return frmMain.ImageNames.IndexOf("inv_folder_trash");

                    case "lostandfoundfolder":
                        return frmMain.ImageNames.IndexOf("inv_folder_lostandfound");

                    case "lsltext":
                        return frmMain.ImageNames.IndexOf("inv_folder_script");
                }
                return frmMain.ImageNames.IndexOf("inv_folder_plain_closed");
            }
            return res;
        }

        public static int GetItemImageIndex(string t)
        {
            int res = frmMain.ImageNames.IndexOf("inv_item_" + t);
            if (res == -1)
            {
                if (t == "lsltext")
                {
                    return frmMain.ImageNames.IndexOf("inv_item_script");
                }
                else if (t == "callingcard")
                {
                    return frmMain.ImageNames.IndexOf("inv_item_callingcard_offline");
                }
            }
            return res;
        }

        TreeNode AddBase(TreeNode parent, InventoryBase obj)
        {
            if (obj is InventoryItem)
            {
                return AddItem(parent, (InventoryItem)obj);
            }
            else
            {
                return AddDir(parent, (InventoryFolder)obj);
            }
        }

        TreeNode AddDir(TreeNode parentNode, InventoryFolder f)
        {
            TreeNode dirNode = new TreeNode();
            dirNode.Name = f.UUID.ToString();
            dirNode.Text = f.Name;
            dirNode.Tag = f;
            dirNode.ImageIndex = GetDirImageIndex(f.PreferredType.ToString().ToLower());
            dirNode.SelectedImageIndex = dirNode.ImageIndex;
            if (parentNode == null)
            {
                invTree.Nodes.Add(dirNode);
            }
            else
            {
                parentNode.Nodes.Add(dirNode);
            }
            lock (UUID2NodeCache)
            {
                if (!UUID2NodeCache.ContainsKey(f.UUID))
                {
                    UUID2NodeCache.Add(f.UUID, dirNode);
                }
            }
            return dirNode;
        }


        TreeNode AddItem(TreeNode parent, InventoryItem item)
        {
            TreeNode itemNode = new TreeNode();
            itemNode.Name = item.UUID.ToString();
            itemNode.Text = ItemLabel(item, false);
            itemNode.Tag = item;
            int img = -1;
            if (item is InventoryWearable)
            {
                InventoryWearable w = item as InventoryWearable;
                img = GetItemImageIndex(w.WearableType.ToString().ToLower());
            }
            else
            {
                img = GetItemImageIndex(item.AssetType.ToString().ToLower());
            }
            itemNode.ImageIndex = img;
            itemNode.SelectedImageIndex = img;
            parent.Nodes.Add(itemNode);
            lock (UUID2NodeCache)
            {
                UUID2NodeCache.Add(item.UUID, itemNode);
            }
            return itemNode;
        }

        TreeNode findNodeForItem(UUID itemID)
        {
            lock (UUID2NodeCache)
            {
                if (UUID2NodeCache.ContainsKey(itemID))
                {
                    return UUID2NodeCache[itemID];
                }
            }
            return null;
        }

        void removeNode(TreeNode node)
        {
            InventoryBase item = (InventoryBase)node.Tag;
            if (item != null)
            {
                lock (UUID2NodeCache)
                {
                    UUID2NodeCache.Remove(item.UUID);
                }
            }
            node.Remove();
        }

        #endregion

        #region Private methods
        private void UpdateStatus(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate() { UpdateStatus(text); }));
                return;
            }

            if (text == "OK")
            {
                saveAllTToolStripMenuItem.Enabled = true;
            }

            tlabelStatus.Text = text;
        }

        private void AddFolderFromStore(TreeNode parent, InventoryFolder f)
        {
            List<InventoryBase> contents = Inventory.GetContents(f);
            foreach (InventoryBase item in contents)
            {
                TreeNode node = AddBase(parent, item);
                if (item is InventoryFolder)
                {
                    AddFolderFromStore(node, (InventoryFolder)item);
                }
            }
        }

        private void TraverseNodes(InventoryNode start)
        {
            bool has_items = false;

            foreach (InventoryNode node in start.Nodes.Values)
            {
                if (node.Data is InventoryItem)
                {
                    has_items = true;
                    break;
                }
            }

            if (!has_items || start.NeedsUpdate)
            {
                InventoryFolder f = (InventoryFolder)start.Data;
                AutoResetEvent gotFolderEvent = new AutoResetEvent(false);
                bool success = false;

                InventoryManager.FolderUpdatedCallback callback = delegate(UUID folderID)
                {
                    if (f.UUID == folderID)
                    {
                        if (((InventoryFolder)Inventory.Items[folderID].Data).DescendentCount <= Inventory.Items[folderID].Nodes.Count)
                        {
                            success = true;
                            gotFolderEvent.Set();
                        }
                    }
                };

                client.Inventory.OnFolderUpdated += callback;
                fetchFolder(f.UUID, f.OwnerID, true);
                gotFolderEvent.WaitOne(30 * 1000, false);
                client.Inventory.OnFolderUpdated -= callback;

                if (!success)
                {
                    Logger.Log(string.Format("Failed fetching folder {0}, got {1} items out of {2}", f.Name, Inventory.Items[f.UUID].Nodes.Count, ((InventoryFolder)Inventory.Items[f.UUID].Data).DescendentCount), Helpers.LogLevel.Error, client);
                }
            }

            foreach (InventoryBase item in Inventory.GetContents((InventoryFolder)start.Data))
            {
                if (item is InventoryFolder)
                {
                    TraverseNodes(Inventory.GetNodeFor(item.UUID));
                }
            }
        }

        private void StartTraverseNodes()
        {
            UpdateStatus("Loading...");
            TreeUpdateInProgress = true;
            TreeUpdateTimer = new System.Threading.Timer(TreeUpdateTimerTick, null, updateInterval, System.Threading.Timeout.Infinite);
            TraverseNodes(Inventory.RootNode);
            TreeUpdateTimer.Dispose();
            TreeUpdateTimer = null;
            TreeUpdateTimerTick(null);
            TreeUpdateInProgress = false;
            UpdateStatus("OK");

            // Updated labels on clothes that we are wearing
            UpdateWornLabels();

            // Update attachments now that we are done
            lock (attachments)
            {
                foreach (AttachmentInfo a in attachments.Values)
                {
                    if (a.Item == null)
                    {
                        if (Inventory.Contains(a.InventoryID))
                        {
                            a.MarkedAttached = true;
                            a.Item = (InventoryItem)Inventory[a.InventoryID];
                            Store_OnInventoryObjectUpdated(a.Item, a.Item);
                        }
                        else
                        {
                            client.Inventory.RequestFetchInventory(a.InventoryID, client.Self.AgentID);
                            return;
                        }
                    }
                }
            }

            Logger.Log("Finished updating invenory folders, saving cache...", Helpers.LogLevel.Debug, client);
            ThreadPool.QueueUserWorkItem((object state) => Inventory.SaveToDisk(instance.InventoryCacheFileName));
        }

        private void reloadInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TreeUpdateInProgress)
            {
                InventoryUpdate.Abort();
                InventoryUpdate = null;
            }

            saveAllTToolStripMenuItem.Enabled = false;

            Inventory.Items = new Dictionary<UUID, InventoryNode>();
            Inventory.RootFolder = Inventory.RootFolder;

            invTree.Nodes.Clear();
            UUID2NodeCache.Clear();
            invRootNode = AddDir(null, Inventory.RootFolder);

            InventoryUpdate = new Thread(new ThreadStart(StartTraverseNodes));
            InventoryUpdate.Name = "InventoryUpdate";
            InventoryUpdate.IsBackground = true;
            InventoryUpdate.Start();
            invRootNode.Expand();
        }


        private void TreeUpdateTimerTick(Object sender)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        TreeUpdateTimerTick(sender);
                    }
                    ));
                }
                catch (Exception) { }
                return;
            }

            lock (ItemsToAdd)
            {
                if (ItemsToAdd.Count > 0)
                {
                    invTree.BeginUpdate();
                    while (ItemsToAdd.Count > 0)
                    {
                        InventoryBase item = ItemsToAdd.Dequeue();
                        TreeNode node = findNodeForItem(item.ParentUUID);
                        if (node != null)
                        {
                            AddBase(node, item);
                        }
                    }
                    invTree.EndUpdate();
                }
            }

            lock (ItemsToUpdate)
            {
                if (ItemsToUpdate.Count > 0)
                {
                    invTree.BeginUpdate();
                    while (ItemsToUpdate.Count > 0)
                    {
                        InventoryBase item = ItemsToUpdate.Dequeue();
                        Exec_OnInventoryObjectUpdated(item, item);
                    }
                    invTree.EndUpdate();
                }
            }

            UpdateStatus("Loading... " + UUID2NodeCache.Count.ToString() + " items");
            try
            {
                if (TreeUpdateTimer != null)
                {
                    TreeUpdateTimer.Change(updateInterval, System.Threading.Timeout.Infinite);
                }
            }
            catch (Exception) { }
        }

        #endregion

        private void btnProfile_Click(object sender, EventArgs e)
        {
            instance.MainForm.ShowAgentProfile(txtCreator.Text, txtCreator.AgentID);
        }

        void UpdateItemInfo(InventoryItem item)
        {
            btnProfile.Enabled = true;
            txtItemName.Text = item.Name;
            txtCreator.AgentID = item.CreatorID;
            txtCreator.Tag = item.CreatorID;
            txtCreated.Text = item.CreationDate.ToString();

            if (item.AssetUUID != UUID.Zero)
            {
                txtAssetID.Text = item.AssetUUID.ToString();
            }
            else
            {
                txtAssetID.Text = String.Empty;
            }
            foreach (Control c in pnlDetail.Controls)
            {
                c.Dispose();
            }
            pnlDetail.Controls.Clear();

            switch (item.AssetType)
            {
                case AssetType.Texture:
                    SLImageHandler image = new SLImageHandler(instance, item.AssetUUID, item.Name);
                    image.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(image);
                    break;

                case AssetType.Notecard:
                    Notecard note = new Notecard(instance, (InventoryNotecard)item);
                    note.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(note);
                    break;

                case AssetType.Landmark:
                    Landmark landmark = new Landmark(instance, (InventoryLandmark)item);
                    landmark.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(landmark);
                    break;

                case AssetType.LSLText:
                    ScriptEditor script = new ScriptEditor(instance, (InventoryLSL)item);
                    script.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(script);
                    break;

                case AssetType.Gesture:
                    Guesture gesture = new Guesture(instance, (InventoryGesture)item);
                    gesture.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(gesture);
                    break;

            }
        }

        void invTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is InventoryItem)
            {
                InventoryItem item = e.Node.Tag as InventoryItem;
                switch (item.AssetType)
                {

                    case AssetType.Landmark:
                        instance.TabConsole.DisplayNotificationInChat("Teleporting to " + item.Name);
                        client.Self.RequestTeleport(item.AssetUUID);
                        break;

                    case AssetType.Gesture:
                        client.Self.PlayGesture(item.AssetUUID);
                        break;

                    case AssetType.Notecard:
                        Notecard note = new Notecard(instance, (InventoryNotecard)item);
                        note.Dock = DockStyle.Fill;
                        note.ShowDetached();
                        break;

                    case AssetType.LSLText:
                        ScriptEditor script = new ScriptEditor(instance, (InventoryLSL)item);
                        script.Dock = DockStyle.Fill;
                        script.ShowDetached();
                        break;
                }
            }
        }

        private void fetchFolder(UUID folderID, UUID ownerID, bool force)
        {
            if (force || !fetchedFolders.Contains(folderID))
            {
                if (!fetchedFolders.Contains(folderID))
                {
                    fetchedFolders.Add(folderID);
                }
                client.Inventory.RequestFolderContents(folderID, ownerID, true, true, InventorySortOrder.ByDate);
            }
        }

        public bool IsWorn(InventoryItem item)
        {
            bool worn = client.Appearance.IsItemWorn(item) != WearableType.Invalid;

            lock (WornItems)
            {
                if (worn && !WornItems.Contains(item.UUID))
                    WornItems.Add(item.UUID);
            }
            return worn;
        }

        public AttachmentPoint AttachedTo(InventoryItem item)
        {
            lock (attachments)
            {
                if (attachments.ContainsKey(item.UUID))
                {
                    return attachments[item.UUID].Point;
                }
            }

            return AttachmentPoint.Default;
        }

        public bool IsAttached(InventoryItem item)
        {
            lock (attachments)
            {
                return attachments.ContainsKey(item.UUID);
            }
        }

        public InventoryItem AttachmentAt(AttachmentPoint point)
        {
            lock (attachments)
            {
                foreach (KeyValuePair<UUID, AttachmentInfo> att in attachments)
                {
                    if (att.Value.Point == point)
                    {
                        return att.Value.Item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns text of the label
        /// </summary>
        /// <param name="invBase">Inventory item</param>
        /// <param name="returnRaw">Should we return raw text, or if false decorated text with (worn) info, and (no copy) etc. permission info</param>
        /// <returns></returns>
        public string ItemLabel(InventoryBase invBase, bool returnRaw)
        {
            if (returnRaw || (invBase is InventoryFolder))
                return invBase.Name;

            InventoryItem item = (InventoryItem)invBase;

            string raw = item.Name;

            if ((item.Permissions.OwnerMask & PermissionMask.Modify) == 0)
                raw += " (no modify)";

            if ((item.Permissions.OwnerMask & PermissionMask.Copy) == 0)
                raw += " (no copy)";

            if ((item.Permissions.OwnerMask & PermissionMask.Transfer) == 0)
                raw += " (no transfer)";

            if (IsWorn(item))
                raw += " (worn)";

            if (IsAttached(item))
            {
                raw += " (worn on " + AttachedTo(item).ToString() + ")";
            }

            return raw;
        }

        void invTree_MouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;

            if (e.Button == MouseButtons.Left)
            {
                invTree.SelectedNode = node;
                if (node.Tag is InventoryItem)
                {
                    UpdateItemInfo(node.Tag as InventoryItem);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                invTree.SelectedNode = node;
                ctxInv.Show(invTree,e.X,e.Y);
                Logger.Log("Right click on node: " + node.Name, Helpers.LogLevel.Debug, client);
            }
        }

        private void ctxInv_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = invTree.SelectedNode;
            if (node==null)
            {
                e.Cancel = true;
            }
            else
            {
                if (node.Tag is InventoryFolder)
                {
                    InventoryFolder folder = (InventoryFolder)node.Tag;
                    ctxInv.Items.Clear();

                    ToolStripMenuItem ctxItem;

                    ctxItem = new ToolStripMenuItem("New folder", null, OnInvContextClick);
                    ctxItem.Name = "new_folder";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("Refresh", null, OnInvContextClick);
                    ctxItem.Name = "refresh";
                    ctxInv.Items.Add(ctxItem);

                    ctxInv.Items.Add(new ToolStripSeparator());

                    ctxItem = new ToolStripMenuItem("Expand", null, OnInvContextClick);
                    ctxItem.Name = "expand";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("Expand all", null, OnInvContextClick);
                    ctxItem.Name = "expand_all";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("Collapse", null, OnInvContextClick);
                    ctxItem.Name = "collapse";
                    ctxInv.Items.Add(ctxItem);

                    if (folder.PreferredType == AssetType.TrashFolder)
                    {
                        ctxItem = new ToolStripMenuItem("Empty trash", null, OnInvContextClick);
                        ctxItem.Name = "empty_trash";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (folder.PreferredType == AssetType.LostAndFoundFolder)
                    {
                        ctxItem = new ToolStripMenuItem("Empty lost and found", null, OnInvContextClick);
                        ctxItem.Name = "empty_lost_found";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (folder.PreferredType == AssetType.Unknown)
                    {
                        ctxItem = new ToolStripMenuItem("Rename", null, OnInvContextClick);
                        ctxItem.Name = "rename_folder";
                        ctxInv.Items.Add(ctxItem);

                        ctxInv.Items.Add(new ToolStripSeparator());

                        ctxItem = new ToolStripMenuItem("Delete", null, OnInvContextClick);
                        ctxItem.Name = "delete_folder";
                        ctxInv.Items.Add(ctxItem);

                        ctxInv.Items.Add(new ToolStripSeparator());

                        ctxItem = new ToolStripMenuItem("Take off Items", null, OnInvContextClick);
                        ctxItem.Name = "outfit_take_off";
                        ctxInv.Items.Add(ctxItem);

                        ctxItem = new ToolStripMenuItem("Add to Outfit", null, OnInvContextClick);
                        ctxItem.Name = "outfit_add";
                        ctxInv.Items.Add(ctxItem);

                        ctxItem = new ToolStripMenuItem("Replace Outfit", null, OnInvContextClick);
                        ctxItem.Name = "outfit_replace";
                        ctxInv.Items.Add(ctxItem);
                    }

                    instance.ContextActionManager.AddContributions(ctxInv, folder);
                }
                else if (node.Tag is InventoryItem)
                {
                    InventoryItem item = (InventoryItem)node.Tag;
                    ctxInv.Items.Clear();

                    ToolStripMenuItem ctxItem;

                    if (item.InventoryType == InventoryType.LSL)
                    {
                        ctxItem = new ToolStripMenuItem("Edit script", null, OnInvContextClick);
                        ctxItem.Name = "edit_script";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (item.InventoryType == InventoryType.Landmark)
                    {
                        ctxItem = new ToolStripMenuItem("Teleport", null, OnInvContextClick);
                        ctxItem.Name = "lm_teleport";
                        ctxInv.Items.Add(ctxItem);

                        ctxItem = new ToolStripMenuItem("Info", null, OnInvContextClick);
                        ctxItem.Name = "lm_info";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (item.InventoryType == InventoryType.Notecard)
                    {
                        ctxItem = new ToolStripMenuItem("Open", null, OnInvContextClick);
                        ctxItem.Name = "notecard_open";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (item.InventoryType == InventoryType.Gesture)
                    {
                        ctxItem = new ToolStripMenuItem("Play", null, OnInvContextClick);
                        ctxItem.Name = "gesture_play";
                        ctxInv.Items.Add(ctxItem);

                        ctxItem = new ToolStripMenuItem("Info", null, OnInvContextClick);
                        ctxItem.Name = "gesture_info";
                        ctxInv.Items.Add(ctxItem);
                    }

                    ctxItem = new ToolStripMenuItem("Rename", null, OnInvContextClick);
                    ctxItem.Name = "rename_item";
                    ctxInv.Items.Add(ctxItem);

                    ctxInv.Items.Add(new ToolStripSeparator());
                    ctxItem = new ToolStripMenuItem("Delete", null, OnInvContextClick);
                    ctxItem.Name = "delete_item";
                    if (IsAttached(item) || IsWorn(item))
                    {
                        ctxItem.Enabled = false;
                    }
                    ctxInv.Items.Add(ctxItem);

                    if (IsAttached(item))
                    {
                        ctxItem = new ToolStripMenuItem("Detach from yourself", null, OnInvContextClick);
                        ctxItem.Name = "detach";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (!IsAttached(item) && (item.InventoryType == InventoryType.Object || item.InventoryType == InventoryType.Attachment))
                    {
                        ToolStripMenuItem ctxItemAttach = new ToolStripMenuItem("Attach to");
                        ctxInv.Items.Add(ctxItemAttach);

                        ToolStripMenuItem ctxItemAttachHUD = new ToolStripMenuItem("Attach to HUD");
                        ctxInv.Items.Add(ctxItemAttachHUD);

                        foreach (AttachmentPoint pt in Enum.GetValues(typeof(AttachmentPoint)))
                        {
                            if (!pt.ToString().StartsWith("HUD"))
                            {
                                string name = pt.ToString();

                                InventoryItem alreadyAttached = null;
                                if ((alreadyAttached = AttachmentAt(pt)) != null)
                                {
                                    name += " (" + alreadyAttached.Name + ")";
                                }

                                ToolStripMenuItem ptItem = new ToolStripMenuItem(name, null, OnInvContextClick);
                                ptItem.Name = pt.ToString();
                                ptItem.Tag = pt;
                                ptItem.Name = "attach_to";
                                ctxItemAttach.DropDownItems.Add(ptItem);
                            }
                            else
                            {
                                string name = pt.ToString().Substring(3);

                                InventoryItem alreadyAttached = null;
                                if ((alreadyAttached = AttachmentAt(pt)) != null)
                                {
                                    name += " (" + alreadyAttached.Name + ")";
                                }

                                ToolStripMenuItem ptItem = new ToolStripMenuItem(name, null, OnInvContextClick);
                                ptItem.Name = pt.ToString();
                                ptItem.Tag = pt;
                                ptItem.Name = "attach_to";
                                ctxItemAttachHUD.DropDownItems.Add(ptItem);
                            }
                        }

                        ctxItem = new ToolStripMenuItem("Wear", null, OnInvContextClick);
                        ctxItem.Name = "wear_attachment";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (item is InventoryWearable)
                    {
                        ctxInv.Items.Add(new ToolStripSeparator());

                        if (IsWorn(item))
                        {
                            ctxItem = new ToolStripMenuItem("Take off", null, OnInvContextClick);
                            ctxItem.Name = "item_take_off";
                            ctxInv.Items.Add(ctxItem);
                        }
                        else
                        {
                            ctxItem = new ToolStripMenuItem("Wear", null, OnInvContextClick);
                            ctxItem.Name = "item_wear";
                            ctxInv.Items.Add(ctxItem);
                        }
                    }

                    instance.ContextActionManager.AddContributions(ctxInv, item);

                }
            }
        }

        #region Context menu folder
        private void OnInvContextClick(object sender, EventArgs e)
        {
            if (invTree.SelectedNode == null || !(invTree.SelectedNode.Tag is InventoryBase))
            {
                return;
            }

            string cmd = ((ToolStripMenuItem)sender).Name;

            if (invTree.SelectedNode.Tag is InventoryFolder)
            {
                #region Folder actions
                InventoryFolder f = (InventoryFolder)invTree.SelectedNode.Tag;

                switch (cmd)
                {
                    case "refresh":
                        fetchFolder(f.UUID, f.OwnerID, true);
                        break;

                    case "expand":
                        invTree.SelectedNode.Expand();
                        break;

                    case "expand_all":
                        invTree.SelectedNode.ExpandAll();
                        break;

                    case "collapse":
                        invTree.SelectedNode.Collapse();
                        break;

                    case "new_folder":
                        newItemName = "New folder";
                        client.Inventory.CreateFolder(f.UUID, "New folder");
                        break;

                    case "delete_folder":
                        client.Inventory.MoveFolder(f.UUID, client.Inventory.FindFolderForType(AssetType.TrashFolder), f.Name);
                        break;

                    case "empty_trash":
                        {
                            DialogResult res = MessageBox.Show("Are you sure you want to empty your trash?", "Confirmation", MessageBoxButtons.OKCancel);
                            if (res == DialogResult.OK)
                            {
                                client.Inventory.EmptyTrash();
                            }
                        }
                        break;

                    case "empty_lost_found":
                        {
                            DialogResult res = MessageBox.Show("Are you sure you want to empty your lost and found folder?", "Confirmation", MessageBoxButtons.OKCancel);
                            if (res == DialogResult.OK)
                            {
                                client.Inventory.EmptyLostAndFound();
                            }
                        }
                        break;

                    case "rename_folder":
                        invTree.SelectedNode.BeginEdit();
                        break;

                    case "outfit_replace":
                        List<InventoryItem> newOutfit = new List<InventoryItem>();
                        foreach (InventoryBase item in Inventory.GetContents(f))
                        {
                            if (item is InventoryItem)
                                newOutfit.Add((InventoryItem)item);
                        }
                        appearnceWasBusy = client.Appearance.ManagerBusy;
                        client.Appearance.ReplaceOutfit(newOutfit);
                        client.Appearance.RequestSetAppearance(true);
                        UpdateWornLabels();
                        break;

                    case "outfit_add":
                        List<InventoryItem> addToOutfit = new List<InventoryItem>();
                        foreach (InventoryBase item in Inventory.GetContents(f))
                        {
                            if (item is InventoryItem)
                                addToOutfit.Add((InventoryItem)item);
                        }
                        appearnceWasBusy = client.Appearance.ManagerBusy;
                        client.Appearance.AddToOutfit(addToOutfit);
                        UpdateWornLabels();
                        break;

                    case "outfit_take_off":
                        List<InventoryItem> removeFromOutfit = new List<InventoryItem>();
                        foreach (InventoryBase item in Inventory.GetContents(f))
                        {
                            if (item is InventoryItem)
                                removeFromOutfit.Add((InventoryItem)item);
                        }
                        appearnceWasBusy = client.Appearance.ManagerBusy;
                        client.Appearance.RemoveFromOutfit(removeFromOutfit);
                        UpdateWornLabels();
                        break;
                }
                #endregion
            }
            else if (invTree.SelectedNode.Tag is InventoryItem)
            {
                #region Item actions
                InventoryItem item = (InventoryItem)invTree.SelectedNode.Tag;

                switch (cmd)
                {
                    case "delete_item":
                        client.Inventory.MoveItem(item.UUID, client.Inventory.FindFolderForType(AssetType.TrashFolder), item.Name);
                        break;

                    case "rename_item":
                        invTree.SelectedNode.BeginEdit();
                        break;

                    case "detach":
                        client.Appearance.Detach(item.UUID);
                        attachments.Remove(item.UUID);
                        invTree.SelectedNode.Text = ItemLabel(item, false);
                        break;

                    case "wear_attachment":
                        client.Appearance.Attach(item, AttachmentPoint.Default);
                        break;

                    case "attach_to":
                        AttachmentPoint pt = (AttachmentPoint)((ToolStripMenuItem)sender).Tag;
                        client.Appearance.Attach(item, pt);
                        break;

                    case "edit_script":
                        ScriptEditor se = new ScriptEditor(instance, (InventoryLSL)item);
                        se.Detached = true;
                        return;

                    case "item_take_off":
                        appearnceWasBusy = client.Appearance.ManagerBusy;
                        client.Appearance.RemoveFromOutfit(item);
                        invTree.SelectedNode.Text = ItemLabel(item, false);
                        lock (WornItems)
                        {
                            if (WornItems.Contains(item.UUID))
                            {
                                WornItems.Remove(item.UUID);
                            }
                        }
                        break;

                    case "item_wear":
                        appearnceWasBusy = client.Appearance.ManagerBusy;
                        client.Appearance.AddToOutfit(item);
                        invTree.SelectedNode.Text = ItemLabel(item, false);
                        break;

                    case "lm_teleport":
                        instance.TabConsole.DisplayNotificationInChat("Teleporting to " + item.Name);
                        client.Self.RequestTeleport(item.AssetUUID);
                        break;

                    case "lm_info":
                        UpdateItemInfo(item);
                        break;

                    case "notecard_open":
                        UpdateItemInfo(item);
                        break;

                    case "gesture_info":
                        UpdateItemInfo(item);
                        break;

                    case "gesture_play":
                        client.Self.PlayGesture(item.AssetUUID);
                        break;
                }
                #endregion
            }
        }
        #endregion

        private void UpdateWornLabels()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(UpdateWornLabels));
                return;
            }

            invTree.BeginUpdate();
            foreach (UUID itemID in WornItems)
            {
                TreeNode node = findNodeForItem(itemID);
                if (node != null)
                {
                    node.Text = ItemLabel((InventoryBase)node.Tag, false);
                }
            }
            WornItems.Clear();
            foreach (AppearanceManager.WearableData wearable in client.Appearance.GetWearables().Values)
            {
                TreeNode node = findNodeForItem(wearable.ItemID);
                if (node != null)
                {
                    node.Text = ItemLabel((InventoryBase)node.Tag, false);
                }
            }
            invTree.EndUpdate();
        }

        void TreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            // Check if we need to go into edit mode for new items
            if (newItemName != string.Empty)
            {
                foreach (TreeNode n in e.Node.Nodes)
                {
                    if (n.Name == newItemName)
                    {
                        n.BeginEdit();
                        break;
                    }
                }
                newItemName = string.Empty;
            }
        }

        private bool _EditingNode = false;

        private void OnLabelEditTimer(object sender)
        {
            if (_EditNode == null || !(_EditNode.Tag is InventoryBase))
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        OnLabelEditTimer(sender);
                    }
                ));
                return;
            }

            _EditingNode = true;
            _EditNode.Text = ItemLabel((InventoryBase)_EditNode.Tag, true);
            _EditNode.BeginEdit();
        }

        private void invTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node == null ||
                !(e.Node.Tag is InventoryBase) ||
                (e.Node.Tag is InventoryFolder && ((InventoryFolder)e.Node.Tag).PreferredType != AssetType.Unknown)
                )
            {
                e.CancelEdit = true;
                return;
            }

            if (_EditingNode)
            {
                _EditingNode = false;
            }
            else
            {
                e.CancelEdit = true;
                _EditNode = e.Node;
                _EditTimer.Change(20, System.Threading.Timeout.Infinite);
            }
        }

        private void invTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Label))
            {
                if (e.Node.Tag is InventoryBase)
                {
                    e.Node.Text = ItemLabel((InventoryBase)e.Node.Tag, false);
                }
                e.CancelEdit = true;
                return;
            }

            if (e.Node.Tag is InventoryFolder)
            {
                InventoryFolder f = (InventoryFolder)e.Node.Tag;
                f.Name = e.Label;
                client.Inventory.MoveFolder(f.UUID, f.ParentUUID, f.Name);
            }
            else if (e.Node.Tag is InventoryItem)
            {
                InventoryItem item = (InventoryItem)e.Node.Tag;
                item.Name = e.Label;
                e.Node.Text = ItemLabel((InventoryBase)item, false);
                client.Inventory.MoveItem(item.UUID, item.ParentUUID, item.Name);
            }

        }

        private void invTree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && invTree.SelectedNode != null)
            {
                invTree.SelectedNode.BeginEdit();
            }
            else if (e.KeyCode == Keys.F5 && invTree.SelectedNode != null)
            {
                if (invTree.SelectedNode.Tag is InventoryFolder)
                {
                    InventoryFolder f = (InventoryFolder)invTree.SelectedNode.Tag;
                    fetchFolder(f.UUID, f.OwnerID, true);
                }
            }
            else if (e.KeyCode == Keys.Delete && invTree.SelectedNode != null)
            {
                if (invTree.SelectedNode.Tag is InventoryItem)
                {
                    InventoryItem item = invTree.SelectedNode.Tag as InventoryItem;
                    client.Inventory.MoveItem(item.UUID, client.Inventory.FindFolderForType(AssetType.TrashFolder), item.Name);
                }
                else if (invTree.SelectedNode.Tag is InventoryFolder)
                {
                    InventoryFolder f = invTree.SelectedNode.Tag as InventoryFolder;
                    client.Inventory.MoveFolder(f.UUID, client.Inventory.FindFolderForType(AssetType.TrashFolder), f.Name);
                }
            } else if (e.KeyCode == Keys.Apps && invTree.SelectedNode != null)
            {
                ctxInv.Show();
            }
        }

        #region Drag and Drop
        private void invTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            invTree.SelectedNode = e.Item as TreeNode;
            if (invTree.SelectedNode.Tag is InventoryFolder && ((InventoryFolder)invTree.SelectedNode.Tag).PreferredType != AssetType.Unknown)
            {
                return;
            }
            invTree.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void invTree_DragDrop(object sender, DragEventArgs e)
        {
            if (highlightedNode != null)
            {
                highlightedNode.BackColor = invTree.BackColor;
                highlightedNode = null;
            }

            TreeNode sourceNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (sourceNode == null) return;

            Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            TreeNode destinationNode = ((TreeView)sender).GetNodeAt(pt);

            if (destinationNode == null) return;

            if (sourceNode == destinationNode) return;

            // If droping to item within folder drop to its folder
            if (destinationNode.Tag is InventoryItem)
            {
                destinationNode = destinationNode.Parent;
            }

            InventoryFolder dest = destinationNode.Tag as InventoryFolder;

            if (dest == null) return;

            if (sourceNode.Tag is InventoryItem)
            {
                InventoryItem item = (InventoryItem)sourceNode.Tag;
                client.Inventory.MoveItem(item.UUID, dest.UUID, item.Name);
            }
            else if (sourceNode.Tag is InventoryFolder)
            {
                InventoryFolder f = (InventoryFolder)sourceNode.Tag;
                client.Inventory.MoveFolder(f.UUID, dest.UUID, f.Name);
            }
        }

        private void invTree_DragEnter(object sender, DragEventArgs e)
        {
            TreeNode node = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (node == null)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        TreeNode highlightedNode = null;

        private void invTree_DragOver(object sender, DragEventArgs e)
        {
            TreeNode node = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (node == null)
            {
                e.Effect = DragDropEffects.None;
            }

            Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            TreeNode destinationNode = ((TreeView)sender).GetNodeAt(pt);

            if (highlightedNode != destinationNode)
            {
                if (highlightedNode != null)
                {
                    highlightedNode.BackColor = invTree.BackColor;
                    highlightedNode = null;
                }

                highlightedNode = destinationNode;
                highlightedNode.BackColor = Color.LightSlateGray;
            }

            if (destinationNode == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;

        }
        #endregion

        private void saveAllTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new InventoryBackup(instance)).Show();
        }


    }

    #region Sorter class
    // Create a node sorter that implements the IComparer interface.
    public class InvNodeSorter : System.Collections.IComparer
    {
        bool _sysfirst = true;

        int CompareFolders(InventoryFolder x, InventoryFolder y)
        {
            if (_sysfirst)
            {
                if (x.PreferredType != AssetType.Unknown && y.PreferredType == AssetType.Unknown)
                {
                    return -1;
                }
                else if (x.PreferredType == AssetType.Unknown && y.PreferredType != AssetType.Unknown)
                {
                    return 1;
                }
            }
            return String.Compare(x.Name, y.Name);
        }

        public bool SystemFoldersFirst { set { _sysfirst = value; } get { return _sysfirst; } }

        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            if (tx.Tag is InventoryFolder && ty.Tag is InventoryFolder)
            {
                return CompareFolders(tx.Tag as InventoryFolder, ty.Tag as InventoryFolder);
            }
            else if (tx.Tag is InventoryFolder && ty.Tag is InventoryItem)
            {
                return -1;
            }
            else if (tx.Tag is InventoryItem && ty.Tag is InventoryFolder)
            {
                return 1;
            }

            // Two items
            if (!(tx.Tag is InventoryItem) || !(ty.Tag is InventoryItem))
            {
                return 0;
            }
            InventoryItem item1 = (InventoryItem)tx.Tag;
            InventoryItem item2 = (InventoryItem)ty.Tag;
            if (item1.CreationDate < item2.CreationDate)
            {
                return 1;
            }
            else if (item1.CreationDate > item2.CreationDate)
            {
                return -1;
            }
            return string.Compare(item1.Name, item2.Name);
        }
    }
    #endregion
}
