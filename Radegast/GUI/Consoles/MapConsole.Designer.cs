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
    partial class MapConsole
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
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.ddOnlineFriends = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDestination = new System.Windows.Forms.Button();
            this.btnMyPos = new System.Windows.Forms.Button();
            this.btnGoHome = new System.Windows.Forms.Button();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.prgTeleport = new System.Windows.Forms.ProgressBar();
            this.btnTeleport = new System.Windows.Forms.Button();
            this.nudX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudY = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nudZ = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.lstRegions = new Radegast.ListViewNoFlicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.zoomTracker = new System.Windows.Forms.TrackBar();
            this.pnlMap = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlSearch.SuspendLayout();
            this.pnlProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTracker)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.btnRefresh);
            this.pnlSearch.Controls.Add(this.ddOnlineFriends);
            this.pnlSearch.Controls.Add(this.lblStatus);
            this.pnlSearch.Controls.Add(this.btnDestination);
            this.pnlSearch.Controls.Add(this.btnMyPos);
            this.pnlSearch.Controls.Add(this.btnGoHome);
            this.pnlSearch.Controls.Add(this.pnlProgress);
            this.pnlSearch.Controls.Add(this.btnTeleport);
            this.pnlSearch.Controls.Add(this.nudX);
            this.pnlSearch.Controls.Add(this.label3);
            this.pnlSearch.Controls.Add(this.nudY);
            this.pnlSearch.Controls.Add(this.label5);
            this.pnlSearch.Controls.Add(this.nudZ);
            this.pnlSearch.Controls.Add(this.label4);
            this.pnlSearch.Controls.Add(this.lstRegions);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtRegion);
            this.pnlSearch.Controls.Add(this.zoomTracker);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSearch.Location = new System.Drawing.Point(560, 0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(194, 426);
            this.pnlSearch.TabIndex = 0;
            // 
            // ddOnlineFriends
            // 
            this.ddOnlineFriends.AccessibleName = "Find online friends on the grid";
            this.ddOnlineFriends.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddOnlineFriends.FormattingEnabled = true;
            this.ddOnlineFriends.Items.AddRange(new object[] {
            "Online Friends"});
            this.ddOnlineFriends.Location = new System.Drawing.Point(6, 195);
            this.ddOnlineFriends.Name = "ddOnlineFriends";
            this.ddOnlineFriends.Size = new System.Drawing.Size(182, 21);
            this.ddOnlineFriends.TabIndex = 3;
            this.ddOnlineFriends.SelectedIndexChanged += new System.EventHandler(this.ddOnlineFriends_SelectedIndexChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(3, 379);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(185, 16);
            this.lblStatus.TabIndex = 22;
            this.lblStatus.Text = "Teleport status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDestination
            // 
            this.btnDestination.Location = new System.Drawing.Point(91, 291);
            this.btnDestination.Name = "btnDestination";
            this.btnDestination.Size = new System.Drawing.Size(75, 23);
            this.btnDestination.TabIndex = 22;
            this.btnDestination.Text = "Destination";
            this.btnDestination.UseVisualStyleBackColor = true;
            this.btnDestination.Click += new System.EventHandler(this.btnDestination_Click);
            // 
            // btnMyPos
            // 
            this.btnMyPos.Location = new System.Drawing.Point(10, 291);
            this.btnMyPos.Name = "btnMyPos";
            this.btnMyPos.Size = new System.Drawing.Size(75, 23);
            this.btnMyPos.TabIndex = 21;
            this.btnMyPos.Text = "My position";
            this.btnMyPos.UseVisualStyleBackColor = true;
            this.btnMyPos.Click += new System.EventHandler(this.btnMyPos_Click);
            // 
            // btnGoHome
            // 
            this.btnGoHome.Location = new System.Drawing.Point(91, 262);
            this.btnGoHome.Name = "btnGoHome";
            this.btnGoHome.Size = new System.Drawing.Size(75, 23);
            this.btnGoHome.TabIndex = 19;
            this.btnGoHome.Text = "Go home";
            this.btnGoHome.UseVisualStyleBackColor = true;
            this.btnGoHome.Click += new System.EventHandler(this.btnGoHome_Click);
            // 
            // pnlProgress
            // 
            this.pnlProgress.Controls.Add(this.prgTeleport);
            this.pnlProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlProgress.Location = new System.Drawing.Point(0, 397);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Size = new System.Drawing.Size(194, 29);
            this.pnlProgress.TabIndex = 20;
            // 
            // prgTeleport
            // 
            this.prgTeleport.Enabled = false;
            this.prgTeleport.Location = new System.Drawing.Point(6, 3);
            this.prgTeleport.MarqueeAnimationSpeed = 50;
            this.prgTeleport.Name = "prgTeleport";
            this.prgTeleport.Size = new System.Drawing.Size(182, 23);
            this.prgTeleport.TabIndex = 0;
            // 
            // btnTeleport
            // 
            this.btnTeleport.Enabled = false;
            this.btnTeleport.Location = new System.Drawing.Point(10, 262);
            this.btnTeleport.Name = "btnTeleport";
            this.btnTeleport.Size = new System.Drawing.Size(75, 23);
            this.btnTeleport.TabIndex = 18;
            this.btnTeleport.Text = "Teleport";
            this.btnTeleport.UseVisualStyleBackColor = true;
            this.btnTeleport.Click += new System.EventHandler(this.btnTeleport_Click);
            // 
            // nudX
            // 
            this.nudX.Location = new System.Drawing.Point(10, 235);
            this.nudX.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudX.Name = "nudX";
            this.nudX.Size = new System.Drawing.Size(48, 20);
            this.nudX.TabIndex = 12;
            this.nudX.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 219);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "X";
            // 
            // nudY
            // 
            this.nudY.Location = new System.Drawing.Point(64, 235);
            this.nudY.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudY.Name = "nudY";
            this.nudY.Size = new System.Drawing.Size(48, 20);
            this.nudY.TabIndex = 14;
            this.nudY.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 219);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Z";
            // 
            // nudZ
            // 
            this.nudZ.Location = new System.Drawing.Point(118, 235);
            this.nudZ.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.nudZ.Name = "nudZ";
            this.nudZ.Size = new System.Drawing.Size(48, 20);
            this.nudZ.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(61, 219);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Y";
            // 
            // lstRegions
            // 
            this.lstRegions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstRegions.Location = new System.Drawing.Point(6, 38);
            this.lstRegions.MultiSelect = false;
            this.lstRegions.Name = "lstRegions";
            this.lstRegions.ShowGroups = false;
            this.lstRegions.Size = new System.Drawing.Size(182, 151);
            this.lstRegions.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstRegions.TabIndex = 2;
            this.lstRegions.UseCompatibleStateImageBehavior = false;
            this.lstRegions.View = System.Windows.Forms.View.List;
            this.lstRegions.SelectedIndexChanged += new System.EventHandler(this.lstRegions_SelectedIndexChanged);
            this.lstRegions.Enter += new System.EventHandler(this.lstRegions_Enter);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(136, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(52, 21);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtRegion
            // 
            this.txtRegion.Location = new System.Drawing.Point(6, 12);
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.Size = new System.Drawing.Size(124, 20);
            this.txtRegion.TabIndex = 0;
            this.txtRegion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRegion_KeyDown);
            // 
            // zoomTracker
            // 
            this.zoomTracker.Location = new System.Drawing.Point(6, 345);
            this.zoomTracker.Maximum = 100;
            this.zoomTracker.Name = "zoomTracker";
            this.zoomTracker.Size = new System.Drawing.Size(181, 45);
            this.zoomTracker.TabIndex = 24;
            this.zoomTracker.TickStyle = System.Windows.Forms.TickStyle.None;
            this.zoomTracker.Visible = false;
            this.zoomTracker.Scroll += new System.EventHandler(this.zoomTracker_Scroll);
            // 
            // pnlMap
            // 
            this.pnlMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMap.Location = new System.Drawing.Point(0, 0);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(560, 426);
            this.pnlMap.TabIndex = 1;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(10, 320);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 25;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // MapConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMap);
            this.Controls.Add(this.pnlSearch);
            this.Name = "MapConsole";
            this.Size = new System.Drawing.Size(754, 426);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlProgress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTracker)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel pnlSearch;
        public System.Windows.Forms.Panel pnlMap;
        public System.Windows.Forms.TextBox txtRegion;
        public ListViewNoFlicker lstRegions;
        public System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.NumericUpDown nudX;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown nudY;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown nudZ;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Button btnTeleport;
        public System.Windows.Forms.Panel pnlProgress;
        public System.Windows.Forms.ProgressBar prgTeleport;
        public System.Windows.Forms.Button btnGoHome;
        public System.Windows.Forms.Button btnDestination;
        public System.Windows.Forms.Button btnMyPos;
        public System.Windows.Forms.Label lblStatus;
        public System.Windows.Forms.TrackBar zoomTracker;
        public System.Windows.Forms.ComboBox ddOnlineFriends;
        public System.Windows.Forms.Button btnRefresh;

    }
}