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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

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
            invTree.AfterExpand += new TreeViewEventHandler(TreeView_AfterExpand);
            invTree.MouseClick += new MouseEventHandler(invTree_MouseClick);
            invTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(invTree_NodeMouseDoubleClick);
            invTree.TreeViewNodeSorter = new InvNodeSorter();
            invRootNode = AddDir(null, Inventory.RootFolder);
            invTree.Nodes[0].Expand();

            // Callbacks
            client.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client.Inventory.Store.OnInventoryObjectAdded += new Inventory.InventoryObjectAdded(Store_OnInventoryObjectAdded);
            client.Inventory.Store.OnInventoryObjectUpdated += new Inventory.InventoryObjectUpdated(Store_OnInventoryObjectUpdated);
            client.Inventory.Store.OnInventoryObjectRemoved += new Inventory.InventoryObjectRemoved(Store_OnInventoryObjectRemoved);

        }

        void InventoryConsole_Disposed(object sender, EventArgs e)
        {
            client.Avatars.OnAvatarNames -= new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client.Inventory.Store.OnInventoryObjectAdded -= new Inventory.InventoryObjectAdded(Store_OnInventoryObjectAdded);
            client.Inventory.Store.OnInventoryObjectUpdated -= new Inventory.InventoryObjectUpdated(Store_OnInventoryObjectUpdated);
            client.Inventory.Store.OnInventoryObjectRemoved -= new Inventory.InventoryObjectRemoved(Store_OnInventoryObjectRemoved);
        }
        #endregion

        #region Network callbacks
        void Store_OnInventoryObjectAdded(InventoryBase obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Store_OnInventoryObjectAdded(obj);
                    }
                ));
                return;
            }

            Logger.DebugLog("Inv created:" + obj.Name);

            TreeNode parent = findNodeForItem(invRootNode, obj.ParentUUID);

            if (parent != null)
            {
                AddBase(parent, obj);
            }
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

            TreeNode currentNode = findNodeForItem(invRootNode, obj.UUID);
            if (currentNode != null)
            {
                invTree.Nodes.Remove(currentNode);
            }
            Logger.DebugLog("Inv removed:" + obj.Name);
        }

        void Store_OnInventoryObjectUpdated(InventoryBase oldObject, InventoryBase newObject)
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

            TreeNode currentNode = findNodeForItem(invRootNode, newObject.UUID);
            if (currentNode != null)
            {
                invTree.Nodes.Remove(currentNode);
            }

            TreeNode parent = findNodeForItem(invRootNode, newObject.ParentUUID);

            if (parent != null)
            {
                AddBase(parent, newObject);
            }

            Logger.DebugLog("Inv updated:" + newObject.Name);
        }


        void Avatars_OnAvatarNames(Dictionary<UUID, string> names)
        {
            if (txtCreator.Tag == null) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Avatars_OnAvatarNames(names);
                }));
                return;
            }

            if (names.ContainsKey((UUID)txtCreator.Tag))
            {
                txtCreator.Text = names[(UUID)txtCreator.Tag];
            }
        }
        #endregion

        private void btnProfile_Click(object sender, EventArgs e)
        {
            (new frmProfile(instance, txtCreator.Text, (UUID)txtCreator.Tag)).Show();
            
        }

        void UpdateItemInfo(InventoryItem item)
        {
            btnProfile.Enabled = true;
            txtItemName.Text = item.Name;
            txtCreator.Text = instance.getAvatarName(item.CreatorID);
            txtCreator.Tag = item.CreatorID;
            txtCreated.Text = item.CreationDate.ToString();

            if (item.AssetUUID != null && item.AssetUUID != UUID.Zero)
            {
                txtAssetID.Text = item.AssetUUID.ToString();
            }
            else
            {
                txtAssetID.Text = String.Empty;
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
                }
            }
        }

        void invTree_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode node = invTree.GetNodeAt(new Point(e.X, e.Y));
            if (node == null) return;

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
                if (node.Tag is InventoryFolder)
                {
                    InventoryFolder folder = (InventoryFolder)node.Tag;
                    ctxInv.Items.Clear();

                    ToolStripMenuItem ctxItem = new ToolStripMenuItem("Refresh", null, OnInvContextClick);
                    ctxItem.Name = "refresh";
                    ctxInv.Items.Add(ctxItem);

                    if (folder.PreferredType == AssetType.Unknown)
                    {
                        ctxItem = new ToolStripMenuItem("Delete", null, OnInvContextClick);
                        ctxItem.Name = "delete";
                        ctxInv.Items.Add(ctxItem);
                    }

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


                    ctxInv.Show(invTree, new Point(e.X, e.Y));
                }
                Logger.Log("Right click on node: " + node.Name, Helpers.LogLevel.Debug, client);
            }
        }

        #region Context menu folder
        private void OnInvContextClick(object sender, EventArgs e)
        {
            if (invTree.SelectedNode == null || !(invTree.SelectedNode.Tag is InventoryBase))
            {
                return;
            }

            if (invTree.SelectedNode.Tag is InventoryFolder)
            {
                InventoryFolder f = (InventoryFolder)invTree.SelectedNode.Tag;
                string cmd = ((ToolStripMenuItem)sender).Name;

                switch (cmd)
                {
                    case "refresh":
                        invTree.SelectedNode.Nodes.Clear();
                        client.Inventory.RequestFolderContents(f.UUID, f.OwnerID, true, true, InventorySortOrder.ByDate);
                        break;

                    case "delete":
                        client.Inventory.MoveFolder(f.UUID, client.Inventory.FindFolderForType(AssetType.TrashFolder), f.Name);
                        break;

                    case "empty_trash":
                        client.Inventory.EmptyTrash();
                        break;

                    case "empty_lost_found":
                        client.Inventory.EmptyLostAndFound();
                        break;

                }
            }

        }

        #endregion


        void TreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            InventoryFolder f = (InventoryFolder)node.Tag;
            if (f == null)
            {
                return;
            }

            TreeNode dummy = null;
            foreach (TreeNode n in node.Nodes)
            {
                if (n.Name == "DummyTreeNode")
                {
                    dummy = n;
                    break;
                }
            }

            if (dummy != null)
            {
                try
                {
                    List<InventoryBase> contents = client.Inventory.Store.GetContents(f);
                    if (contents.Count == 0)
                        throw new InventoryException("Refetch required");
                    foreach (InventoryBase item in contents)
                    {
                        UpdateBase(node, item);
                    }
                }
                catch (Exception)
                {
                    client.Inventory.RequestFolderContents(f.UUID, f.OwnerID, true, true, InventorySortOrder.ByDate);
                }
                node.Nodes.Remove(dummy);
            }
        }

        private void invTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            invTree.SelectedNode = e.Item as TreeNode;
            invTree.DoDragDrop(e.Item, DragDropEffects.Copy);
        }

     }

    // Create a node sorter that implements the IComparer interface.
    public class InvNodeSorter : IComparer
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

}
