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
// $Id: ChatConsole.Designer.cs 371 2009-10-26 10:26:04Z latifer $
//
using System.Windows.Forms;

namespace Radegast
{
    partial class VoiceConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoiceConsole));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.participants = new Radegast.ListViewNoFlicker();
            this.avatarContext = new Radegast.RadegastContextMenuStrip(this.components);
            this.ctxProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxPay = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxStartIM = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxFollow = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxTextures = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxAttach = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMaster = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxAnim = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSource = new System.Windows.Forms.ToolStripMenuItem();
            this.chkVoiceEnable = new System.Windows.Forms.CheckBox();
            this.TalkStates = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.avatarContext.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.chkVoiceEnable);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.participants);
            this.splitContainer1.Size = new System.Drawing.Size(516, 334);
            this.splitContainer1.SplitterDistance = 366;
            this.splitContainer1.TabIndex = 7;
            this.splitContainer1.TabStop = false;
            // 
            // participants
            // 
            this.participants.AllowDrop = true;
            this.participants.ContextMenuStrip = this.avatarContext;
            this.participants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.participants.FullRowSelect = true;
            this.participants.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.participants.HideSelection = false;
            this.participants.LabelWrap = false;
            this.participants.Location = new System.Drawing.Point(0, 0);
            this.participants.MultiSelect = false;
            this.participants.Name = "participants";
            this.participants.Size = new System.Drawing.Size(146, 334);
            this.participants.StateImageList = this.TalkStates;
            this.participants.TabIndex = 8;
            this.participants.UseCompatibleStateImageBehavior = false;
            this.participants.View = System.Windows.Forms.View.List;
            this.participants.SelectedIndexChanged += new System.EventHandler(this.lvwObjects_SelectedIndexChanged);
            // 
            // avatarContext
            // 
            this.avatarContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxProfile,
            this.ctxPay,
            this.ctxStartIM,
            this.ctxFollow,
            this.ctxTextures,
            this.ctxAttach,
            this.ctxMaster,
            this.ctxAnim,
            this.ctxPoint,
            this.ctxSource});
            this.avatarContext.Name = "avatarContext";
            this.avatarContext.Size = new System.Drawing.Size(68, 224);
            this.avatarContext.Opening += new System.ComponentModel.CancelEventHandler(this.avatarContext_Opening);
            // 
            // ctxProfile
            // 
            this.ctxProfile.Name = "ctxProfile";
            this.ctxProfile.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxPay
            // 
            this.ctxPay.Name = "ctxPay";
            this.ctxPay.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxStartIM
            // 
            this.ctxStartIM.Name = "ctxStartIM";
            this.ctxStartIM.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxFollow
            // 
            this.ctxFollow.Name = "ctxFollow";
            this.ctxFollow.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxTextures
            // 
            this.ctxTextures.Name = "ctxTextures";
            this.ctxTextures.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxAttach
            // 
            this.ctxAttach.Name = "ctxAttach";
            this.ctxAttach.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxMaster
            // 
            this.ctxMaster.Name = "ctxMaster";
            this.ctxMaster.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxAnim
            // 
            this.ctxAnim.Name = "ctxAnim";
            this.ctxAnim.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxPoint
            // 
            this.ctxPoint.Name = "ctxPoint";
            this.ctxPoint.Size = new System.Drawing.Size(67, 22);
            // 
            // ctxSource
            // 
            this.ctxSource.Name = "ctxSource";
            this.ctxSource.Size = new System.Drawing.Size(67, 22);
            // 
            // chkVoiceEnable
            // 
            this.chkVoiceEnable.AutoSize = true;
            this.chkVoiceEnable.Location = new System.Drawing.Point(13, 12);
            this.chkVoiceEnable.Name = "chkVoiceEnable";
            this.chkVoiceEnable.Size = new System.Drawing.Size(86, 17);
            this.chkVoiceEnable.TabIndex = 0;
            this.chkVoiceEnable.Text = "Enable voice";
            this.chkVoiceEnable.UseVisualStyleBackColor = true;
            // 
            // TalkStates
            // 
            this.TalkStates.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TalkStates.ImageStream")));
            this.TalkStates.TransparentColor = System.Drawing.Color.Transparent;
            this.TalkStates.Images.SetKeyName(0, "TalkIdle.png");
            this.TalkStates.Images.SetKeyName(1, "Talking.png");
            this.TalkStates.Images.SetKeyName(2, "TalkMute.png");
            // 
            // VoiceConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "VoiceConsole";
            this.Size = new System.Drawing.Size(516, 334);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.avatarContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public ListViewNoFlicker participants;
        public SplitContainer splitContainer1;
        public RadegastContextMenuStrip avatarContext;
        public ToolStripMenuItem ctxProfile;
        public ToolStripMenuItem ctxStartIM;
        public ToolStripMenuItem ctxFollow;
        public ToolStripMenuItem ctxTextures;
        public ToolStripMenuItem ctxAttach;
        public ToolStripMenuItem ctxMaster;
        public ToolStripMenuItem ctxAnim;
        public ToolStripMenuItem ctxPoint;
        public ToolStripMenuItem ctxSource;
        public ToolStripMenuItem ctxPay;
        private CheckBox chkVoiceEnable;
        private ImageList TalkStates;
    }
}
