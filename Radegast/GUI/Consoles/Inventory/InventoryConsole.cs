// 
// Radegast Metaverse Client
// Copyright (c) 2009-2011, Radegast Development Team
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{

    public partial class InventoryConsole : UserControl
    {
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
        private System.Timers.Timer TreeUpdateTimer;
        private Queue<InventoryBase> ItemsToAdd = new Queue<InventoryBase>();
        private Queue<InventoryBase> ItemsToUpdate = new Queue<InventoryBase>();
        private bool TreeUpdateInProgress = false;
        private Dictionary<UUID, TreeNode> UUID2NodeCache = new Dictionary<UUID, TreeNode>();
        private int updateInterval = 1000;
        private Thread InventoryUpdate;
        private List<UUID> WornItems = new List<UUID>();
        private bool appearnceWasBusy;
        private InvNodeSorter sorter;

        #region Construction and disposal
        public InventoryConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(InventoryConsole_Disposed);

            TreeUpdateTimer = new System.Timers.Timer()
            {
                Interval = updateInterval,
                Enabled = false,
                SynchronizingObject = invTree
            };
            TreeUpdateTimer.Elapsed += TreeUpdateTimerTick;

            this.instance = instance;
            Manager = client.Inventory;
            Inventory = Manager.Store;
            Inventory.RootFolder.OwnerID = client.Self.AgentID;
            invTree.ImageList = frmMain.ResourceImages;
            invRootNode = AddDir(null, Inventory.RootFolder);
            Logger.Log("Reading inventory cache from " + instance.InventoryCacheFileName, Helpers.LogLevel.Debug, client);
            Inventory.RestoreFromDisk(instance.InventoryCacheFileName);
            AddFolderFromStore(invRootNode, Inventory.RootFolder);

            sorter = new InvNodeSorter();

            if (!instance.GlobalSettings.ContainsKey("inv_sort_bydate"))
                instance.GlobalSettings["inv_sort_bydate"] = OSD.FromBoolean(true);
            if (!instance.GlobalSettings.ContainsKey("inv_sort_sysfirst"))
                instance.GlobalSettings["inv_sort_sysfirst"] = OSD.FromBoolean(true);

            sorter.ByDate = instance.GlobalSettings["inv_sort_bydate"].AsBoolean();
            sorter.SystemFoldersFirst = instance.GlobalSettings["inv_sort_sysfirst"].AsBoolean();

            tbtnSortByDate.Checked = sorter.ByDate;
            tbtbSortByName.Checked = !sorter.ByDate;
            tbtnSystemFoldersFirst.Checked = sorter.SystemFoldersFirst;

            invTree.TreeViewNodeSorter = sorter;

            if (instance.MonoRuntime)
            {
                invTree.BackColor = Color.FromKnownColor(KnownColor.Window);
                invTree.ForeColor = invTree.LineColor = Color.FromKnownColor(KnownColor.WindowText);
                InventoryFolder f = new InventoryFolder(UUID.Random());
                f.Name = "";
                f.ParentUUID = UUID.Zero;
                f.PreferredType = AssetType.Unknown;
                TreeNode dirNode = new TreeNode();
                dirNode.Name = f.UUID.ToString();
                dirNode.Text = f.Name;
                dirNode.Tag = f;
                dirNode.ImageIndex = GetDirImageIndex(f.PreferredType.ToString().ToLower());
                dirNode.SelectedImageIndex = dirNode.ImageIndex;
                invTree.Nodes.Add(dirNode);
                invTree.Sort();
            }

            saveAllTToolStripMenuItem.Enabled = false;
            InventoryUpdate = new Thread(new ThreadStart(StartTraverseNodes));
            InventoryUpdate.Name = "InventoryUpdate";
            InventoryUpdate.IsBackground = true;
            InventoryUpdate.Start();

            invRootNode.Expand();

            invTree.AfterExpand += new TreeViewEventHandler(TreeView_AfterExpand);
            invTree.NodeMouseClick += new TreeNodeMouseClickEventHandler(invTree_MouseClick);
            invTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(invTree_NodeMouseDoubleClick);

            _EditTimer = new System.Threading.Timer(OnLabelEditTimer, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            // Callbacks
            Inventory.InventoryObjectAdded += new EventHandler<InventoryObjectAddedEventArgs>(Inventory_InventoryObjectAdded);
            Inventory.InventoryObjectUpdated += new EventHandler<InventoryObjectUpdatedEventArgs>(Inventory_InventoryObjectUpdated);
            Inventory.InventoryObjectRemoved += new EventHandler<InventoryObjectRemovedEventArgs>(Inventory_InventoryObjectRemoved);

            client.Objects.ObjectUpdate += new EventHandler<PrimEventArgs>(Objects_AttachmentUpdate);
            client.Objects.KillObject += new EventHandler<KillObjectEventArgs>(Objects_KillObject);
            client.Appearance.AppearanceSet += new EventHandler<AppearanceSetEventArgs>(Appearance_AppearanceSet);
        }

        void InventoryConsole_Disposed(object sender, EventArgs e)
        {
            if (TreeUpdateTimer != null)
            {
                TreeUpdateTimer.Stop();
                TreeUpdateTimer.Dispose();
                TreeUpdateTimer = null;
            }
            if (InventoryUpdate != null)
            {
                if (InventoryUpdate.IsAlive)
                    InventoryUpdate.Abort();
                InventoryUpdate = null;
            }

            Inventory.InventoryObjectAdded -= new EventHandler<InventoryObjectAddedEventArgs>(Inventory_InventoryObjectAdded);
            Inventory.InventoryObjectUpdated -= new EventHandler<InventoryObjectUpdatedEventArgs>(Inventory_InventoryObjectUpdated);
            Inventory.InventoryObjectRemoved -= new EventHandler<InventoryObjectRemovedEventArgs>(Inventory_InventoryObjectRemoved);

            client.Objects.ObjectUpdate -= new EventHandler<PrimEventArgs>(Objects_AttachmentUpdate);
            client.Objects.KillObject -= new EventHandler<KillObjectEventArgs>(Objects_KillObject);
            client.Appearance.AppearanceSet -= new EventHandler<AppearanceSetEventArgs>(Appearance_AppearanceSet);
        }
        #endregion

        #region Network callbacks
        void Appearance_AppearanceSet(object sender, AppearanceSetEventArgs e)
        {
            UpdateWornLabels();
            if (appearnceWasBusy)
            {
                appearnceWasBusy = false;
                client.Appearance.RequestSetAppearance(true);
            }
        }

        void Objects_KillObject(object sender, KillObjectEventArgs e)
        {
            AttachmentInfo attachment = null;
            lock (attachments)
            {
                foreach (AttachmentInfo att in attachments.Values)
                {
                    if (att.Prim != null && att.Prim.LocalID == e.ObjectLocalID)
                    {
                        attachment = att;
                        break;
                    }
                }

                if (attachment != null)
                {
                    attachments.Remove(attachment.InventoryID);
                    Inventory_InventoryObjectUpdated(this, new InventoryObjectUpdatedEventArgs(attachment.Item, attachment.Item));
                }
            }
        }

        void Objects_AttachmentUpdate(object sender, PrimEventArgs e)
        {
            Primitive prim = e.Prim;

            if (client.Self.LocalID == 0 ||
                prim.ParentID != client.Self.LocalID ||
                prim.NameValues == null) return;

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
                                Inventory_InventoryObjectUpdated(this, new InventoryObjectUpdatedEventArgs(oldAttachment.Item, oldAttachment.Item));
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
                                    Inventory_InventoryObjectUpdated(this, new InventoryObjectUpdatedEventArgs(attachments[attachment.InventoryID].Item, attachments[attachment.InventoryID].Item));
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

        void Inventory_InventoryObjectAdded(object sender, InventoryObjectAddedEventArgs e)
        {
            if (TreeUpdateInProgress)
            {
                lock (ItemsToAdd)
                {
                    ItemsToAdd.Enqueue(e.Obj);
                }
            }
            else
            {
                Exec_OnInventoryObjectAdded(e.Obj);
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

        void Inventory_InventoryObjectRemoved(object sender, InventoryObjectRemovedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Inventory_InventoryObjectRemoved(sender, e)));
                return;
            }

            lock (attachments)
            {
                if (attachments.ContainsKey(e.Obj.UUID))
                {
                    attachments.Remove(e.Obj.UUID);
                }
            }

            TreeNode currentNode = findNodeForItem(e.Obj.UUID);
            if (currentNode != null)
            {
                removeNode(currentNode);
            }
        }

        void Inventory_InventoryObjectUpdated(object sender, InventoryObjectUpdatedEventArgs e)
        {
            if (TreeUpdateInProgress)
            {
                lock (ItemsToUpdate)
                {
                    if (e.NewObject is InventoryFolder)
                    {
                        TreeNode currentNode = findNodeForItem(e.NewObject.UUID);
                        if (currentNode != null && currentNode.Text == e.NewObject.Name) return;
                    }

                    if (!ItemsToUpdate.Contains(e.NewObject))
                    {
                        ItemsToUpdate.Enqueue(e.NewObject);
                    }
                }
            }
            else
            {
                Exec_OnInventoryObjectUpdated(e.OldObject, e.NewObject);
            }
        }

        void Exec_OnInventoryObjectUpdated(InventoryBase oldObject, InventoryBase newObject)
        {
            if (newObject == null) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Exec_OnInventoryObjectUpdated(oldObject, newObject)));
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
                    TreeNode movedNode = (TreeNode)currentNode.Clone();
                    movedNode.Tag = newObject;
                    parent.Nodes.Add(movedNode);
                    removeNode(currentNode);
                    cacheNode(movedNode);
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
                UUID2NodeCache[f.UUID] = dirNode;
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
            InventoryItem linkedItem = null;

            if (item.IsLink() && Inventory.Contains(item.AssetUUID) && Inventory[item.AssetUUID] is InventoryItem)
            {
                linkedItem = (InventoryItem)Inventory[item.AssetUUID];
            }
            else
            {
                linkedItem = item;
            }

            if (linkedItem is InventoryWearable)
            {
                InventoryWearable w = linkedItem as InventoryWearable;
                img = GetItemImageIndex(w.WearableType.ToString().ToLower());
            }
            else
            {
                img = GetItemImageIndex(linkedItem.AssetType.ToString().ToLower());
            }

            itemNode.ImageIndex = img;
            itemNode.SelectedImageIndex = img;
            parent.Nodes.Add(itemNode);
            lock (UUID2NodeCache)
            {
                UUID2NodeCache[item.UUID] = itemNode;
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

        void cacheNode(TreeNode node)
        {
            InventoryBase item = (InventoryBase)node.Tag;
            if (item != null)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    cacheNode(child);
                }
                lock (UUID2NodeCache)
                {
                    UUID2NodeCache[item.UUID] = node;
                }
            }
        }

        void removeNode(TreeNode node)
        {
            InventoryBase item = (InventoryBase)node.Tag;
            if (item != null)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    removeNode(child);
                }

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

                EventHandler<FolderUpdatedEventArgs> callback = delegate(object sender, FolderUpdatedEventArgs ea)
                {
                    if (f.UUID == ea.FolderID)
                    {
                        if (((InventoryFolder)Inventory.Items[ea.FolderID].Data).DescendentCount <= Inventory.Items[ea.FolderID].Nodes.Count)
                        {
                            success = true;
                            gotFolderEvent.Set();
                        }
                    }
                };

                client.Inventory.FolderUpdated += callback;
                fetchFolder(f.UUID, f.OwnerID, true);
                gotFolderEvent.WaitOne(30 * 1000, false);
                client.Inventory.FolderUpdated -= callback;

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
            TreeUpdateTimer.Start();
            TraverseNodes(Inventory.RootNode);
            TreeUpdateTimer.Stop();
            Invoke(new MethodInvoker(() => TreeUpdateTimerTick(null, null)));
            TreeUpdateInProgress = false;
            UpdateStatus("OK");
            instance.TabConsole.DisplayNotificationInChat("Inventory update completed.");

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
                            Inventory_InventoryObjectUpdated(this, new InventoryObjectUpdatedEventArgs(a.Item, a.Item));
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

            if (!instance.MonoRuntime || IsHandleCreated)
                Invoke(new MethodInvoker(() =>
                    {
                        invTree.Sort();
                    }
            ));


        }

        public void ReloadInventory()
        {
            if (TreeUpdateInProgress)
            {
                TreeUpdateTimer.Stop();
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

        private void reloadInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadInventory();
        }

        private void TreeUpdateTimerTick(Object sender, EventArgs e)
        {
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
        }

        #endregion

        private void btnProfile_Click(object sender, EventArgs e)
        {
            instance.MainForm.ShowAgentProfile(txtCreator.Text, txtCreator.AgentID);
        }

        void UpdateItemInfo(InventoryItem item)
        {
            foreach (Control c in pnlDetail.Controls)
            {
                c.Dispose();
            }
            pnlDetail.Controls.Clear();

            if (item == null)
            {
                pnlItemProperties.Visible = false;
                return;
            }

            pnlItemProperties.Visible = true;
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

            switch (item.AssetType)
            {
                case AssetType.Texture:
                    SLImageHandler image = new SLImageHandler(instance, item.AssetUUID, item.Name, IsFullPerm(item));
                    image.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(image);
                    break;

                case AssetType.Notecard:
                    Notecard note = new Notecard(instance, (InventoryNotecard)item);
                    note.Dock = DockStyle.Fill;
                    note.TabIndex = 3;
                    note.TabStop = true;
                    pnlDetail.Controls.Add(note);
                    note.rtbContent.Focus();
                    break;

                case AssetType.Landmark:
                    Landmark landmark = new Landmark(instance, (InventoryLandmark)item);
                    landmark.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(landmark);
                    break;

                case AssetType.LSLText:
                    ScriptEditor script = new ScriptEditor(instance, (InventoryLSL)item);
                    script.Dock = DockStyle.Fill;
                    script.TabIndex = 3;
                    script.TabStop = true;
                    pnlDetail.Controls.Add(script);
                    break;

                case AssetType.Gesture:
                    Guesture gesture = new Guesture(instance, (InventoryGesture)item);
                    gesture.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(gesture);
                    break;

            }

            tabsInventory.SelectedTab = tabDetail;
        }

        void invTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (invTree.SelectedNode.Tag is InventoryItem)
            {
                InventoryItem item = invTree.SelectedNode.Tag as InventoryItem;
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

            if (item.IsLink())
            {
                raw += " (link)";
                if (Inventory.Contains(item.AssetUUID) && Inventory[item.AssetUUID] is InventoryItem)
                {
                    item = (InventoryItem)Inventory[item.AssetUUID];
                }
            }

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

        bool IsFullPerm(InventoryItem item)
        {
            if (
                ((item.Permissions.OwnerMask & PermissionMask.Modify) != 0) &&
                ((item.Permissions.OwnerMask & PermissionMask.Copy) != 0) &&
                ((item.Permissions.OwnerMask & PermissionMask.Transfer) != 0)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
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
                else
                {
                    UpdateItemInfo(null);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                invTree.SelectedNode = node;
                ctxInv.Show(invTree, e.X, e.Y);
            }
        }

        private void ctxInv_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = invTree.SelectedNode;
            if (node == null)
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

                    if ((int)folder.PreferredType >= (int)AssetType.EnsembleStart &&
                        (int)folder.PreferredType <= (int)AssetType.EnsembleEnd)
                    {
                        ctxItem = new ToolStripMenuItem("Fix type", null, OnInvContextClick);
                        ctxItem.Name = "fix_type";
                        ctxInv.Items.Add(ctxItem);
                        ctxInv.Items.Add(new ToolStripSeparator());
                    }

                    ctxItem = new ToolStripMenuItem("New Folder", null, OnInvContextClick);
                    ctxItem.Name = "new_folder";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("New Note", null, OnInvContextClick);
                    ctxItem.Name = "new_notecard";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("New Script", null, OnInvContextClick);
                    ctxItem.Name = "new_script";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("Refresh", null, OnInvContextClick);
                    ctxItem.Name = "refresh";
                    ctxInv.Items.Add(ctxItem);

                    ctxInv.Items.Add(new ToolStripSeparator());

                    ctxItem = new ToolStripMenuItem("Expand", null, OnInvContextClick);
                    ctxItem.Name = "expand";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("Expand All", null, OnInvContextClick);
                    ctxItem.Name = "expand_all";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("Collapse", null, OnInvContextClick);
                    ctxItem.Name = "collapse";
                    ctxInv.Items.Add(ctxItem);

                    if (folder.PreferredType == AssetType.TrashFolder)
                    {
                        ctxItem = new ToolStripMenuItem("Empty Trash", null, OnInvContextClick);
                        ctxItem.Name = "empty_trash";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (folder.PreferredType == AssetType.LostAndFoundFolder)
                    {
                        ctxItem = new ToolStripMenuItem("Empty Lost and Found", null, OnInvContextClick);
                        ctxItem.Name = "empty_lost_found";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (folder.PreferredType == AssetType.Unknown)
                    {
                        ctxItem = new ToolStripMenuItem("Rename", null, OnInvContextClick);
                        ctxItem.Name = "rename_folder";
                        ctxInv.Items.Add(ctxItem);

                        ctxInv.Items.Add(new ToolStripSeparator());

                        ctxItem = new ToolStripMenuItem("Cut", null, OnInvContextClick);
                        ctxItem.Name = "cut_folder";
                        ctxInv.Items.Add(ctxItem);

                        ctxItem = new ToolStripMenuItem("Copy", null, OnInvContextClick);
                        ctxItem.Name = "copy_folder";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (instance.InventoryClipboard != null)
                    {
                        ctxItem = new ToolStripMenuItem("Paste", null, OnInvContextClick);
                        ctxItem.Name = "paste_folder";
                        ctxInv.Items.Add(ctxItem);

                        if (instance.InventoryClipboard.Item is InventoryItem)
                        {
                            ctxItem = new ToolStripMenuItem("Paste as Link", null, OnInvContextClick);
                            ctxItem.Name = "paste_folder_link";
                            ctxInv.Items.Add(ctxItem);
                        }
                    }

                    if (folder.PreferredType == AssetType.Unknown)
                    {
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

                    if (item.AssetType == AssetType.Texture)
                    {
                        ctxItem = new ToolStripMenuItem("View", null, OnInvContextClick);
                        ctxItem.Name = "view_image";
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

                    if (item.InventoryType == InventoryType.Animation)
                    {
                        if (!client.Self.SignaledAnimations.ContainsKey(item.AssetUUID))
                        {
                            ctxItem = new ToolStripMenuItem("Play", null, OnInvContextClick);
                            ctxItem.Name = "animation_play";
                            ctxInv.Items.Add(ctxItem);
                        }
                        else
                        {
                            ctxItem = new ToolStripMenuItem("Stop", null, OnInvContextClick);
                            ctxItem.Name = "animation_stop";
                            ctxInv.Items.Add(ctxItem);
                        }
                    }

                    if (item.InventoryType == InventoryType.Object)
                    {
                        ctxItem = new ToolStripMenuItem("Rez inworld", null, OnInvContextClick);
                        ctxItem.Name = "rez_inworld";
                        ctxInv.Items.Add(ctxItem);
                    }

                    ctxItem = new ToolStripMenuItem("Rename", null, OnInvContextClick);
                    ctxItem.Name = "rename_item";
                    ctxInv.Items.Add(ctxItem);

                    ctxInv.Items.Add(new ToolStripSeparator());

                    ctxItem = new ToolStripMenuItem("Cut", null, OnInvContextClick);
                    ctxItem.Name = "cut_item";
                    ctxInv.Items.Add(ctxItem);

                    ctxItem = new ToolStripMenuItem("Copy", null, OnInvContextClick);
                    ctxItem.Name = "copy_item";
                    ctxInv.Items.Add(ctxItem);

                    if (instance.InventoryClipboard != null)
                    {
                        ctxItem = new ToolStripMenuItem("Paste", null, OnInvContextClick);
                        ctxItem.Name = "paste_item";
                        ctxInv.Items.Add(ctxItem);

                        if (instance.InventoryClipboard.Item is InventoryItem)
                        {
                            ctxItem = new ToolStripMenuItem("Paste as Link", null, OnInvContextClick);
                            ctxItem.Name = "paste_item_link";
                            ctxInv.Items.Add(ctxItem);
                        }
                    }

                    ctxItem = new ToolStripMenuItem("Delete", null, OnInvContextClick);
                    ctxItem.Name = "delete_item";

                    if (IsAttached(item) || IsWorn(item))
                    {
                        ctxItem.Enabled = false;
                    }
                    ctxInv.Items.Add(ctxItem);

                    if (IsAttached(item) && instance.RLV.AllowDetach(attachments[item.UUID]))
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
                                string name = Utils.EnumToText(pt);

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
                                string name = Utils.EnumToText(pt).Substring(3);

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
                        foreach (TreeNode old in invTree.SelectedNode.Nodes)
                        {
                            if (old.Tag is InventoryFolder)
                            {
                                removeNode(old);
                            }
                        }
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

                    case "fix_type":
                        client.Inventory.UpdateFolderProperties(f.UUID, f.ParentUUID, f.Name, AssetType.Unknown);
                        invTree.Sort();
                        break;

                    case "new_notecard":
                        client.Inventory.RequestCreateItem(f.UUID, "New Note", "Radegast note: " + DateTime.Now.ToString(),
                            AssetType.Notecard, UUID.Zero, InventoryType.Notecard, PermissionMask.All, NotecardCreated);
                        break;

                    case "new_script":
                        client.Inventory.RequestCreateItem(f.UUID, "New script", "Radegast script: " + DateTime.Now.ToString(),
                            AssetType.LSLText, UUID.Zero, InventoryType.LSL, PermissionMask.All, ScriptCreated);
                        break;

                    case "cut_folder":
                        instance.InventoryClipboard = new InventoryClipboard(ClipboardOperation.Cut, f);
                        break;

                    case "copy_folder":
                        instance.InventoryClipboard = new InventoryClipboard(ClipboardOperation.Copy, f);
                        break;

                    case "paste_folder":
                        PerformClipboardOperation(invTree.SelectedNode.Tag as InventoryFolder);
                        break;

                    case "paste_folder_link":
                        PerformLinkOperation(invTree.SelectedNode.Tag as InventoryFolder);
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
                    case "copy_item":
                        instance.InventoryClipboard = new InventoryClipboard(ClipboardOperation.Copy, item);
                        break;

                    case "cut_item":
                        instance.InventoryClipboard = new InventoryClipboard(ClipboardOperation.Cut, item);
                        break;

                    case "paste_item":
                        PerformClipboardOperation(invTree.SelectedNode.Parent.Tag as InventoryFolder);
                        break;

                    case "paste_item_link":
                        PerformLinkOperation(invTree.SelectedNode.Parent.Tag as InventoryFolder);
                        break;

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

                    case "view_image":
                        UpdateItemInfo(item);
                        break;

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

                    case "animation_play":
                        Dictionary<UUID, bool> anim = new Dictionary<UUID, bool>();
                        anim.Add(item.AssetUUID, true);
                        client.Self.Animate(anim, true);
                        break;

                    case "animation_stop":
                        Dictionary<UUID, bool> animStop = new Dictionary<UUID, bool>();
                        animStop.Add(item.AssetUUID, false);
                        client.Self.Animate(animStop, true);
                        break;

                    case "rez_inworld":
                        Vector3 rezpos = new Vector3(2, 0, 0);
                        rezpos = client.Self.SimPosition + rezpos * client.Self.Movement.BodyRotation;
                        client.Inventory.RequestRezFromInventory(client.Network.CurrentSim, Quaternion.Identity, rezpos, item);
                        break;
                }
                #endregion
            }
        }

        void NotecardCreated(bool success, InventoryItem item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => NotecardCreated(success, item)));
                return;
            }

            if (!success)
            {
                instance.TabConsole.DisplayNotificationInChat("Creation of notecard failed");
                return;
            }

            instance.TabConsole.DisplayNotificationInChat("New notecard created, enter notecard name and press enter", ChatBufferTextStyle.Invisible);
            var node = findNodeForItem(item.ParentUUID);
            if (node != null) node.Expand();
            node = findNodeForItem(item.UUID);
            if (node != null)
            {
                invTree.SelectedNode = node;
                node.BeginEdit();
            }
        }

        void ScriptCreated(bool success, InventoryItem item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => ScriptCreated(success, item)));
                return;
            }

            if (!success)
            {
                instance.TabConsole.DisplayNotificationInChat("Creation of script failed");
                return;
            }

            instance.TabConsole.DisplayNotificationInChat("New script created, enter script name and press enter", ChatBufferTextStyle.Invisible);
            var node = findNodeForItem(item.ParentUUID);
            if (node != null) node.Expand();
            node = findNodeForItem(item.UUID);
            if (node != null)
            {
                invTree.SelectedNode = node;
                node.BeginEdit();
            }
        }

        void PerformClipboardOperation(InventoryFolder dest)
        {
            if (instance.InventoryClipboard == null) return;

            if (dest == null) return;

            if (instance.InventoryClipboard.Operation == ClipboardOperation.Cut)
            {
                if (instance.InventoryClipboard.Item is InventoryItem)
                {
                    client.Inventory.MoveItem(instance.InventoryClipboard.Item.UUID, dest.UUID, instance.InventoryClipboard.Item.Name);
                }
                else if (instance.InventoryClipboard.Item is InventoryFolder)
                {
                    if (instance.InventoryClipboard.Item.UUID != dest.UUID)
                    {
                        client.Inventory.MoveFolder(instance.InventoryClipboard.Item.UUID, dest.UUID, instance.InventoryClipboard.Item.Name);
                    }
                }

                instance.InventoryClipboard = null;
            }
            else if (instance.InventoryClipboard.Operation == ClipboardOperation.Copy)
            {
                if (instance.InventoryClipboard.Item is InventoryItem)
                {
                    client.Inventory.RequestCopyItem(instance.InventoryClipboard.Item.UUID, dest.UUID, instance.InventoryClipboard.Item.Name, instance.InventoryClipboard.Item.OwnerID, (InventoryBase target) =>
                    {
                    }
                    );
                }
                else if (instance.InventoryClipboard.Item is InventoryFolder)
                {
                    ThreadPool.QueueUserWorkItem((object state) =>
                        {
                            UUID newFolderID = client.Inventory.CreateFolder(dest.UUID, instance.InventoryClipboard.Item.Name, AssetType.Unknown);
                            Thread.Sleep(500);

                            // FIXME: for some reason copying a bunch of items in one operation does not work

                            //List<UUID> items = new List<UUID>();
                            //List<UUID> folders = new List<UUID>();
                            //List<string> names = new List<string>();
                            //UUID oldOwner = UUID.Zero;

                            foreach (InventoryBase oldItem in Inventory.GetContents((InventoryFolder)instance.InventoryClipboard.Item))
                            {
                                //folders.Add(newFolderID);
                                //names.Add(oldItem.Name);
                                //items.Add(oldItem.UUID);
                                //oldOwner = oldItem.OwnerID;
                                client.Inventory.RequestCopyItem(oldItem.UUID, newFolderID, oldItem.Name, oldItem.OwnerID, (InventoryBase target) => { });
                            }

                            //if (folders.Count > 0)
                            //{
                            //    client.Inventory.RequestCopyItems(items, folders, names, oldOwner, (InventoryBase target) => { });
                            //}
                        }
                    );
                }
            }
        }

        void PerformLinkOperation(InventoryFolder dest)
        {
            if (instance.InventoryClipboard == null) return;

            if (dest == null) return;

            client.Inventory.CreateLink(dest.UUID, instance.InventoryClipboard.Item, (bool success, InventoryItem item) => { });
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
                UpdateItemInfo(item);
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
            }
            else if (e.KeyCode == Keys.Apps && invTree.SelectedNode != null)
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

                if (destinationNode != null)
                {
                    highlightedNode = destinationNode;
                    highlightedNode.BackColor = Color.LightSlateGray;
                }
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

        private void tbtnSystemFoldersFirst_Click(object sender, EventArgs e)
        {
            sorter.SystemFoldersFirst = tbtnSystemFoldersFirst.Checked = !sorter.SystemFoldersFirst;
            instance.GlobalSettings["inv_sort_sysfirst"] = OSD.FromBoolean(sorter.SystemFoldersFirst);
            invTree.Sort();
        }

        private void tbtbSortByName_Click(object sender, EventArgs e)
        {
            if (tbtbSortByName.Checked) return;

            tbtbSortByName.Checked = true;
            tbtnSortByDate.Checked = sorter.ByDate = false;
            instance.GlobalSettings["inv_sort_bydate"] = OSD.FromBoolean(sorter.ByDate);

            invTree.Sort();
        }

        private void tbtnSortByDate_Click(object sender, EventArgs e)
        {
            if (tbtnSortByDate.Checked) return;

            tbtbSortByName.Checked = false;
            tbtnSortByDate.Checked = sorter.ByDate = true;
            instance.GlobalSettings["inv_sort_bydate"] = OSD.FromBoolean(sorter.ByDate);

            invTree.Sort();
        }

        #region Search

        public class SearchResult
        {
            public InventoryBase Inv;
            public int Level;

            public SearchResult(InventoryBase inv, int level)
            {
                this.Inv = inv;
                this.Level = level;
            }
        }

        List<SearchResult> searchRes;
        string searchString;
        Dictionary<int, ListViewItem> searchItemCache = new Dictionary<int, ListViewItem>();
        ListViewItem emptyItem = null;
        int found;

        void PerformRecursiveSearch(int level, UUID folderID)
        {
            var me = Inventory.Items[folderID].Data;
            searchRes.Add(new SearchResult(me, level));
            var sorted = Inventory.GetContents(folderID);

            sorted.Sort((InventoryBase b1, InventoryBase b2) =>
            {
                if (b1 is InventoryFolder && !(b2 is InventoryFolder))
                {
                    return -1;
                }
                else if (!(b1 is InventoryFolder) && b2 is InventoryFolder)
                {
                    return 1;
                }
                else
                {
                    return string.Compare(b1.Name, b2.Name);
                }
            });

            foreach (var item in sorted)
            {
                if (item is InventoryFolder)
                {
                    PerformRecursiveSearch(level + 1, item.UUID);
                }
                else
                {
                    var it = item as InventoryItem;
                    bool add = false;

                    if (cbSrchName.Checked && it.Name.ToLower().Contains(searchString))
                    {
                        add = true;
                    }
                    else if (cbSrchDesc.Checked && it.Description.ToLower().Contains(searchString))
                    {
                        add = true;
                    }
                    
                    if (cbSrchWorn.Checked && add &&
                            !(
                                (it.InventoryType == InventoryType.Wearable && IsWorn(it)) ||
                                ((it.InventoryType == InventoryType.Attachment || it.InventoryType == InventoryType.Object) && IsAttached(it))
                            )
                    )
                    {
                        add = false;
                    }

                    if (cbSrchRecent.Checked && add && it.CreationDate < instance.StartupTimeUTC)
                    {
                        add = false;
                    }

                    if (add)
                    {
                        found++;
                        searchRes.Add(new SearchResult(it, level + 1));
                    }
                }
            }

            if (searchRes[searchRes.Count - 1].Inv == me)
            {
                searchRes.RemoveAt(searchRes.Count - 1);
            }
        }

        public void UpdateSearch()
        {
            found = 0;

            if (instance.MonoRuntime)
            {
                lstInventorySearch.VirtualMode = false;
                lstInventorySearch.Items.Clear();
                lstInventorySearch.VirtualMode = true;
            }

            lstInventorySearch.VirtualListSize = 0;
            searchString = txtSearch.Text.Trim().ToLower();

            //if (searchString == string.Empty && rbSrchAll.Checked)
            //{
            //    lblSearchStatus.Text = "0 results";
            //    return;
            //}

            if (emptyItem == null)
            {
                emptyItem = new ListViewItem(string.Empty);
            }

            searchRes = new List<SearchResult>(Inventory.Items.Count);
            searchItemCache.Clear();
            PerformRecursiveSearch(0, Inventory.RootFolder.UUID);
            lstInventorySearch.VirtualListSize = searchRes.Count;
            lblSearchStatus.Text = string.Format("{0} results", found);
        }

        private void lstInventorySearch_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (searchItemCache.ContainsKey(e.ItemIndex))
            {
                e.Item = searchItemCache[e.ItemIndex];
            }
            else if (e.ItemIndex < searchRes.Count)
            {
                InventoryBase inv = searchRes[e.ItemIndex].Inv;
                string desc = inv.Name;
                if (inv is InventoryItem)
                {
                    desc += string.Format(" - {0}", ((InventoryItem)inv).Description);
                }
                ListViewItem item = new ListViewItem(desc);
                item.Tag = searchRes[e.ItemIndex];
                e.Item = item;
                searchItemCache[e.ItemIndex] = item;
            }
            else
            {
                e.Item = emptyItem;
            }
        }

        private void btnInvSearch_Click(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        private void cbSrchName_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbSrchName.Checked && !cbSrchDesc.Checked && !cbSrchCreator.Checked)
            {
                cbSrchName.Checked = true;
            }
            UpdateSearch();
        }

        private void cbSrchWorn_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                if (txtSearch.Text.Trim().Length > 0)
                {
                    UpdateSearch();
                }
            }
        }

        private void lstInventorySearch_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Graphics g = e.Graphics;
            e.DrawBackground();

            if (!(e.Item.Tag is SearchResult))
                return;

            if (e.Item.Selected)
            {
                g.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }

            SearchResult res = e.Item.Tag as SearchResult;
            int offset = 20 * (res.Level + 1);
            Rectangle rec = new Rectangle(e.Bounds.X + offset, e.Bounds.Y, e.Bounds.Width - offset, e.Bounds.Height);

            Image icon = null;
            int iconIx = 0;

            if (res.Inv is InventoryFolder)
            {
                iconIx = GetDirImageIndex(((InventoryFolder)res.Inv).PreferredType.ToString().ToLower());
            }
            else if (res.Inv is InventoryWearable)
            {
                iconIx = GetItemImageIndex(((InventoryWearable)res.Inv).WearableType.ToString().ToLower());
            }
            else if (res.Inv is InventoryItem)
            {
                iconIx = GetItemImageIndex(((InventoryItem)res.Inv).AssetType.ToString().ToLower());
            }

            if (iconIx < 0)
            {
                iconIx = 0;
            }

            try
            {
                icon = frmMain.ResourceImages.Images[iconIx];
                g.DrawImageUnscaled(icon, e.Bounds.X + offset - 18, e.Bounds.Y);
            }
            catch { }

            using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit))
            {
                string label = ItemLabel(res.Inv, false);
                SizeF len = e.Graphics.MeasureString(label, lstInventorySearch.Font, rec.Width, sf);

                e.Graphics.DrawString(
                    ItemLabel(res.Inv, false),
                    lstInventorySearch.Font,
                    e.Item.Selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText,
                    rec,
                    sf);

                if (res.Inv is InventoryItem)
                {
                    string desc = ((InventoryItem)res.Inv).Description.Trim();
                    if (desc != string.Empty)
                    {
                        using (Font descFont = new Font(lstInventorySearch.Font, FontStyle.Italic))
                        {
                            e.Graphics.DrawString(desc,
                                descFont,
                                e.Item.Selected ? SystemBrushes.HighlightText : SystemBrushes.GrayText,
                                rec.X + len.Width + 5,
                                rec.Y,
                                sf);
                        }
                    }
                }
            }

        }

        private void lstInventorySearch_SizeChanged(object sender, EventArgs e)
        {
            chResItemName.Width = lstInventorySearch.Width - 30;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        private void rbSrchAll_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        private void lstInventorySearch_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Apps) || (e.Control && e.KeyCode == RadegastContextMenuStrip.ContexMenuKeyCode))
            {
                lstInventorySearch_MouseClick(sender, new MouseEventArgs(MouseButtons.Right, 1, 50, 150, 0));
            }
        }

        private void lstInventorySearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (lstInventorySearch.SelectedIndices.Count != 1)
                return;

            try
            {
                SearchResult res = searchRes[lstInventorySearch.SelectedIndices[0]];
                TreeNode node = findNodeForItem(res.Inv.UUID);
                if (node == null)
                    return;
                invTree.SelectedNode = node;
                if (e.Button == MouseButtons.Right)
                {
                    ctxInv.Show(lstInventorySearch, e.X, e.Y);
                }
            }
            catch { }
        }

        private void lstInventorySearch_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstInventorySearch.SelectedIndices.Count != 1)
                return;

            try
            {
                SearchResult res = searchRes[lstInventorySearch.SelectedIndices[0]];
                TreeNode node = findNodeForItem(res.Inv.UUID);
                if (node == null)
                    return;
                invTree.SelectedNode = node;
                invTree_NodeMouseDoubleClick(null, null);
            }
            catch { }

        }
        #endregion Search
    }

    #region Sorter class
    // Create a node sorter that implements the IComparer interface.
    public class InvNodeSorter : System.Collections.IComparer
    {
        bool _sysfirst = true;
        bool _bydate = true;

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
        public bool ByDate { set { _bydate = value; } get { return _bydate; } }

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

            if (_bydate)
            {
                if (item1.CreationDate < item2.CreationDate)
                {
                    return 1;
                }
                else if (item1.CreationDate > item2.CreationDate)
                {
                    return -1;
                }
            }
            return string.Compare(item1.Name, item2.Name);
        }
    }
    #endregion

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


}
