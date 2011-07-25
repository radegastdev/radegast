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
            this.cbMisc = new System.Windows.Forms.CheckBox();
            this.lblDrawDistance = new System.Windows.Forms.Label();
            this.tbDrawDistance = new System.Windows.Forms.TrackBar();
            this.button_driver = new System.Windows.Forms.Button();
            this.textBox_driveramount = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox_driver = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_morphamount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_morph = new System.Windows.Forms.ComboBox();
            this.textBox_sz = new System.Windows.Forms.TextBox();
            this.textBox_sy = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_sx = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_z = new System.Windows.Forms.TextBox();
            this.textBox_y = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button_vparam = new System.Windows.Forms.Button();
            this.textBox_x = new System.Windows.Forms.TextBox();
            this.hsLOD = new System.Windows.Forms.HScrollBar();
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
            ((System.ComponentModel.ISupportInitialize)(this.tbDrawDistance)).BeginInit();
            this.ctxObjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbZoom
            // 
            this.gbZoom.Controls.Add(this.cbMisc);
            this.gbZoom.Controls.Add(this.lblDrawDistance);
            this.gbZoom.Controls.Add(this.tbDrawDistance);
            this.gbZoom.Controls.Add(this.button_driver);
            this.gbZoom.Controls.Add(this.textBox_driveramount);
            this.gbZoom.Controls.Add(this.label6);
            this.gbZoom.Controls.Add(this.comboBox_driver);
            this.gbZoom.Controls.Add(this.button1);
            this.gbZoom.Controls.Add(this.textBox_morphamount);
            this.gbZoom.Controls.Add(this.label5);
            this.gbZoom.Controls.Add(this.comboBox_morph);
            this.gbZoom.Controls.Add(this.textBox_sz);
            this.gbZoom.Controls.Add(this.textBox_sy);
            this.gbZoom.Controls.Add(this.label4);
            this.gbZoom.Controls.Add(this.textBox_sx);
            this.gbZoom.Controls.Add(this.label3);
            this.gbZoom.Controls.Add(this.label1);
            this.gbZoom.Controls.Add(this.textBox_z);
            this.gbZoom.Controls.Add(this.textBox_y);
            this.gbZoom.Controls.Add(this.comboBox1);
            this.gbZoom.Controls.Add(this.button_vparam);
            this.gbZoom.Controls.Add(this.textBox_x);
            this.gbZoom.Controls.Add(this.hsLOD);
            this.gbZoom.Controls.Add(this.hsSpecular);
            this.gbZoom.Controls.Add(this.hsDiffuse);
            this.gbZoom.Controls.Add(this.hsAmbient);
            this.gbZoom.Controls.Add(this.label2);
            this.gbZoom.Controls.Add(this.btnResetView);
            this.gbZoom.Controls.Add(this.cbAA);
            this.gbZoom.Controls.Add(this.chkWireFrame);
            this.gbZoom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbZoom.Location = new System.Drawing.Point(0, 441);
            this.gbZoom.Name = "gbZoom";
            this.gbZoom.Size = new System.Drawing.Size(886, 163);
            this.gbZoom.TabIndex = 8;
            this.gbZoom.TabStop = false;
            this.gbZoom.Enter += new System.EventHandler(this.gbZoom_Enter);
            // 
            // cbMisc
            // 
            this.cbMisc.AutoSize = true;
            this.cbMisc.Checked = true;
            this.cbMisc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMisc.Location = new System.Drawing.Point(358, 142);
            this.cbMisc.Name = "cbMisc";
            this.cbMisc.Size = new System.Drawing.Size(48, 17);
            this.cbMisc.TabIndex = 49;
            this.cbMisc.Text = "Misc";
            this.cbMisc.UseVisualStyleBackColor = true;
            this.cbMisc.CheckedChanged += new System.EventHandler(this.cbMisc_CheckedChanged);
            // 
            // lblDrawDistance
            // 
            this.lblDrawDistance.AutoSize = true;
            this.lblDrawDistance.Location = new System.Drawing.Point(542, 140);
            this.lblDrawDistance.Name = "lblDrawDistance";
            this.lblDrawDistance.Size = new System.Drawing.Size(93, 13);
            this.lblDrawDistance.TabIndex = 48;
            this.lblDrawDistance.Text = "Draw distance: 48";
            this.lblDrawDistance.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbDrawDistance
            // 
            this.tbDrawDistance.Location = new System.Drawing.Point(358, 117);
            this.tbDrawDistance.Maximum = 176;
            this.tbDrawDistance.Minimum = 32;
            this.tbDrawDistance.Name = "tbDrawDistance";
            this.tbDrawDistance.Size = new System.Drawing.Size(277, 45);
            this.tbDrawDistance.TabIndex = 47;
            this.tbDrawDistance.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbDrawDistance.Value = 32;
            this.tbDrawDistance.Scroll += new System.EventHandler(this.tbDrawDistance_Scroll);
            // 
            // button_driver
            // 
            this.button_driver.Location = new System.Drawing.Point(277, 140);
            this.button_driver.Name = "button_driver";
            this.button_driver.Size = new System.Drawing.Size(75, 23);
            this.button_driver.TabIndex = 46;
            this.button_driver.Text = "Driver";
            this.button_driver.UseVisualStyleBackColor = true;
            this.button_driver.Click += new System.EventHandler(this.button_driver_Click);
            // 
            // textBox_driveramount
            // 
            this.textBox_driveramount.Location = new System.Drawing.Point(204, 142);
            this.textBox_driveramount.Name = "textBox_driveramount";
            this.textBox_driveramount.Size = new System.Drawing.Size(67, 20);
            this.textBox_driveramount.TabIndex = 45;
            this.textBox_driveramount.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Driver";
            // 
            // comboBox_driver
            // 
            this.comboBox_driver.FormattingEnabled = true;
            this.comboBox_driver.Location = new System.Drawing.Point(74, 141);
            this.comboBox_driver.Name = "comboBox_driver";
            this.comboBox_driver.Size = new System.Drawing.Size(121, 21);
            this.comboBox_driver.TabIndex = 43;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(277, 114);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 42;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_morphamount
            // 
            this.textBox_morphamount.Location = new System.Drawing.Point(204, 116);
            this.textBox_morphamount.Name = "textBox_morphamount";
            this.textBox_morphamount.Size = new System.Drawing.Size(67, 20);
            this.textBox_morphamount.TabIndex = 41;
            this.textBox_morphamount.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 40;
            this.label5.Text = "Morph";
            // 
            // comboBox_morph
            // 
            this.comboBox_morph.FormattingEnabled = true;
            this.comboBox_morph.Items.AddRange(new object[] {
            "mPelvis",
            "mTorso",
            "mChest",
            "mNeck",
            "mHead",
            "mCollarLeft",
            "mCollarRight",
            "mShoulderLeft",
            "mShoulderRight",
            "mElbowLeft",
            "mElbowRight",
            "mWristRight",
            "mWristLeft",
            "mHipRight",
            "mHipLeft",
            "mKneeRight",
            "mKneeLeft",
            "mAnkleRight",
            "mAnkleLeft",
            "mFootLeft",
            "mFootRight",
            "mToeLeft",
            "mToeRight"});
            this.comboBox_morph.Location = new System.Drawing.Point(74, 115);
            this.comboBox_morph.Name = "comboBox_morph";
            this.comboBox_morph.Size = new System.Drawing.Size(121, 21);
            this.comboBox_morph.TabIndex = 39;
            this.comboBox_morph.SelectedIndexChanged += new System.EventHandler(this.comboBox_morph_SelectedIndexChanged);
            // 
            // textBox_sz
            // 
            this.textBox_sz.Location = new System.Drawing.Point(466, 90);
            this.textBox_sz.Name = "textBox_sz";
            this.textBox_sz.Size = new System.Drawing.Size(32, 20);
            this.textBox_sz.TabIndex = 38;
            this.textBox_sz.Text = "0";
            // 
            // textBox_sy
            // 
            this.textBox_sy.Location = new System.Drawing.Point(428, 91);
            this.textBox_sy.Name = "textBox_sy";
            this.textBox_sy.Size = new System.Drawing.Size(32, 20);
            this.textBox_sy.TabIndex = 37;
            this.textBox_sy.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(353, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "Scale";
            // 
            // textBox_sx
            // 
            this.textBox_sx.Location = new System.Drawing.Point(390, 91);
            this.textBox_sx.Name = "textBox_sx";
            this.textBox_sx.Size = new System.Drawing.Size(32, 20);
            this.textBox_sx.TabIndex = 33;
            this.textBox_sx.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(201, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Rot";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Bone/Joint";
            // 
            // textBox_z
            // 
            this.textBox_z.Location = new System.Drawing.Point(315, 90);
            this.textBox_z.Name = "textBox_z";
            this.textBox_z.Size = new System.Drawing.Size(32, 20);
            this.textBox_z.TabIndex = 30;
            this.textBox_z.Text = "0";
            this.textBox_z.TextChanged += new System.EventHandler(this.textBox_z_TextChanged);
            // 
            // textBox_y
            // 
            this.textBox_y.Location = new System.Drawing.Point(277, 90);
            this.textBox_y.Name = "textBox_y";
            this.textBox_y.Size = new System.Drawing.Size(32, 20);
            this.textBox_y.TabIndex = 29;
            this.textBox_y.Text = "0";
            this.textBox_y.TextChanged += new System.EventHandler(this.textBox_y_TextChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "mPelvis",
            "mTorso",
            "mChest",
            "mNeck",
            "mHead",
            "mCollarLeft",
            "mCollarRight",
            "mShoulderLeft",
            "mShoulderRight",
            "mElbowLeft",
            "mElbowRight",
            "mWristRight",
            "mWristLeft",
            "mHipRight",
            "mHipLeft",
            "mKneeRight",
            "mKneeLeft",
            "mAnkleRight",
            "mAnkleLeft",
            "mFootLeft",
            "mFootRight",
            "mToeLeft",
            "mToeRight"});
            this.comboBox1.Location = new System.Drawing.Point(74, 88);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 28;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button_vparam
            // 
            this.button_vparam.Location = new System.Drawing.Point(514, 88);
            this.button_vparam.Name = "button_vparam";
            this.button_vparam.Size = new System.Drawing.Size(75, 23);
            this.button_vparam.TabIndex = 27;
            this.button_vparam.Text = "Apply";
            this.button_vparam.UseVisualStyleBackColor = true;
            this.button_vparam.Click += new System.EventHandler(this.button_vparam_Click);
            // 
            // textBox_x
            // 
            this.textBox_x.Location = new System.Drawing.Point(239, 89);
            this.textBox_x.Name = "textBox_x";
            this.textBox_x.Size = new System.Drawing.Size(32, 20);
            this.textBox_x.TabIndex = 26;
            this.textBox_x.Text = "0";
            this.textBox_x.TextChanged += new System.EventHandler(this.textBox_vparamid_TextChanged);
            // 
            // hsLOD
            // 
            this.hsLOD.Location = new System.Drawing.Point(343, 67);
            this.hsLOD.Name = "hsLOD";
            this.hsLOD.Size = new System.Drawing.Size(292, 17);
            this.hsLOD.TabIndex = 24;
            this.hsLOD.Value = 25;
            this.hsLOD.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsLOD_Scroll);
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
            this.ClientSize = new System.Drawing.Size(886, 604);
            this.Controls.Add(this.gbZoom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SceneWindow";
            this.Text = "Scene Viewer";
            this.Load += new System.EventHandler(this.ControlLoaded);
            this.gbZoom.ResumeLayout(false);
            this.gbZoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbDrawDistance)).EndInit();
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
        private System.Windows.Forms.HScrollBar hsLOD;
        private System.Windows.Forms.Button button_vparam;
        private System.Windows.Forms.TextBox textBox_x;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox_z;
        private System.Windows.Forms.TextBox textBox_y;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_sx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_sz;
        private System.Windows.Forms.TextBox textBox_sy;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_morphamount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_morph;
        private System.Windows.Forms.Button button_driver;
        private System.Windows.Forms.TextBox textBox_driveramount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox_driver;
        private System.Windows.Forms.TrackBar tbDrawDistance;
        private System.Windows.Forms.Label lblDrawDistance;
        private System.Windows.Forms.CheckBox cbMisc;

    }
}

