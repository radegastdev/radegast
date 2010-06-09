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
    partial class frmPrimWorkshop
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
                glControl.DestroyContexts();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrimWorkshop));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.glControl = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cboFace = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cboPrim = new System.Windows.Forms.ComboBox();
            this.scrollRoll = new System.Windows.Forms.HScrollBar();
            this.scrollPitch = new System.Windows.Forms.HScrollBar();
            this.scrollYaw = new System.Windows.Forms.HScrollBar();
            this.picTexture = new System.Windows.Forms.PictureBox();
            this.scrollZoom = new System.Windows.Forms.HScrollBar();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.glControl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer.Size = new System.Drawing.Size(996, 536);
            this.splitContainer.SplitterDistance = 550;
            this.splitContainer.TabIndex = 6;
            // 
            // glControl
            // 
            this.glControl.AccumBits = ((byte)(0));
            this.glControl.AutoCheckErrors = false;
            this.glControl.AutoFinish = false;
            this.glControl.AutoMakeCurrent = true;
            this.glControl.AutoSwapBuffers = true;
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.ColorBits = ((byte)(32));
            this.glControl.DepthBits = ((byte)(16));
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(550, 536);
            this.glControl.StencilBits = ((byte)(0));
            this.glControl.TabIndex = 5;
            this.glControl.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
            this.glControl.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.scrollRoll, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.scrollPitch, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.scrollYaw, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.picTexture, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.scrollZoom, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(442, 536);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cboFace);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 117);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(436, 24);
            this.panel2.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Face:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboFace
            // 
            this.cboFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFace.FormattingEnabled = true;
            this.cboFace.Location = new System.Drawing.Point(40, 2);
            this.cboFace.Name = "cboFace";
            this.cboFace.Size = new System.Drawing.Size(174, 21);
            this.cboFace.TabIndex = 15;
            this.cboFace.SelectedIndexChanged += new System.EventHandler(this.cboFace_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cboPrim);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 87);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(436, 24);
            this.panel1.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Prim:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboPrim
            // 
            this.cboPrim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrim.FormattingEnabled = true;
            this.cboPrim.Location = new System.Drawing.Point(40, 2);
            this.cboPrim.Name = "cboPrim";
            this.cboPrim.Size = new System.Drawing.Size(174, 21);
            this.cboPrim.TabIndex = 15;
            this.cboPrim.SelectedIndexChanged += new System.EventHandler(this.cboPrim_SelectedIndexChanged);
            // 
            // scrollRoll
            // 
            this.scrollRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollRoll.Location = new System.Drawing.Point(0, 2);
            this.scrollRoll.Maximum = 360;
            this.scrollRoll.Name = "scrollRoll";
            this.scrollRoll.Size = new System.Drawing.Size(442, 16);
            this.scrollRoll.TabIndex = 9;
            this.scrollRoll.ValueChanged += new System.EventHandler(this.scroll_ValueChanged);
            // 
            // scrollPitch
            // 
            this.scrollPitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollPitch.Location = new System.Drawing.Point(0, 22);
            this.scrollPitch.Maximum = 360;
            this.scrollPitch.Name = "scrollPitch";
            this.scrollPitch.Size = new System.Drawing.Size(442, 16);
            this.scrollPitch.TabIndex = 10;
            this.scrollPitch.ValueChanged += new System.EventHandler(this.scroll_ValueChanged);
            // 
            // scrollYaw
            // 
            this.scrollYaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollYaw.Location = new System.Drawing.Point(0, 42);
            this.scrollYaw.Maximum = 360;
            this.scrollYaw.Name = "scrollYaw";
            this.scrollYaw.Size = new System.Drawing.Size(442, 16);
            this.scrollYaw.TabIndex = 11;
            this.scrollYaw.ValueChanged += new System.EventHandler(this.scroll_ValueChanged);
            // 
            // picTexture
            // 
            this.picTexture.BackColor = System.Drawing.Color.Black;
            this.picTexture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picTexture.Location = new System.Drawing.Point(3, 147);
            this.picTexture.Name = "picTexture";
            this.picTexture.Size = new System.Drawing.Size(436, 386);
            this.picTexture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTexture.TabIndex = 17;
            this.picTexture.TabStop = false;
            this.picTexture.MouseLeave += new System.EventHandler(this.picTexture_MouseLeave);
            this.picTexture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picTexture_MouseMove);
            this.picTexture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTexture_MouseDown);
            this.picTexture.Paint += new System.Windows.Forms.PaintEventHandler(this.picTexture_Paint);
            this.picTexture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picTexture_MouseUp);
            // 
            // scrollZoom
            // 
            this.scrollZoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollZoom.LargeChange = 1;
            this.scrollZoom.Location = new System.Drawing.Point(0, 60);
            this.scrollZoom.Maximum = 0;
            this.scrollZoom.Minimum = -200;
            this.scrollZoom.Name = "scrollZoom";
            this.scrollZoom.Size = new System.Drawing.Size(442, 24);
            this.scrollZoom.TabIndex = 19;
            this.scrollZoom.Value = -50;
            this.scrollZoom.ValueChanged += new System.EventHandler(this.scrollZoom_ValueChanged);
            // 
            // frmPrimWorkshop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 536);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPrimWorkshop";
            this.Text = "Prim Workshop";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.SplitContainer splitContainer;
        public Tao.Platform.Windows.SimpleOpenGlControl glControl;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.HScrollBar scrollRoll;
        public System.Windows.Forms.HScrollBar scrollPitch;
        public System.Windows.Forms.HScrollBar scrollYaw;
        public System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox cboFace;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cboPrim;
        public System.Windows.Forms.PictureBox picTexture;
        public System.Windows.Forms.HScrollBar scrollZoom;

    }
}

