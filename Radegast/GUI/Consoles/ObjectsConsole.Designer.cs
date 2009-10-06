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
            this.btnWalkTo = new System.Windows.Forms.Button();
            this.btnTurnTo = new System.Windows.Forms.Button();
            this.btnBuy = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.btnPay = new System.Windows.Forms.Button();
            this.btnSource = new System.Windows.Forms.Button();
            this.btnTouch = new System.Windows.Forms.Button();
            this.btnSitOn = new System.Windows.Forms.Button();
            this.btnPointAt = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.lstPrims = new Radegast.ListViewNoFlicker();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ctxMenuObjects = new Radegast.RadegastContextMenuStrip(this.components);
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
            this.gbxInworld.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.gbxObjectDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxInworld
            // 
            this.gbxInworld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxInworld.Controls.Add(this.btnWalkTo);
            this.gbxInworld.Controls.Add(this.btnTurnTo);
            this.gbxInworld.Controls.Add(this.btnBuy);
            this.gbxInworld.Controls.Add(this.btnView);
            this.gbxInworld.Controls.Add(this.btnPay);
            this.gbxInworld.Controls.Add(this.btnSource);
            this.gbxInworld.Controls.Add(this.btnTouch);
            this.gbxInworld.Controls.Add(this.btnSitOn);
            this.gbxInworld.Controls.Add(this.btnPointAt);
            this.gbxInworld.Enabled = false;
            this.gbxInworld.Location = new System.Drawing.Point(384, 15);
            this.gbxInworld.Name = "gbxInworld";
            this.gbxInworld.Size = new System.Drawing.Size(255, 149);
            this.gbxInworld.TabIndex = 2;
            this.gbxInworld.TabStop = false;
            this.gbxInworld.Text = "In-world";
            // 
            // btnWalkTo
            // 
            this.btnWalkTo.Location = new System.Drawing.Point(89, 78);
            this.btnWalkTo.Name = "btnWalkTo";
            this.btnWalkTo.Size = new System.Drawing.Size(75, 23);
            this.btnWalkTo.TabIndex = 11;
            this.btnWalkTo.Text = "Walk to";
            this.btnWalkTo.UseVisualStyleBackColor = true;
            this.btnWalkTo.Click += new System.EventHandler(this.btnWalkTo_Click);
            // 
            // btnTurnTo
            // 
            this.btnTurnTo.Location = new System.Drawing.Point(5, 78);
            this.btnTurnTo.Name = "btnTurnTo";
            this.btnTurnTo.Size = new System.Drawing.Size(78, 23);
            this.btnTurnTo.TabIndex = 10;
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
            this.btnBuy.TabIndex = 6;
            this.btnBuy.Text = "Buy";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(172, 78);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(77, 23);
            this.btnView.TabIndex = 12;
            this.btnView.Text = "3D Wireframe";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnPay
            // 
            this.btnPay.Location = new System.Drawing.Point(89, 20);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(77, 23);
            this.btnPay.TabIndex = 5;
            this.btnPay.Text = "Pay";
            this.btnPay.UseVisualStyleBackColor = true;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // btnSource
            // 
            this.btnSource.Location = new System.Drawing.Point(172, 49);
            this.btnSource.Name = "btnSource";
            this.btnSource.Size = new System.Drawing.Size(77, 23);
            this.btnSource.TabIndex = 9;
            this.btnSource.Text = "Set source";
            this.btnSource.UseVisualStyleBackColor = true;
            this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
            // 
            // btnTouch
            // 
            this.btnTouch.Location = new System.Drawing.Point(6, 20);
            this.btnTouch.Name = "btnTouch";
            this.btnTouch.Size = new System.Drawing.Size(77, 23);
            this.btnTouch.TabIndex = 4;
            this.btnTouch.Text = "Touch/Click";
            this.btnTouch.UseVisualStyleBackColor = true;
            this.btnTouch.Click += new System.EventHandler(this.btnTouch_Click);
            // 
            // btnSitOn
            // 
            this.btnSitOn.Location = new System.Drawing.Point(6, 49);
            this.btnSitOn.Name = "btnSitOn";
            this.btnSitOn.Size = new System.Drawing.Size(77, 23);
            this.btnSitOn.TabIndex = 7;
            this.btnSitOn.Text = "Sit On";
            this.btnSitOn.UseVisualStyleBackColor = true;
            this.btnSitOn.Click += new System.EventHandler(this.btnSitOn_Click);
            // 
            // btnPointAt
            // 
            this.btnPointAt.Location = new System.Drawing.Point(89, 49);
            this.btnPointAt.Name = "btnPointAt";
            this.btnPointAt.Size = new System.Drawing.Size(75, 23);
            this.btnPointAt.TabIndex = 8;
            this.btnPointAt.Text = "Point At";
            this.btnPointAt.UseVisualStyleBackColor = true;
            this.btnPointAt.Click += new System.EventHandler(this.btnPointAt_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(62, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(133, 21);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Search:";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(201, 10);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(54, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lstPrims
            // 
            this.lstPrims.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPrims.AutoArrange = false;
            this.lstPrims.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstPrims.ContextMenuStrip = this.ctxMenuObjects;
            this.lstPrims.FullRowSelect = true;
            this.lstPrims.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstPrims.HideSelection = false;
            this.lstPrims.LabelWrap = false;
            this.lstPrims.Location = new System.Drawing.Point(12, 39);
            this.lstPrims.MultiSelect = false;
            this.lstPrims.Name = "lstPrims";
            this.lstPrims.ShowGroups = false;
            this.lstPrims.Size = new System.Drawing.Size(365, 353);
            this.lstPrims.TabIndex = 10;
            this.lstPrims.UseCompatibleStateImageBehavior = false;
            this.lstPrims.View = System.Windows.Forms.View.Details;
            this.lstPrims.SelectedIndexChanged += new System.EventHandler(this.lstPrims_SelectedIndexChanged);
            this.lstPrims.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstPrims_MouseUp);
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
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(539, 369);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 23);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // nudRadius
            // 
            this.nudRadius.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudRadius.Location = new System.Drawing.Point(261, 12);
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
            this.nudRadius.KeyUp += new System.Windows.Forms.KeyEventHandler(this.nudRadius_KeyUp);
            this.nudRadius.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudRadius_KeyDown);
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(318, 15);
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
            this.groupBox1.Location = new System.Drawing.Point(383, 356);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(149, 37);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sort by";
            // 
            // rbName
            // 
            this.rbName.AutoSize = true;
            this.rbName.Location = new System.Drawing.Point(78, 17);
            this.rbName.Name = "rbName";
            this.rbName.Size = new System.Drawing.Size(52, 17);
            this.rbName.TabIndex = 0;
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
            this.rbDistance.TabIndex = 0;
            this.rbDistance.TabStop = true;
            this.rbDistance.Text = "Distance";
            this.rbDistance.UseVisualStyleBackColor = true;
            this.rbDistance.CheckedChanged += new System.EventHandler(this.rbDistance_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 395);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(651, 22);
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
            this.gbxObjectDetails.Location = new System.Drawing.Point(384, 167);
            this.gbxObjectDetails.Name = "gbxObjectDetails";
            this.gbxObjectDetails.Size = new System.Drawing.Size(255, 187);
            this.gbxObjectDetails.TabIndex = 16;
            this.gbxObjectDetails.TabStop = false;
            this.gbxObjectDetails.Text = "Object details";
            // 
            // cbNextOwnTransfer
            // 
            this.cbNextOwnTransfer.AutoSize = true;
            this.cbNextOwnTransfer.Location = new System.Drawing.Point(195, 164);
            this.cbNextOwnTransfer.Name = "cbNextOwnTransfer";
            this.cbNextOwnTransfer.Size = new System.Drawing.Size(54, 17);
            this.cbNextOwnTransfer.TabIndex = 6;
            this.cbNextOwnTransfer.Text = "Resell";
            this.cbNextOwnTransfer.UseVisualStyleBackColor = true;
            // 
            // cbNextOwnCopy
            // 
            this.cbNextOwnCopy.AutoSize = true;
            this.cbNextOwnCopy.Location = new System.Drawing.Point(140, 164);
            this.cbNextOwnCopy.Name = "cbNextOwnCopy";
            this.cbNextOwnCopy.Size = new System.Drawing.Size(51, 17);
            this.cbNextOwnCopy.TabIndex = 6;
            this.cbNextOwnCopy.Text = "Copy";
            this.cbNextOwnCopy.UseVisualStyleBackColor = true;
            // 
            // cbOwnerTransfer
            // 
            this.cbOwnerTransfer.AutoSize = true;
            this.cbOwnerTransfer.Location = new System.Drawing.Point(195, 143);
            this.cbOwnerTransfer.Name = "cbOwnerTransfer";
            this.cbOwnerTransfer.Size = new System.Drawing.Size(54, 17);
            this.cbOwnerTransfer.TabIndex = 6;
            this.cbOwnerTransfer.Text = "Resell";
            this.cbOwnerTransfer.UseVisualStyleBackColor = true;
            // 
            // cbNextOwnModify
            // 
            this.cbNextOwnModify.AutoSize = true;
            this.cbNextOwnModify.Location = new System.Drawing.Point(90, 164);
            this.cbNextOwnModify.Name = "cbNextOwnModify";
            this.cbNextOwnModify.Size = new System.Drawing.Size(46, 17);
            this.cbNextOwnModify.TabIndex = 6;
            this.cbNextOwnModify.Text = "Mod";
            this.cbNextOwnModify.UseVisualStyleBackColor = true;
            // 
            // cbOwnerCopy
            // 
            this.cbOwnerCopy.AutoSize = true;
            this.cbOwnerCopy.Location = new System.Drawing.Point(140, 143);
            this.cbOwnerCopy.Name = "cbOwnerCopy";
            this.cbOwnerCopy.Size = new System.Drawing.Size(51, 17);
            this.cbOwnerCopy.TabIndex = 6;
            this.cbOwnerCopy.Text = "Copy";
            this.cbOwnerCopy.UseVisualStyleBackColor = true;
            // 
            // cbOwnerModify
            // 
            this.cbOwnerModify.AutoSize = true;
            this.cbOwnerModify.Location = new System.Drawing.Point(90, 143);
            this.cbOwnerModify.Name = "cbOwnerModify";
            this.cbOwnerModify.Size = new System.Drawing.Size(46, 17);
            this.cbOwnerModify.TabIndex = 6;
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
            this.txtPrims.TabIndex = 5;
            // 
            // txtCreator
            // 
            this.txtCreator.AgentID = ((OpenMetaverse.UUID)(resources.GetObject("txtCreator.AgentID")));
            this.txtCreator.BackColor = System.Drawing.SystemColors.Window;
            this.txtCreator.Location = new System.Drawing.Point(61, 116);
            this.txtCreator.Name = "txtCreator";
            this.txtCreator.ReadOnly = true;
            this.txtCreator.Size = new System.Drawing.Size(130, 21);
            this.txtCreator.TabIndex = 5;
            // 
            // txtOwner
            // 
            this.txtOwner.AgentID = ((OpenMetaverse.UUID)(resources.GetObject("txtOwner.AgentID")));
            this.txtOwner.BackColor = System.Drawing.SystemColors.Window;
            this.txtOwner.Location = new System.Drawing.Point(61, 91);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.ReadOnly = true;
            this.txtOwner.Size = new System.Drawing.Size(130, 21);
            this.txtOwner.TabIndex = 4;
            // 
            // txtHover
            // 
            this.txtHover.Location = new System.Drawing.Point(61, 66);
            this.txtHover.Name = "txtHover";
            this.txtHover.Size = new System.Drawing.Size(188, 21);
            this.txtHover.TabIndex = 3;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(61, 41);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(188, 21);
            this.txtDescription.TabIndex = 2;
            // 
            // txtObjectName
            // 
            this.txtObjectName.Location = new System.Drawing.Point(61, 16);
            this.txtObjectName.Name = "txtObjectName";
            this.txtObjectName.Size = new System.Drawing.Size(188, 21);
            this.txtObjectName.TabIndex = 1;
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
            // ObjectsConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxObjectDetails);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.nudRadius);
            this.Controls.Add(this.lstPrims);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblDistance);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.gbxInworld);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(508, 417);
            this.Name = "ObjectsConsole";
            this.Size = new System.Drawing.Size(651, 417);
            this.gbxInworld.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbxObjectDetails.ResumeLayout(false);
            this.gbxObjectDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.GroupBox gbxInworld;
        public System.Windows.Forms.Button btnSitOn;
        public System.Windows.Forms.Button btnPointAt;
        public System.Windows.Forms.Button btnTouch;
        public System.Windows.Forms.TextBox txtSearch;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnClear;
        public ListViewNoFlicker lstPrims;
        public System.Windows.Forms.ColumnHeader columnHeader1;
        public System.Windows.Forms.Button btnRefresh;
        public System.Windows.Forms.Button btnSource;
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
        public AgentNameTextBox txtCreator;
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

    }
}