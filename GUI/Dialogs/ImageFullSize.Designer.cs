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
namespace Radegast
{
    partial class ImageFullSize
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tbtnCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnCopyUUID = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsImage
            // 
            this.cmsImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnCopy,
            this.tbtnCopyUUID,
            this.tbtnSave});
            this.cmsImage.Name = "cmsImage";
            this.cmsImage.Size = new System.Drawing.Size(200, 92);
            // 
            // tbtnCopy
            // 
            this.tbtnCopy.Name = "tbtnCopy";
            this.tbtnCopy.Size = new System.Drawing.Size(199, 22);
            this.tbtnCopy.Text = "Copy";
            this.tbtnCopy.Click += new System.EventHandler(this.tbtnCopy_Click);
            // 
            // tbtnCopyUUID
            // 
            this.tbtnCopyUUID.Name = "tbtnCopyUUID";
            this.tbtnCopyUUID.Size = new System.Drawing.Size(199, 22);
            this.tbtnCopyUUID.Text = "Copy UUID to clipboard";
            this.tbtnCopyUUID.Click += new System.EventHandler(this.tbtnCopyUUID_Click);
            // 
            // tbtnSave
            // 
            this.tbtnSave.Name = "tbtnSave";
            this.tbtnSave.Size = new System.Drawing.Size(199, 22);
            this.tbtnSave.Text = "Save";
            this.tbtnSave.Click += new System.EventHandler(this.tbtnSave_Click);
            // 
            // ImageFullSize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Name = "ImageFullSize";
            this.ShowIcon = false;
            this.Text = "ImageFullSize";
            this.cmsImage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmsImage;
        private System.Windows.Forms.ToolStripMenuItem tbtnCopy;
        private System.Windows.Forms.ToolStripMenuItem tbtnCopyUUID;
        private System.Windows.Forms.ToolStripMenuItem tbtnSave;

    }
}