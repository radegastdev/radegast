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
    partial class SLImageHandler
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
            if (disposing && (components != null)) {
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
            this.lblProgress = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmsImage = new Radegast.RadegastContextMenuStrip(this.components);
            this.tbtnViewFullSize = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnCopyUUID = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnClear = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tbtbInvShow = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.cmsImage.SuspendLayout();
            this.progressBar1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblProgress.AutoSize = true;
            this.lblProgress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProgress.Location = new System.Drawing.Point(132, 6);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(51, 13);
            this.lblProgress.TabIndex = 2;
            this.lblProgress.Text = "0 of 0 KB";
            // 
            // pictureBox1
            // 
            this.pictureBox1.ContextMenuStrip = this.cmsImage;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(304, 286);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragDrop);
            this.pictureBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragEnter);
            // 
            // cmsImage
            // 
            this.cmsImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnViewFullSize,
            this.tbtnCopy,
            this.tbtnCopyUUID,
            this.tbtnSave,
            this.toolStripMenuItem1,
            this.tbtnClear,
            this.tbtnPaste,
            this.tbtbInvShow});
            this.cmsImage.Name = "cmsImage";
            this.cmsImage.Size = new System.Drawing.Size(200, 186);
            this.cmsImage.Opening += new System.ComponentModel.CancelEventHandler(this.cmsImage_Opening);
            // 
            // tbtnViewFullSize
            // 
            this.tbtnViewFullSize.Name = "tbtnViewFullSize";
            this.tbtnViewFullSize.Size = new System.Drawing.Size(199, 22);
            this.tbtnViewFullSize.Text = "View full size";
            this.tbtnViewFullSize.ToolTipText = " View full size ";
            this.tbtnViewFullSize.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // tbtnCopy
            // 
            this.tbtnCopy.Name = "tbtnCopy";
            this.tbtnCopy.Size = new System.Drawing.Size(199, 22);
            this.tbtnCopy.Text = "Copy";
            this.tbtnCopy.ToolTipText = " Copy ";
            this.tbtnCopy.Click += new System.EventHandler(this.tbtnCopy_Click);
            // 
            // tbtnCopyUUID
            // 
            this.tbtnCopyUUID.Name = "tbtnCopyUUID";
            this.tbtnCopyUUID.Size = new System.Drawing.Size(199, 22);
            this.tbtnCopyUUID.Text = "Copy UUID to clipboard";
            this.tbtnCopyUUID.ToolTipText = " Copy UUID to clipboard ";
            this.tbtnCopyUUID.Click += new System.EventHandler(this.copyUUIDToClipboardToolStripMenuItem_Click);
            // 
            // tbtnSave
            // 
            this.tbtnSave.Name = "tbtnSave";
            this.tbtnSave.Size = new System.Drawing.Size(199, 22);
            this.tbtnSave.Text = "Save";
            this.tbtnSave.ToolTipText = " Save ";
            this.tbtnSave.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(196, 6);
            // 
            // tbtnClear
            // 
            this.tbtnClear.Name = "tbtnClear";
            this.tbtnClear.Size = new System.Drawing.Size(199, 22);
            this.tbtnClear.Text = "Clear";
            this.tbtnClear.ToolTipText = " Clear ";
            this.tbtnClear.Click += new System.EventHandler(this.tbtnClear_Click);
            // 
            // tbtnPaste
            // 
            this.tbtnPaste.Name = "tbtnPaste";
            this.tbtnPaste.Size = new System.Drawing.Size(199, 22);
            this.tbtnPaste.Text = "Paste from Inventory";
            this.tbtnPaste.ToolTipText = " Paste from Inventory ";
            this.tbtnPaste.Click += new System.EventHandler(this.tbtnPaste_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Controls.Add(this.lblProgress);
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 264);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(304, 22);
            this.progressBar1.Step = 100;
            this.progressBar1.TabIndex = 6;
            this.progressBar1.Visible = false;
            // 
            // tbtbInvShow
            // 
            this.tbtbInvShow.Name = "tbtbInvShow";
            this.tbtbInvShow.Size = new System.Drawing.Size(199, 22);
            this.tbtbInvShow.Text = "Show in Inventory";
            this.tbtbInvShow.ToolTipText = " Show in Inventory ";
            this.tbtbInvShow.Click += new System.EventHandler(this.tbtbInvShow_Click);
            // 
            // SLImageHandler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "SLImageHandler";
            this.Size = new System.Drawing.Size(304, 286);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.cmsImage.ResumeLayout(false);
            this.progressBar1.ResumeLayout(false);
            this.progressBar1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblProgress;
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.ProgressBar progressBar1;
        public RadegastContextMenuStrip cmsImage;
        public System.Windows.Forms.ToolStripMenuItem tbtnViewFullSize;
        public System.Windows.Forms.ToolStripMenuItem tbtnCopyUUID;
        public System.Windows.Forms.ToolStripMenuItem tbtnSave;
        public System.Windows.Forms.ToolStripMenuItem tbtnCopy;
        public System.Windows.Forms.ToolStripMenuItem tbtbInvShow;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        public System.Windows.Forms.ToolStripMenuItem tbtnClear;
        public System.Windows.Forms.ToolStripMenuItem tbtnPaste;


    }
}
