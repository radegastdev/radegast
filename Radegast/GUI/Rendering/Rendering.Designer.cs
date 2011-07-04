// 
// Radegast Metaverse Client
// Copyright (c) 2009-2011, Radegast Development Team
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
namespace Radegast.Rendering
{
    partial class SceneWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SceneWindow));
            this.gbZoom = new System.Windows.Forms.GroupBox();
            this.hsSpecular = new System.Windows.Forms.HScrollBar();
            this.hsDiffuse = new System.Windows.Forms.HScrollBar();
            this.hsAmbient = new System.Windows.Forms.HScrollBar();
            this.label2 = new System.Windows.Forms.Label();
            this.btnResetView = new System.Windows.Forms.Button();
            this.cbAA = new System.Windows.Forms.CheckBox();
            this.chkWireFrame = new System.Windows.Forms.CheckBox();
            this.ctxObjects = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.touchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.takeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.returnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbZoom.SuspendLayout();
            this.ctxObjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbZoom
            // 
            this.gbZoom.Controls.Add(this.hsSpecular);
            this.gbZoom.Controls.Add(this.hsDiffuse);
            this.gbZoom.Controls.Add(this.hsAmbient);
            this.gbZoom.Controls.Add(this.label2);
            this.gbZoom.Controls.Add(this.btnResetView);
            this.gbZoom.Controls.Add(this.cbAA);
            this.gbZoom.Controls.Add(this.chkWireFrame);
            this.gbZoom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbZoom.Location = new System.Drawing.Point(0, 516);
            this.gbZoom.Name = "gbZoom";
            this.gbZoom.Size = new System.Drawing.Size(644, 89);
            this.gbZoom.TabIndex = 8;
            this.gbZoom.TabStop = false;
            // 
            // hsSpecular
            // 
            this.hsSpecular.Location = new System.Drawing.Point(343, 50);
            this.hsSpecular.Name = "hsSpecular";
            this.hsSpecular.Size = new System.Drawing.Size(292, 17);
            this.hsSpecular.TabIndex = 24;
            this.hsSpecular.Value = 30;
            this.hsSpecular.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsSpecular_Scroll);
            // 
            // hsDiffuse
            // 
            this.hsDiffuse.Location = new System.Drawing.Point(343, 33);
            this.hsDiffuse.Name = "hsDiffuse";
            this.hsDiffuse.Size = new System.Drawing.Size(292, 17);
            this.hsDiffuse.TabIndex = 24;
            this.hsDiffuse.Value = 25;
            this.hsDiffuse.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsDiffuse_Scroll);
            // 
            // hsAmbient
            // 
            this.hsAmbient.Location = new System.Drawing.Point(343, 16);
            this.hsAmbient.Name = "hsAmbient";
            this.hsAmbient.Size = new System.Drawing.Size(292, 17);
            this.hsAmbient.TabIndex = 24;
            this.hsAmbient.Value = 20;
            this.hsAmbient.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsAmbient_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(140, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Alt-Drag to Zoom, Ctrl-Drag to pan";
            // 
            // btnResetView
            // 
            this.btnResetView.Location = new System.Drawing.Point(12, 31);
            this.btnResetView.Name = "btnResetView";
            this.btnResetView.Size = new System.Drawing.Size(94, 23);
            this.btnResetView.TabIndex = 22;
            this.btnResetView.Text = "Reset View";
            this.btnResetView.UseVisualStyleBackColor = true;
            this.btnResetView.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // cbAA
            // 
            this.cbAA.AutoSize = true;
            this.cbAA.Location = new System.Drawing.Point(89, 11);
            this.cbAA.Name = "cbAA";
            this.cbAA.Size = new System.Drawing.Size(82, 17);
            this.cbAA.TabIndex = 21;
            this.cbAA.Text = "Anti-aliasing";
            this.cbAA.UseVisualStyleBackColor = true;
            // 
            // chkWireFrame
            // 
            this.chkWireFrame.AutoSize = true;
            this.chkWireFrame.Location = new System.Drawing.Point(9, 10);
            this.chkWireFrame.Name = "chkWireFrame";
            this.chkWireFrame.Size = new System.Drawing.Size(74, 17);
            this.chkWireFrame.TabIndex = 21;
            this.chkWireFrame.Text = "Wireframe";
            this.chkWireFrame.UseVisualStyleBackColor = true;
            this.chkWireFrame.CheckedChanged += new System.EventHandler(this.chkWireFrame_CheckedChanged);
            // 
            // ctxObjects
            // 
            this.ctxObjects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.touchToolStripMenuItem,
            this.sitToolStripMenuItem,
            this.toolStripMenuItem1,
            this.takeToolStripMenuItem,
            this.returnToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.ctxObjects.Name = "ctxObjects";
            this.ctxObjects.Size = new System.Drawing.Size(110, 120);
            this.ctxObjects.Opening += new System.ComponentModel.CancelEventHandler(this.ctxObjects_Opening);
            // 
            // touchToolStripMenuItem
            // 
            this.touchToolStripMenuItem.Name = "touchToolStripMenuItem";
            this.touchToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.touchToolStripMenuItem.Text = "Touch";
            this.touchToolStripMenuItem.Click += new System.EventHandler(this.touchToolStripMenuItem_Click);
            // 
            // sitToolStripMenuItem
            // 
            this.sitToolStripMenuItem.Name = "sitToolStripMenuItem";
            this.sitToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.sitToolStripMenuItem.Text = "Sit";
            this.sitToolStripMenuItem.Click += new System.EventHandler(this.sitToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(106, 6);
            // 
            // takeToolStripMenuItem
            // 
            this.takeToolStripMenuItem.Name = "takeToolStripMenuItem";
            this.takeToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.takeToolStripMenuItem.Text = "Take";
            this.takeToolStripMenuItem.Click += new System.EventHandler(this.takeToolStripMenuItem_Click);
            // 
            // returnToolStripMenuItem
            // 
            this.returnToolStripMenuItem.Name = "returnToolStripMenuItem";
            this.returnToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.returnToolStripMenuItem.Text = "Return";
            this.returnToolStripMenuItem.Click += new System.EventHandler(this.returnToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // SceneWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 605);
            this.Controls.Add(this.gbZoom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SceneWindow";
            this.Text = "Scene Viewer";
            this.Shown += new System.EventHandler(this.frmPrimWorkshop_Shown);
            this.gbZoom.ResumeLayout(false);
            this.gbZoom.PerformLayout();
            this.ctxObjects.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox gbZoom;
        public System.Windows.Forms.ContextMenuStrip ctxObjects;
        public System.Windows.Forms.CheckBox cbAA;
        public System.Windows.Forms.CheckBox chkWireFrame;
        public System.Windows.Forms.Button btnResetView;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ToolStripMenuItem touchToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem sitToolStripMenuItem;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        public System.Windows.Forms.ToolStripMenuItem takeToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem returnToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.HScrollBar hsSpecular;
        private System.Windows.Forms.HScrollBar hsDiffuse;
        private System.Windows.Forms.HScrollBar hsAmbient;

    }
}

