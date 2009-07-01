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
    partial class GroupDetails
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
            this.tcGroupDetails = new System.Windows.Forms.TabControl();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.lvwGeneralMembers = new System.Windows.Forms.ListView();
            this.chGenMemberName = new System.Windows.Forms.ColumnHeader();
            this.chGenTitle = new System.Windows.Forms.ColumnHeader();
            this.chGenLastOn = new System.Windows.Forms.ColumnHeader();
            this.lblOwners = new System.Windows.Forms.Label();
            this.tbxCharter = new System.Windows.Forms.TextBox();
            this.lblCharter = new System.Windows.Forms.Label();
            this.gbPreferences = new System.Windows.Forms.GroupBox();
            this.cbxShowGroupInList = new System.Windows.Forms.CheckBox();
            this.cbxReceiveNotices = new System.Windows.Forms.CheckBox();
            this.cbxActiveTitle = new System.Windows.Forms.ComboBox();
            this.lblActiveTitle = new System.Windows.Forms.Label();
            this.nudEnrollmentFee = new System.Windows.Forms.NumericUpDown();
            this.cbxMaturity = new System.Windows.Forms.ComboBox();
            this.cbxEnrollmentFee = new System.Windows.Forms.CheckBox();
            this.cbxOpenEnrollment = new System.Windows.Forms.CheckBox();
            this.cbxShowInSearch = new System.Windows.Forms.CheckBox();
            this.lblInsignia = new System.Windows.Forms.Label();
            this.pnlInsignia = new System.Windows.Forms.Panel();
            this.lblFounded = new System.Windows.Forms.Label();
            this.lblGroupName = new System.Windows.Forms.Label();
            this.tpMembersRoles = new System.Windows.Forms.TabPage();
            this.tpNotices = new System.Windows.Forms.TabPage();
            this.pnlBottomControls = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.tcGroupDetails.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.gbPreferences.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnrollmentFee)).BeginInit();
            this.pnlBottomControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcGroupDetails
            // 
            this.tcGroupDetails.Controls.Add(this.tpGeneral);
            this.tcGroupDetails.Controls.Add(this.tpMembersRoles);
            this.tcGroupDetails.Controls.Add(this.tpNotices);
            this.tcGroupDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcGroupDetails.Location = new System.Drawing.Point(0, 0);
            this.tcGroupDetails.Name = "tcGroupDetails";
            this.tcGroupDetails.SelectedIndex = 0;
            this.tcGroupDetails.Size = new System.Drawing.Size(408, 500);
            this.tcGroupDetails.TabIndex = 0;
            // 
            // tpGeneral
            // 
            this.tpGeneral.BackColor = System.Drawing.SystemColors.Control;
            this.tpGeneral.Controls.Add(this.lvwGeneralMembers);
            this.tpGeneral.Controls.Add(this.lblOwners);
            this.tpGeneral.Controls.Add(this.tbxCharter);
            this.tpGeneral.Controls.Add(this.lblCharter);
            this.tpGeneral.Controls.Add(this.gbPreferences);
            this.tpGeneral.Controls.Add(this.lblInsignia);
            this.tpGeneral.Controls.Add(this.pnlInsignia);
            this.tpGeneral.Controls.Add(this.lblFounded);
            this.tpGeneral.Controls.Add(this.lblGroupName);
            this.tpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tpGeneral.Size = new System.Drawing.Size(400, 474);
            this.tpGeneral.TabIndex = 0;
            this.tpGeneral.Text = "General";
            // 
            // lvwGeneralMembers
            // 
            this.lvwGeneralMembers.AllowColumnReorder = true;
            this.lvwGeneralMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwGeneralMembers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chGenMemberName,
            this.chGenTitle,
            this.chGenLastOn});
            this.lvwGeneralMembers.GridLines = true;
            this.lvwGeneralMembers.HideSelection = false;
            this.lvwGeneralMembers.Location = new System.Drawing.Point(9, 232);
            this.lvwGeneralMembers.MultiSelect = false;
            this.lvwGeneralMembers.Name = "lvwGeneralMembers";
            this.lvwGeneralMembers.ShowGroups = false;
            this.lvwGeneralMembers.Size = new System.Drawing.Size(385, 131);
            this.lvwGeneralMembers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwGeneralMembers.TabIndex = 8;
            this.lvwGeneralMembers.UseCompatibleStateImageBehavior = false;
            this.lvwGeneralMembers.View = System.Windows.Forms.View.Details;
            this.lvwGeneralMembers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwGeneralMembers_ColumnClick);
            // 
            // chGenMemberName
            // 
            this.chGenMemberName.Text = "Member name";
            this.chGenMemberName.Width = 130;
            // 
            // chGenTitle
            // 
            this.chGenTitle.Text = "Title";
            this.chGenTitle.Width = 130;
            // 
            // chGenLastOn
            // 
            this.chGenLastOn.Text = "Last login";
            this.chGenLastOn.Width = 90;
            // 
            // lblOwners
            // 
            this.lblOwners.AutoSize = true;
            this.lblOwners.Location = new System.Drawing.Point(6, 213);
            this.lblOwners.Name = "lblOwners";
            this.lblOwners.Size = new System.Drawing.Size(141, 13);
            this.lblOwners.TabIndex = 7;
            this.lblOwners.Text = "Owners and visible members";
            // 
            // tbxCharter
            // 
            this.tbxCharter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxCharter.Location = new System.Drawing.Point(155, 36);
            this.tbxCharter.Multiline = true;
            this.tbxCharter.Name = "tbxCharter";
            this.tbxCharter.Size = new System.Drawing.Size(239, 190);
            this.tbxCharter.TabIndex = 6;
            // 
            // lblCharter
            // 
            this.lblCharter.AutoSize = true;
            this.lblCharter.Location = new System.Drawing.Point(152, 20);
            this.lblCharter.Name = "lblCharter";
            this.lblCharter.Size = new System.Drawing.Size(72, 13);
            this.lblCharter.TabIndex = 5;
            this.lblCharter.Text = "Group charter";
            // 
            // gbPreferences
            // 
            this.gbPreferences.Controls.Add(this.cbxShowGroupInList);
            this.gbPreferences.Controls.Add(this.cbxReceiveNotices);
            this.gbPreferences.Controls.Add(this.cbxActiveTitle);
            this.gbPreferences.Controls.Add(this.lblActiveTitle);
            this.gbPreferences.Controls.Add(this.nudEnrollmentFee);
            this.gbPreferences.Controls.Add(this.cbxMaturity);
            this.gbPreferences.Controls.Add(this.cbxEnrollmentFee);
            this.gbPreferences.Controls.Add(this.cbxOpenEnrollment);
            this.gbPreferences.Controls.Add(this.cbxShowInSearch);
            this.gbPreferences.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbPreferences.Location = new System.Drawing.Point(3, 369);
            this.gbPreferences.Name = "gbPreferences";
            this.gbPreferences.Size = new System.Drawing.Size(394, 102);
            this.gbPreferences.TabIndex = 4;
            this.gbPreferences.TabStop = false;
            this.gbPreferences.Text = "Group Preferences";
            // 
            // cbxShowGroupInList
            // 
            this.cbxShowGroupInList.AutoSize = true;
            this.cbxShowGroupInList.Location = new System.Drawing.Point(211, 82);
            this.cbxShowGroupInList.Name = "cbxShowGroupInList";
            this.cbxShowGroupInList.Size = new System.Drawing.Size(130, 17);
            this.cbxShowGroupInList.TabIndex = 8;
            this.cbxShowGroupInList.Text = "List group in my profile";
            this.cbxShowGroupInList.UseVisualStyleBackColor = true;
            // 
            // cbxReceiveNotices
            // 
            this.cbxReceiveNotices.AutoSize = true;
            this.cbxReceiveNotices.Location = new System.Drawing.Point(211, 59);
            this.cbxReceiveNotices.Name = "cbxReceiveNotices";
            this.cbxReceiveNotices.Size = new System.Drawing.Size(133, 17);
            this.cbxReceiveNotices.TabIndex = 7;
            this.cbxReceiveNotices.Text = "Receive group notices";
            this.cbxReceiveNotices.UseVisualStyleBackColor = true;
            // 
            // cbxActiveTitle
            // 
            this.cbxActiveTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxActiveTitle.FormattingEnabled = true;
            this.cbxActiveTitle.Location = new System.Drawing.Point(211, 32);
            this.cbxActiveTitle.Name = "cbxActiveTitle";
            this.cbxActiveTitle.Size = new System.Drawing.Size(151, 21);
            this.cbxActiveTitle.TabIndex = 6;
            // 
            // lblActiveTitle
            // 
            this.lblActiveTitle.AutoSize = true;
            this.lblActiveTitle.Location = new System.Drawing.Point(208, 16);
            this.lblActiveTitle.Name = "lblActiveTitle";
            this.lblActiveTitle.Size = new System.Drawing.Size(72, 13);
            this.lblActiveTitle.TabIndex = 5;
            this.lblActiveTitle.Text = "My active title";
            // 
            // nudEnrollmentFee
            // 
            this.nudEnrollmentFee.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudEnrollmentFee.Location = new System.Drawing.Point(114, 55);
            this.nudEnrollmentFee.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.nudEnrollmentFee.Name = "nudEnrollmentFee";
            this.nudEnrollmentFee.Size = new System.Drawing.Size(53, 20);
            this.nudEnrollmentFee.TabIndex = 4;
            // 
            // cbxMaturity
            // 
            this.cbxMaturity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMaturity.FormattingEnabled = true;
            this.cbxMaturity.Items.AddRange(new object[] {
            "PG Content",
            "Mature Content",
            "Adult Content"});
            this.cbxMaturity.Location = new System.Drawing.Point(6, 77);
            this.cbxMaturity.Name = "cbxMaturity";
            this.cbxMaturity.Size = new System.Drawing.Size(121, 21);
            this.cbxMaturity.TabIndex = 3;
            // 
            // cbxEnrollmentFee
            // 
            this.cbxEnrollmentFee.AutoSize = true;
            this.cbxEnrollmentFee.Location = new System.Drawing.Point(15, 56);
            this.cbxEnrollmentFee.Name = "cbxEnrollmentFee";
            this.cbxEnrollmentFee.Size = new System.Drawing.Size(93, 17);
            this.cbxEnrollmentFee.TabIndex = 2;
            this.cbxEnrollmentFee.Text = "Enrollment fee";
            this.cbxEnrollmentFee.UseVisualStyleBackColor = true;
            // 
            // cbxOpenEnrollment
            // 
            this.cbxOpenEnrollment.AutoSize = true;
            this.cbxOpenEnrollment.Location = new System.Drawing.Point(6, 38);
            this.cbxOpenEnrollment.Name = "cbxOpenEnrollment";
            this.cbxOpenEnrollment.Size = new System.Drawing.Size(103, 17);
            this.cbxOpenEnrollment.TabIndex = 1;
            this.cbxOpenEnrollment.Text = "Open enrollment";
            this.cbxOpenEnrollment.UseVisualStyleBackColor = true;
            // 
            // cbxShowInSearch
            // 
            this.cbxShowInSearch.AutoSize = true;
            this.cbxShowInSearch.Location = new System.Drawing.Point(6, 19);
            this.cbxShowInSearch.Name = "cbxShowInSearch";
            this.cbxShowInSearch.Size = new System.Drawing.Size(99, 17);
            this.cbxShowInSearch.TabIndex = 0;
            this.cbxShowInSearch.Text = "Show in search";
            this.cbxShowInSearch.UseVisualStyleBackColor = true;
            // 
            // lblInsignia
            // 
            this.lblInsignia.AutoSize = true;
            this.lblInsignia.Location = new System.Drawing.Point(6, 179);
            this.lblInsignia.Name = "lblInsignia";
            this.lblInsignia.Size = new System.Drawing.Size(75, 13);
            this.lblInsignia.TabIndex = 3;
            this.lblInsignia.Text = "Group Insignia";
            // 
            // pnlInsignia
            // 
            this.pnlInsignia.Location = new System.Drawing.Point(9, 36);
            this.pnlInsignia.Name = "pnlInsignia";
            this.pnlInsignia.Size = new System.Drawing.Size(140, 140);
            this.pnlInsignia.TabIndex = 2;
            // 
            // lblFounded
            // 
            this.lblFounded.AutoSize = true;
            this.lblFounded.Location = new System.Drawing.Point(6, 20);
            this.lblFounded.Name = "lblFounded";
            this.lblFounded.Size = new System.Drawing.Size(115, 13);
            this.lblFounded.TabIndex = 1;
            this.lblFounded.Text = "Founded by: Test User";
            // 
            // lblGroupName
            // 
            this.lblGroupName.AutoSize = true;
            this.lblGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGroupName.Location = new System.Drawing.Point(6, 3);
            this.lblGroupName.Name = "lblGroupName";
            this.lblGroupName.Size = new System.Drawing.Size(89, 17);
            this.lblGroupName.TabIndex = 0;
            this.lblGroupName.Text = "Group Name";
            // 
            // tpMembersRoles
            // 
            this.tpMembersRoles.BackColor = System.Drawing.SystemColors.Control;
            this.tpMembersRoles.Location = new System.Drawing.Point(4, 22);
            this.tpMembersRoles.Name = "tpMembersRoles";
            this.tpMembersRoles.Padding = new System.Windows.Forms.Padding(3);
            this.tpMembersRoles.Size = new System.Drawing.Size(400, 474);
            this.tpMembersRoles.TabIndex = 1;
            this.tpMembersRoles.Text = "Members & Roles";
            // 
            // tpNotices
            // 
            this.tpNotices.BackColor = System.Drawing.SystemColors.Control;
            this.tpNotices.Location = new System.Drawing.Point(4, 22);
            this.tpNotices.Name = "tpNotices";
            this.tpNotices.Padding = new System.Windows.Forms.Padding(3);
            this.tpNotices.Size = new System.Drawing.Size(400, 474);
            this.tpNotices.TabIndex = 2;
            this.tpNotices.Text = "Notices";
            // 
            // pnlBottomControls
            // 
            this.pnlBottomControls.Controls.Add(this.btnClose);
            this.pnlBottomControls.Controls.Add(this.btnApply);
            this.pnlBottomControls.Controls.Add(this.btnRefresh);
            this.pnlBottomControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottomControls.Location = new System.Drawing.Point(0, 500);
            this.pnlBottomControls.Name = "pnlBottomControls";
            this.pnlBottomControls.Size = new System.Drawing.Size(408, 34);
            this.pnlBottomControls.TabIndex = 6;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(329, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(248, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(13, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // GroupDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcGroupDetails);
            this.Controls.Add(this.pnlBottomControls);
            this.Name = "GroupDetails";
            this.Size = new System.Drawing.Size(408, 534);
            this.tcGroupDetails.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.gbPreferences.ResumeLayout(false);
            this.gbPreferences.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnrollmentFee)).EndInit();
            this.pnlBottomControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcGroupDetails;
        private System.Windows.Forms.TabPage tpGeneral;
        private System.Windows.Forms.TabPage tpMembersRoles;
        private System.Windows.Forms.TabPage tpNotices;
        private System.Windows.Forms.Panel pnlInsignia;
        private System.Windows.Forms.Label lblFounded;
        private System.Windows.Forms.Label lblGroupName;
        private System.Windows.Forms.Label lblInsignia;
        private System.Windows.Forms.GroupBox gbPreferences;
        private System.Windows.Forms.Panel pnlBottomControls;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.CheckBox cbxEnrollmentFee;
        private System.Windows.Forms.CheckBox cbxOpenEnrollment;
        private System.Windows.Forms.CheckBox cbxShowInSearch;
        private System.Windows.Forms.ComboBox cbxMaturity;
        private System.Windows.Forms.ComboBox cbxActiveTitle;
        private System.Windows.Forms.Label lblActiveTitle;
        private System.Windows.Forms.NumericUpDown nudEnrollmentFee;
        private System.Windows.Forms.CheckBox cbxShowGroupInList;
        private System.Windows.Forms.CheckBox cbxReceiveNotices;
        private System.Windows.Forms.Label lblCharter;
        private System.Windows.Forms.ListView lvwGeneralMembers;
        private System.Windows.Forms.Label lblOwners;
        private System.Windows.Forms.TextBox tbxCharter;
        private System.Windows.Forms.ColumnHeader chGenMemberName;
        private System.Windows.Forms.ColumnHeader chGenTitle;
        private System.Windows.Forms.ColumnHeader chGenLastOn;
    }
}
