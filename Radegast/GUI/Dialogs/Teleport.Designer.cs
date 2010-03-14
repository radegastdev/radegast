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
    partial class frmTeleport
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearchFor = new System.Windows.Forms.TextBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.nudX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudY = new System.Windows.Forms.NumericUpDown();
            this.nudZ = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnTeleport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbxRegionSearch = new System.Windows.Forms.ListBox();
            this.pnlTeleporting = new System.Windows.Forms.Panel();
            this.proTeleporting = new System.Windows.Forms.ProgressBar();
            this.lblTeleportStatus = new System.Windows.Forms.Label();
            this.pnlTeleportOptions = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.trkIconSize = new System.Windows.Forms.TrackBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZ)).BeginInit();
            this.pnlTeleporting.SuspendLayout();
            this.pnlTeleportOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkIconSize)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search for:";
            // 
            // txtSearchFor
            // 
            this.txtSearchFor.Location = new System.Drawing.Point(79, 12);
            this.txtSearchFor.Name = "txtSearchFor";
            this.txtSearchFor.Size = new System.Drawing.Size(219, 21);
            this.txtSearchFor.TabIndex = 1;
            this.txtSearchFor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchFor_KeyDown);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(304, 10);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(72, 23);
            this.btnFind.TabIndex = 2;
            this.btnFind.Text = "Search";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Region";
            // 
            // txtRegion
            // 
            this.txtRegion.Location = new System.Drawing.Point(6, 25);
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.ReadOnly = true;
            this.txtRegion.Size = new System.Drawing.Size(156, 21);
            this.txtRegion.TabIndex = 5;
            // 
            // nudX
            // 
            this.nudX.Location = new System.Drawing.Point(6, 65);
            this.nudX.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudX.Name = "nudX";
            this.nudX.Size = new System.Drawing.Size(48, 21);
            this.nudX.TabIndex = 6;
            this.nudX.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "X";
            // 
            // nudY
            // 
            this.nudY.Location = new System.Drawing.Point(60, 65);
            this.nudY.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudY.Name = "nudY";
            this.nudY.Size = new System.Drawing.Size(48, 21);
            this.nudY.TabIndex = 8;
            this.nudY.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // nudZ
            // 
            this.nudZ.Location = new System.Drawing.Point(114, 65);
            this.nudZ.Maximum = new decimal(new int[] {
            768,
            0,
            0,
            0});
            this.nudZ.Name = "nudZ";
            this.nudZ.Size = new System.Drawing.Size(48, 21);
            this.nudZ.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Z";
            // 
            // btnTeleport
            // 
            this.btnTeleport.Enabled = false;
            this.btnTeleport.Location = new System.Drawing.Point(382, 373);
            this.btnTeleport.Name = "btnTeleport";
            this.btnTeleport.Size = new System.Drawing.Size(172, 23);
            this.btnTeleport.TabIndex = 13;
            this.btnTeleport.Text = "Teleport";
            this.btnTeleport.UseVisualStyleBackColor = true;
            this.btnTeleport.Click += new System.EventHandler(this.btnTeleport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(382, 402);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(172, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbxRegionSearch
            // 
            this.lbxRegionSearch.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbxRegionSearch.FormattingEnabled = true;
            this.lbxRegionSearch.IntegralHeight = false;
            this.lbxRegionSearch.ItemHeight = 74;
            this.lbxRegionSearch.Location = new System.Drawing.Point(12, 39);
            this.lbxRegionSearch.Name = "lbxRegionSearch";
            this.lbxRegionSearch.Size = new System.Drawing.Size(364, 386);
            this.lbxRegionSearch.TabIndex = 15;
            this.lbxRegionSearch.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbxRegionSearch_DrawItem);
            this.lbxRegionSearch.DoubleClick += new System.EventHandler(this.lbxRegionSearch_DoubleClick);
            this.lbxRegionSearch.Click += new System.EventHandler(this.lbxRegionSearch_DoubleClick);
            // 
            // pnlTeleporting
            // 
            this.pnlTeleporting.Controls.Add(this.proTeleporting);
            this.pnlTeleporting.Controls.Add(this.lblTeleportStatus);
            this.pnlTeleporting.Location = new System.Drawing.Point(382, 331);
            this.pnlTeleporting.Name = "pnlTeleporting";
            this.pnlTeleporting.Size = new System.Drawing.Size(172, 36);
            this.pnlTeleporting.TabIndex = 16;
            this.pnlTeleporting.Visible = false;
            // 
            // proTeleporting
            // 
            this.proTeleporting.Location = new System.Drawing.Point(3, 16);
            this.proTeleporting.MarqueeAnimationSpeed = 50;
            this.proTeleporting.Name = "proTeleporting";
            this.proTeleporting.Size = new System.Drawing.Size(166, 16);
            this.proTeleporting.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.proTeleporting.TabIndex = 1;
            // 
            // lblTeleportStatus
            // 
            this.lblTeleportStatus.AutoSize = true;
            this.lblTeleportStatus.Location = new System.Drawing.Point(3, 0);
            this.lblTeleportStatus.Name = "lblTeleportStatus";
            this.lblTeleportStatus.Size = new System.Drawing.Size(73, 13);
            this.lblTeleportStatus.TabIndex = 0;
            this.lblTeleportStatus.Text = "Teleporting...";
            // 
            // pnlTeleportOptions
            // 
            this.pnlTeleportOptions.Controls.Add(this.label6);
            this.pnlTeleportOptions.Controls.Add(this.trkIconSize);
            this.pnlTeleportOptions.Controls.Add(this.label2);
            this.pnlTeleportOptions.Controls.Add(this.txtRegion);
            this.pnlTeleportOptions.Controls.Add(this.nudX);
            this.pnlTeleportOptions.Controls.Add(this.label3);
            this.pnlTeleportOptions.Controls.Add(this.nudY);
            this.pnlTeleportOptions.Controls.Add(this.label5);
            this.pnlTeleportOptions.Controls.Add(this.nudZ);
            this.pnlTeleportOptions.Controls.Add(this.label4);
            this.pnlTeleportOptions.Location = new System.Drawing.Point(382, 0);
            this.pnlTeleportOptions.Name = "pnlTeleportOptions";
            this.pnlTeleportOptions.Size = new System.Drawing.Size(172, 325);
            this.pnlTeleportOptions.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Zoom";
            // 
            // trkIconSize
            // 
            this.trkIconSize.AutoSize = false;
            this.trkIconSize.LargeChange = 16;
            this.trkIconSize.Location = new System.Drawing.Point(42, 92);
            this.trkIconSize.Maximum = 128;
            this.trkIconSize.Minimum = 32;
            this.trkIconSize.Name = "trkIconSize";
            this.trkIconSize.Size = new System.Drawing.Size(127, 22);
            this.trkIconSize.SmallChange = 8;
            this.trkIconSize.TabIndex = 18;
            this.trkIconSize.TickFrequency = 32;
            this.trkIconSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkIconSize.Value = 64;
            this.trkIconSize.Scroll += new System.EventHandler(this.trkIconSize_Scroll);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 430);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(566, 22);
            this.statusStrip1.TabIndex = 18;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(85, 17);
            this.statusLabel.Text = "Teleport status";
            // 
            // frmTeleport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 452);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pnlTeleportOptions);
            this.Controls.Add(this.pnlTeleporting);
            this.Controls.Add(this.lbxRegionSearch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTeleport);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.txtSearchFor);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTeleport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Teleport";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTeleport_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZ)).EndInit();
            this.pnlTeleporting.ResumeLayout(false);
            this.pnlTeleporting.PerformLayout();
            this.pnlTeleportOptions.ResumeLayout(false);
            this.pnlTeleportOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkIconSize)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtSearchFor;
        public System.Windows.Forms.Button btnFind;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtRegion;
        public System.Windows.Forms.NumericUpDown nudX;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown nudY;
        public System.Windows.Forms.NumericUpDown nudZ;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Button btnTeleport;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.ListBox lbxRegionSearch;
        public System.Windows.Forms.Panel pnlTeleporting;
        public System.Windows.Forms.ProgressBar proTeleporting;
        public System.Windows.Forms.Label lblTeleportStatus;
        public System.Windows.Forms.Panel pnlTeleportOptions;
        public System.Windows.Forms.TrackBar trkIconSize;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripStatusLabel statusLabel;

    }
}