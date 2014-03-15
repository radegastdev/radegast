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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupDetails));
            this.tcGroupDetails = new System.Windows.Forms.TabControl();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.txtGroupID = new System.Windows.Forms.TextBox();
            this.btnJoin = new System.Windows.Forms.Button();
            this.lvwGeneralMembers = new Radegast.ListViewNoFlicker();
            this.chGenMemberName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGenTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGenLastOn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.memberListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.memberListContextMenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.lblOwners = new System.Windows.Forms.Label();
            this.tbxCharter = new System.Windows.Forms.TextBox();
            this.lblCharter = new System.Windows.Forms.Label();
            this.gbPreferences = new System.Windows.Forms.GroupBox();
            this.cbxListInProfile = new System.Windows.Forms.CheckBox();
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
            this.tcMembersRoles = new System.Windows.Forms.TabControl();
            this.tpMembers = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvwAllowedAbilities = new Radegast.ListViewNoFlicker();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvwAssignedRoles = new Radegast.ListViewNoFlicker();
            this.chHasRole = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chRoleName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnInviteNewMember = new System.Windows.Forms.Button();
            this.btnBanMember = new System.Windows.Forms.Button();
            this.btnEjectMember = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblAssignedRoles = new System.Windows.Forms.Label();
            this.lvwMemberDetails = new Radegast.ListViewNoFlicker();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpRoles = new System.Windows.Forms.TabPage();
            this.pnlRoleDetaiils = new System.Windows.Forms.Panel();
            this.btnSaveRole = new System.Windows.Forms.Button();
            this.lvwRoleAbilitis = new Radegast.ListViewNoFlicker();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvwAssignedMembers = new Radegast.ListViewNoFlicker();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDeleteRole = new System.Windows.Forms.Button();
            this.btnCreateNewRole = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRoleTitle = new System.Windows.Forms.TextBox();
            this.txtRoleDescription = new System.Windows.Forms.TextBox();
            this.txtRoleName = new System.Windows.Forms.TextBox();
            this.lvwRoles = new Radegast.ListViewNoFlicker();
            this.chRoleListName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chRoleTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cbRoleID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rolesContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyRoleIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.tpNotices = new System.Windows.Forms.TabPage();
            this.btnNewNotice = new System.Windows.Forms.Button();
            this.lblGroupNoticesArchive = new System.Windows.Forms.Label();
            this.lvwNoticeArchive = new Radegast.ListViewNoFlicker();
            this.ntcArchAttachment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ntcArchSubject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ntcArchFrom = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ntcArchDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlNewNotice = new System.Windows.Forms.Panel();
            this.pnlNoticeAttachment = new System.Windows.Forms.GroupBox();
            this.btnPasteInv = new System.Windows.Forms.Button();
            this.btnRemoveAttachment = new System.Windows.Forms.Button();
            this.icnNewNoticeAtt = new System.Windows.Forms.PictureBox();
            this.txtNewNoteAtt = new System.Windows.Forms.TextBox();
            this.txtNewNoticeTitle = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtNewNoticeBody = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.pnlArchivedNotice = new System.Windows.Forms.Panel();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.icnItem = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtNotice = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSentBy = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tpBanned = new System.Windows.Forms.TabPage();
            this.lwBannedMembers = new Radegast.ListViewNoFlicker();
            this.chBannedName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chBannedOn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlBannedBottom = new System.Windows.Forms.Panel();
            this.btnUnban = new System.Windows.Forms.Button();
            this.btnBan = new System.Windows.Forms.Button();
            this.pnlBannedTop = new System.Windows.Forms.Panel();
            this.lblGroupBansTitle = new System.Windows.Forms.Label();
            this.pnlBottomControls = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.tcGroupDetails.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.memberListContextMenu.SuspendLayout();
            this.gbPreferences.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnrollmentFee)).BeginInit();
            this.tpMembersRoles.SuspendLayout();
            this.tcMembersRoles.SuspendLayout();
            this.tpMembers.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tpRoles.SuspendLayout();
            this.pnlRoleDetaiils.SuspendLayout();
            this.rolesContextMenu.SuspendLayout();
            this.tpNotices.SuspendLayout();
            this.pnlNewNotice.SuspendLayout();
            this.pnlNoticeAttachment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icnNewNoticeAtt)).BeginInit();
            this.pnlArchivedNotice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icnItem)).BeginInit();
            this.tpBanned.SuspendLayout();
            this.pnlBannedBottom.SuspendLayout();
            this.pnlBannedTop.SuspendLayout();
            this.pnlBottomControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcGroupDetails
            // 
            this.tcGroupDetails.Controls.Add(this.tpGeneral);
            this.tcGroupDetails.Controls.Add(this.tpMembersRoles);
            this.tcGroupDetails.Controls.Add(this.tpNotices);
            this.tcGroupDetails.Controls.Add(this.tpBanned);
            this.tcGroupDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcGroupDetails.Location = new System.Drawing.Point(0, 0);
            this.tcGroupDetails.Name = "tcGroupDetails";
            this.tcGroupDetails.SelectedIndex = 0;
            this.tcGroupDetails.Size = new System.Drawing.Size(408, 465);
            this.tcGroupDetails.TabIndex = 0;
            this.tcGroupDetails.SelectedIndexChanged += new System.EventHandler(this.tcGroupDetails_SelectedIndexChanged);
            // 
            // tpGeneral
            // 
            this.tpGeneral.BackColor = System.Drawing.Color.Transparent;
            this.tpGeneral.Controls.Add(this.txtGroupID);
            this.tpGeneral.Controls.Add(this.btnJoin);
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
            this.tpGeneral.Size = new System.Drawing.Size(400, 439);
            this.tpGeneral.TabIndex = 0;
            this.tpGeneral.Text = "General";
            this.tpGeneral.UseVisualStyleBackColor = true;
            // 
            // txtGroupID
            // 
            this.txtGroupID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGroupID.Location = new System.Drawing.Point(155, 198);
            this.txtGroupID.Name = "txtGroupID";
            this.txtGroupID.ReadOnly = true;
            this.txtGroupID.Size = new System.Drawing.Size(239, 20);
            this.txtGroupID.TabIndex = 7;
            // 
            // btnJoin
            // 
            this.btnJoin.Location = new System.Drawing.Point(9, 195);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(75, 23);
            this.btnJoin.TabIndex = 9;
            this.btnJoin.Text = "Join $L";
            this.btnJoin.UseVisualStyleBackColor = true;
            this.btnJoin.Visible = false;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
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
            this.lvwGeneralMembers.ContextMenuStrip = this.memberListContextMenu;
            this.lvwGeneralMembers.FullRowSelect = true;
            this.lvwGeneralMembers.GridLines = true;
            this.lvwGeneralMembers.HideSelection = false;
            this.lvwGeneralMembers.Location = new System.Drawing.Point(9, 248);
            this.lvwGeneralMembers.MultiSelect = false;
            this.lvwGeneralMembers.Name = "lvwGeneralMembers";
            this.lvwGeneralMembers.ShowGroups = false;
            this.lvwGeneralMembers.Size = new System.Drawing.Size(385, 82);
            this.lvwGeneralMembers.TabIndex = 8;
            this.lvwGeneralMembers.UseCompatibleStateImageBehavior = false;
            this.lvwGeneralMembers.View = System.Windows.Forms.View.Details;
            this.lvwGeneralMembers.VirtualMode = true;
            this.lvwGeneralMembers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwGeneralMembers_ColumnClick);
            this.lvwGeneralMembers.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvwGeneralMembers_RetrieveVirtualItem);
            this.lvwGeneralMembers.SearchForVirtualItem += new System.Windows.Forms.SearchForVirtualItemEventHandler(this.lvwMemberDetails_SearchForVirtualItem);
            this.lvwGeneralMembers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwGeneralMembers_MouseDoubleClick);
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
            // memberListContextMenu
            // 
            this.memberListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.memberListContextMenuSave});
            this.memberListContextMenu.Name = "contextMenuStrip1";
            this.memberListContextMenu.Size = new System.Drawing.Size(168, 26);
            // 
            // memberListContextMenuSave
            // 
            this.memberListContextMenuSave.Name = "memberListContextMenuSave";
            this.memberListContextMenuSave.Size = new System.Drawing.Size(167, 22);
            this.memberListContextMenuSave.Text = "Save Member List";
            this.memberListContextMenuSave.Click += new System.EventHandler(this.memberListContextMenuSave_Click);
            // 
            // lblOwners
            // 
            this.lblOwners.AutoSize = true;
            this.lblOwners.Location = new System.Drawing.Point(6, 232);
            this.lblOwners.Name = "lblOwners";
            this.lblOwners.Size = new System.Drawing.Size(141, 13);
            this.lblOwners.TabIndex = 7;
            this.lblOwners.Text = "Owners and visible members";
            // 
            // tbxCharter
            // 
            this.tbxCharter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxCharter.BackColor = System.Drawing.SystemColors.Window;
            this.tbxCharter.Location = new System.Drawing.Point(155, 36);
            this.tbxCharter.Multiline = true;
            this.tbxCharter.Name = "tbxCharter";
            this.tbxCharter.ReadOnly = true;
            this.tbxCharter.Size = new System.Drawing.Size(239, 156);
            this.tbxCharter.TabIndex = 6;
            this.tbxCharter.TextChanged += new System.EventHandler(this.tbxCharter_TextChanged);
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
            this.gbPreferences.Controls.Add(this.cbxListInProfile);
            this.gbPreferences.Controls.Add(this.cbxReceiveNotices);
            this.gbPreferences.Controls.Add(this.cbxActiveTitle);
            this.gbPreferences.Controls.Add(this.lblActiveTitle);
            this.gbPreferences.Controls.Add(this.nudEnrollmentFee);
            this.gbPreferences.Controls.Add(this.cbxMaturity);
            this.gbPreferences.Controls.Add(this.cbxEnrollmentFee);
            this.gbPreferences.Controls.Add(this.cbxOpenEnrollment);
            this.gbPreferences.Controls.Add(this.cbxShowInSearch);
            this.gbPreferences.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbPreferences.Location = new System.Drawing.Point(3, 334);
            this.gbPreferences.Name = "gbPreferences";
            this.gbPreferences.Size = new System.Drawing.Size(394, 102);
            this.gbPreferences.TabIndex = 4;
            this.gbPreferences.TabStop = false;
            this.gbPreferences.Text = "Group Preferences";
            // 
            // cbxListInProfile
            // 
            this.cbxListInProfile.AutoSize = true;
            this.cbxListInProfile.Location = new System.Drawing.Point(211, 82);
            this.cbxListInProfile.Name = "cbxListInProfile";
            this.cbxListInProfile.Size = new System.Drawing.Size(130, 17);
            this.cbxListInProfile.TabIndex = 8;
            this.cbxListInProfile.Text = "List group in my profile";
            this.cbxListInProfile.UseVisualStyleBackColor = true;
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
            this.tpMembersRoles.Controls.Add(this.tcMembersRoles);
            this.tpMembersRoles.Controls.Add(this.label2);
            this.tpMembersRoles.Location = new System.Drawing.Point(4, 22);
            this.tpMembersRoles.Name = "tpMembersRoles";
            this.tpMembersRoles.Padding = new System.Windows.Forms.Padding(3);
            this.tpMembersRoles.Size = new System.Drawing.Size(400, 439);
            this.tpMembersRoles.TabIndex = 3;
            this.tpMembersRoles.Text = "Members && Roles";
            this.tpMembersRoles.UseVisualStyleBackColor = true;
            // 
            // tcMembersRoles
            // 
            this.tcMembersRoles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMembersRoles.Controls.Add(this.tpMembers);
            this.tcMembersRoles.Controls.Add(this.tpRoles);
            this.tcMembersRoles.Location = new System.Drawing.Point(9, 23);
            this.tcMembersRoles.Name = "tcMembersRoles";
            this.tcMembersRoles.SelectedIndex = 0;
            this.tcMembersRoles.Size = new System.Drawing.Size(385, 406);
            this.tcMembersRoles.TabIndex = 2;
            this.tcMembersRoles.SelectedIndexChanged += new System.EventHandler(this.tcMembersRoles_SelectedIndexChanged);
            // 
            // tpMembers
            // 
            this.tpMembers.Controls.Add(this.panel1);
            this.tpMembers.Controls.Add(this.lvwMemberDetails);
            this.tpMembers.Location = new System.Drawing.Point(4, 22);
            this.tpMembers.Name = "tpMembers";
            this.tpMembers.Padding = new System.Windows.Forms.Padding(3);
            this.tpMembers.Size = new System.Drawing.Size(377, 380);
            this.tpMembers.TabIndex = 0;
            this.tpMembers.Text = "Members";
            this.tpMembers.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.lvwAllowedAbilities);
            this.panel1.Controls.Add(this.lvwAssignedRoles);
            this.panel1.Controls.Add(this.btnInviteNewMember);
            this.panel1.Controls.Add(this.btnBanMember);
            this.panel1.Controls.Add(this.btnEjectMember);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lblAssignedRoles);
            this.panel1.Location = new System.Drawing.Point(0, 132);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(377, 248);
            this.panel1.TabIndex = 12;
            // 
            // lvwAllowedAbilities
            // 
            this.lvwAllowedAbilities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.lvwAllowedAbilities.FullRowSelect = true;
            this.lvwAllowedAbilities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwAllowedAbilities.Location = new System.Drawing.Point(185, 45);
            this.lvwAllowedAbilities.Name = "lvwAllowedAbilities";
            this.lvwAllowedAbilities.Size = new System.Drawing.Size(189, 197);
            this.lvwAllowedAbilities.TabIndex = 12;
            this.lvwAllowedAbilities.UseCompatibleStateImageBehavior = false;
            this.lvwAllowedAbilities.View = System.Windows.Forms.View.Details;
            this.lvwAllowedAbilities.SizeChanged += new System.EventHandler(this.lvwAllowedAbilities_SizeChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "";
            this.columnHeader4.Width = 167;
            // 
            // lvwAssignedRoles
            // 
            this.lvwAssignedRoles.CheckBoxes = true;
            this.lvwAssignedRoles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chHasRole,
            this.chRoleName});
            this.lvwAssignedRoles.FullRowSelect = true;
            this.lvwAssignedRoles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwAssignedRoles.Location = new System.Drawing.Point(9, 45);
            this.lvwAssignedRoles.Name = "lvwAssignedRoles";
            this.lvwAssignedRoles.Size = new System.Drawing.Size(170, 197);
            this.lvwAssignedRoles.TabIndex = 12;
            this.lvwAssignedRoles.UseCompatibleStateImageBehavior = false;
            this.lvwAssignedRoles.View = System.Windows.Forms.View.Details;
            this.lvwAssignedRoles.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvwAssignedRoles_ItemChecked);
            // 
            // chHasRole
            // 
            this.chHasRole.Text = "";
            this.chHasRole.Width = 20;
            // 
            // chRoleName
            // 
            this.chRoleName.Text = "";
            this.chRoleName.Width = 115;
            // 
            // btnInviteNewMember
            // 
            this.btnInviteNewMember.AccessibleName = "Invite new member";
            this.btnInviteNewMember.Enabled = false;
            this.btnInviteNewMember.Location = new System.Drawing.Point(6, 3);
            this.btnInviteNewMember.Name = "btnInviteNewMember";
            this.btnInviteNewMember.Size = new System.Drawing.Size(80, 23);
            this.btnInviteNewMember.TabIndex = 11;
            this.btnInviteNewMember.Text = "Invite";
            this.btnInviteNewMember.UseVisualStyleBackColor = true;
            this.btnInviteNewMember.Click += new System.EventHandler(this.btnInviteNewMember_Click);
            // 
            // btnBanMember
            // 
            this.btnBanMember.AccessibleName = "Eject from group";
            this.btnBanMember.Enabled = false;
            this.btnBanMember.Location = new System.Drawing.Point(178, 3);
            this.btnBanMember.Name = "btnBanMember";
            this.btnBanMember.Size = new System.Drawing.Size(80, 23);
            this.btnBanMember.TabIndex = 11;
            this.btnBanMember.Text = "Ban";
            this.btnBanMember.UseVisualStyleBackColor = false;
            this.btnBanMember.Click += new System.EventHandler(this.btnBanMember_Click);
            // 
            // btnEjectMember
            // 
            this.btnEjectMember.AccessibleName = "Eject from group";
            this.btnEjectMember.Enabled = false;
            this.btnEjectMember.Location = new System.Drawing.Point(92, 3);
            this.btnEjectMember.Name = "btnEjectMember";
            this.btnEjectMember.Size = new System.Drawing.Size(80, 23);
            this.btnEjectMember.TabIndex = 11;
            this.btnEjectMember.Text = "Eject";
            this.btnEjectMember.UseVisualStyleBackColor = false;
            this.btnEjectMember.Click += new System.EventHandler(this.btnEjectMember_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(182, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Allowed Abilities";
            // 
            // lblAssignedRoles
            // 
            this.lblAssignedRoles.AutoSize = true;
            this.lblAssignedRoles.Location = new System.Drawing.Point(6, 29);
            this.lblAssignedRoles.Name = "lblAssignedRoles";
            this.lblAssignedRoles.Size = new System.Drawing.Size(80, 13);
            this.lblAssignedRoles.TabIndex = 10;
            this.lblAssignedRoles.Text = "Assigned Roles";
            // 
            // lvwMemberDetails
            // 
            this.lvwMemberDetails.AllowColumnReorder = true;
            this.lvwMemberDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwMemberDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvwMemberDetails.FullRowSelect = true;
            this.lvwMemberDetails.GridLines = true;
            this.lvwMemberDetails.HideSelection = false;
            this.lvwMemberDetails.Location = new System.Drawing.Point(0, 0);
            this.lvwMemberDetails.MultiSelect = false;
            this.lvwMemberDetails.Name = "lvwMemberDetails";
            this.lvwMemberDetails.ShowGroups = false;
            this.lvwMemberDetails.Size = new System.Drawing.Size(377, 126);
            this.lvwMemberDetails.TabIndex = 9;
            this.lvwMemberDetails.UseCompatibleStateImageBehavior = false;
            this.lvwMemberDetails.View = System.Windows.Forms.View.Details;
            this.lvwMemberDetails.VirtualMode = true;
            this.lvwMemberDetails.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwGeneralMembers_ColumnClick);
            this.lvwMemberDetails.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvwMemberDetails_RetrieveVirtualItem);
            this.lvwMemberDetails.SearchForVirtualItem += new System.Windows.Forms.SearchForVirtualItemEventHandler(this.lvwMemberDetails_SearchForVirtualItem);
            this.lvwMemberDetails.SelectedIndexChanged += new System.EventHandler(this.lvwMemberDetails_SelectedIndexChanged);
            this.lvwMemberDetails.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwMemberDetails_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Member name";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Donated Tier";
            this.columnHeader2.Width = 130;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last login";
            this.columnHeader3.Width = 90;
            // 
            // tpRoles
            // 
            this.tpRoles.Controls.Add(this.pnlRoleDetaiils);
            this.tpRoles.Controls.Add(this.lvwRoles);
            this.tpRoles.Location = new System.Drawing.Point(4, 22);
            this.tpRoles.Name = "tpRoles";
            this.tpRoles.Padding = new System.Windows.Forms.Padding(3);
            this.tpRoles.Size = new System.Drawing.Size(377, 380);
            this.tpRoles.TabIndex = 1;
            this.tpRoles.Text = "Roles";
            this.tpRoles.UseVisualStyleBackColor = true;
            // 
            // pnlRoleDetaiils
            // 
            this.pnlRoleDetaiils.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRoleDetaiils.Controls.Add(this.btnSaveRole);
            this.pnlRoleDetaiils.Controls.Add(this.lvwRoleAbilitis);
            this.pnlRoleDetaiils.Controls.Add(this.lvwAssignedMembers);
            this.pnlRoleDetaiils.Controls.Add(this.btnDeleteRole);
            this.pnlRoleDetaiils.Controls.Add(this.btnCreateNewRole);
            this.pnlRoleDetaiils.Controls.Add(this.label8);
            this.pnlRoleDetaiils.Controls.Add(this.label7);
            this.pnlRoleDetaiils.Controls.Add(this.label5);
            this.pnlRoleDetaiils.Controls.Add(this.label6);
            this.pnlRoleDetaiils.Controls.Add(this.label4);
            this.pnlRoleDetaiils.Controls.Add(this.txtRoleTitle);
            this.pnlRoleDetaiils.Controls.Add(this.txtRoleDescription);
            this.pnlRoleDetaiils.Controls.Add(this.txtRoleName);
            this.pnlRoleDetaiils.Location = new System.Drawing.Point(-4, 132);
            this.pnlRoleDetaiils.Name = "pnlRoleDetaiils";
            this.pnlRoleDetaiils.Size = new System.Drawing.Size(384, 252);
            this.pnlRoleDetaiils.TabIndex = 11;
            // 
            // btnSaveRole
            // 
            this.btnSaveRole.Location = new System.Drawing.Point(303, 81);
            this.btnSaveRole.Name = "btnSaveRole";
            this.btnSaveRole.Size = new System.Drawing.Size(75, 23);
            this.btnSaveRole.TabIndex = 5;
            this.btnSaveRole.Text = "Save Role";
            this.btnSaveRole.UseVisualStyleBackColor = true;
            this.btnSaveRole.Click += new System.EventHandler(this.btnSaveRole_Click);
            // 
            // lvwRoleAbilitis
            // 
            this.lvwRoleAbilitis.AccessibleName = "Allowed Members";
            this.lvwRoleAbilitis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwRoleAbilitis.CheckBoxes = true;
            this.lvwRoleAbilitis.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7});
            this.lvwRoleAbilitis.FullRowSelect = true;
            this.lvwRoleAbilitis.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwRoleAbilitis.Location = new System.Drawing.Point(182, 123);
            this.lvwRoleAbilitis.Name = "lvwRoleAbilitis";
            this.lvwRoleAbilitis.Size = new System.Drawing.Size(199, 125);
            this.lvwRoleAbilitis.TabIndex = 7;
            this.lvwRoleAbilitis.UseCompatibleStateImageBehavior = false;
            this.lvwRoleAbilitis.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "";
            this.columnHeader6.Width = 20;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "";
            this.columnHeader7.Width = 115;
            // 
            // lvwAssignedMembers
            // 
            this.lvwAssignedMembers.AccessibleName = "Assigned Members";
            this.lvwAssignedMembers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5});
            this.lvwAssignedMembers.FullRowSelect = true;
            this.lvwAssignedMembers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwAssignedMembers.Location = new System.Drawing.Point(7, 123);
            this.lvwAssignedMembers.Name = "lvwAssignedMembers";
            this.lvwAssignedMembers.Size = new System.Drawing.Size(160, 125);
            this.lvwAssignedMembers.TabIndex = 6;
            this.lvwAssignedMembers.UseCompatibleStateImageBehavior = false;
            this.lvwAssignedMembers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "";
            this.columnHeader5.Width = 130;
            // 
            // btnDeleteRole
            // 
            this.btnDeleteRole.AccessibleName = "";
            this.btnDeleteRole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteRole.Location = new System.Drawing.Point(239, 3);
            this.btnDeleteRole.Name = "btnDeleteRole";
            this.btnDeleteRole.Size = new System.Drawing.Size(142, 23);
            this.btnDeleteRole.TabIndex = 1;
            this.btnDeleteRole.Text = "Delete Role";
            this.btnDeleteRole.UseVisualStyleBackColor = true;
            this.btnDeleteRole.Click += new System.EventHandler(this.btnDeleteRole_Click);
            // 
            // btnCreateNewRole
            // 
            this.btnCreateNewRole.AccessibleName = "";
            this.btnCreateNewRole.Location = new System.Drawing.Point(7, 3);
            this.btnCreateNewRole.Name = "btnCreateNewRole";
            this.btnCreateNewRole.Size = new System.Drawing.Size(142, 23);
            this.btnCreateNewRole.TabIndex = 0;
            this.btnCreateNewRole.Text = "New Role";
            this.btnCreateNewRole.UseVisualStyleBackColor = true;
            this.btnCreateNewRole.Click += new System.EventHandler(this.btnCreateNewRole_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(182, 107);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Allowed Abilities";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Assigned Members";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Title";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(182, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Description";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Name";
            // 
            // txtRoleTitle
            // 
            this.txtRoleTitle.AccessibleName = "Role Title";
            this.txtRoleTitle.Location = new System.Drawing.Point(7, 84);
            this.txtRoleTitle.Name = "txtRoleTitle";
            this.txtRoleTitle.Size = new System.Drawing.Size(160, 20);
            this.txtRoleTitle.TabIndex = 3;
            // 
            // txtRoleDescription
            // 
            this.txtRoleDescription.AccessibleName = "Role Description";
            this.txtRoleDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoleDescription.Location = new System.Drawing.Point(182, 45);
            this.txtRoleDescription.Name = "txtRoleDescription";
            this.txtRoleDescription.Size = new System.Drawing.Size(196, 20);
            this.txtRoleDescription.TabIndex = 4;
            // 
            // txtRoleName
            // 
            this.txtRoleName.AccessibleName = "Role Name";
            this.txtRoleName.Location = new System.Drawing.Point(7, 45);
            this.txtRoleName.Name = "txtRoleName";
            this.txtRoleName.Size = new System.Drawing.Size(160, 20);
            this.txtRoleName.TabIndex = 2;
            // 
            // lvwRoles
            // 
            this.lvwRoles.AccessibleName = "Roles";
            this.lvwRoles.AllowColumnReorder = true;
            this.lvwRoles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwRoles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chRoleListName,
            this.chRoleTitle,
            this.cbRoleID});
            this.lvwRoles.ContextMenuStrip = this.rolesContextMenu;
            this.lvwRoles.FullRowSelect = true;
            this.lvwRoles.GridLines = true;
            this.lvwRoles.HideSelection = false;
            this.lvwRoles.Location = new System.Drawing.Point(0, 0);
            this.lvwRoles.MultiSelect = false;
            this.lvwRoles.Name = "lvwRoles";
            this.lvwRoles.ShowGroups = false;
            this.lvwRoles.Size = new System.Drawing.Size(377, 126);
            this.lvwRoles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwRoles.TabIndex = 10;
            this.lvwRoles.UseCompatibleStateImageBehavior = false;
            this.lvwRoles.View = System.Windows.Forms.View.Details;
            this.lvwRoles.SelectedIndexChanged += new System.EventHandler(this.lvwRoles_SelectedIndexChanged);
            // 
            // chRoleListName
            // 
            this.chRoleListName.Text = "Role Name";
            this.chRoleListName.Width = 130;
            // 
            // chRoleTitle
            // 
            this.chRoleTitle.Text = "Role Title";
            this.chRoleTitle.Width = 198;
            // 
            // cbRoleID
            // 
            this.cbRoleID.Text = "Role ID";
            this.cbRoleID.Width = 100;
            // 
            // rolesContextMenu
            // 
            this.rolesContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyRoleIDToolStripMenuItem});
            this.rolesContextMenu.Name = "rolesContextMenu";
            this.rolesContextMenu.Size = new System.Drawing.Size(143, 26);
            // 
            // copyRoleIDToolStripMenuItem
            // 
            this.copyRoleIDToolStripMenuItem.Name = "copyRoleIDToolStripMenuItem";
            this.copyRoleIDToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.copyRoleIDToolStripMenuItem.Text = "Copy Role ID";
            this.copyRoleIDToolStripMenuItem.Click += new System.EventHandler(this.copyRoleIDToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Members && Roles";
            // 
            // tpNotices
            // 
            this.tpNotices.BackColor = System.Drawing.Color.Transparent;
            this.tpNotices.Controls.Add(this.btnNewNotice);
            this.tpNotices.Controls.Add(this.lblGroupNoticesArchive);
            this.tpNotices.Controls.Add(this.lvwNoticeArchive);
            this.tpNotices.Controls.Add(this.pnlNewNotice);
            this.tpNotices.Controls.Add(this.pnlArchivedNotice);
            this.tpNotices.Location = new System.Drawing.Point(4, 22);
            this.tpNotices.Name = "tpNotices";
            this.tpNotices.Padding = new System.Windows.Forms.Padding(3);
            this.tpNotices.Size = new System.Drawing.Size(400, 439);
            this.tpNotices.TabIndex = 2;
            this.tpNotices.Text = "Notices";
            this.tpNotices.UseVisualStyleBackColor = true;
            // 
            // btnNewNotice
            // 
            this.btnNewNotice.Location = new System.Drawing.Point(9, 192);
            this.btnNewNotice.Name = "btnNewNotice";
            this.btnNewNotice.Size = new System.Drawing.Size(132, 23);
            this.btnNewNotice.TabIndex = 10;
            this.btnNewNotice.Text = "Create &New Notice";
            this.btnNewNotice.UseVisualStyleBackColor = true;
            this.btnNewNotice.Click += new System.EventHandler(this.btnNewNotice_Click);
            // 
            // lblGroupNoticesArchive
            // 
            this.lblGroupNoticesArchive.AutoSize = true;
            this.lblGroupNoticesArchive.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGroupNoticesArchive.Location = new System.Drawing.Point(6, 3);
            this.lblGroupNoticesArchive.Name = "lblGroupNoticesArchive";
            this.lblGroupNoticesArchive.Size = new System.Drawing.Size(150, 17);
            this.lblGroupNoticesArchive.TabIndex = 10;
            this.lblGroupNoticesArchive.Text = "Group Notices Archive";
            // 
            // lvwNoticeArchive
            // 
            this.lvwNoticeArchive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwNoticeArchive.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ntcArchAttachment,
            this.ntcArchSubject,
            this.ntcArchFrom,
            this.ntcArchDate});
            this.lvwNoticeArchive.FullRowSelect = true;
            this.lvwNoticeArchive.GridLines = true;
            this.lvwNoticeArchive.HideSelection = false;
            this.lvwNoticeArchive.Location = new System.Drawing.Point(6, 23);
            this.lvwNoticeArchive.MultiSelect = false;
            this.lvwNoticeArchive.Name = "lvwNoticeArchive";
            this.lvwNoticeArchive.ShowGroups = false;
            this.lvwNoticeArchive.Size = new System.Drawing.Size(385, 163);
            this.lvwNoticeArchive.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwNoticeArchive.TabIndex = 9;
            this.lvwNoticeArchive.UseCompatibleStateImageBehavior = false;
            this.lvwNoticeArchive.View = System.Windows.Forms.View.Details;
            this.lvwNoticeArchive.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwNoticeArchive_ColumnClick);
            this.lvwNoticeArchive.SelectedIndexChanged += new System.EventHandler(this.lvwNoticeArchive_SelectedIndexChanged);
            // 
            // ntcArchAttachment
            // 
            this.ntcArchAttachment.Text = "";
            this.ntcArchAttachment.Width = 17;
            // 
            // ntcArchSubject
            // 
            this.ntcArchSubject.Text = "Subject";
            this.ntcArchSubject.Width = 196;
            // 
            // ntcArchFrom
            // 
            this.ntcArchFrom.Text = "From";
            this.ntcArchFrom.Width = 90;
            // 
            // ntcArchDate
            // 
            this.ntcArchDate.Text = "Date";
            this.ntcArchDate.Width = 72;
            // 
            // pnlNewNotice
            // 
            this.pnlNewNotice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNewNotice.Controls.Add(this.pnlNoticeAttachment);
            this.pnlNewNotice.Controls.Add(this.txtNewNoticeTitle);
            this.pnlNewNotice.Controls.Add(this.btnSend);
            this.pnlNewNotice.Controls.Add(this.txtNewNoticeBody);
            this.pnlNewNotice.Controls.Add(this.label10);
            this.pnlNewNotice.Controls.Add(this.label11);
            this.pnlNewNotice.Location = new System.Drawing.Point(6, 224);
            this.pnlNewNotice.Name = "pnlNewNotice";
            this.pnlNewNotice.Size = new System.Drawing.Size(385, 212);
            this.pnlNewNotice.TabIndex = 25;
            this.pnlNewNotice.Visible = false;
            // 
            // pnlNoticeAttachment
            // 
            this.pnlNoticeAttachment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNoticeAttachment.Controls.Add(this.btnPasteInv);
            this.pnlNoticeAttachment.Controls.Add(this.btnRemoveAttachment);
            this.pnlNoticeAttachment.Controls.Add(this.icnNewNoticeAtt);
            this.pnlNoticeAttachment.Controls.Add(this.txtNewNoteAtt);
            this.pnlNoticeAttachment.Location = new System.Drawing.Point(3, 167);
            this.pnlNoticeAttachment.Name = "pnlNoticeAttachment";
            this.pnlNoticeAttachment.Size = new System.Drawing.Size(276, 44);
            this.pnlNoticeAttachment.TabIndex = 3;
            this.pnlNoticeAttachment.TabStop = false;
            this.pnlNoticeAttachment.Text = "Notice attachment";
            // 
            // btnPasteInv
            // 
            this.btnPasteInv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPasteInv.Location = new System.Drawing.Point(202, 15);
            this.btnPasteInv.Name = "btnPasteInv";
            this.btnPasteInv.Size = new System.Drawing.Size(68, 23);
            this.btnPasteInv.TabIndex = 3;
            this.btnPasteInv.Text = "&Paste Inv";
            this.btnPasteInv.UseVisualStyleBackColor = true;
            this.btnPasteInv.Click += new System.EventHandler(this.btnPasteInv_Click);
            // 
            // btnRemoveAttachment
            // 
            this.btnRemoveAttachment.AccessibleName = "Remove attachment";
            this.btnRemoveAttachment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveAttachment.Enabled = false;
            this.btnRemoveAttachment.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveAttachment.Image")));
            this.btnRemoveAttachment.Location = new System.Drawing.Point(170, 15);
            this.btnRemoveAttachment.Name = "btnRemoveAttachment";
            this.btnRemoveAttachment.Size = new System.Drawing.Size(26, 23);
            this.btnRemoveAttachment.TabIndex = 2;
            this.btnRemoveAttachment.UseVisualStyleBackColor = true;
            this.btnRemoveAttachment.Click += new System.EventHandler(this.btnRemoveAttachment_Click);
            // 
            // icnNewNoticeAtt
            // 
            this.icnNewNoticeAtt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.icnNewNoticeAtt.Location = new System.Drawing.Point(1, 17);
            this.icnNewNoticeAtt.Name = "icnNewNoticeAtt";
            this.icnNewNoticeAtt.Size = new System.Drawing.Size(16, 16);
            this.icnNewNoticeAtt.TabIndex = 23;
            this.icnNewNoticeAtt.TabStop = false;
            this.icnNewNoticeAtt.Visible = false;
            // 
            // txtNewNoteAtt
            // 
            this.txtNewNoteAtt.AccessibleName = "Notice attachment";
            this.txtNewNoteAtt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewNoteAtt.Location = new System.Drawing.Point(23, 15);
            this.txtNewNoteAtt.Name = "txtNewNoteAtt";
            this.txtNewNoteAtt.ReadOnly = true;
            this.txtNewNoteAtt.Size = new System.Drawing.Size(141, 20);
            this.txtNewNoteAtt.TabIndex = 1;
            // 
            // txtNewNoticeTitle
            // 
            this.txtNewNoticeTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewNoticeTitle.Location = new System.Drawing.Point(7, 35);
            this.txtNewNoticeTitle.Name = "txtNewNoticeTitle";
            this.txtNewNoticeTitle.Size = new System.Drawing.Size(375, 20);
            this.txtNewNoticeTitle.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(285, 182);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(97, 23);
            this.btnSend.TabIndex = 6;
            this.btnSend.Text = "&Send Notice";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtNewNoticeBody
            // 
            this.txtNewNoticeBody.AccessibleName = "Notice text";
            this.txtNewNoticeBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewNoticeBody.BackColor = System.Drawing.SystemColors.Window;
            this.txtNewNoticeBody.Location = new System.Drawing.Point(7, 58);
            this.txtNewNoticeBody.Multiline = true;
            this.txtNewNoticeBody.Name = "txtNewNoticeBody";
            this.txtNewNoticeBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNewNoticeBody.Size = new System.Drawing.Size(375, 103);
            this.txtNewNoticeBody.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Title";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(166, 24);
            this.label11.TabIndex = 22;
            this.label11.Text = "New Group Notice";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pnlArchivedNotice
            // 
            this.pnlArchivedNotice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlArchivedNotice.Controls.Add(this.txtItemName);
            this.pnlArchivedNotice.Controls.Add(this.icnItem);
            this.pnlArchivedNotice.Controls.Add(this.btnSave);
            this.pnlArchivedNotice.Controls.Add(this.txtNotice);
            this.pnlArchivedNotice.Controls.Add(this.lblTitle);
            this.pnlArchivedNotice.Controls.Add(this.lblSentBy);
            this.pnlArchivedNotice.Controls.Add(this.label1);
            this.pnlArchivedNotice.Location = new System.Drawing.Point(6, 224);
            this.pnlArchivedNotice.Name = "pnlArchivedNotice";
            this.pnlArchivedNotice.Size = new System.Drawing.Size(385, 209);
            this.pnlArchivedNotice.TabIndex = 11;
            this.pnlArchivedNotice.Visible = false;
            // 
            // txtItemName
            // 
            this.txtItemName.AccessibleName = "Notice attachment";
            this.txtItemName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItemName.Location = new System.Drawing.Point(25, 185);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.ReadOnly = true;
            this.txtItemName.Size = new System.Drawing.Size(302, 20);
            this.txtItemName.TabIndex = 24;
            this.txtItemName.Visible = false;
            // 
            // icnItem
            // 
            this.icnItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.icnItem.Location = new System.Drawing.Point(3, 187);
            this.icnItem.Name = "icnItem";
            this.icnItem.Size = new System.Drawing.Size(16, 16);
            this.icnItem.TabIndex = 23;
            this.icnItem.TabStop = false;
            this.icnItem.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(333, 185);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(49, 23);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtNotice
            // 
            this.txtNotice.AccessibleName = "Notice text";
            this.txtNotice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotice.BackColor = System.Drawing.SystemColors.Window;
            this.txtNotice.Location = new System.Drawing.Point(7, 58);
            this.txtNotice.Multiline = true;
            this.txtNotice.Name = "txtNotice";
            this.txtNotice.ReadOnly = true;
            this.txtNotice.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotice.Size = new System.Drawing.Size(375, 121);
            this.txtNotice.TabIndex = 19;
            // 
            // lblTitle
            // 
            this.lblTitle.AccessibleName = "Notice title";
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.BackColor = System.Drawing.SystemColors.Window;
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(7, 37);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(375, 18);
            this.lblTitle.TabIndex = 20;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSentBy
            // 
            this.lblSentBy.AutoSize = true;
            this.lblSentBy.Location = new System.Drawing.Point(4, 24);
            this.lblSentBy.Name = "lblSentBy";
            this.lblSentBy.Size = new System.Drawing.Size(52, 13);
            this.lblSentBy.TabIndex = 21;
            this.lblSentBy.Text = "Sent by...";
            this.lblSentBy.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 24);
            this.label1.TabIndex = 22;
            this.label1.Text = "Group Notice";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tpBanned
            // 
            this.tpBanned.Controls.Add(this.lwBannedMembers);
            this.tpBanned.Controls.Add(this.pnlBannedBottom);
            this.tpBanned.Controls.Add(this.pnlBannedTop);
            this.tpBanned.Location = new System.Drawing.Point(4, 22);
            this.tpBanned.Name = "tpBanned";
            this.tpBanned.Size = new System.Drawing.Size(400, 439);
            this.tpBanned.TabIndex = 4;
            this.tpBanned.Text = "Banned Residents";
            this.tpBanned.UseVisualStyleBackColor = true;
            // 
            // lwBannedMembers
            // 
            this.lwBannedMembers.AllowColumnReorder = true;
            this.lwBannedMembers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chBannedName,
            this.chBannedOn});
            this.lwBannedMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lwBannedMembers.FullRowSelect = true;
            this.lwBannedMembers.GridLines = true;
            this.lwBannedMembers.HideSelection = false;
            this.lwBannedMembers.Location = new System.Drawing.Point(0, 28);
            this.lwBannedMembers.Name = "lwBannedMembers";
            this.lwBannedMembers.ShowGroups = false;
            this.lwBannedMembers.Size = new System.Drawing.Size(400, 372);
            this.lwBannedMembers.TabIndex = 10;
            this.lwBannedMembers.UseCompatibleStateImageBehavior = false;
            this.lwBannedMembers.View = System.Windows.Forms.View.Details;
            this.lwBannedMembers.SelectedIndexChanged += new System.EventHandler(this.lwBannedMembers_SelectedIndexChanged);
            // 
            // chBannedName
            // 
            this.chBannedName.Text = "Resident name";
            this.chBannedName.Width = 268;
            // 
            // chBannedOn
            // 
            this.chBannedOn.Text = "Banned on";
            this.chBannedOn.Width = 90;
            // 
            // pnlBannedBottom
            // 
            this.pnlBannedBottom.Controls.Add(this.btnUnban);
            this.pnlBannedBottom.Controls.Add(this.btnBan);
            this.pnlBannedBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBannedBottom.Location = new System.Drawing.Point(0, 400);
            this.pnlBannedBottom.Name = "pnlBannedBottom";
            this.pnlBannedBottom.Size = new System.Drawing.Size(400, 39);
            this.pnlBannedBottom.TabIndex = 4;
            // 
            // btnUnban
            // 
            this.btnUnban.Enabled = false;
            this.btnUnban.Location = new System.Drawing.Point(84, 3);
            this.btnUnban.Name = "btnUnban";
            this.btnUnban.Size = new System.Drawing.Size(75, 23);
            this.btnUnban.TabIndex = 1;
            this.btnUnban.Text = "Unban";
            this.btnUnban.UseVisualStyleBackColor = true;
            this.btnUnban.Click += new System.EventHandler(this.btnUnban_Click);
            // 
            // btnBan
            // 
            this.btnBan.Location = new System.Drawing.Point(3, 3);
            this.btnBan.Name = "btnBan";
            this.btnBan.Size = new System.Drawing.Size(75, 23);
            this.btnBan.TabIndex = 0;
            this.btnBan.Text = "Ban...";
            this.btnBan.UseVisualStyleBackColor = true;
            this.btnBan.Click += new System.EventHandler(this.btnBan_Click);
            // 
            // pnlBannedTop
            // 
            this.pnlBannedTop.Controls.Add(this.lblGroupBansTitle);
            this.pnlBannedTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBannedTop.Location = new System.Drawing.Point(0, 0);
            this.pnlBannedTop.Name = "pnlBannedTop";
            this.pnlBannedTop.Size = new System.Drawing.Size(400, 28);
            this.pnlBannedTop.TabIndex = 3;
            // 
            // lblGroupBansTitle
            // 
            this.lblGroupBansTitle.AutoSize = true;
            this.lblGroupBansTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGroupBansTitle.Location = new System.Drawing.Point(3, 3);
            this.lblGroupBansTitle.Name = "lblGroupBansTitle";
            this.lblGroupBansTitle.Size = new System.Drawing.Size(222, 17);
            this.lblGroupBansTitle.TabIndex = 2;
            this.lblGroupBansTitle.Text = "Residents banned from this group";
            // 
            // pnlBottomControls
            // 
            this.pnlBottomControls.Controls.Add(this.btnClose);
            this.pnlBottomControls.Controls.Add(this.btnApply);
            this.pnlBottomControls.Controls.Add(this.btnRefresh);
            this.pnlBottomControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottomControls.Location = new System.Drawing.Point(0, 465);
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
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(248, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(13, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // GroupDetails
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tcGroupDetails);
            this.Controls.Add(this.pnlBottomControls);
            this.MinimumSize = new System.Drawing.Size(300, 400);
            this.Name = "GroupDetails";
            this.Size = new System.Drawing.Size(408, 499);
            this.tcGroupDetails.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.memberListContextMenu.ResumeLayout(false);
            this.gbPreferences.ResumeLayout(false);
            this.gbPreferences.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnrollmentFee)).EndInit();
            this.tpMembersRoles.ResumeLayout(false);
            this.tpMembersRoles.PerformLayout();
            this.tcMembersRoles.ResumeLayout(false);
            this.tpMembers.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tpRoles.ResumeLayout(false);
            this.pnlRoleDetaiils.ResumeLayout(false);
            this.pnlRoleDetaiils.PerformLayout();
            this.rolesContextMenu.ResumeLayout(false);
            this.tpNotices.ResumeLayout(false);
            this.tpNotices.PerformLayout();
            this.pnlNewNotice.ResumeLayout(false);
            this.pnlNewNotice.PerformLayout();
            this.pnlNoticeAttachment.ResumeLayout(false);
            this.pnlNoticeAttachment.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icnNewNoticeAtt)).EndInit();
            this.pnlArchivedNotice.ResumeLayout(false);
            this.pnlArchivedNotice.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icnItem)).EndInit();
            this.tpBanned.ResumeLayout(false);
            this.pnlBannedBottom.ResumeLayout(false);
            this.pnlBannedTop.ResumeLayout(false);
            this.pnlBannedTop.PerformLayout();
            this.pnlBottomControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TabControl tcGroupDetails;
        public System.Windows.Forms.TabPage tpGeneral;
        public System.Windows.Forms.TabPage tpNotices;
        public System.Windows.Forms.Panel pnlInsignia;
        public System.Windows.Forms.Label lblFounded;
        public System.Windows.Forms.Label lblGroupName;
        public System.Windows.Forms.Label lblInsignia;
        public System.Windows.Forms.GroupBox gbPreferences;
        public System.Windows.Forms.Panel pnlBottomControls;
        public System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.Button btnApply;
        public System.Windows.Forms.Button btnRefresh;
        public System.Windows.Forms.CheckBox cbxEnrollmentFee;
        public System.Windows.Forms.CheckBox cbxOpenEnrollment;
        public System.Windows.Forms.CheckBox cbxShowInSearch;
        public System.Windows.Forms.ComboBox cbxMaturity;
        public System.Windows.Forms.ComboBox cbxActiveTitle;
        public System.Windows.Forms.Label lblActiveTitle;
        public System.Windows.Forms.NumericUpDown nudEnrollmentFee;
        public System.Windows.Forms.CheckBox cbxListInProfile;
        public System.Windows.Forms.CheckBox cbxReceiveNotices;
        public System.Windows.Forms.Label lblCharter;
        public ListViewNoFlicker lvwGeneralMembers;
        public System.Windows.Forms.Label lblOwners;
        public System.Windows.Forms.TextBox tbxCharter;
        public System.Windows.Forms.ColumnHeader chGenMemberName;
        public System.Windows.Forms.ColumnHeader chGenTitle;
        public System.Windows.Forms.ColumnHeader chGenLastOn;
        public ListViewNoFlicker lvwNoticeArchive;
        public System.Windows.Forms.ColumnHeader ntcArchAttachment;
        public System.Windows.Forms.ColumnHeader ntcArchSubject;
        public System.Windows.Forms.ColumnHeader ntcArchFrom;
        public System.Windows.Forms.ColumnHeader ntcArchDate;
        public System.Windows.Forms.Label lblGroupNoticesArchive;
        public System.Windows.Forms.Panel pnlArchivedNotice;
        public System.Windows.Forms.TextBox txtItemName;
        public System.Windows.Forms.PictureBox icnItem;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.TextBox txtNotice;
        public System.Windows.Forms.Label lblTitle;
        public System.Windows.Forms.Label lblSentBy;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TabPage tpMembersRoles;
        public System.Windows.Forms.Label label2;
        public ListViewNoFlicker lvwMemberDetails;
        public System.Windows.Forms.ColumnHeader columnHeader1;
        public System.Windows.Forms.ColumnHeader columnHeader2;
        public System.Windows.Forms.ColumnHeader columnHeader3;
        public System.Windows.Forms.Button btnJoin;
        public System.Windows.Forms.TabControl tcMembersRoles;
        public System.Windows.Forms.TabPage tpMembers;
        public System.Windows.Forms.TabPage tpRoles;
        public System.Windows.Forms.Label lblAssignedRoles;
        public System.Windows.Forms.Button btnInviteNewMember;
        public System.Windows.Forms.Button btnEjectMember;
        public System.Windows.Forms.Panel panel1;
        public ListViewNoFlicker lvwAssignedRoles;
        public System.Windows.Forms.ColumnHeader chHasRole;
        public System.Windows.Forms.ColumnHeader chRoleName;
        public ListViewNoFlicker lvwAllowedAbilities;
        public System.Windows.Forms.ColumnHeader columnHeader4;
        public System.Windows.Forms.Label label3;
        public ListViewNoFlicker lvwRoles;
        public System.Windows.Forms.ColumnHeader chRoleListName;
        public System.Windows.Forms.ColumnHeader chRoleTitle;
        private System.Windows.Forms.Panel pnlRoleDetaiils;
        private System.Windows.Forms.Button btnDeleteRole;
        private System.Windows.Forms.Button btnCreateNewRole;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRoleName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtRoleTitle;
        private System.Windows.Forms.TextBox txtRoleDescription;
        public ListViewNoFlicker lvwAssignedMembers;
        public System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        public ListViewNoFlicker lvwRoleAbilitis;
        public System.Windows.Forms.ColumnHeader columnHeader6;
        public System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Button btnSaveRole;
        private System.Windows.Forms.Button btnNewNotice;
        public System.Windows.Forms.Panel pnlNewNotice;
        public System.Windows.Forms.TextBox txtNewNoteAtt;
        public System.Windows.Forms.PictureBox icnNewNoticeAtt;
        public System.Windows.Forms.Button btnSend;
        public System.Windows.Forms.TextBox txtNewNoticeBody;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtNewNoticeTitle;
        private System.Windows.Forms.Button btnPasteInv;
        private System.Windows.Forms.Button btnRemoveAttachment;
        private System.Windows.Forms.GroupBox pnlNoticeAttachment;
        private System.Windows.Forms.ContextMenuStrip memberListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem memberListContextMenuSave;
        public System.Windows.Forms.TextBox txtGroupID;
        private System.Windows.Forms.ColumnHeader cbRoleID;
        private System.Windows.Forms.ContextMenuStrip rolesContextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyRoleIDToolStripMenuItem;
        private System.Windows.Forms.TabPage tpBanned;
        public ListViewNoFlicker lwBannedMembers;
        public System.Windows.Forms.ColumnHeader chBannedName;
        public System.Windows.Forms.ColumnHeader chBannedOn;
        private System.Windows.Forms.Panel pnlBannedBottom;
        private System.Windows.Forms.Button btnUnban;
        private System.Windows.Forms.Button btnBan;
        private System.Windows.Forms.Panel pnlBannedTop;
        public System.Windows.Forms.Label lblGroupBansTitle;
        public System.Windows.Forms.Button btnBanMember;

    }
}
