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
using System.ComponentModel;

namespace Radegast
{
    partial class InventoryConsole
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (InventoryUpdate != null)
                {
                    InventoryUpdate.Abort();
                    InventoryUpdate = null;
                }

                if (_EditTimer != null)
                {
                    _EditTimer.Dispose();
                    _EditTimer = null;
                }

                if (TreeUpdateTimer != null)
                {
                    TreeUpdateTimer.Dispose();
                    TreeUpdateTimer = null;
                }

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InventoryConsole));
            this.invTree = new System.Windows.Forms.TreeView();
            this.ctxInv = new Radegast.RadegastContextMenuStrip(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tstripInventory = new System.Windows.Forms.ToolStrip();
            this.tlabelStatus = new System.Windows.Forms.ToolStripLabel();
            this.tbtnFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.reloadInventoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtbSort = new System.Windows.Forms.ToolStripDropDownButton();
            this.tbtbSortByName = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnSortByDate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtbFoldersByName = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnSystemFoldersFirst = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlDetail = new System.Windows.Forms.Panel();
            this.pnlItemProperties = new System.Windows.Forms.Panel();
            this.btnProfile = new System.Windows.Forms.Button();
            this.txtCreator = new Radegast.AgentNameTextBox();
            this.txtCreated = new System.Windows.Forms.TextBox();
            this.txtAssetID = new System.Windows.Forms.TextBox();
            this.lblCreated = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.lblAsset = new System.Windows.Forms.Label();
            this.lblCreator = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tstripInventory.SuspendLayout();
            this.pnlItemProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // invTree
            // 
            this.invTree.AllowDrop = true;
            this.invTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.invTree.ContextMenuStrip = this.ctxInv;
            this.invTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.invTree.ForeColor = System.Drawing.Color.White;
            this.invTree.LabelEdit = true;
            this.invTree.LineColor = System.Drawing.Color.White;
            this.invTree.Location = new System.Drawing.Point(0, 25);
            this.invTree.Name = "invTree";
            this.invTree.Size = new System.Drawing.Size(331, 458);
            this.invTree.TabIndex = 0;
            this.invTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.invTree_AfterLabelEdit);
            this.invTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.invTree_DragDrop);
            this.invTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.invTree_DragEnter);
            this.invTree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.invTree_KeyUp);
            this.invTree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.invTree_BeforeLabelEdit);
            this.invTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.invTree_ItemDrag);
            this.invTree.DragOver += new System.Windows.Forms.DragEventHandler(this.invTree_DragOver);
            // 
            // ctxInv
            // 
            this.ctxInv.Name = "folderContext";
            this.ctxInv.ShowImageMargin = false;
            this.ctxInv.Size = new System.Drawing.Size(36, 4);
            this.ctxInv.Text = "Inventory Folder";
            this.ctxInv.Opening += new System.ComponentModel.CancelEventHandler(this.ctxInv_Opening);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.invTree);
            this.splitContainer1.Panel1.Controls.Add(this.tstripInventory);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlDetail);
            this.splitContainer1.Panel2.Controls.Add(this.pnlItemProperties);
            this.splitContainer1.Size = new System.Drawing.Size(756, 483);
            this.splitContainer1.SplitterDistance = 331;
            this.splitContainer1.TabIndex = 1;
            // 
            // tstripInventory
            // 
            this.tstripInventory.GripMargin = new System.Windows.Forms.Padding(0);
            this.tstripInventory.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tstripInventory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlabelStatus,
            this.tbtnFile,
            this.tbtbSort});
            this.tstripInventory.Location = new System.Drawing.Point(0, 0);
            this.tstripInventory.Name = "tstripInventory";
            this.tstripInventory.Size = new System.Drawing.Size(331, 25);
            this.tstripInventory.TabIndex = 1;
            this.tstripInventory.TabStop = true;
            this.tstripInventory.Text = "toolStrip1";
            // 
            // tlabelStatus
            // 
            this.tlabelStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tlabelStatus.Name = "tlabelStatus";
            this.tlabelStatus.Size = new System.Drawing.Size(59, 22);
            this.tlabelStatus.Text = "Loading...";
            this.tlabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbtnFile
            // 
            this.tbtnFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadInventoryToolStripMenuItem,
            this.saveAllTToolStripMenuItem});
            this.tbtnFile.Image = ((System.Drawing.Image)(resources.GetObject("tbtnFile.Image")));
            this.tbtnFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnFile.Name = "tbtnFile";
            this.tbtnFile.Size = new System.Drawing.Size(70, 22);
            this.tbtnFile.Text = "&Inventory";
            // 
            // reloadInventoryToolStripMenuItem
            // 
            this.reloadInventoryToolStripMenuItem.Name = "reloadInventoryToolStripMenuItem";
            this.reloadInventoryToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.reloadInventoryToolStripMenuItem.Text = "&Reload Inventory";
            this.reloadInventoryToolStripMenuItem.ToolTipText = "Clears inventory cache, and downloads whole inventory from server again";
            this.reloadInventoryToolStripMenuItem.Click += new System.EventHandler(this.reloadInventoryToolStripMenuItem_Click);
            // 
            // saveAllTToolStripMenuItem
            // 
            this.saveAllTToolStripMenuItem.Enabled = false;
            this.saveAllTToolStripMenuItem.Name = "saveAllTToolStripMenuItem";
            this.saveAllTToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.saveAllTToolStripMenuItem.Text = "&Save all text";
            this.saveAllTToolStripMenuItem.ToolTipText = "Saves all notecards and scripts to folder on local disk";
            this.saveAllTToolStripMenuItem.Click += new System.EventHandler(this.saveAllTToolStripMenuItem_Click);
            // 
            // tbtbSort
            // 
            this.tbtbSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtbSort.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtbSortByName,
            this.tbtnSortByDate,
            this.toolStripMenuItem1,
            this.tbtbFoldersByName,
            this.tbtnSystemFoldersFirst});
            this.tbtbSort.Image = ((System.Drawing.Image)(resources.GetObject("tbtbSort.Image")));
            this.tbtbSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtbSort.Name = "tbtbSort";
            this.tbtbSort.Size = new System.Drawing.Size(41, 22);
            this.tbtbSort.Text = "S&ort";
            // 
            // tbtbSortByName
            // 
            this.tbtbSortByName.Name = "tbtbSortByName";
            this.tbtbSortByName.Size = new System.Drawing.Size(203, 22);
            this.tbtbSortByName.Text = "By Name";
            this.tbtbSortByName.Click += new System.EventHandler(this.tbtbSortByName_Click);
            // 
            // tbtnSortByDate
            // 
            this.tbtnSortByDate.Name = "tbtnSortByDate";
            this.tbtnSortByDate.Size = new System.Drawing.Size(203, 22);
            this.tbtnSortByDate.Text = "By Date";
            this.tbtnSortByDate.Click += new System.EventHandler(this.tbtnSortByDate_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(200, 6);
            // 
            // tbtbFoldersByName
            // 
            this.tbtbFoldersByName.Checked = true;
            this.tbtbFoldersByName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tbtbFoldersByName.Enabled = false;
            this.tbtbFoldersByName.Name = "tbtbFoldersByName";
            this.tbtbFoldersByName.Size = new System.Drawing.Size(203, 22);
            this.tbtbFoldersByName.Text = "Folders Always By Name";
            this.tbtbFoldersByName.Visible = false;
            // 
            // tbtnSystemFoldersFirst
            // 
            this.tbtnSystemFoldersFirst.Name = "tbtnSystemFoldersFirst";
            this.tbtnSystemFoldersFirst.Size = new System.Drawing.Size(203, 22);
            this.tbtnSystemFoldersFirst.Text = "System Folders On Top";
            this.tbtnSystemFoldersFirst.Click += new System.EventHandler(this.tbtnSystemFoldersFirst_Click);
            // 
            // pnlDetail
            // 
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(421, 336);
            this.pnlDetail.TabIndex = 2;
            // 
            // pnlItemProperties
            // 
            this.pnlItemProperties.Controls.Add(this.btnProfile);
            this.pnlItemProperties.Controls.Add(this.txtCreator);
            this.pnlItemProperties.Controls.Add(this.txtCreated);
            this.pnlItemProperties.Controls.Add(this.txtAssetID);
            this.pnlItemProperties.Controls.Add(this.lblCreated);
            this.pnlItemProperties.Controls.Add(this.txtItemName);
            this.pnlItemProperties.Controls.Add(this.lblAsset);
            this.pnlItemProperties.Controls.Add(this.lblCreator);
            this.pnlItemProperties.Controls.Add(this.lblItemName);
            this.pnlItemProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlItemProperties.Location = new System.Drawing.Point(0, 336);
            this.pnlItemProperties.Name = "pnlItemProperties";
            this.pnlItemProperties.Size = new System.Drawing.Size(421, 147);
            this.pnlItemProperties.TabIndex = 0;
            // 
            // btnProfile
            // 
            this.btnProfile.AccessibleDescription = "Open profile";
            this.btnProfile.Enabled = false;
            this.btnProfile.Image = global::Radegast.Properties.Resources.applications_16;
            this.btnProfile.Location = new System.Drawing.Point(54, 36);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(26, 23);
            this.btnProfile.TabIndex = 12;
            this.btnProfile.UseVisualStyleBackColor = true;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // txtCreator
            // 
            this.txtCreator.AgentID = ((OpenMetaverse.UUID)(resources.GetObject("txtCreator.AgentID")));
            this.txtCreator.BackColor = System.Drawing.SystemColors.Window;
            this.txtCreator.Location = new System.Drawing.Point(80, 36);
            this.txtCreator.Name = "txtCreator";
            this.txtCreator.ReadOnly = true;
            this.txtCreator.Size = new System.Drawing.Size(338, 20);
            this.txtCreator.TabIndex = 11;
            // 
            // txtCreated
            // 
            this.txtCreated.Location = new System.Drawing.Point(80, 62);
            this.txtCreated.Name = "txtCreated";
            this.txtCreated.ReadOnly = true;
            this.txtCreated.Size = new System.Drawing.Size(144, 20);
            this.txtCreated.TabIndex = 13;
            // 
            // txtAssetID
            // 
            this.txtAssetID.Location = new System.Drawing.Point(80, 88);
            this.txtAssetID.Name = "txtAssetID";
            this.txtAssetID.ReadOnly = true;
            this.txtAssetID.Size = new System.Drawing.Size(338, 20);
            this.txtAssetID.TabIndex = 14;
            // 
            // lblCreated
            // 
            this.lblCreated.AutoSize = true;
            this.lblCreated.Location = new System.Drawing.Point(3, 62);
            this.lblCreated.Name = "lblCreated";
            this.lblCreated.Size = new System.Drawing.Size(44, 13);
            this.lblCreated.TabIndex = 0;
            this.lblCreated.Text = "Created";
            // 
            // txtItemName
            // 
            this.txtItemName.Location = new System.Drawing.Point(80, 10);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(338, 20);
            this.txtItemName.TabIndex = 10;
            // 
            // lblAsset
            // 
            this.lblAsset.AutoSize = true;
            this.lblAsset.Location = new System.Drawing.Point(3, 88);
            this.lblAsset.Name = "lblAsset";
            this.lblAsset.Size = new System.Drawing.Size(47, 13);
            this.lblAsset.TabIndex = 0;
            this.lblAsset.Text = "Asset ID";
            // 
            // lblCreator
            // 
            this.lblCreator.AutoSize = true;
            this.lblCreator.Location = new System.Drawing.Point(3, 36);
            this.lblCreator.Name = "lblCreator";
            this.lblCreator.Size = new System.Drawing.Size(41, 13);
            this.lblCreator.TabIndex = 0;
            this.lblCreator.Text = "Creator";
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Location = new System.Drawing.Point(3, 10);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(27, 13);
            this.lblItemName.TabIndex = 0;
            this.lblItemName.Text = "Item";
            // 
            // InventoryConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "InventoryConsole";
            this.Size = new System.Drawing.Size(756, 483);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tstripInventory.ResumeLayout(false);
            this.tstripInventory.PerformLayout();
            this.pnlItemProperties.ResumeLayout(false);
            this.pnlItemProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView invTree;
        public System.Windows.Forms.SplitContainer splitContainer1;
        public RadegastContextMenuStrip ctxInv;
        public System.Windows.Forms.Panel pnlItemProperties;
        public System.Windows.Forms.TextBox txtItemName;
        public System.Windows.Forms.Label lblCreator;
        public System.Windows.Forms.Label lblItemName;
        public AgentNameTextBox txtCreator;
        public System.Windows.Forms.TextBox txtAssetID;
        public System.Windows.Forms.Label lblAsset;
        public System.Windows.Forms.Panel pnlDetail;
        public System.Windows.Forms.Button btnProfile;
        public System.Windows.Forms.TextBox txtCreated;
        public System.Windows.Forms.Label lblCreated;
        public System.Windows.Forms.ToolStrip tstripInventory;
        public System.Windows.Forms.ToolStripLabel tlabelStatus;
        public System.Windows.Forms.ToolStripDropDownButton tbtnFile;
        public System.Windows.Forms.ToolStripMenuItem saveAllTToolStripMenuItem;
        public System.Windows.Forms.ToolStripDropDownButton tbtbSort;
        public System.Windows.Forms.ToolStripMenuItem tbtbSortByName;
        public System.Windows.Forms.ToolStripMenuItem tbtnSortByDate;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        public System.Windows.Forms.ToolStripMenuItem tbtbFoldersByName;
        public System.Windows.Forms.ToolStripMenuItem tbtnSystemFoldersFirst;
        public System.Windows.Forms.ToolStripMenuItem reloadInventoryToolStripMenuItem;
    }
}
