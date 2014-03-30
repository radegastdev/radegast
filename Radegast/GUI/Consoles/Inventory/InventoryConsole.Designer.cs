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
                if (_EditTimer != null)
                {
                    _EditTimer.Dispose();
                    _EditTimer = null;
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.invTree = new System.Windows.Forms.TreeView();
            this.ctxInv = new Radegast.RadegastContextMenuStrip(this.components);
            this.tstripInventory = new System.Windows.Forms.ToolStrip();
            this.tlabelStatus = new System.Windows.Forms.ToolStripLabel();
            this.tbtnFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.saveAllTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtbSort = new System.Windows.Forms.ToolStripDropDownButton();
            this.tbtbSortByName = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnSortByDate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtbFoldersByName = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnSystemFoldersFirst = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlInvDetail = new System.Windows.Forms.Panel();
            this.pnlDetail = new System.Windows.Forms.Panel();
            this.pnlItemProperties = new System.Windows.Forms.Panel();
            this.txtInvID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAssetID = new System.Windows.Forms.TextBox();
            this.gbxPerms = new System.Windows.Forms.GroupBox();
            this.cbNextOwnTransfer = new System.Windows.Forms.CheckBox();
            this.cbNextOwnCopy = new System.Windows.Forms.CheckBox();
            this.cbOwnerTransfer = new System.Windows.Forms.CheckBox();
            this.cbNextOwnModify = new System.Windows.Forms.CheckBox();
            this.cbOwnerCopy = new System.Windows.Forms.CheckBox();
            this.cbOwnerModify = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnProfile = new System.Windows.Forms.Button();
            this.txtCreator = new Radegast.AgentNameTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtItemDescription = new System.Windows.Forms.TextBox();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCreated = new System.Windows.Forms.TextBox();
            this.lblCreated = new System.Windows.Forms.Label();
            this.lblAsset = new System.Windows.Forms.Label();
            this.lblCreator = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tstripInventory.SuspendLayout();
            this.pnlInvDetail.SuspendLayout();
            this.pnlItemProperties.SuspendLayout();
            this.gbxPerms.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.pnlInvDetail);
            this.splitContainer1.Size = new System.Drawing.Size(834, 483);
            this.splitContainer1.SplitterDistance = 364;
            this.splitContainer1.TabIndex = 1;
            // 
            // invTree
            // 
            this.invTree.AllowDrop = true;
            this.invTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.invTree.ContextMenuStrip = this.ctxInv;
            this.invTree.ForeColor = System.Drawing.Color.White;
            this.invTree.HideSelection = false;
            this.invTree.LabelEdit = true;
            this.invTree.LineColor = System.Drawing.Color.White;
            this.invTree.Location = new System.Drawing.Point(3, 166);
            this.invTree.Name = "invTree";
            this.invTree.Size = new System.Drawing.Size(204, 278);
            this.invTree.TabIndex = 0;
            this.invTree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.invTree_BeforeLabelEdit);
            this.invTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.invTree_AfterLabelEdit);
            this.invTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.invTree_ItemDrag);
            this.invTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.invTree_DragDrop);
            this.invTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.invTree_DragEnter);
            this.invTree.DragOver += new System.Windows.Forms.DragEventHandler(this.invTree_DragOver);
            this.invTree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.invTree_KeyUp);
            // 
            // ctxInv
            // 
            this.ctxInv.Name = "folderContext";
            this.ctxInv.ShowImageMargin = false;
            this.ctxInv.Size = new System.Drawing.Size(36, 4);
            this.ctxInv.Text = "Inventory Folder";
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
            this.tstripInventory.Size = new System.Drawing.Size(364, 25);
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
            this.saveAllTToolStripMenuItem,
            this.newPanelToolStripMenuItem});
            this.tbtnFile.Image = ((System.Drawing.Image)(resources.GetObject("tbtnFile.Image")));
            this.tbtnFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnFile.Name = "tbtnFile";
            this.tbtnFile.Size = new System.Drawing.Size(70, 22);
            this.tbtnFile.Text = "&Inventory";
            // 
            // saveAllTToolStripMenuItem
            // 
            this.saveAllTToolStripMenuItem.Name = "saveAllTToolStripMenuItem";
            this.saveAllTToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.saveAllTToolStripMenuItem.Text = "&Backup";
            this.saveAllTToolStripMenuItem.ToolTipText = "Saves all notecards and scripts to folder on local disk";
            this.saveAllTToolStripMenuItem.Click += new System.EventHandler(this.saveAllTToolStripMenuItem_Click);
            // 
            // newPanelToolStripMenuItem
            // 
            this.newPanelToolStripMenuItem.Name = "newPanelToolStripMenuItem";
            this.newPanelToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.newPanelToolStripMenuItem.Text = "&New Panel";
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
            // pnlInvDetail
            // 
            this.pnlInvDetail.Controls.Add(this.pnlDetail);
            this.pnlInvDetail.Controls.Add(this.pnlItemProperties);
            this.pnlInvDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInvDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlInvDetail.Name = "pnlInvDetail";
            this.pnlInvDetail.Size = new System.Drawing.Size(466, 483);
            this.pnlInvDetail.TabIndex = 0;
            // 
            // pnlDetail
            // 
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(466, 321);
            this.pnlDetail.TabIndex = 6;
            // 
            // pnlItemProperties
            // 
            this.pnlItemProperties.Controls.Add(this.txtInvID);
            this.pnlItemProperties.Controls.Add(this.label2);
            this.pnlItemProperties.Controls.Add(this.txtAssetID);
            this.pnlItemProperties.Controls.Add(this.gbxPerms);
            this.pnlItemProperties.Controls.Add(this.btnProfile);
            this.pnlItemProperties.Controls.Add(this.txtCreator);
            this.pnlItemProperties.Controls.Add(this.textBox1);
            this.pnlItemProperties.Controls.Add(this.label3);
            this.pnlItemProperties.Controls.Add(this.txtItemDescription);
            this.pnlItemProperties.Controls.Add(this.txtItemName);
            this.pnlItemProperties.Controls.Add(this.label4);
            this.pnlItemProperties.Controls.Add(this.label5);
            this.pnlItemProperties.Controls.Add(this.label6);
            this.pnlItemProperties.Controls.Add(this.label9);
            this.pnlItemProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlItemProperties.Location = new System.Drawing.Point(0, 321);
            this.pnlItemProperties.Name = "pnlItemProperties";
            this.pnlItemProperties.Size = new System.Drawing.Size(466, 162);
            this.pnlItemProperties.TabIndex = 5;
            // 
            // txtInvID
            // 
            this.txtInvID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInvID.Location = new System.Drawing.Point(258, 81);
            this.txtInvID.Name = "txtInvID";
            this.txtInvID.ReadOnly = true;
            this.txtInvID.Size = new System.Drawing.Size(199, 20);
            this.txtInvID.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Inv ID";
            // 
            // txtAssetID
            // 
            this.txtAssetID.Location = new System.Drawing.Point(51, 81);
            this.txtAssetID.Name = "txtAssetID";
            this.txtAssetID.ReadOnly = true;
            this.txtAssetID.Size = new System.Drawing.Size(156, 20);
            this.txtAssetID.TabIndex = 14;
            // 
            // gbxPerms
            // 
            this.gbxPerms.Controls.Add(this.cbNextOwnTransfer);
            this.gbxPerms.Controls.Add(this.cbNextOwnCopy);
            this.gbxPerms.Controls.Add(this.cbOwnerTransfer);
            this.gbxPerms.Controls.Add(this.cbNextOwnModify);
            this.gbxPerms.Controls.Add(this.cbOwnerCopy);
            this.gbxPerms.Controls.Add(this.cbOwnerModify);
            this.gbxPerms.Controls.Add(this.label8);
            this.gbxPerms.Controls.Add(this.label7);
            this.gbxPerms.Location = new System.Drawing.Point(6, 100);
            this.gbxPerms.Name = "gbxPerms";
            this.gbxPerms.Size = new System.Drawing.Size(267, 58);
            this.gbxPerms.TabIndex = 17;
            this.gbxPerms.TabStop = false;
            // 
            // cbNextOwnTransfer
            // 
            this.cbNextOwnTransfer.AutoSize = true;
            this.cbNextOwnTransfer.Location = new System.Drawing.Point(195, 36);
            this.cbNextOwnTransfer.Name = "cbNextOwnTransfer";
            this.cbNextOwnTransfer.Size = new System.Drawing.Size(55, 17);
            this.cbNextOwnTransfer.TabIndex = 69;
            this.cbNextOwnTransfer.Text = "Resell";
            this.cbNextOwnTransfer.UseVisualStyleBackColor = true;
            // 
            // cbNextOwnCopy
            // 
            this.cbNextOwnCopy.AutoSize = true;
            this.cbNextOwnCopy.Location = new System.Drawing.Point(140, 36);
            this.cbNextOwnCopy.Name = "cbNextOwnCopy";
            this.cbNextOwnCopy.Size = new System.Drawing.Size(50, 17);
            this.cbNextOwnCopy.TabIndex = 68;
            this.cbNextOwnCopy.Text = "Copy";
            this.cbNextOwnCopy.UseVisualStyleBackColor = true;
            // 
            // cbOwnerTransfer
            // 
            this.cbOwnerTransfer.AutoSize = true;
            this.cbOwnerTransfer.Enabled = false;
            this.cbOwnerTransfer.Location = new System.Drawing.Point(195, 15);
            this.cbOwnerTransfer.Name = "cbOwnerTransfer";
            this.cbOwnerTransfer.Size = new System.Drawing.Size(55, 17);
            this.cbOwnerTransfer.TabIndex = 66;
            this.cbOwnerTransfer.Text = "Resell";
            this.cbOwnerTransfer.UseVisualStyleBackColor = true;
            // 
            // cbNextOwnModify
            // 
            this.cbNextOwnModify.AutoSize = true;
            this.cbNextOwnModify.Location = new System.Drawing.Point(90, 36);
            this.cbNextOwnModify.Name = "cbNextOwnModify";
            this.cbNextOwnModify.Size = new System.Drawing.Size(47, 17);
            this.cbNextOwnModify.TabIndex = 67;
            this.cbNextOwnModify.Text = "Mod";
            this.cbNextOwnModify.UseVisualStyleBackColor = true;
            // 
            // cbOwnerCopy
            // 
            this.cbOwnerCopy.AutoSize = true;
            this.cbOwnerCopy.Enabled = false;
            this.cbOwnerCopy.Location = new System.Drawing.Point(140, 15);
            this.cbOwnerCopy.Name = "cbOwnerCopy";
            this.cbOwnerCopy.Size = new System.Drawing.Size(50, 17);
            this.cbOwnerCopy.TabIndex = 65;
            this.cbOwnerCopy.Text = "Copy";
            this.cbOwnerCopy.UseVisualStyleBackColor = true;
            // 
            // cbOwnerModify
            // 
            this.cbOwnerModify.AutoSize = true;
            this.cbOwnerModify.Enabled = false;
            this.cbOwnerModify.Location = new System.Drawing.Point(90, 15);
            this.cbOwnerModify.Name = "cbOwnerModify";
            this.cbOwnerModify.Size = new System.Drawing.Size(47, 17);
            this.cbOwnerModify.TabIndex = 64;
            this.cbOwnerModify.Text = "Mod";
            this.cbOwnerModify.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 62;
            this.label8.Text = "Owner perm.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 63;
            this.label7.Text = "Next own perm.";
            // 
            // btnProfile
            // 
            this.btnProfile.AccessibleDescription = "Open profile";
            this.btnProfile.Enabled = false;
            this.btnProfile.Image = global::Radegast.Properties.Resources.applications_16;
            this.btnProfile.Location = new System.Drawing.Point(48, 53);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(26, 23);
            this.btnProfile.TabIndex = 12;
            this.btnProfile.UseVisualStyleBackColor = true;
            // 
            // txtCreator
            // 
            this.txtCreator.BackColor = System.Drawing.SystemColors.Window;
            this.txtCreator.Location = new System.Drawing.Point(80, 55);
            this.txtCreator.Name = "txtCreator";
            this.txtCreator.ReadOnly = true;
            this.txtCreator.Size = new System.Drawing.Size(169, 20);
            this.txtCreator.TabIndex = 12;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(305, 55);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(152, 20);
            this.textBox1.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(255, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Created";
            // 
            // txtItemDescription
            // 
            this.txtItemDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItemDescription.Location = new System.Drawing.Point(80, 29);
            this.txtItemDescription.Name = "txtItemDescription";
            this.txtItemDescription.Size = new System.Drawing.Size(377, 20);
            this.txtItemDescription.TabIndex = 11;
            // 
            // txtItemName
            // 
            this.txtItemName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItemName.Location = new System.Drawing.Point(80, 3);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(377, 20);
            this.txtItemName.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Asset ID";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Creator";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Description";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Item";
            // 
            // txtCreated
            // 
            this.txtCreated.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCreated.Location = new System.Drawing.Point(305, 55);
            this.txtCreated.Name = "txtCreated";
            this.txtCreated.ReadOnly = true;
            this.txtCreated.Size = new System.Drawing.Size(138, 20);
            this.txtCreated.TabIndex = 13;
            // 
            // lblCreated
            // 
            this.lblCreated.AutoSize = true;
            this.lblCreated.Location = new System.Drawing.Point(255, 58);
            this.lblCreated.Name = "lblCreated";
            this.lblCreated.Size = new System.Drawing.Size(44, 13);
            this.lblCreated.TabIndex = 0;
            // 
            // lblAsset
            // 
            this.lblAsset.AutoSize = true;
            this.lblAsset.Location = new System.Drawing.Point(3, 84);
            this.lblAsset.Name = "lblAsset";
            this.lblAsset.Size = new System.Drawing.Size(47, 13);
            this.lblAsset.TabIndex = 0;
            // 
            // lblCreator
            // 
            this.lblCreator.AutoSize = true;
            this.lblCreator.Location = new System.Drawing.Point(3, 58);
            this.lblCreator.Name = "lblCreator";
            this.lblCreator.Size = new System.Drawing.Size(41, 13);
            this.lblCreator.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Location = new System.Drawing.Point(3, 6);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(27, 13);
            this.lblItemName.TabIndex = 0;
            // 
            // InventoryConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "InventoryConsole";
            this.Size = new System.Drawing.Size(834, 483);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tstripInventory.ResumeLayout(false);
            this.tstripInventory.PerformLayout();
            this.pnlInvDetail.ResumeLayout(false);
            this.pnlItemProperties.ResumeLayout(false);
            this.pnlItemProperties.PerformLayout();
            this.gbxPerms.ResumeLayout(false);
            this.gbxPerms.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView invTree;
        public System.Windows.Forms.SplitContainer splitContainer1;
        public RadegastContextMenuStrip ctxInv;
        public System.Windows.Forms.Label lblCreator;
        public System.Windows.Forms.Label lblItemName;
        public System.Windows.Forms.Label lblAsset;
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
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ToolStripMenuItem newPanelToolStripMenuItem;
        public System.Windows.Forms.Panel pnlInvDetail;
        public System.Windows.Forms.Panel pnlDetail;
        public System.Windows.Forms.Panel pnlItemProperties;
        public System.Windows.Forms.TextBox txtInvID;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtAssetID;
        public System.Windows.Forms.GroupBox gbxPerms;
        public System.Windows.Forms.CheckBox cbNextOwnTransfer;
        public System.Windows.Forms.CheckBox cbNextOwnCopy;
        public System.Windows.Forms.CheckBox cbOwnerTransfer;
        public System.Windows.Forms.CheckBox cbNextOwnModify;
        public System.Windows.Forms.CheckBox cbOwnerCopy;
        public System.Windows.Forms.CheckBox cbOwnerModify;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Button btnProfile;
        public AgentNameTextBox txtCreator;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox txtItemDescription;
        public System.Windows.Forms.TextBox txtItemName;
    }
}
