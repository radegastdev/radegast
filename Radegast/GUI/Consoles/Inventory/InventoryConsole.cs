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
// $Id$
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;

using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{

    public partial class InventoryConsole : UserControl
    {
        public List<InventoryPanel> InvPanels = new List<InventoryPanel>();

        RadegastInstance instance;
        GridClient client { get { return instance.Client; } }
        Dictionary<UUID, TreeNode> FolderNodes = new Dictionary<UUID, TreeNode>();

        private InventoryManager Manager;
        private OpenMetaverse.Inventory Inventory;
        private string newItemName = string.Empty;
        private System.Threading.Timer _EditTimer;
        private ListViewItem _EditNode;
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

            var p = new InventoryPanel(instance, this);
            p.Dock = DockStyle.Fill;
            InvPanels.Add(p);
            splitContainer1.Panel1.Controls.Remove(invTree);
            splitContainer1.Panel1.Controls.Add(p);
            splitContainer1.Panel1.Controls.SetChildIndex(p, 0);
            
            /*
            tbtnSortByDate.Checked = InvTreeView.Sorter.ByDate;
            tbtbSortByName.Checked = !InvTreeView.Sorter.ByDate;
            tbtnSystemFoldersFirst.Checked = InvTreeView.Sorter.SystemFoldersFirst;
            */

            UpdateStatus(client.Inventory.Store.Items.Count.ToString());
            saveAllTToolStripMenuItem.Enabled = false;
            invTree.AfterExpand += new TreeViewEventHandler(TreeView_AfterExpand);
            invTree.NodeMouseClick += new TreeNodeMouseClickEventHandler(invTree_MouseClick);
            invTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(invTree_NodeMouseDoubleClick);

            _EditTimer = new System.Threading.Timer(OnLabelEditTimer, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            // Callbacks
            Inventory.InventoryObjectAdded += new EventHandler<InventoryObjectAddedEventArgs>(Inventory_InventoryObjectAdded);
            Inventory.InventoryObjectUpdated += new EventHandler<InventoryObjectUpdatedEventArgs>(Inventory_InventoryObjectUpdated);
            Inventory.InventoryObjectRemoved += new EventHandler<InventoryObjectRemovedEventArgs>(Inventory_InventoryObjectRemoved);

            client.Objects.ObjectUpdate += new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
            client.Objects.KillObject += new EventHandler<KillObjectEventArgs>(Objects_KillObject);
            client.Appearance.AppearanceSet += new EventHandler<AppearanceSetEventArgs>(Appearance_AppearanceSet);
        }

        void InventoryConsole_Disposed(object sender, EventArgs e)
        {
            Inventory.InventoryObjectAdded -= new EventHandler<InventoryObjectAddedEventArgs>(Inventory_InventoryObjectAdded);
            Inventory.InventoryObjectUpdated -= new EventHandler<InventoryObjectUpdatedEventArgs>(Inventory_InventoryObjectUpdated);
            Inventory.InventoryObjectRemoved -= new EventHandler<InventoryObjectRemovedEventArgs>(Inventory_InventoryObjectRemoved);

            client.Objects.ObjectUpdate -= new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
            client.Objects.KillObject -= new EventHandler<KillObjectEventArgs>(Objects_KillObject);
            client.Appearance.AppearanceSet -= new EventHandler<AppearanceSetEventArgs>(Appearance_AppearanceSet);
        }
        #endregion

        #region Network callbacks
        void Objects_ObjectUpdate(object sender, PrimEventArgs e)
        {
        }

        void Appearance_AppearanceSet(object sender, AppearanceSetEventArgs e)
        {
        }

        int tickLastUpdate = 0;
        int updateInterval = 500;
        object lastUpdateState;
        System.Threading.Timer lastUpdte = null;

        void CheckUpdateTimer()
        {
            if (lastUpdte == null)
            {
                lastUpdte = new System.Threading.Timer(UpdateTimerTick, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
        }

        void UpdateTimerTick(object sender)
        {
            tickLastUpdate = 0;
            RefreshTrees();
            lastUpdte.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        void RefreshTrees()
        {
            CheckUpdateTimer();

            if (Environment.TickCount - tickLastUpdate < updateInterval)
            {
                lastUpdte.Change(updateInterval, System.Threading.Timeout.Infinite);
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => RefreshTrees()));
                return;
            }


            tickLastUpdate = Environment.TickCount;
            foreach (var p in InvPanels)
            {
                p.UpdateMapper();
            }
            UpdateStatus(client.Inventory.Store.Items.Count.ToString());
        }

        void Objects_KillObject(object sender, KillObjectEventArgs e)
        {
        }


        void Inventory_InventoryObjectAdded(object sender, InventoryObjectAddedEventArgs e)
        {
            RefreshTrees();
        }


        void Inventory_InventoryObjectRemoved(object sender, InventoryObjectRemovedEventArgs e)
        {
            RefreshTrees();
        }

        void Inventory_InventoryObjectUpdated(object sender, InventoryObjectUpdatedEventArgs e)
        {
        }

        #endregion

        public static int GetIconIndex(InventoryBase inv)
        {
            int iconIx = 0;

            if (inv is InventoryFolder)
            {
                iconIx = GetDirImageIndex(((InventoryFolder)inv).PreferredType.ToString().ToLower());
            }
            else if (inv is InventoryWearable)
            {
                iconIx = GetItemImageIndex(((InventoryWearable)inv).WearableType.ToString().ToLower());
            }
            else if (inv is InventoryItem)
            {
                iconIx = GetItemImageIndex(((InventoryItem)inv).AssetType.ToString().ToLower());
            }

            if (iconIx < 0)
            {
                iconIx = 0;
            }

            return iconIx;
        }

        public static int GetDirImageIndex(string t)
        {
            t = System.Text.RegularExpressions.Regex.Replace(t, @"folder$", "");
            int res = frmMain.ImageNames.IndexOf("inv_folder_" + t);
            if (res == -1)
            {
                switch (t)
                {
                    case "currentoutfit":
                    case "myoutfits":
                        return frmMain.ImageNames.IndexOf("inv_folder_outfit");
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

        private void UpdateStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { UpdateStatus(text); }));
                return;
            }

            if (!saveAllTToolStripMenuItem.Enabled && !instance.InvLoader.IsRunning())
            {
                saveAllTToolStripMenuItem.Enabled = true;
            }

            tlabelStatus.Text = text;
        }

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
            pnlItemProperties.Tag = item;

            if (item == null)
            {
                pnlItemProperties.Visible = false;
                return;
            }

            pnlItemProperties.Visible = true;
            btnProfile.Enabled = true;
            txtItemName.Text = item.Name;
            txtItemDescription.Text = item.Description;
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

            txtInvID.Text = item.UUID.ToString();

            Permissions p = item.Permissions;
            cbOwnerModify.Checked = (p.OwnerMask & PermissionMask.Modify) != 0;
            cbOwnerCopy.Checked = (p.OwnerMask & PermissionMask.Copy) != 0;
            cbOwnerTransfer.Checked = (p.OwnerMask & PermissionMask.Transfer) != 0;

            cbNextOwnModify.CheckedChanged -= cbNextOwnerUpdate_CheckedChanged;
            cbNextOwnCopy.CheckedChanged -= cbNextOwnerUpdate_CheckedChanged;
            cbNextOwnTransfer.CheckedChanged -= cbNextOwnerUpdate_CheckedChanged;

            cbNextOwnModify.Checked = (p.NextOwnerMask & PermissionMask.Modify) != 0;
            cbNextOwnCopy.Checked = (p.NextOwnerMask & PermissionMask.Copy) != 0;
            cbNextOwnTransfer.Checked = (p.NextOwnerMask & PermissionMask.Transfer) != 0;

            cbNextOwnModify.CheckedChanged += cbNextOwnerUpdate_CheckedChanged;
            cbNextOwnCopy.CheckedChanged += cbNextOwnerUpdate_CheckedChanged;
            cbNextOwnTransfer.CheckedChanged += cbNextOwnerUpdate_CheckedChanged;


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

        void cbNextOwnerUpdate_CheckedChanged(object sender, EventArgs e)
        {
            InventoryItem item = null;
            if (pnlItemProperties.Tag != null && pnlItemProperties.Tag is InventoryItem)
            {
                item = (InventoryItem)pnlItemProperties.Tag;
            }
            if (item == null) return;

            PermissionMask pm = PermissionMask.Move;
            if (cbNextOwnCopy.Checked) pm |= PermissionMask.Copy;
            if (cbNextOwnModify.Checked) pm |= PermissionMask.Modify;
            if (cbNextOwnTransfer.Checked) pm |= PermissionMask.Transfer;
            item.Permissions.NextOwnerMask = pm;

            client.Inventory.RequestUpdateItem(item);
            client.Inventory.RequestFetchInventory(item.UUID, item.OwnerID);
        }

        private void txtItemName_Leave(object sender, EventArgs e)
        {
            InventoryItem item = null;
            if (pnlItemProperties.Tag != null && pnlItemProperties.Tag is InventoryItem)
            {
                item = (InventoryItem)pnlItemProperties.Tag;
            }
            if (item == null) return;

            item.Name = txtItemName.Text;

            client.Inventory.RequestUpdateItem(item);
            client.Inventory.RequestFetchInventory(item.UUID, item.OwnerID);
        }

        private void txtItemDescription_Leave(object sender, EventArgs e)
        {
            InventoryItem item = null;
            if (pnlItemProperties.Tag != null && pnlItemProperties.Tag is InventoryItem)
            {
                item = (InventoryItem)pnlItemProperties.Tag;
            }
            if (item == null) return;

            item.Description = txtItemDescription.Text;

            client.Inventory.RequestUpdateItem(item);
            client.Inventory.RequestFetchInventory(item.UUID, item.OwnerID);
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

        public InventoryItem AttachmentAt(AttachmentPoint point)
        {
            // Prim.PrimData.AttachmentPoint
            List<Primitive> myAtt = client.Network.CurrentSim.ObjectsPrimitives.FindAll((Primitive p) => p.ParentID == client.Self.LocalID);
            foreach (Primitive prim in myAtt)
            {
                if (prim.PrimData.AttachmentPoint == point)
                {
                    UUID id = CurrentOutfitFolder.GetAttachmentItem(prim);
                    if (Inventory.Contains(id) && Inventory[id] is InventoryItem)
                    {
                        return (InventoryItem)Inventory[id];
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
                item = instance.COF.RealInventoryItem(item);
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

            if (instance.COF.IsWorn(item))
                raw += " (worn)";

            if (instance.COF.IsAttached(item))
            {
                // TODO
                //raw += " (worn on " + AttachedTo(item).ToString() + ")";
            }

            return raw;
        }

        public static bool IsFullPerm(InventoryItem item)
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
                    UpdateItemInfo(instance.COF.RealInventoryItem(node.Tag as InventoryItem));
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
            e.Cancel = false;
            TreeNode node = invTree.SelectedNode;
            if (node == null)
            {
                e.Cancel = true;
            }
            else
            {
                #region Folder context menu
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

                    ctxItem = new ToolStripMenuItem("Backup...", null, OnInvContextClick);
                    ctxItem.Name = "backup";
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

                    if (folder.PreferredType == AssetType.Unknown ||
                        folder.PreferredType == AssetType.OutfitFolder)
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

                    if (folder.PreferredType == AssetType.Unknown ||
                        folder.PreferredType == AssetType.OutfitFolder)
                    {
                        ctxItem = new ToolStripMenuItem("Delete", null, OnInvContextClick);
                        ctxItem.Name = "delete_folder";
                        ctxInv.Items.Add(ctxItem);

                        ctxInv.Items.Add(new ToolStripSeparator());
                    }

                    if (folder.PreferredType == AssetType.Unknown || folder.PreferredType == AssetType.OutfitFolder)
                    {
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
                #endregion Folder context menu
                }
                else if (node.Tag is InventoryItem)
                {
                    #region Item context menu
                    InventoryItem item = instance.COF.RealInventoryItem((InventoryItem)node.Tag);
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

                    if (instance.COF.IsAttached(item) || instance.COF.IsWorn(item))
                    {
                        ctxItem.Enabled = false;
                    }
                    ctxInv.Items.Add(ctxItem);

                    if (instance.COF.IsAttached(item) && instance.RLV.AllowDetach(item))
                    {
                        ctxItem = new ToolStripMenuItem("Detach from yourself", null, OnInvContextClick);
                        ctxItem.Name = "detach";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (!instance.COF.IsAttached(item) && (item.InventoryType == InventoryType.Object || item.InventoryType == InventoryType.Attachment))
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

                        ctxItem = new ToolStripMenuItem("Add to Worn", null, OnInvContextClick);
                        ctxItem.Name = "wear_attachment_add";
                        ctxInv.Items.Add(ctxItem);

                        ctxItem = new ToolStripMenuItem("Wear", null, OnInvContextClick);
                        ctxItem.Name = "wear_attachment";
                        ctxInv.Items.Add(ctxItem);
                    }

                    if (item is InventoryWearable)
                    {
                        ctxInv.Items.Add(new ToolStripSeparator());

                        if (instance.COF.IsWorn(item))
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
                    #endregion Item context menu
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
                    case "backup":
                        (new InventoryBackup(instance, f.UUID)).Show();
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
                        instance.COF.ReplaceOutfit(newOutfit);
                        break;

                    case "outfit_add":
                        List<InventoryItem> addToOutfit = new List<InventoryItem>();
                        foreach (InventoryBase item in Inventory.GetContents(f))
                        {
                            if (item is InventoryItem)
                                addToOutfit.Add((InventoryItem)item);
                        }
                        appearnceWasBusy = client.Appearance.ManagerBusy;
                        instance.COF.AddToOutfit(addToOutfit, true);
                        break;

                    case "outfit_take_off":
                        List<InventoryItem> removeFromOutfit = new List<InventoryItem>();
                        foreach (InventoryBase item in Inventory.GetContents(f))
                        {
                            if (item is InventoryItem)
                                removeFromOutfit.Add((InventoryItem)item);
                        }
                        appearnceWasBusy = client.Appearance.ManagerBusy;
                        instance.COF.RemoveFromOutfit(removeFromOutfit);
                        break;
                }
                #endregion
            }
            else if (invTree.SelectedNode.Tag is InventoryItem)
            {
                #region Item actions
                InventoryItem item = (InventoryItem)invTree.SelectedNode.Tag;

                // Copy, cut, and delete works on links directly
                // The rest operate on the item that is pointed by the link
                if (cmd != "copy_item" && cmd != "cut_item" && cmd != "delete_item")
                {
                    item = instance.COF.RealInventoryItem(item);
                }

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
                        instance.COF.Detach(item);
                        invTree.SelectedNode.Text = ItemLabel(item, false);
                        break;

                    case "wear_attachment":
                        instance.COF.Attach(item, AttachmentPoint.Default, true);
                        break;

                    case "wear_attachment_add":
                        instance.COF.Attach(item, AttachmentPoint.Default, false);
                        break;

                    case "attach_to":
                        AttachmentPoint pt = (AttachmentPoint)((ToolStripMenuItem)sender).Tag;
                        instance.COF.Attach(item, pt, true);
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
                        instance.COF.RemoveFromOutfit(item);
                        invTree.SelectedNode.Text = ItemLabel(item, false);
                        break;

                    case "item_wear":
                        appearnceWasBusy = client.Appearance.ManagerBusy;
                        instance.COF.AddToOutfit(item, true);
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
                        instance.MediaManager.PlayUISound(UISounds.ObjectRez);
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
            /* TODO
            var node = findNodeForItem(item.ParentUUID);
            if (node != null) node.Expand();
            node = findNodeForItem(item.UUID);
            if (node != null)
            {
                invTree.SelectedNode = node;
                node.BeginEdit();
            }
             */
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
            /* TODO
            var node = findNodeForItem(item.ParentUUID);
            if (node != null) node.Expand();
            node = findNodeForItem(item.UUID);
            if (node != null)
            {
                invTree.SelectedNode = node;
                node.BeginEdit();
            }
             */
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
                    WorkPool.QueueUserWorkItem((object state) =>
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
                (e.Node.Tag is InventoryFolder &&
                ((InventoryFolder)e.Node.Tag).PreferredType != AssetType.Unknown &&
                ((InventoryFolder)e.Node.Tag).PreferredType != AssetType.OutfitFolder)
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
                // TODO
                // _EditNode = e.Node;
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
            (new InventoryBackup(instance, Inventory.RootFolder.UUID)).Show();
        }

        void ReloadPanels()
        {
            foreach (var p in InvPanels)
            {
                p.UpdateMapper();
            }
        }

        private void tbtnSystemFoldersFirst_Click(object sender, EventArgs e)
        {
            InvTreeView.Sorter.SystemFoldersFirst = tbtnSystemFoldersFirst.Checked = !InvTreeView.Sorter.SystemFoldersFirst;
            instance.GlobalSettings["inv_sort_sysfirst"] = OSD.FromBoolean(InvTreeView.Sorter.SystemFoldersFirst);
            ReloadPanels();
        }

        private void tbtbSortByName_Click(object sender, EventArgs e)
        {
            if (tbtbSortByName.Checked) return;

            tbtbSortByName.Checked = true;
            tbtnSortByDate.Checked = InvTreeView.Sorter.ByDate = false;
            instance.GlobalSettings["inv_sort_bydate"] = OSD.FromBoolean(InvTreeView.Sorter.ByDate);
            ReloadPanels();
        }

        private void tbtnSortByDate_Click(object sender, EventArgs e)
        {
            if (tbtnSortByDate.Checked) return;

            tbtbSortByName.Checked = false;
            tbtnSortByDate.Checked = InvTreeView.Sorter.ByDate = true;
            instance.GlobalSettings["inv_sort_bydate"] = OSD.FromBoolean(InvTreeView.Sorter.ByDate);
            ReloadPanels();
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
                                (it.InventoryType == InventoryType.Wearable && instance.COF.IsWorn(it)) ||
                                ((it.InventoryType == InventoryType.Attachment || it.InventoryType == InventoryType.Object) && instance.COF.IsAttached(it))
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
            int iconIx = GetIconIndex(res.Inv);

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

        /// <summary>
        /// Finds and higlights inventory node
        /// </summary>
        /// <param name="itemID">Inventory of ID of the item to select</param>
        public void SelectInventoryNode(UUID itemID)
        {
            // TODO
        }

        private void lstInventorySearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (lstInventorySearch.SelectedIndices.Count != 1)
                return;

            try
            {
                SearchResult res = searchRes[lstInventorySearch.SelectedIndices[0]];
                /* TODO
                TreeNode node = findNodeForItem(res.Inv.UUID);
                if (node == null)
                    return;
                invTree.SelectedNode = node;
                if (e.Button == MouseButtons.Right)
                {
                    ctxInv.Show(lstInventorySearch, e.X, e.Y);
                }
                */
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
                /* TODO
                TreeNode node = findNodeForItem(res.Inv.UUID);
                if (node == null)
                    return;
                invTree.SelectedNode = node;
                invTree_NodeMouseDoubleClick(null, null);
                */
            }
            catch { }

        }
        #endregion Search

        private void txtAssetID_Enter(object sender, EventArgs e)
        {
            txtAssetID.SelectAll();
        }

        private void txtInvID_Enter(object sender, EventArgs e)
        {
            txtInvID.SelectAll();
        }

        private void copyInitialOutfitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = new FolderCopy(instance);
            c.GetFolders("Initial Outfits");
        }
    }
}
