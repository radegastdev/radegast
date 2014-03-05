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
    partial class TabsConsole
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
            //if (InvokeRequired)
            //{
            //    if (!instance.MonoRuntime || IsHandleCreated)
            //    {
            //        Invoke(new System.Windows.Forms.MethodInvoker(() => Dispose(disposing)));
            //    }
            //    return;
            //}

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                lock (tabs)
                {
                    System.Collections.Generic.List<string> tabNames = new System.Collections.Generic.List<string>(tabs.Keys);
                    for (int i = 0; i < tabNames.Count; i++)
                    {
                        if (tabNames[i] != "chat")
                            ForceCloseTab(tabNames[i]);
                    }
                    ForceCloseTab("chat");
                }
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tstTabs = new System.Windows.Forms.ToolStrip();
            this.ctxTabs = new Radegast.RadegastContextMenuStrip(this.components);
            this.ctxBtnDetach = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxBtnMerge = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxBtnClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnCloseTab = new System.Windows.Forms.ToolStripButton();
            this.tbtnTabOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.tmnuMergeWith = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmnuDetachTab = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tstTabs.SuspendLayout();
            this.ctxTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(623, 436);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(623, 461);
            this.toolStripContainer1.TabIndex = 9;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tstTabs);
            // 
            // tstTabs
            // 
            this.tstTabs.ContextMenuStrip = this.ctxTabs;
            this.tstTabs.Dock = System.Windows.Forms.DockStyle.None;
            this.tstTabs.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tstTabs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnCloseTab,
            this.tbtnTabOptions});
            this.tstTabs.Location = new System.Drawing.Point(0, 0);
            this.tstTabs.Name = "tstTabs";
            this.tstTabs.Size = new System.Drawing.Size(623, 25);
            this.tstTabs.Stretch = true;
            this.tstTabs.TabIndex = 0;
            this.tstTabs.Text = "toolStrip1";
            // 
            // ctxTabs
            // 
            this.ctxTabs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxBtnDetach,
            this.ctxBtnMerge,
            this.toolStripMenuItem2,
            this.ctxBtnClose});
            this.ctxTabs.Name = "ctxTabs";
            this.ctxTabs.Size = new System.Drawing.Size(135, 76);
            this.ctxTabs.Opening += new System.ComponentModel.CancelEventHandler(this.ctxTabs_Opening);
            // 
            // ctxBtnDetach
            // 
            this.ctxBtnDetach.Name = "ctxBtnDetach";
            this.ctxBtnDetach.Size = new System.Drawing.Size(134, 22);
            this.ctxBtnDetach.Text = "Detach";
            this.ctxBtnDetach.Click += new System.EventHandler(this.tmnuDetachTab_Click);
            // 
            // ctxBtnMerge
            // 
            this.ctxBtnMerge.Name = "ctxBtnMerge";
            this.ctxBtnMerge.Size = new System.Drawing.Size(134, 22);
            this.ctxBtnMerge.Text = "Merge with";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(131, 6);
            // 
            // ctxBtnClose
            // 
            this.ctxBtnClose.Name = "ctxBtnClose";
            this.ctxBtnClose.Size = new System.Drawing.Size(134, 22);
            this.ctxBtnClose.Text = "Close";
            this.ctxBtnClose.ToolTipText = " Close ";
            this.ctxBtnClose.Click += new System.EventHandler(this.tbtnCloseTab_Click);
            // 
            // tbtnCloseTab
            // 
            this.tbtnCloseTab.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tbtnCloseTab.AutoToolTip = false;
            this.tbtnCloseTab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnCloseTab.Enabled = false;
            this.tbtnCloseTab.Image = global::Radegast.Properties.Resources.delete_16;
            this.tbtnCloseTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnCloseTab.Name = "tbtnCloseTab";
            this.tbtnCloseTab.Size = new System.Drawing.Size(23, 22);
            this.tbtnCloseTab.ToolTipText = "Close Tab";
            this.tbtnCloseTab.Click += new System.EventHandler(this.tbtnCloseTab_Click);
            // 
            // tbtnTabOptions
            // 
            this.tbtnTabOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tbtnTabOptions.AutoToolTip = false;
            this.tbtnTabOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnTabOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmnuMergeWith,
            this.toolStripMenuItem1,
            this.tmnuDetachTab});
            this.tbtnTabOptions.Image = global::Radegast.Properties.Resources.applications_16;
            this.tbtnTabOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnTabOptions.Name = "tbtnTabOptions";
            this.tbtnTabOptions.Size = new System.Drawing.Size(29, 22);
            this.tbtnTabOptions.Visible = false;
            this.tbtnTabOptions.Click += new System.EventHandler(this.tbtnTabOptions_Click);
            // 
            // tmnuMergeWith
            // 
            this.tmnuMergeWith.Name = "tmnuMergeWith";
            this.tmnuMergeWith.Size = new System.Drawing.Size(136, 22);
            this.tmnuMergeWith.Text = "Merge With";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(133, 6);
            // 
            // tmnuDetachTab
            // 
            this.tmnuDetachTab.Image = global::Radegast.Properties.Resources.copy_16;
            this.tmnuDetachTab.Name = "tmnuDetachTab";
            this.tmnuDetachTab.Size = new System.Drawing.Size(136, 22);
            this.tmnuDetachTab.Text = "Detach Tab";
            this.tmnuDetachTab.Click += new System.EventHandler(this.tmnuDetachTab_Click);
            // 
            // TabsConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TabsConsole";
            this.Size = new System.Drawing.Size(623, 461);
            this.Load += new System.EventHandler(this.TabsConsole_Load);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tstTabs.ResumeLayout(false);
            this.tstTabs.PerformLayout();
            this.ctxTabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ToolStripContainer toolStripContainer1;
        public System.Windows.Forms.ToolStrip tstTabs;
        public System.Windows.Forms.ToolStripDropDownButton tbtnTabOptions;
        public System.Windows.Forms.ToolStripMenuItem tmnuMergeWith;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        public System.Windows.Forms.ToolStripMenuItem tmnuDetachTab;
        public System.Windows.Forms.ToolStripButton tbtnCloseTab;
        public RadegastContextMenuStrip ctxTabs;
        public System.Windows.Forms.ToolStripMenuItem ctxBtnDetach;
        public System.Windows.Forms.ToolStripMenuItem ctxBtnClose;
        public System.Windows.Forms.ToolStripMenuItem ctxBtnMerge;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    }
}
