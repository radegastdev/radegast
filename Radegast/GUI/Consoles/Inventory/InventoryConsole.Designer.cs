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
﻿namespace Radegast
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
            this.invTree = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlDetail = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnProfile = new System.Windows.Forms.Button();
            this.txtCreator = new System.Windows.Forms.TextBox();
            this.txtCreated = new System.Windows.Forms.TextBox();
            this.txtAssetID = new System.Windows.Forms.TextBox();
            this.lblCreated = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.lblAsset = new System.Windows.Forms.Label();
            this.lblCreator = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.ctxInv = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // invTree
            // 
            this.invTree.AllowDrop = true;
            this.invTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.invTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.invTree.ForeColor = System.Drawing.Color.White;
            this.invTree.LabelEdit = true;
            this.invTree.LineColor = System.Drawing.Color.White;
            this.invTree.Location = new System.Drawing.Point(0, 0);
            this.invTree.Name = "invTree";
            this.invTree.ShowRootLines = false;
            this.invTree.Size = new System.Drawing.Size(331, 483);
            this.invTree.TabIndex = 0;
            this.invTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.invTree_AfterLabelEdit);
            this.invTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.invTree_DragDrop);
            this.invTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.invTree_DragEnter);
            this.invTree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.invTree_KeyUp);
            this.invTree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.invTree_BeforeLabelEdit);
            this.invTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.invTree_ItemDrag);
            this.invTree.DragOver += new System.Windows.Forms.DragEventHandler(this.invTree_DragOver);
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
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlDetail);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(756, 483);
            this.splitContainer1.SplitterDistance = 331;
            this.splitContainer1.TabIndex = 1;
            // 
            // pnlDetail
            // 
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(421, 336);
            this.pnlDetail.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnProfile);
            this.panel1.Controls.Add(this.txtCreator);
            this.panel1.Controls.Add(this.txtCreated);
            this.panel1.Controls.Add(this.txtAssetID);
            this.panel1.Controls.Add(this.lblCreated);
            this.panel1.Controls.Add(this.txtItemName);
            this.panel1.Controls.Add(this.lblAsset);
            this.panel1.Controls.Add(this.lblCreator);
            this.panel1.Controls.Add(this.lblItemName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 336);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(421, 147);
            this.panel1.TabIndex = 0;
            // 
            // btnProfile
            // 
            this.btnProfile.AccessibleDescription = "Open profile";
            this.btnProfile.Enabled = false;
            this.btnProfile.Image = global::Radegast.Properties.Resources.applications_16;
            this.btnProfile.Location = new System.Drawing.Point(54, 36);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(26, 23);
            this.btnProfile.TabIndex = 2;
            this.btnProfile.UseVisualStyleBackColor = true;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // txtCreator
            // 
            this.txtCreator.BackColor = System.Drawing.SystemColors.Window;
            this.txtCreator.Location = new System.Drawing.Point(80, 36);
            this.txtCreator.Name = "txtCreator";
            this.txtCreator.ReadOnly = true;
            this.txtCreator.Size = new System.Drawing.Size(338, 20);
            this.txtCreator.TabIndex = 1;
            // 
            // txtCreated
            // 
            this.txtCreated.Location = new System.Drawing.Point(80, 62);
            this.txtCreated.Name = "txtCreated";
            this.txtCreated.ReadOnly = true;
            this.txtCreated.Size = new System.Drawing.Size(144, 20);
            this.txtCreated.TabIndex = 1;
            // 
            // txtAssetID
            // 
            this.txtAssetID.Location = new System.Drawing.Point(80, 88);
            this.txtAssetID.Name = "txtAssetID";
            this.txtAssetID.ReadOnly = true;
            this.txtAssetID.Size = new System.Drawing.Size(338, 20);
            this.txtAssetID.TabIndex = 1;
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
            this.txtItemName.TabIndex = 1;
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
            // ctxInv
            // 
            this.ctxInv.Name = "folderContext";
            this.ctxInv.ShowImageMargin = false;
            this.ctxInv.Size = new System.Drawing.Size(36, 4);
            this.ctxInv.Text = "Inventory Folder";
            // 
            // InventoryConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "InventoryConsole";
            this.Size = new System.Drawing.Size(756, 483);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView invTree;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip ctxInv;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label lblCreator;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.TextBox txtCreator;
        private System.Windows.Forms.TextBox txtAssetID;
        private System.Windows.Forms.Label lblAsset;
        private System.Windows.Forms.Panel pnlDetail;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.TextBox txtCreated;
        private System.Windows.Forms.Label lblCreated;
    }
}
