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
namespace Radegast
{
    partial class Notecard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Notecard));
            this.rtbContent = new Radegast.RRichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbtnFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.tbtnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnSaveToDisk = new System.Windows.Forms.ToolStripMenuItem();
            this.sprtExit = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnAttachments = new System.Windows.Forms.ToolStripDropDownButton();
            this.tbtnAttach = new System.Windows.Forms.ToolStripButton();
            this.tlblStatus = new System.Windows.Forms.ToolStripLabel();
            this.btnKeep = new System.Windows.Forms.Button();
            this.btnDiscard = new System.Windows.Forms.Button();
            this.pnlKeepDiscard = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            this.pnlKeepDiscard.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbContent
            // 
            this.rtbContent.AcceptsTab = true;
            this.rtbContent.DetectUrls = false;
            this.rtbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbContent.HideSelection = false;
            this.rtbContent.Location = new System.Drawing.Point(0, 25);
            this.rtbContent.Name = "rtbContent";
            this.rtbContent.Size = new System.Drawing.Size(382, 309);
            this.rtbContent.TabIndex = 3;
            this.rtbContent.Text = "";
            this.rtbContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbContent_KeyDown);
            this.rtbContent.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbContent_LinkClicked);
            this.rtbContent.Enter += new System.EventHandler(this.rtbContent_Enter);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnFile,
            this.tbtnAttachments,
            this.tbtnAttach,
            this.tlblStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(382, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.TabStop = true;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbtnFile
            // 
            this.tbtnFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnSave,
            this.tbtnSaveToDisk,
            this.sprtExit,
            this.tbtnExit});
            this.tbtnFile.Image = ((System.Drawing.Image)(resources.GetObject("tbtnFile.Image")));
            this.tbtnFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnFile.Name = "tbtnFile";
            this.tbtnFile.Size = new System.Drawing.Size(69, 22);
            this.tbtnFile.Text = "Notecard";
            // 
            // tbtnSave
            // 
            this.tbtnSave.Name = "tbtnSave";
            this.tbtnSave.ShortcutKeyDisplayString = "Ctrl-S";
            this.tbtnSave.Size = new System.Drawing.Size(212, 22);
            this.tbtnSave.Text = "Save";
            this.tbtnSave.Click += new System.EventHandler(this.tbtnSave_Click);
            // 
            // tbtnSaveToDisk
            // 
            this.tbtnSaveToDisk.Name = "tbtnSaveToDisk";
            this.tbtnSaveToDisk.ShortcutKeyDisplayString = "Ctrl-Shift-S";
            this.tbtnSaveToDisk.Size = new System.Drawing.Size(212, 22);
            this.tbtnSaveToDisk.Text = "Save to Disk...";
            // 
            // sprtExit
            // 
            this.sprtExit.Name = "sprtExit";
            this.sprtExit.Size = new System.Drawing.Size(209, 6);
            // 
            // tbtnExit
            // 
            this.tbtnExit.Enabled = false;
            this.tbtnExit.Name = "tbtnExit";
            this.tbtnExit.Size = new System.Drawing.Size(212, 22);
            this.tbtnExit.Text = "E&xit";
            this.tbtnExit.Click += new System.EventHandler(this.tbtnExit_Click);
            // 
            // tbtnAttachments
            // 
            this.tbtnAttachments.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnAttachments.Enabled = false;
            this.tbtnAttachments.Image = ((System.Drawing.Image)(resources.GetObject("tbtnAttachments.Image")));
            this.tbtnAttachments.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnAttachments.Name = "tbtnAttachments";
            this.tbtnAttachments.Size = new System.Drawing.Size(88, 22);
            this.tbtnAttachments.Text = "Attachments";
            this.tbtnAttachments.Visible = false;
            // 
            // tbtnAttach
            // 
            this.tbtnAttach.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnAttach.Image = ((System.Drawing.Image)(resources.GetObject("tbtnAttach.Image")));
            this.tbtnAttach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnAttach.Name = "tbtnAttach";
            this.tbtnAttach.Size = new System.Drawing.Size(48, 22);
            this.tbtnAttach.Text = "Detach";
            this.tbtnAttach.Click += new System.EventHandler(this.tbtnAttach_Click);
            // 
            // tlblStatus
            // 
            this.tlblStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tlblStatus.Name = "tlblStatus";
            this.tlblStatus.Size = new System.Drawing.Size(38, 22);
            this.tlblStatus.Text = "status";
            // 
            // btnKeep
            // 
            this.btnKeep.Location = new System.Drawing.Point(3, 3);
            this.btnKeep.Name = "btnKeep";
            this.btnKeep.Size = new System.Drawing.Size(75, 23);
            this.btnKeep.TabIndex = 4;
            this.btnKeep.Text = "Keep";
            this.btnKeep.UseVisualStyleBackColor = true;
            this.btnKeep.Click += new System.EventHandler(this.btnKeep_Click);
            // 
            // btnDiscard
            // 
            this.btnDiscard.Location = new System.Drawing.Point(84, 3);
            this.btnDiscard.Name = "btnDiscard";
            this.btnDiscard.Size = new System.Drawing.Size(75, 23);
            this.btnDiscard.TabIndex = 5;
            this.btnDiscard.Text = "Discard";
            this.btnDiscard.UseVisualStyleBackColor = true;
            this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
            // 
            // pnlKeepDiscard
            // 
            this.pnlKeepDiscard.Controls.Add(this.btnKeep);
            this.pnlKeepDiscard.Controls.Add(this.btnDiscard);
            this.pnlKeepDiscard.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlKeepDiscard.Location = new System.Drawing.Point(0, 334);
            this.pnlKeepDiscard.Name = "pnlKeepDiscard";
            this.pnlKeepDiscard.Size = new System.Drawing.Size(382, 30);
            this.pnlKeepDiscard.TabIndex = 6;
            this.pnlKeepDiscard.Visible = false;
            // 
            // Notecard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtbContent);
            this.Controls.Add(this.pnlKeepDiscard);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Notecard";
            this.Size = new System.Drawing.Size(382, 364);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlKeepDiscard.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public RRichTextBox rtbContent;
        public System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripDropDownButton tbtnFile;
        public System.Windows.Forms.ToolStripMenuItem tbtnSave;
        public System.Windows.Forms.ToolStripMenuItem tbtnSaveToDisk;
        public System.Windows.Forms.ToolStripSeparator sprtExit;
        public System.Windows.Forms.ToolStripMenuItem tbtnExit;
        public System.Windows.Forms.ToolStripDropDownButton tbtnAttachments;
        public System.Windows.Forms.ToolStripButton tbtnAttach;
        public System.Windows.Forms.ToolStripLabel tlblStatus;
        public System.Windows.Forms.Button btnKeep;
        public System.Windows.Forms.Button btnDiscard;
        public System.Windows.Forms.Panel pnlKeepDiscard;

    }
}
