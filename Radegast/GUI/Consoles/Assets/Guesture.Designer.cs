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
    partial class Guesture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Guesture));
            this.lnPanel = new Radegast.LineNumberPanel();
            this.rtbInfo = new Radegast.RRichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbtnPlay = new System.Windows.Forms.ToolStripButton();
            this.tbtnReupload = new System.Windows.Forms.ToolStripButton();
            this.tbtnAttach = new System.Windows.Forms.ToolStripButton();
            this.tlblStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lnPanel
            // 
            this.lnPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.lnPanel.Location = new System.Drawing.Point(0, 25);
            this.lnPanel.Name = "lnPanel";
            this.lnPanel.RTB = this.rtbInfo;
            this.lnPanel.Size = new System.Drawing.Size(30, 309);
            this.lnPanel.TabIndex = 0;
            // 
            // rtbInfo
            // 
            this.rtbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInfo.Location = new System.Drawing.Point(30, 25);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.Size = new System.Drawing.Size(353, 309);
            this.rtbInfo.TabIndex = 1;
            this.rtbInfo.Text = "";
            this.rtbInfo.WordWrap = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnPlay,
            this.tbtnReupload,
            this.tbtnAttach,
            this.tlblStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(383, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbtnPlay
            // 
            this.tbtnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnPlay.Enabled = false;
            this.tbtnPlay.Image = ((System.Drawing.Image)(resources.GetObject("tbtnPlay.Image")));
            this.tbtnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnPlay.Name = "tbtnPlay";
            this.tbtnPlay.Size = new System.Drawing.Size(33, 22);
            this.tbtnPlay.Text = "Play";
            this.tbtnPlay.ToolTipText = "Play guesture in world";
            this.tbtnPlay.Click += new System.EventHandler(this.tbtnPlay_Click);
            // 
            // tbtnReupload
            // 
            this.tbtnReupload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnReupload.Image = ((System.Drawing.Image)(resources.GetObject("tbtnReupload.Image")));
            this.tbtnReupload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnReupload.Name = "tbtnReupload";
            this.tbtnReupload.Size = new System.Drawing.Size(61, 22);
            this.tbtnReupload.Text = "Reupload";
            this.tbtnReupload.Click += new System.EventHandler(this.tbtnReupload_Click);
            // 
            // tbtnAttach
            // 
            this.tbtnAttach.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnAttach.Image = ((System.Drawing.Image)(resources.GetObject("tbtnAttach.Image")));
            this.tbtnAttach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnAttach.Name = "tbtnAttach";
            this.tbtnAttach.Size = new System.Drawing.Size(48, 22);
            this.tbtnAttach.Text = "Detach";
            this.tbtnAttach.ToolTipText = "Detach this panel to separate window";
            this.tbtnAttach.Click += new System.EventHandler(this.tbtnAttach_Click);
            // 
            // tlblStatus
            // 
            this.tlblStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tlblStatus.Name = "tlblStatus";
            this.tlblStatus.Size = new System.Drawing.Size(39, 22);
            this.tlblStatus.Text = "Status";
            this.tlblStatus.ToolTipText = "Status";
            // 
            // Guesture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtbInfo);
            this.Controls.Add(this.lnPanel);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Guesture";
            this.Size = new System.Drawing.Size(383, 334);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public LineNumberPanel lnPanel;
        public RRichTextBox rtbInfo;
        public System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripButton tbtnPlay;
        public System.Windows.Forms.ToolStripButton tbtnAttach;
        public System.Windows.Forms.ToolStripLabel tlblStatus;
        public System.Windows.Forms.ToolStripButton tbtnReupload;

    }
}
