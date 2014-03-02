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
// $Id: RadegastInstance.cs 152 2009-08-24 14:19:58Z latifer@gmail.com $
//
namespace Radegast
{
    partial class InventoryBackup
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InventoryBackup));
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripLabel();
            this.sbrProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lvwFiles = new Radegast.ListViewNoFlicker();
            this.InventoryItem = new System.Windows.Forms.ColumnHeader();
            this.clFileName = new System.Windows.Forms.ColumnHeader();
            this.Progress = new System.Windows.Forms.ColumnHeader();
            this.cbNoteCards = new System.Windows.Forms.CheckBox();
            this.cbScripts = new System.Windows.Forms.CheckBox();
            this.cbImages = new System.Windows.Forms.CheckBox();
            this.cbList = new System.Windows.Forms.CheckBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFolderName
            // 
            this.txtFolderName.Location = new System.Drawing.Point(12, 12);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.ReadOnly = true;
            this.txtFolderName.Size = new System.Drawing.Size(308, 20);
            this.txtFolderName.TabIndex = 0;
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(326, 10);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(75, 23);
            this.btnFolder.TabIndex = 1;
            this.btnFolder.Text = "Select folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.AddExtension = false;
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.FileName = "use this folder";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.sbrProgress});
            this.toolStrip1.Location = new System.Drawing.Point(0, 361);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(774, 25);
            this.toolStrip1.TabIndex = 11;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 22);
            this.lblStatus.Text = "Status";
            // 
            // sbrProgress
            // 
            this.sbrProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sbrProgress.Name = "sbrProgress";
            this.sbrProgress.Size = new System.Drawing.Size(100, 22);
            this.sbrProgress.Step = 1;
            // 
            // lvwFiles
            // 
            this.lvwFiles.AllowColumnReorder = true;
            this.lvwFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.InventoryItem,
            this.clFileName,
            this.Progress});
            this.lvwFiles.FullRowSelect = true;
            this.lvwFiles.GridLines = true;
            this.lvwFiles.HideSelection = false;
            this.lvwFiles.Location = new System.Drawing.Point(0, 39);
            this.lvwFiles.MultiSelect = false;
            this.lvwFiles.Name = "lvwFiles";
            this.lvwFiles.ShowGroups = false;
            this.lvwFiles.ShowItemToolTips = true;
            this.lvwFiles.Size = new System.Drawing.Size(774, 319);
            this.lvwFiles.TabIndex = 10;
            this.lvwFiles.UseCompatibleStateImageBehavior = false;
            this.lvwFiles.View = System.Windows.Forms.View.Details;
            this.lvwFiles.DoubleClick += new System.EventHandler(this.lvwFiles_DoubleClick);
            // 
            // InventoryItem
            // 
            this.InventoryItem.Text = "Inventory item";
            this.InventoryItem.Width = 200;
            // 
            // clFileName
            // 
            this.clFileName.Text = "File";
            this.clFileName.Width = 350;
            // 
            // Progress
            // 
            this.Progress.Text = "Progress";
            this.Progress.Width = 193;
            // 
            // cbNoteCards
            // 
            this.cbNoteCards.AutoSize = true;
            this.cbNoteCards.Checked = true;
            this.cbNoteCards.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNoteCards.Location = new System.Drawing.Point(407, 14);
            this.cbNoteCards.Name = "cbNoteCards";
            this.cbNoteCards.Size = new System.Drawing.Size(75, 17);
            this.cbNoteCards.TabIndex = 2;
            this.cbNoteCards.Text = "Notecards";
            this.cbNoteCards.UseVisualStyleBackColor = true;
            // 
            // cbScripts
            // 
            this.cbScripts.AutoSize = true;
            this.cbScripts.Checked = true;
            this.cbScripts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbScripts.Location = new System.Drawing.Point(488, 14);
            this.cbScripts.Name = "cbScripts";
            this.cbScripts.Size = new System.Drawing.Size(58, 17);
            this.cbScripts.TabIndex = 3;
            this.cbScripts.Text = "Scripts";
            this.cbScripts.UseVisualStyleBackColor = true;
            // 
            // cbImages
            // 
            this.cbImages.AutoSize = true;
            this.cbImages.Checked = true;
            this.cbImages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbImages.Location = new System.Drawing.Point(552, 14);
            this.cbImages.Name = "cbImages";
            this.cbImages.Size = new System.Drawing.Size(60, 17);
            this.cbImages.TabIndex = 4;
            this.cbImages.Text = "Images";
            this.cbImages.UseVisualStyleBackColor = true;
            // 
            // cbList
            // 
            this.cbList.AutoSize = true;
            this.cbList.Checked = true;
            this.cbList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbList.Location = new System.Drawing.Point(618, 14);
            this.cbList.Name = "cbList";
            this.cbList.Size = new System.Drawing.Size(42, 17);
            this.cbList.TabIndex = 5;
            this.cbList.Text = "List";
            this.cbList.UseVisualStyleBackColor = true;
            // 
            // InventoryBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 386);
            this.Controls.Add(this.cbList);
            this.Controls.Add(this.cbImages);
            this.Controls.Add(this.cbScripts);
            this.Controls.Add(this.cbNoteCards);
            this.Controls.Add(this.lvwFiles);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.txtFolderName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InventoryBackup";
            this.Text = "Inventory Backup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InventoryBackup_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtFolderName;
        public System.Windows.Forms.Button btnFolder;
        public System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripLabel lblStatus;
        public System.Windows.Forms.ToolStripProgressBar sbrProgress;
        public ListViewNoFlicker lvwFiles;
        public System.Windows.Forms.ColumnHeader InventoryItem;
        public System.Windows.Forms.ColumnHeader clFileName;
        public System.Windows.Forms.ColumnHeader Progress;
        private System.Windows.Forms.CheckBox cbNoteCards;
        private System.Windows.Forms.CheckBox cbScripts;
        private System.Windows.Forms.CheckBox cbImages;
        private System.Windows.Forms.CheckBox cbList;

    }
}