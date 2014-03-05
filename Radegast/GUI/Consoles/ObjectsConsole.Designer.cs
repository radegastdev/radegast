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
    partial class ObjectsConsole
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
            if (InvokeRequired) return;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectsConsole));
            this.gbxInworld = new System.Windows.Forms.GroupBox();
            this.btnContents = new System.Windows.Forms.Button();
            this.btnMute = new System.Windows.Forms.Button();
            this.btnTake = new System.Windows.Forms.Button();
            this.btnWalkTo = new System.Windows.Forms.Button();
            this.btnTurnTo = new System.Windows.Forms.Button();
            this.btnBuy = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.btnPay = new System.Windows.Forms.Button();
            this.btnTouch = new System.Windows.Forms.Button();
            this.btnSitOn = new System.Windows.Forms.Button();
            this.btnPointAt = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.nudRadius = new System.Windows.Forms.NumericUpDown();
            this.lblDistance = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbName = new System.Windows.Forms.RadioButton();
            this.rbDistance = new System.Windows.Forms.RadioButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbxObjectDetails = new System.Windows.Forms.GroupBox();
            this.cbNextOwnTransfer = new System.Windows.Forms.CheckBox();
            this.cbNextOwnCopy = new System.Windows.Forms.CheckBox();
            this.cbOwnerTransfer = new System.Windows.Forms.CheckBox();
            this.cbNextOwnModify = new System.Windows.Forms.CheckBox();
            this.cbOwnerCopy = new System.Windows.Forms.CheckBox();
            this.cbOwnerModify = new System.Windows.Forms.CheckBox();
            this.txtPrims = new System.Windows.Forms.TextBox();
            this.txtCreator = new Radegast.AgentNameTextBox();
            this.txtOwner = new Radegast.AgentNameTextBox();
            this.txtHover = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtObjectName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.gbxContents = new System.Windows.Forms.GroupBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lstContents = new Radegast.ListViewNoFlicker();
            this.invIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.invName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxContents = new Radegast.RadegastContextMenuStrip(this.components);
            this.btnCloseContents = new System.Windows.Forms.Button();
            this.pnlList = new System.Windows.Forms.Panel();
            this.lstPrims = new Radegast.ListViewNoFlicker();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxMenuObjects = new Radegast.RadegastContextMenuStrip(this.components);
            this.lstChildren = new Radegast.ListViewNoFlicker();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.comboFilter = new System.Windows.Forms.ComboBox();
            this.ctxopen = new Radegast.RadegastContextMenuStrip(this.components);
            this.gbxInworld.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.gbxObjectDetails.SuspendLayout();
            this.gbxContents.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.gbSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxInworld
            // 
            this.gbxInworld.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.gbxInworld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxInworld.Controls.Add(this.btnContents);
            this.gbxInworld.Controls.Add(this.btnMute);
            this.gbxInworld.Controls.Add(this.btnTake);
            this.gbxInworld.Controls.Add(this.btnWalkTo);
            this.gbxInworld.Controls.Add(this.btnTurnTo);
            this.gbxInworld.Controls.Add(this.btnBuy);
            this.gbxInworld.Controls.Add(this.btnView);
            this.gbxInworld.Controls.Add(this.btnPay);
            this.gbxInworld.Controls.Add(this.btnTouch);
            this.gbxInworld.Controls.Add(this.btnSitOn);
            this.gbxInworld.Controls.Add(this.btnPointAt);
            this.gbxInworld.Enabled = false;
            this.gbxInworld.Location = new System.Drawing.Point(443, 3);
            this.gbxInworld.Name = "gbxInworld";
            this.gbxInworld.Size = new System.Drawing.Size(255, 133);
            this.gbxInworld.TabIndex = 4;
            this.gbxInworld.TabStop = false;
            this.gbxInworld.Text = "In-world";
            // 
            // btnContents
            // 
            this.btnContents.Location = new System.Drawing.Point(89, 107);
            this.btnContents.Name = "btnContents";
            this.btnContents.Size = new System.Drawing.Size(158, 23);
            this.btnContents.TabIndex = 16;
            this.btnContents.Text = "Contents";
            this.btnContents.UseVisualStyleBackColor = true;
            this.btnContents.Click += new System.EventHandler(this.btnContents_Click);
            // 
            // btnMute
            // 
            this.btnMute.Location = new System.Drawing.Point(6, 107);
            this.btnMute.Name = "btnMute";
            this.btnMute.Size = new System.Drawing.Size(77, 23);
            this.btnMute.TabIndex = 15;
            this.btnMute.Text = "Mute";
            this.btnMute.UseVisualStyleBackColor = true;
            this.btnMute.Click += new System.EventHandler(this.btnMute_Click);
            // 
            // btnTake
            // 
            this.btnTake.Location = new System.Drawing.Point(170, 49);
            this.btnTake.Name = "btnTake";
            this.btnTake.Size = new System.Drawing.Size(77, 23);
            this.btnTake.TabIndex = 10;
            this.btnTake.Text = "Take";
            this.btnTake.UseVisualStyleBackColor = true;
            this.btnTake.Click += new System.EventHandler(this.btnTake_Click);
            // 
            // btnWalkTo
            // 
            this.btnWalkTo.Location = new System.Drawing.Point(89, 78);
            this.btnWalkTo.Name = "btnWalkTo";
            this.btnWalkTo.Size = new System.Drawing.Size(75, 23);
            this.btnWalkTo.TabIndex = 12;
            this.btnWalkTo.Text = "Walk to";
            this.btnWalkTo.UseVisualStyleBackColor = true;
            this.btnWalkTo.Click += new System.EventHandler(this.btnWalkTo_Click);
            // 
            // btnTurnTo
            // 
            this.btnTurnTo.Location = new System.Drawing.Point(5, 78);
            this.btnTurnTo.Name = "btnTurnTo";
            this.btnTurnTo.Size = new System.Drawing.Size(78, 23);
            this.btnTurnTo.TabIndex = 11;
            this.btnTurnTo.Text = "Turn to";
            this.btnTurnTo.UseVisualStyleBackColor = true;
            this.btnTurnTo.Click += new System.EventHandler(this.btnTurnTo_Click);
            // 
            // btnBuy
            // 
            this.btnBuy.Enabled = false;
            this.btnBuy.Location = new System.Drawing.Point(172, 20);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(77, 23);
            this.btnBuy.TabIndex = 7;
            this.btnBuy.Text = "Buy";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(172, 78);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(77, 23);
            this.btnView.TabIndex = 13;
            this.btnView.Text = "3D View";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnPay
            // 
            this.btnPay.Location = new System.Drawing.Point(89, 20);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(77, 23);
            this.btnPay.TabIndex = 6;
            this.btnPay.Text = "Pay";
            this.btnPay.UseVisualStyleBackColor = true;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // btnTouch
            // 
            this.btnTouch.Location = new System.Drawing.Point(6, 20);
            this.btnTouch.Name = "btnTouch";
            this.btnTouch.Size = new System.Drawing.Size(77, 23);
            this.btnTouch.TabIndex = 5;
            this.btnTouch.Text = "Touch/Click";
            this.btnTouch.UseVisualStyleBackColor = true;
            this.btnTouch.Click += new System.EventHandler(this.btnTouch_Click);
            // 
            // btnSitOn
            // 
            this.btnSitOn.Location = new System.Drawing.Point(6, 49);
            this.btnSitOn.Name = "btnSitOn";
            this.btnSitOn.Size = new System.Drawing.Size(77, 23);
            this.btnSitOn.TabIndex = 8;
            this.btnSitOn.Text = "Sit On";
            this.btnSitOn.UseVisualStyleBackColor = true;
            this.btnSitOn.Click += new System.EventHandler(this.btnSitOn_Click);
            // 
            // btnPointAt
            // 
            this.btnPointAt.Location = new System.Drawing.Point(89, 49);
            this.btnPointAt.Name = "btnPointAt";
            this.btnPointAt.Size = new System.Drawing.Size(75, 23);
            this.btnPointAt.TabIndex = 9;
            this.btnPointAt.Text = "Point At";
            this.btnPointAt.UseVisualStyleBackColor = true;
            this.btnPointAt.Click += new System.EventHandler(this.btnPointAt_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.AccessibleDescription = "";
            this.txtSearch.AccessibleName = "Search filter";
            this.txtSearch.Location = new System.Drawing.Point(6, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(133, 21);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(145, 13);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(54, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(598, 408);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 23);
            this.btnRefresh.TabIndex = 73;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // nudRadius
            // 
            this.nudRadius.AccessibleName = "Search Radius";
            this.nudRadius.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudRadius.Location = new System.Drawing.Point(205, 15);
            this.nudRadius.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudRadius.Name = "nudRadius";
            this.nudRadius.Size = new System.Drawing.Size(51, 21);
            this.nudRadius.TabIndex = 3;
            this.nudRadius.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.nudRadius.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudRadius_KeyDown);
            this.nudRadius.KeyUp += new System.Windows.Forms.KeyEventHandler(this.nudRadius_KeyUp);
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(262, 18);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(55, 13);
            this.lblDistance.TabIndex = 7;
            this.lblDistance.Text = "radius (m)";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rbName);
            this.groupBox1.Controls.Add(this.rbDistance);
            this.groupBox1.Location = new System.Drawing.Point(442, 395);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(149, 37);
            this.groupBox1.TabIndex = 70;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sort by";
            // 
            // rbName
            // 
            this.rbName.AutoSize = true;
            this.rbName.Location = new System.Drawing.Point(78, 17);
            this.rbName.Name = "rbName";
            this.rbName.Size = new System.Drawing.Size(52, 17);
            this.rbName.TabIndex = 72;
            this.rbName.Text = "Name";
            this.rbName.UseVisualStyleBackColor = true;
            this.rbName.CheckedChanged += new System.EventHandler(this.rbName_CheckedChanged);
            // 
            // rbDistance
            // 
            this.rbDistance.AutoSize = true;
            this.rbDistance.Checked = true;
            this.rbDistance.Location = new System.Drawing.Point(6, 17);
            this.rbDistance.Name = "rbDistance";
            this.rbDistance.Size = new System.Drawing.Size(66, 17);
            this.rbDistance.TabIndex = 71;
            this.rbDistance.TabStop = true;
            this.rbDistance.Text = "Distance";
            this.rbDistance.UseVisualStyleBackColor = true;
            this.rbDistance.CheckedChanged += new System.EventHandler(this.rbDistance_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 434);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(710, 22);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(62, 17);
            this.lblStatus.Text = "Tracking...";
            // 
            // gbxObjectDetails
            // 
            this.gbxObjectDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxObjectDetails.Controls.Add(this.cbNextOwnTransfer);
            this.gbxObjectDetails.Controls.Add(this.cbNextOwnCopy);
            this.gbxObjectDetails.Controls.Add(this.cbOwnerTransfer);
            this.gbxObjectDetails.Controls.Add(this.cbNextOwnModify);
            this.gbxObjectDetails.Controls.Add(this.cbOwnerCopy);
            this.gbxObjectDetails.Controls.Add(this.cbOwnerModify);
            this.gbxObjectDetails.Controls.Add(this.txtPrims);
            this.gbxObjectDetails.Controls.Add(this.txtCreator);
            this.gbxObjectDetails.Controls.Add(this.txtOwner);
            this.gbxObjectDetails.Controls.Add(this.txtHover);
            this.gbxObjectDetails.Controls.Add(this.txtDescription);
            this.gbxObjectDetails.Controls.Add(this.txtObjectName);
            this.gbxObjectDetails.Controls.Add(this.label8);
            this.gbxObjectDetails.Controls.Add(this.label7);
            this.gbxObjectDetails.Controls.Add(this.label6);
            this.gbxObjectDetails.Controls.Add(this.label5);
            this.gbxObjectDetails.Controls.Add(this.label4);
            this.gbxObjectDetails.Controls.Add(this.label3);
            this.gbxObjectDetails.Controls.Add(this.label2);
            this.gbxObjectDetails.Controls.Add(this.lblName);
            this.gbxObjectDetails.Location = new System.Drawing.Point(443, 206);
            this.gbxObjectDetails.Name = "gbxObjectDetails";
            this.gbxObjectDetails.Size = new System.Drawing.Size(255, 187);
            this.gbxObjectDetails.TabIndex = 49;
            this.gbxObjectDetails.TabStop = false;
            this.gbxObjectDetails.Text = "Object details";
            // 
            // cbNextOwnTransfer
            // 
            this.cbNextOwnTransfer.AutoSize = true;
            this.cbNextOwnTransfer.Enabled = false;
            this.cbNextOwnTransfer.Location = new System.Drawing.Point(195, 164);
            this.cbNextOwnTransfer.Name = "cbNextOwnTransfer";
            this.cbNextOwnTransfer.Size = new System.Drawing.Size(54, 17);
            this.cbNextOwnTransfer.TabIndex = 61;
            this.cbNextOwnTransfer.Text = "Resell";
            this.cbNextOwnTransfer.UseVisualStyleBackColor = true;
            this.cbNextOwnTransfer.CheckedChanged += new System.EventHandler(this.cbNextOwnerUpdate_CheckedChanged);
            // 
            // cbNextOwnCopy
            // 
            this.cbNextOwnCopy.AutoSize = true;
            this.cbNextOwnCopy.Enabled = false;
            this.cbNextOwnCopy.Location = new System.Drawing.Point(140, 164);
            this.cbNextOwnCopy.Name = "cbNextOwnCopy";
            this.cbNextOwnCopy.Size = new System.Drawing.Size(51, 17);
            this.cbNextOwnCopy.TabIndex = 60;
            this.cbNextOwnCopy.Text = "Copy";
            this.cbNextOwnCopy.UseVisualStyleBackColor = true;
            this.cbNextOwnCopy.CheckedChanged += new System.EventHandler(this.cbNextOwnerUpdate_CheckedChanged);
            // 
            // cbOwnerTransfer
            // 
            this.cbOwnerTransfer.AutoSize = true;
            this.cbOwnerTransfer.Enabled = false;
            this.cbOwnerTransfer.Location = new System.Drawing.Point(195, 143);
            this.cbOwnerTransfer.Name = "cbOwnerTransfer";
            this.cbOwnerTransfer.Size = new System.Drawing.Size(54, 17);
            this.cbOwnerTransfer.TabIndex = 58;
            this.cbOwnerTransfer.Text = "Resell";
            this.cbOwnerTransfer.UseVisualStyleBackColor = true;
            // 
            // cbNextOwnModify
            // 
            this.cbNextOwnModify.AutoSize = true;
            this.cbNextOwnModify.Enabled = false;
            this.cbNextOwnModify.Location = new System.Drawing.Point(90, 164);
            this.cbNextOwnModify.Name = "cbNextOwnModify";
            this.cbNextOwnModify.Size = new System.Drawing.Size(46, 17);
            this.cbNextOwnModify.TabIndex = 59;
            this.cbNextOwnModify.Text = "Mod";
            this.cbNextOwnModify.UseVisualStyleBackColor = true;
            this.cbNextOwnModify.CheckedChanged += new System.EventHandler(this.cbNextOwnerUpdate_CheckedChanged);
            // 
            // cbOwnerCopy
            // 
            this.cbOwnerCopy.AutoSize = true;
            this.cbOwnerCopy.Enabled = false;
            this.cbOwnerCopy.Location = new System.Drawing.Point(140, 143);
            this.cbOwnerCopy.Name = "cbOwnerCopy";
            this.cbOwnerCopy.Size = new System.Drawing.Size(51, 17);
            this.cbOwnerCopy.TabIndex = 57;
            this.cbOwnerCopy.Text = "Copy";
            this.cbOwnerCopy.UseVisualStyleBackColor = true;
            // 
            // cbOwnerModify
            // 
            this.cbOwnerModify.AutoSize = true;
            this.cbOwnerModify.Enabled = false;
            this.cbOwnerModify.Location = new System.Drawing.Point(90, 143);
            this.cbOwnerModify.Name = "cbOwnerModify";
            this.cbOwnerModify.Size = new System.Drawing.Size(46, 17);
            this.cbOwnerModify.TabIndex = 56;
            this.cbOwnerModify.Text = "Mod";
            this.cbOwnerModify.UseVisualStyleBackColor = true;
            // 
            // txtPrims
            // 
            this.txtPrims.BackColor = System.Drawing.SystemColors.Window;
            this.txtPrims.Location = new System.Drawing.Point(195, 116);
            this.txtPrims.Name = "txtPrims";
            this.txtPrims.ReadOnly = true;
            this.txtPrims.Size = new System.Drawing.Size(54, 21);
            this.txtPrims.TabIndex = 55;
            // 
            // txtCreator
            // 
            this.txtCreator.BackColor = System.Drawing.SystemColors.Window;
            this.txtCreator.Location = new System.Drawing.Point(61, 116);
            this.txtCreator.Name = "txtCreator";
            this.txtCreator.ReadOnly = true;
            this.txtCreator.Size = new System.Drawing.Size(130, 21);
            this.txtCreator.TabIndex = 54;
            // 
            // txtOwner
            // 
            this.txtOwner.BackColor = System.Drawing.SystemColors.Window;
            this.txtOwner.Location = new System.Drawing.Point(61, 91);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.ReadOnly = true;
            this.txtOwner.Size = new System.Drawing.Size(130, 21);
            this.txtOwner.TabIndex = 53;
            // 
            // txtHover
            // 
            this.txtHover.BackColor = System.Drawing.SystemColors.Window;
            this.txtHover.Location = new System.Drawing.Point(61, 66);
            this.txtHover.Name = "txtHover";
            this.txtHover.ReadOnly = true;
            this.txtHover.Size = new System.Drawing.Size(188, 21);
            this.txtHover.TabIndex = 52;
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.SystemColors.Window;
            this.txtDescription.Location = new System.Drawing.Point(61, 41);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.Size = new System.Drawing.Size(188, 21);
            this.txtDescription.TabIndex = 51;
            this.txtDescription.Leave += new System.EventHandler(this.txtDescription_Leave);
            // 
            // txtObjectName
            // 
            this.txtObjectName.BackColor = System.Drawing.SystemColors.Window;
            this.txtObjectName.Location = new System.Drawing.Point(61, 16);
            this.txtObjectName.Name = "txtObjectName";
            this.txtObjectName.ReadOnly = true;
            this.txtObjectName.Size = new System.Drawing.Size(188, 21);
            this.txtObjectName.TabIndex = 50;
            this.txtObjectName.Leave += new System.EventHandler(this.txtObjectName_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 144);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Owner perm.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Next own perm.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(217, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Prims";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Creator";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Owner";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Hovertext";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Desc.";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 18);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(34, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // gbxContents
            // 
            this.gbxContents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxContents.Controls.Add(this.btnOpen);
            this.gbxContents.Controls.Add(this.lstContents);
            this.gbxContents.Controls.Add(this.btnCloseContents);
            this.gbxContents.Location = new System.Drawing.Point(442, 3);
            this.gbxContents.Name = "gbxContents";
            this.gbxContents.Size = new System.Drawing.Size(255, 185);
            this.gbxContents.TabIndex = 16;
            this.gbxContents.TabStop = false;
            this.gbxContents.Text = "Contents";
            this.gbxContents.Visible = false;
            // 
            // btnOpen
            // 
            this.btnOpen.AccessibleDescription = "Copy object\'s contents to invetory";
            this.btnOpen.AccessibleName = "Open";
            this.btnOpen.Enabled = false;
            this.btnOpen.Location = new System.Drawing.Point(99, 156);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 19;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.OpenObject);
            // 
            // lstContents
            // 
            this.lstContents.AccessibleName = "Contents";
            this.lstContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstContents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.invIcon,
            this.invName});
            this.lstContents.ContextMenuStrip = this.ctxContents;
            this.lstContents.FullRowSelect = true;
            this.lstContents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstContents.HideSelection = false;
            this.lstContents.Location = new System.Drawing.Point(9, 20);
            this.lstContents.Name = "lstContents";
            this.lstContents.ShowGroups = false;
            this.lstContents.Size = new System.Drawing.Size(246, 130);
            this.lstContents.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstContents.TabIndex = 17;
            this.lstContents.UseCompatibleStateImageBehavior = false;
            this.lstContents.View = System.Windows.Forms.View.Details;
            this.lstContents.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstContents_KeyDown);
            this.lstContents.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstContents_MouseDoubleClick);
            // 
            // invIcon
            // 
            this.invIcon.Text = "";
            this.invIcon.Width = 25;
            // 
            // invName
            // 
            this.invName.Text = "";
            this.invName.Width = 196;
            // 
            // ctxContents
            // 
            this.ctxContents.Name = "ctxContents";
            this.ctxContents.Size = new System.Drawing.Size(61, 4);
            this.ctxContents.Opening += new System.ComponentModel.CancelEventHandler(this.ctxContents_Opening);
            // 
            // btnCloseContents
            // 
            this.btnCloseContents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseContents.Location = new System.Drawing.Point(180, 156);
            this.btnCloseContents.Name = "btnCloseContents";
            this.btnCloseContents.Size = new System.Drawing.Size(75, 23);
            this.btnCloseContents.TabIndex = 18;
            this.btnCloseContents.Text = "Back";
            this.btnCloseContents.UseVisualStyleBackColor = true;
            this.btnCloseContents.Click += new System.EventHandler(this.btnCloseContents_Click);
            // 
            // pnlList
            // 
            this.pnlList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlList.Controls.Add(this.lstPrims);
            this.pnlList.Controls.Add(this.lstChildren);
            this.pnlList.Location = new System.Drawing.Point(15, 52);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(417, 377);
            this.pnlList.TabIndex = 74;
            // 
            // lstPrims
            // 
            this.lstPrims.AccessibleName = "Objects";
            this.lstPrims.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstPrims.ContextMenuStrip = this.ctxMenuObjects;
            this.lstPrims.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstPrims.FullRowSelect = true;
            this.lstPrims.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstPrims.HideSelection = false;
            this.lstPrims.LabelWrap = false;
            this.lstPrims.Location = new System.Drawing.Point(0, 0);
            this.lstPrims.MultiSelect = false;
            this.lstPrims.Name = "lstPrims";
            this.lstPrims.ShowGroups = false;
            this.lstPrims.Size = new System.Drawing.Size(417, 245);
            this.lstPrims.TabIndex = 0;
            this.lstPrims.UseCompatibleStateImageBehavior = false;
            this.lstPrims.View = System.Windows.Forms.View.Details;
            this.lstPrims.VirtualMode = true;
            this.lstPrims.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lstPrims_RetrieveVirtualItem);
            this.lstPrims.SelectedIndexChanged += new System.EventHandler(this.lstPrims_SelectedIndexChanged);
            this.lstPrims.DoubleClick += new System.EventHandler(this.lstPrims_DoubleClick);
            this.lstPrims.Enter += new System.EventHandler(this.lstPrims_Enter);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 340;
            // 
            // ctxMenuObjects
            // 
            this.ctxMenuObjects.Name = "ctxMenuObjects";
            this.ctxMenuObjects.Size = new System.Drawing.Size(61, 4);
            this.ctxMenuObjects.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMenuObjects_Opening);
            // 
            // lstChildren
            // 
            this.lstChildren.AccessibleName = "Objects";
            this.lstChildren.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lstChildren.ContextMenuStrip = this.ctxMenuObjects;
            this.lstChildren.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lstChildren.FullRowSelect = true;
            this.lstChildren.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstChildren.HideSelection = false;
            this.lstChildren.LabelWrap = false;
            this.lstChildren.Location = new System.Drawing.Point(0, 245);
            this.lstChildren.MultiSelect = false;
            this.lstChildren.Name = "lstChildren";
            this.lstChildren.ShowGroups = false;
            this.lstChildren.Size = new System.Drawing.Size(417, 132);
            this.lstChildren.TabIndex = 1;
            this.lstChildren.UseCompatibleStateImageBehavior = false;
            this.lstChildren.View = System.Windows.Forms.View.Details;
            this.lstChildren.Visible = false;
            this.lstChildren.SelectedIndexChanged += new System.EventHandler(this.lstChildren_SelectedIndexChanged);
            this.lstChildren.Enter += new System.EventHandler(this.lstChildren_Enter);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 340;
            // 
            // gbSearch
            // 
            this.gbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSearch.Controls.Add(this.comboFilter);
            this.gbSearch.Controls.Add(this.txtSearch);
            this.gbSearch.Controls.Add(this.lblDistance);
            this.gbSearch.Controls.Add(this.btnClear);
            this.gbSearch.Controls.Add(this.nudRadius);
            this.gbSearch.Location = new System.Drawing.Point(16, 3);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(416, 43);
            this.gbSearch.TabIndex = 1;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Search";
            // 
            // comboFilter
            // 
            this.comboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFilter.FormattingEnabled = true;
            this.comboFilter.Items.AddRange(new object[] {
            "Rezzed",
            "Attached",
            "Both"});
            this.comboFilter.Location = new System.Drawing.Point(323, 15);
            this.comboFilter.Name = "comboFilter";
            this.comboFilter.Size = new System.Drawing.Size(84, 21);
            this.comboFilter.TabIndex = 4;
            // 
            // ctxopen
            // 
            this.ctxopen.Name = "ctxMenuObjects";
            this.ctxopen.Size = new System.Drawing.Size(61, 4);
            // 
            // ObjectsConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbSearch);
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.gbxObjectDetails);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.gbxInworld);
            this.Controls.Add(this.gbxContents);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(508, 417);
            this.Name = "ObjectsConsole";
            this.Size = new System.Drawing.Size(710, 456);
            this.gbxInworld.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbxObjectDetails.ResumeLayout(false);
            this.gbxObjectDetails.PerformLayout();
            this.gbxContents.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.GroupBox gbxInworld;
        public System.Windows.Forms.Button btnSitOn;
        public System.Windows.Forms.Button btnPointAt;
        public System.Windows.Forms.Button btnTouch;
        public System.Windows.Forms.TextBox txtSearch;
        public System.Windows.Forms.Button btnClear;
        public ListViewNoFlicker lstPrims;
        public System.Windows.Forms.ColumnHeader columnHeader1;
        public System.Windows.Forms.Button btnRefresh;
        public System.Windows.Forms.Button btnPay;
        public System.Windows.Forms.Button btnView;
        public System.Windows.Forms.NumericUpDown nudRadius;
        public System.Windows.Forms.Label lblDistance;
        public System.Windows.Forms.Button btnBuy;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.RadioButton rbName;
        public System.Windows.Forms.RadioButton rbDistance;
        public System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripStatusLabel lblStatus;
        public System.Windows.Forms.Button btnWalkTo;
        public System.Windows.Forms.Button btnTurnTo;
        public System.Windows.Forms.GroupBox gbxObjectDetails;
        public System.Windows.Forms.Label lblName;
        public System.Windows.Forms.TextBox txtDescription;
        public System.Windows.Forms.TextBox txtObjectName;
        public System.Windows.Forms.TextBox txtHover;
        public System.Windows.Forms.Label label2;
        public AgentNameTextBox txtOwner;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox cbOwnerTransfer;
        public System.Windows.Forms.CheckBox cbOwnerCopy;
        public System.Windows.Forms.CheckBox cbOwnerModify;
        public System.Windows.Forms.CheckBox cbNextOwnTransfer;
        public System.Windows.Forms.CheckBox cbNextOwnCopy;
        public System.Windows.Forms.CheckBox cbNextOwnModify;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox txtPrims;
        public RadegastContextMenuStrip ctxMenuObjects;
        public System.Windows.Forms.Button btnTake;
        public RadegastContextMenuStrip ctxopen;
        public ListViewNoFlicker lstContents;
        public System.Windows.Forms.ColumnHeader invIcon;
        public System.Windows.Forms.ColumnHeader invName;
        public System.Windows.Forms.Button btnMute;
        public AgentNameTextBox txtCreator;
        public ListViewNoFlicker lstChildren;
        public System.Windows.Forms.ColumnHeader columnHeader2;
        public System.Windows.Forms.ComboBox comboFilter;
        public System.Windows.Forms.GroupBox gbxContents;
        public System.Windows.Forms.Button btnCloseContents;
        public System.Windows.Forms.Button btnContents;
        public RadegastContextMenuStrip ctxContents;
        public System.Windows.Forms.Button btnOpen;
        public System.Windows.Forms.Panel pnlList;
        public System.Windows.Forms.GroupBox gbSearch;

    }
}