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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Packets;

namespace Radegast
{
    public partial class GroupDetails : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private Group group;
        private Dictionary<UUID, GroupTitle> titles;
        private Dictionary<UUID, Group> myGroups { get { return instance.Groups; } }
        private List<KeyValuePair<UUID, UUID>> roleMembers;
        private Dictionary<UUID, GroupRole> roles;
        private bool isMember;

        private UUID groupTitlesRequest, groupMembersRequest, groupRolesRequest, groupRolesMembersRequest;

        public GroupDetails(RadegastInstance instance, Group group)
        {
            InitializeComponent();
            Disposed += new EventHandler(GroupDetails_Disposed);

            this.instance = instance;
            this.group = group;

            if (group.InsigniaID != UUID.Zero)
            {
                SLImageHandler insignia = new SLImageHandler(instance, group.InsigniaID, string.Empty);
                insignia.Dock = DockStyle.Fill;
                pnlInsignia.Controls.Add(insignia);
            }

            lblGroupName.Text = group.Name;
            lvwGeneralMembers.ListViewItemSorter = new GroupMemberSorter();
            lvwMemberDetails.ListViewItemSorter = new GroupMemberSorter();

            isMember = instance.Groups.ContainsKey(group.ID);

            if (isMember)
            {
            }
            else
            {
                tcGroupDetails.TabPages.Remove(tpMembersRoles);
                tcGroupDetails.TabPages.Remove(tpNotices);
            }

            lvwNoticeArchive.SmallImageList = frmMain.ResourceImages;
            lvwNoticeArchive.ListViewItemSorter = new GroupNoticeSorter();

            // Callbacks
            client.Groups.GroupTitlesReply += new EventHandler<GroupTitlesReplyEventArgs>(Groups_GroupTitlesReply);
            client.Groups.GroupMembersReply += new EventHandler<GroupMembersReplyEventArgs>(Groups_GroupMembersReply);
            client.Groups.GroupProfile += new EventHandler<GroupProfileEventArgs>(Groups_GroupProfile);
            client.Groups.CurrentGroups += new EventHandler<CurrentGroupsEventArgs>(Groups_CurrentGroups);
            client.Groups.GroupNoticesListReply += new EventHandler<GroupNoticesListReplyEventArgs>(Groups_GroupNoticesListReply);
            client.Groups.GroupJoinedReply += new EventHandler<GroupOperationEventArgs>(Groups_GroupJoinedReply);
            client.Groups.GroupLeaveReply += new EventHandler<GroupOperationEventArgs>(Groups_GroupLeaveReply);
            client.Groups.GroupRoleDataReply += new EventHandler<GroupRolesDataReplyEventArgs>(Groups_GroupRoleDataReply);
            client.Groups.GroupMemberEjected += new EventHandler<GroupOperationEventArgs>(Groups_GroupMemberEjected);
            client.Groups.GroupRoleMembersReply += new EventHandler<GroupRolesMembersReplyEventArgs>(Groups_GroupRoleMembersReply);
            client.Self.IM += new EventHandler<InstantMessageEventArgs>(Self_IM);
            instance.Names.NameUpdated += new EventHandler<UUIDNameReplyEventArgs>(Names_NameUpdated);
            RefreshControlsAvailability();
            RefreshGroupInfo();
        }


        void GroupDetails_Disposed(object sender, EventArgs e)
        {
            client.Groups.GroupTitlesReply -= new EventHandler<GroupTitlesReplyEventArgs>(Groups_GroupTitlesReply);
            client.Groups.GroupMembersReply -= new EventHandler<GroupMembersReplyEventArgs>(Groups_GroupMembersReply);
            client.Groups.GroupProfile -= new EventHandler<GroupProfileEventArgs>(Groups_GroupProfile);
            client.Groups.CurrentGroups -= new EventHandler<CurrentGroupsEventArgs>(Groups_CurrentGroups);
            client.Groups.GroupNoticesListReply -= new EventHandler<GroupNoticesListReplyEventArgs>(Groups_GroupNoticesListReply);
            client.Groups.GroupJoinedReply -= new EventHandler<GroupOperationEventArgs>(Groups_GroupJoinedReply);
            client.Groups.GroupLeaveReply -= new EventHandler<GroupOperationEventArgs>(Groups_GroupLeaveReply);
            client.Groups.GroupRoleDataReply -= new EventHandler<GroupRolesDataReplyEventArgs>(Groups_GroupRoleDataReply);
            client.Groups.GroupRoleMembersReply -= new EventHandler<GroupRolesMembersReplyEventArgs>(Groups_GroupRoleMembersReply);
            client.Groups.GroupMemberEjected -= new EventHandler<GroupOperationEventArgs>(Groups_GroupMemberEjected);
            client.Self.IM -= new EventHandler<InstantMessageEventArgs>(Self_IM);
            instance.Names.NameUpdated -= new EventHandler<UUIDNameReplyEventArgs>(Names_NameUpdated);
        }

        #region Network callbacks

        void Groups_GroupMemberEjected(object sender, GroupOperationEventArgs e)
        {
            if (e.GroupID != group.ID) return;

            if (e.Success)
            {
                BeginInvoke(new MethodInvoker(() => { RefreshMembersRoles(); }));
                instance.TabConsole.DisplayNotificationInChat("Group member ejected.");
            }
            else
            {
                instance.TabConsole.DisplayNotificationInChat("Failed to eject group member.");
            }
        }

        void Groups_GroupRoleMembersReply(object sender, GroupRolesMembersReplyEventArgs e)
        {
            if (e.GroupID == group.ID && e.RequestID == groupRolesMembersRequest)
            {
                roleMembers = e.RolesMembers;
                BeginInvoke(new MethodInvoker(() =>
                    {
                        btnInviteNewMember.Enabled = HasPower(GroupPowers.Invite);
                        btnEjectMember.Enabled = HasPower(GroupPowers.Eject);
                        lvwMemberDetails_SelectedIndexChanged(null, null);
                    }
                ));
            }
        }

        void Groups_GroupRoleDataReply(object sender, GroupRolesDataReplyEventArgs e)
        {
            if (e.GroupID == group.ID && e.RequestID == groupRolesRequest)
            {
                groupRolesMembersRequest = client.Groups.RequestGroupRolesMembers(group.ID);
                if (roles == null) roles = e.Roles;
                else lock (roles) roles = e.Roles;
                BeginInvoke(new MethodInvoker(() => DisplayGroupRoles()));
            }
        }

        void Groups_GroupLeaveReply(object sender, GroupOperationEventArgs e)
        {
            if (e.GroupID == group.ID && e.Success)
            {
                BeginInvoke(new MethodInvoker(() => RefreshGroupInfo()));
            }
        }

        void Groups_GroupJoinedReply(object sender, GroupOperationEventArgs e)
        {
            if (e.GroupID == group.ID && e.Success)
            {
                BeginInvoke(new MethodInvoker(() => RefreshGroupInfo()));
            }
        }

        UUID destinationFolderID;

        void Self_IM(object sender, InstantMessageEventArgs e)
        {
            if (e.IM.Dialog != InstantMessageDialog.GroupNoticeRequested || e.IM.FromAgentID != group.ID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Self_IM(sender, e)));
                return;
            }

            InstantMessage msg = e.IM;
            AssetType type;


            if (msg.BinaryBucket.Length > 18 && msg.BinaryBucket[0] != 0)
            {
                type = (AssetType)msg.BinaryBucket[1];
                destinationFolderID = client.Inventory.FindFolderForType(type);
                int icoIndx = InventoryConsole.GetItemImageIndex(type.ToString().ToLower());
                if (icoIndx >= 0)
                {
                    icnItem.Image = frmMain.ResourceImages.Images[icoIndx];
                    icnItem.Visible = true;
                }
                txtItemName.Text = Utils.BytesToString(msg.BinaryBucket, 18, msg.BinaryBucket.Length - 19);
                btnSave.Enabled = true;
                btnSave.Visible = icnItem.Visible = txtItemName.Visible = true;
                btnSave.Tag = msg;
            }

            string text = msg.Message.Replace("\n", System.Environment.NewLine);
            int pos = msg.Message.IndexOf('|');
            string title = msg.Message.Substring(0, pos);
            text = text.Remove(0, pos + 1);
            txtNotice.Text = text;
        }

        void Groups_GroupNoticesListReply(object sender, GroupNoticesListReplyEventArgs e)
        {
            if (e.GroupID != group.ID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Groups_GroupNoticesListReply(sender, e)));
                return;
            }

            lvwNoticeArchive.BeginUpdate();

            foreach (GroupNoticesListEntry notice in e.Notices)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Add(notice.Subject);
                item.SubItems.Add(notice.FromName);
                string noticeDate = string.Empty;
                if (notice.Timestamp != 0)
                {
                    noticeDate = Utils.UnixTimeToDateTime(notice.Timestamp).ToShortDateString();
                }
                item.SubItems.Add(noticeDate);

                if (notice.HasAttachment)
                {
                    item.ImageIndex = InventoryConsole.GetItemImageIndex(notice.AssetType.ToString().ToLower());
                }

                item.Tag = notice;

                lvwNoticeArchive.Items.Add(item);
            }
            lvwNoticeArchive.EndUpdate();
        }

        void Groups_CurrentGroups(object sender, CurrentGroupsEventArgs e)
        {
            BeginInvoke(new MethodInvoker(RefreshControlsAvailability));
        }

        void Groups_GroupProfile(object sender, GroupProfileEventArgs e)
        {
            if (group.ID != e.Group.ID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Groups_GroupProfile(sender, e)));
                return;
            }

            group = e.Group;
            if (group.InsigniaID != UUID.Zero && pnlInsignia.Controls.Count == 0)
            {
                SLImageHandler insignia = new SLImageHandler(instance, group.InsigniaID, string.Empty);
                insignia.Dock = DockStyle.Fill;
                pnlInsignia.Controls.Add(insignia);
            }


            tbxCharter.Text = group.Charter;
            lblFounded.Text = "Founded by: " + instance.Names.Get(group.FounderID);
            cbxShowInSearch.Checked = group.ShowInList;
            cbxOpenEnrollment.Checked = group.OpenEnrollment;

            if (group.MembershipFee > 0)
            {
                cbxEnrollmentFee.Checked = true;
                nudEnrollmentFee.Value = group.MembershipFee;
            }
            else
            {
                cbxEnrollmentFee.Checked = false;
                nudEnrollmentFee.Value = 0;
            }

            if (group.MaturePublish)
            {
                cbxMaturity.SelectedIndex = 1;
            }
            else
            {
                cbxMaturity.SelectedIndex = 0;
            }

            btnJoin.Enabled = btnJoin.Visible = false;

            if (myGroups.ContainsKey(group.ID)) // I am in this group
            {
                cbxReceiveNotices.Checked = myGroups[group.ID].AcceptNotices;
                cbxListInProfile.Checked = myGroups[group.ID].ListInProfile;
                cbxReceiveNotices.CheckedChanged += new EventHandler(cbxListInProfile_CheckedChanged);
                cbxListInProfile.CheckedChanged += new EventHandler(cbxListInProfile_CheckedChanged);
            }
            else if (group.OpenEnrollment) // I am not in this group, but I could join it
            {
                btnJoin.Text = "Join $L" + group.MembershipFee;
                btnJoin.Enabled = btnJoin.Visible = true;
            }

            RefreshControlsAvailability();
        }

        void Names_NameUpdated(object sender, UUIDNameReplyEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Names_NameUpdated(sender, e)));
                return;
            }

            if (e.Names.ContainsKey(group.FounderID))
            {
                lblFounded.Text = "Founded by: " + e.Names[group.FounderID];
            }

            lvwMemberDetails.BeginUpdate();
            lvwGeneralMembers.BeginUpdate();
            bool modified = false;
            foreach (KeyValuePair<UUID, string> name in e.Names)
            {
                if (lvwGeneralMembers.Items.ContainsKey(name.Key.ToString()))
                {
                    lvwGeneralMembers.Items[name.Key.ToString()].Text = name.Value;
                    modified = true;
                }

                if (!isMember)
                    continue;

                if (lvwMemberDetails.Items.ContainsKey(name.Key.ToString()))
                {
                    lvwMemberDetails.Items[name.Key.ToString()].Text = name.Value;
                }
            }
            if (modified)
            {
                lvwGeneralMembers.Sort();
                if (isMember) lvwMemberDetails.Sort();
            }
            lvwGeneralMembers.EndUpdate();
            lvwMemberDetails.EndUpdate();
        }

        void Groups_GroupTitlesReply(object sender, GroupTitlesReplyEventArgs e)
        {
            if (groupTitlesRequest != e.RequestID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Groups_GroupTitlesReply(sender, e)));
                return;
            }

            this.titles = e.Titles;

            foreach (GroupTitle title in titles.Values)
            {
                cbxActiveTitle.Items.Add(title);
                if (title.Selected)
                {
                    cbxActiveTitle.SelectedItem = title;
                }
            }

            cbxActiveTitle.SelectedIndexChanged += cbxActiveTitle_SelectedIndexChanged;
        }

        void Groups_GroupMembersReply(object sender, GroupMembersReplyEventArgs e)
        {
            if (groupMembersRequest != e.RequestID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Groups_GroupMembersReply(sender, e)));
                return;
            }

            lvwGeneralMembers.BeginUpdate();
            List<ListViewItem> newItems = new List<ListViewItem>();
            List<ListViewItem> memberDetails = new List<ListViewItem>();

            foreach (GroupMember baseMember in e.Members.Values)
            {
                EnhancedGroupMember member = new EnhancedGroupMember(baseMember);
                string name;

                name = instance.Names.Get(member.Base.ID);

                {
                    ListViewItem item = new ListViewItem(name);
                    item.Tag = member;
                    item.Name = member.Base.ID.ToString();
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, member.Base.Title));
                    if (member.LastOnline != DateTime.MinValue)
                    {
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, member.Base.OnlineStatus));
                    }

                    newItems.Add(item);
                }

                if (isMember)
                {
                    ListViewItem item = new ListViewItem(name);
                    item.Tag = member;
                    item.Name = member.Base.ID.ToString();
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, member.Base.Contribution.ToString()));
                    if (member.LastOnline != DateTime.MinValue)
                    {
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, member.Base.OnlineStatus));
                    }
                    memberDetails.Add(item);
                }


            }

            lvwGeneralMembers.Items.AddRange(newItems.ToArray());
            lvwGeneralMembers.Sort();
            lvwGeneralMembers.EndUpdate();

            if (isMember && memberDetails.Count > 0)
            {
                lvwMemberDetails.BeginUpdate();
                lvwMemberDetails.Items.AddRange(memberDetails.ToArray());
                lvwMemberDetails.Sort();
                lvwMemberDetails.EndUpdate();
            }
        }
        #endregion

        #region Privatate methods

        private void DisplayGroupRoles()
        {
            lvwRoles.Items.Clear();

            lock (roles)
            {
                foreach (GroupRole role in roles.Values)
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = role.ID.ToString();
                    item.Text = role.Name;
                    item.SubItems.Add(role.Title);
                    item.Tag = role;
                    lvwRoles.Items.Add(item);
                }
            }

        }


        private bool HasPower(GroupPowers power)
        {
            if (!instance.Groups.ContainsKey(group.ID))
                return false;

            return (instance.Groups[group.ID].Powers & power) != 0;
        }

        private void RefreshControlsAvailability()
        {
            if (!HasPower(GroupPowers.ChangeOptions))
            {
                nudEnrollmentFee.ReadOnly = true;
                cbxEnrollmentFee.Enabled = false;
                cbxOpenEnrollment.Enabled = false;
            }

            if (!HasPower(GroupPowers.ChangeIdentity))
            {
                tbxCharter.ReadOnly = true;
                cbxShowInSearch.Enabled = false;
                cbxMaturity.Enabled = false;
            }

            if (!myGroups.ContainsKey(group.ID))
            {
                cbxReceiveNotices.Enabled = false;
                cbxListInProfile.Enabled = false;
            }
        }

        private void RefreshGroupNotices()
        {
            lvwNoticeArchive.Items.Clear();
            client.Groups.RequestGroupNoticesList(group.ID);
            btnNewNotice.Enabled = HasPower(GroupPowers.SendNotices);
        }

        private void RefreshGroupInfo()
        {
            lvwGeneralMembers.Items.Clear();
            if (isMember) lvwMemberDetails.Items.Clear();

            cbxActiveTitle.SelectedIndexChanged -= cbxActiveTitle_SelectedIndexChanged;
            cbxReceiveNotices.CheckedChanged -= new EventHandler(cbxListInProfile_CheckedChanged);
            cbxListInProfile.CheckedChanged -= new EventHandler(cbxListInProfile_CheckedChanged);

            cbxActiveTitle.Items.Clear();

            // Request group info
            client.Groups.RequestGroupProfile(group.ID);
            groupTitlesRequest = client.Groups.RequestGroupTitles(group.ID);
            groupMembersRequest = client.Groups.RequestGroupMembers(group.ID);
        }

        private void RefreshRoles()
        {
            if (!isMember) return;

            lvwRoles.SelectedItems.Clear();
            lvwRoles.Items.Clear();
            btnApply.Enabled = false;
            btnCreateNewRole.Enabled = HasPower(GroupPowers.CreateRole);
            btnDeleteRole.Enabled = HasPower(GroupPowers.DeleteRole);
            txtRoleDescription.Enabled = txtRoleName.Enabled = txtRoleTitle.Enabled = lvwRoleAbilitis.Enabled = btnSaveRole.Enabled = false;
            groupRolesRequest = client.Groups.RequestGroupRoles(group.ID);
        }

        private void RefreshMembersRoles()
        {
            if (!isMember) return;

            btnApply.Enabled = false;
            lvwGeneralMembers.Items.Clear();
            lvwMemberDetails.Items.Clear();
            lvwAllowedAbilities.Items.Clear();
            lvwAssignedRoles.Items.Clear();
            groupMembersRequest = client.Groups.RequestGroupMembers(group.ID);
            groupRolesRequest = client.Groups.RequestGroupRoles(group.ID);
        }
        #endregion

        #region Controls change handlers
        void cbxListInProfile_CheckedChanged(object sender, EventArgs e)
        {
            if (myGroups.ContainsKey(group.ID))
            {
                Group g = myGroups[group.ID];
                // g.AcceptNotices = cbxReceiveNotices.Checked;
                // g.ListInProfile = cbxListInProfile.Checked;
                client.Groups.SetGroupAcceptNotices(g.ID, cbxReceiveNotices.Checked, cbxListInProfile.Checked);
                client.Groups.RequestCurrentGroups();
            }
        }

        private void cbxActiveTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            GroupTitle title = (GroupTitle)cbxActiveTitle.SelectedItem;
            client.Groups.ActivateTitle(title.GroupID, title.RoleID);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            switch (tcGroupDetails.SelectedTab.Name)
            {
                case "tpGeneral":
                    RefreshGroupInfo();
                    break;

                case "tpNotices":
                    RefreshGroupNotices();
                    break;

                case "tpMembersRoles":
                    RefreshMembersRoles();
                    break;
            }
        }
        #endregion

        void lvwGeneralMembers_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView lb = (ListView)sender;
            GroupMemberSorter sorter = (GroupMemberSorter)lb.ListViewItemSorter;
            switch (e.Column)
            {
                case 0:
                    sorter.SortBy = GroupMemberSorter.SortByColumn.Name;
                    break;

                case 1:
                    if (lb.Name == "lvwMemberDetails")
                        sorter.SortBy = GroupMemberSorter.SortByColumn.Contribution;
                    else
                        sorter.SortBy = GroupMemberSorter.SortByColumn.Title;
                    break;

                case 2:
                    sorter.SortBy = GroupMemberSorter.SortByColumn.LastOnline;
                    break;
            }

            if (sorter.CurrentOrder == GroupMemberSorter.SortOrder.Ascending)
                sorter.CurrentOrder = GroupMemberSorter.SortOrder.Descending;
            else
                sorter.CurrentOrder = GroupMemberSorter.SortOrder.Ascending;

            lb.Sort();
        }

        private void lvwNoticeArchive_ColumnClick(object sender, ColumnClickEventArgs e)
        {

            GroupNoticeSorter sorter = (GroupNoticeSorter)lvwNoticeArchive.ListViewItemSorter;

            switch (e.Column)
            {
                case 1:
                    sorter.SortBy = GroupNoticeSorter.SortByColumn.Subject;
                    break;

                case 2:
                    sorter.SortBy = GroupNoticeSorter.SortByColumn.Sender;
                    break;

                case 3:
                    sorter.SortBy = GroupNoticeSorter.SortByColumn.Date;
                    break;
            }

            if (sorter.CurrentOrder == GroupNoticeSorter.SortOrder.Ascending)
                sorter.CurrentOrder = GroupNoticeSorter.SortOrder.Descending;
            else
                sorter.CurrentOrder = GroupNoticeSorter.SortOrder.Ascending;

            lvwNoticeArchive.Sort();
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void tcGroupDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tcGroupDetails.SelectedTab.Name)
            {
                case "tpNotices":
                    RefreshGroupNotices();
                    break;

                case "tpMembersRoles":
                    RefreshMembersRoles();
                    break;
            }
        }

        private void lvwNoticeArchive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwNoticeArchive.SelectedItems.Count == 1)
            {
                if (lvwNoticeArchive.SelectedItems[0].Tag is GroupNoticesListEntry)
                {
                    GroupNoticesListEntry notice = (GroupNoticesListEntry)lvwNoticeArchive.SelectedItems[0].Tag;
                    lblSentBy.Text = "Sent by " + notice.FromName;
                    lblTitle.Text = notice.Subject;
                    txtNotice.Text = string.Empty;
                    btnSave.Enabled = btnSave.Visible = icnItem.Visible = txtItemName.Visible = false;
                    client.Groups.RequestGroupNotice(notice.NoticeID);
                    pnlNewNotice.Visible = false;
                    pnlArchivedNotice.Visible = true;
                    return;
                }
            }
            pnlArchivedNotice.Visible = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Tag is InstantMessage)
            {
                InstantMessage msg = (InstantMessage)btnSave.Tag;
                client.Self.InstantMessage(client.Self.Name, msg.FromAgentID, string.Empty, msg.IMSessionID, InstantMessageDialog.GroupNoticeInventoryAccepted, InstantMessageOnline.Offline, client.Self.SimPosition, client.Network.CurrentSim.RegionID, destinationFolderID.GetBytes());
                btnSave.Enabled = false;
                btnClose.Focus();
            }
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            client.Groups.RequestJoinGroup(group.ID);
        }

        private void lvwGeneralMembers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = lvwGeneralMembers.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                try
                {
                    UUID agentID = new UUID(item.Name);
                    instance.MainForm.ShowAgentProfile(item.Text, agentID);
                }
                catch (Exception) { }
            }
        }

        private void lvwMemberDetails_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = lvwMemberDetails.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                try
                {
                    UUID agentID = new UUID(item.Name);
                    instance.MainForm.ShowAgentProfile(item.Text, agentID);
                }
                catch (Exception) { }
            }
        }

        private void btnEjectMember_Click(object sender, EventArgs e)
        {
            if (lvwMemberDetails.SelectedItems.Count != 1) return;
            EnhancedGroupMember m = (EnhancedGroupMember)lvwMemberDetails.SelectedItems[0].Tag;
            client.Groups.EjectUser(group.ID, m.Base.ID);
        }

        private void lvwMemberDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwMemberDetails.SelectedItems.Count != 1 || roles == null || roleMembers == null) return;
            EnhancedGroupMember m = (EnhancedGroupMember)lvwMemberDetails.SelectedItems[0].Tag;

            btnApply.Enabled = false;

            lvwAssignedRoles.BeginUpdate();
            lvwAssignedRoles.ItemChecked -= lvwAssignedRoles_ItemChecked;
            lvwAssignedRoles.Items.Clear();
            lvwAssignedRoles.Tag = m;

            ListViewItem defaultItem = new ListViewItem();
            defaultItem.Name = "Everyone";
            defaultItem.SubItems.Add(defaultItem.Name);
            defaultItem.Checked = true;
            lvwAssignedRoles.Items.Add(defaultItem);

            GroupPowers abilities = GroupPowers.None;

            lock (roles)
            {
                foreach (var r in roles)
                {
                    GroupRole role = r.Value;

                    if (role.ID == UUID.Zero)
                    {
                        abilities |= role.Powers;
                        continue;
                    }

                    ListViewItem item = new ListViewItem();
                    item.Name = role.Name;
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, role.Name));
                    item.Tag = role;
                    var foundRole = roleMembers.Find((KeyValuePair<UUID, UUID> kvp) => { return kvp.Value == m.Base.ID && kvp.Key == role.ID; });
                    bool hasRole = foundRole.Value == m.Base.ID;
                    item.Checked = hasRole;
                    lvwAssignedRoles.Items.Add(item);

                    if (hasRole)
                        abilities |= role.Powers;
                }
            }

            lvwAssignedRoles.ItemChecked += lvwAssignedRoles_ItemChecked;
            lvwAssignedRoles.EndUpdate();

            lvwAllowedAbilities.BeginUpdate();
            lvwAllowedAbilities.Items.Clear();

            foreach (GroupPowers p in Enum.GetValues(typeof(GroupPowers)))
            {
                if (p != GroupPowers.None && (abilities & p) == p)
                {
                    lvwAllowedAbilities.Items.Add(p.ToString());
                }
            }


            lvwAllowedAbilities.EndUpdate();

        }

        private void UpdateMemberRoles()
        {
            EnhancedGroupMember m = (EnhancedGroupMember)lvwAssignedRoles.Tag;
            GroupRoleChangesPacket p = new GroupRoleChangesPacket();
            p.AgentData.AgentID = client.Self.AgentID;
            p.AgentData.SessionID = client.Self.SessionID;
            p.AgentData.GroupID = group.ID;
            List<GroupRoleChangesPacket.RoleChangeBlock> changes = new List<GroupRoleChangesPacket.RoleChangeBlock>();

            foreach (ListViewItem item in lvwAssignedRoles.Items)
            {
                if (!(item.Tag is GroupRole))
                    continue;

                GroupRole role = (GroupRole)item.Tag;
                var foundRole = roleMembers.Find((KeyValuePair<UUID, UUID> kvp) => { return kvp.Value == m.Base.ID && kvp.Key == role.ID; });
                bool hasRole = foundRole.Value == m.Base.ID;

                if (item.Checked != hasRole)
                {
                    if (item.Checked)
                        roleMembers.Add(new KeyValuePair<UUID, UUID>(role.ID, m.Base.ID));
                    else
                        roleMembers.Remove(foundRole);

                    var rc = new GroupRoleChangesPacket.RoleChangeBlock();
                    rc.MemberID = m.Base.ID;
                    rc.RoleID = role.ID;
                    rc.Change = item.Checked ? 0u : 1u;
                    changes.Add(rc);
                }
            }

            if (changes.Count > 0)
            {
                p.RoleChange = changes.ToArray();
                client.Network.CurrentSim.SendPacket(p);
            }

            btnApply.Enabled = false;
            lvwMemberDetails_SelectedIndexChanged(lvwMemberDetails, EventArgs.Empty);
        }


        private void lvwAssignedRoles_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Tag == null) // click on the default role
            {
                if (!e.Item.Checked)
                    e.Item.Checked = true;

                return;
            }
            if (e.Item.Tag is GroupRole)
            {
                EnhancedGroupMember m = (EnhancedGroupMember)lvwAssignedRoles.Tag;
                bool modified = false;

                foreach (ListViewItem item in lvwAssignedRoles.Items)
                {
                    if (!(item.Tag is GroupRole))
                        continue;

                    GroupRole role = (GroupRole)item.Tag;
                    var foundRole = roleMembers.Find((KeyValuePair<UUID, UUID> kvp) => { return kvp.Value == m.Base.ID && kvp.Key == role.ID; });
                    bool hasRole = foundRole.Value == m.Base.ID;

                    if (item.Checked != hasRole)
                    {
                        modified = true;
                    }
                }

                btnApply.Enabled = modified;
            }
        }

        private void tbxCharter_TextChanged(object sender, EventArgs e)
        {
            btnApply.Enabled = true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            switch (tcGroupDetails.SelectedTab.Name)
            {
                case "tpMembersRoles":
                    UpdateMemberRoles();
                    break;
            }
        }

        private void btnInviteNewMember_Click(object sender, EventArgs e)
        {
            (new GroupInvite(instance, group, roles)).Show();
        }

        private void lvwAllowedAbilities_SizeChanged(object sender, EventArgs e)
        {
            lvwAllowedAbilities.Columns[0].Width = lvwAllowedAbilities.Width - 30;
        }

        private void tcMembersRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tcMembersRoles.SelectedTab.Name)
            {
                case "tpMembers":
                    RefreshMembersRoles();
                    break;

                case "tpRoles":
                    RefreshRoles();
                    break;

            }

        }

        private void lvwRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRoleDescription.Text = txtRoleName.Text = txtRoleTitle.Text = string.Empty;
            txtRoleDescription.Enabled = txtRoleName.Enabled = txtRoleTitle.Enabled = lvwRoleAbilitis.Enabled = btnSaveRole.Enabled = false;
            lvwAssignedMembers.Items.Clear();
            lvwRoleAbilitis.Items.Clear();

            if (lvwRoles.SelectedItems.Count != 1) return;

            GroupRole role = (GroupRole)lvwRoles.SelectedItems[0].Tag;
            txtRoleName.Text = role.Name;
            txtRoleTitle.Text = role.Title;
            txtRoleDescription.Text = role.Description;

            if (HasPower(GroupPowers.RoleProperties))
            {
                txtRoleDescription.Enabled = txtRoleName.Enabled = txtRoleTitle.Enabled = btnSaveRole.Enabled = true;
            }

            if (HasPower(GroupPowers.ChangeActions))
            {
                lvwRoleAbilitis.Enabled = btnSaveRole.Enabled = true;
            }

            btnSaveRole.Tag = role;

            if (role.ID == UUID.Zero)
            {
                foreach (ListViewItem item in lvwMemberDetails.Items)
                    lvwAssignedMembers.Items.Add(item.Text);
            }
            else if (roleMembers != null)
            {
                var mmb = roleMembers.FindAll((KeyValuePair<UUID, UUID> kvp) => { return kvp.Key == role.ID; });
                foreach (var m in mmb)
                {
                    lvwAssignedMembers.Items.Add(instance.Names.Get(m.Value));
                }
            }

            lvwRoleAbilitis.Tag = role;

            foreach (GroupPowers p in Enum.GetValues(typeof(GroupPowers)))
            {
                if (p != GroupPowers.None)
                {
                    ListViewItem item = new ListViewItem();
                    item.Tag = p;
                    item.SubItems.Add(p.ToString());
                    item.Checked = (p & role.Powers) != 0;
                    lvwRoleAbilitis.Items.Add(item);
                }
            }
        }

        private void btnCreateNewRole_Click(object sender, EventArgs e)
        {
            lvwRoles.SelectedItems.Clear();
            txtRoleDescription.Enabled = txtRoleName.Enabled = txtRoleTitle.Enabled = btnSaveRole.Enabled = true;
            btnSaveRole.Tag = null;
            txtRoleName.Focus();
        }

        private void btnSaveRole_Click(object sender, EventArgs e)
        {
            if (btnSaveRole.Tag == null) // new role
            {
                GroupRole role = new GroupRole();
                role.Name = txtRoleName.Text;
                role.Title = txtRoleTitle.Text;
                role.Description = txtRoleDescription.Text;
                client.Groups.CreateRole(group.ID, role);
                System.Threading.Thread.Sleep(100);
                RefreshRoles();
            }
            else if (btnSaveRole.Tag is GroupRole) // update role
            {
                GroupRole role = (GroupRole)btnSaveRole.Tag;

                if (HasPower(GroupPowers.ChangeActions))
                {
                    role.Powers = GroupPowers.None;

                    foreach (ListViewItem item in lvwRoleAbilitis.Items)
                    {
                        if (item.Checked)
                            role.Powers |= (GroupPowers)item.Tag;
                    }
                }

                if (HasPower(GroupPowers.RoleProperties))
                {
                    role.Name = txtRoleName.Text;
                    role.Title = txtRoleTitle.Text;
                    role.Description = txtRoleDescription.Text;
                }

                client.Groups.UpdateRole(role);
                System.Threading.Thread.Sleep(100);
                RefreshRoles();
            }
        }

        private void btnDeleteRole_Click(object sender, EventArgs e)
        {
            if (lvwRoles.SelectedItems.Count == 1)
            {
                client.Groups.DeleteRole(group.ID, ((GroupRole)lvwRoles.SelectedItems[0].Tag).ID);
                System.Threading.Thread.Sleep(100);
                RefreshRoles();
            }
        }

        #region New notice
        private void btnNewNotice_Click(object sender, EventArgs e)
        {
            if (HasPower(GroupPowers.SendNotices))
            {
                pnlArchivedNotice.Visible = false;
                pnlNewNotice.Visible = true;
                txtNewNoticeTitle.Focus();
            }
            else
            {
                instance.TabConsole.DisplayNotificationInChat("Don't have permission to send notices in this group", ChatBufferTextStyle.Error);
            }
        }

        private void btnPasteInv_Click(object sender, EventArgs e)
        {
            if (instance.InventoryClipboard != null && instance.InventoryClipboard.Item is InventoryItem)
            {
                InventoryItem inv = instance.InventoryClipboard.Item as InventoryItem;
                txtNewNoteAtt.Text = inv.Name;
                int icoIndx = InventoryConsole.GetItemImageIndex(inv.AssetType.ToString().ToLower());
                if (icoIndx >= 0)
                {
                    icnNewNoticeAtt.Image = frmMain.ResourceImages.Images[icoIndx];
                    icnNewNoticeAtt.Visible = true;
                }
                txtNewNoteAtt.Tag = inv;
                btnRemoveAttachment.Enabled = true;
            }
        }

        private void btnRemoveAttachment_Click(object sender, EventArgs e)
        {
            txtNewNoteAtt.Tag = null;
            txtNewNoteAtt.Text = string.Empty;
            btnRemoveAttachment.Enabled = false;
            icnNewNoticeAtt.Visible = false;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            GroupNotice ntc = new GroupNotice();
            ntc.Subject = txtNewNoticeTitle.Text;
            ntc.Message = txtNewNoticeBody.Text;
            if (txtNewNoteAtt.Tag != null && txtNewNoteAtt.Tag is InventoryItem)
            {
                InventoryItem inv = txtNewNoteAtt.Tag as InventoryItem;
                ntc.OwnerID = inv.OwnerID;
                ntc.AttachmentID = inv.UUID;
            }
            client.Groups.SendGroupNotice(group.ID, ntc);
            btnRemoveAttachment.PerformClick();
            txtNewNoticeTitle.Text = txtNewNoticeBody.Text = string.Empty;
            pnlNewNotice.Visible = false;
            btnRefresh.PerformClick();
            instance.TabConsole.DisplayNotificationInChat("Notice sent", ChatBufferTextStyle.Invisible);
        }
        #endregion
    }

    public class EnhancedGroupMember
    {
        public GroupMember Base;
        public DateTime LastOnline;

        public EnhancedGroupMember(GroupMember baseMember)
        {
            Base = baseMember;

            if (baseMember.OnlineStatus == "Online")
            {
                LastOnline = DateTime.Now;
            }
            else if (string.IsNullOrEmpty(baseMember.OnlineStatus) || baseMember.OnlineStatus == "unknown")
            {
                LastOnline = DateTime.MinValue;
            }
            else
            {
                try
                {
                    LastOnline = Convert.ToDateTime(baseMember.OnlineStatus, Utils.EnUsCulture);
                }
                catch (FormatException)
                {
                    LastOnline = DateTime.MaxValue;
                }
            }
        }
    }

    #region Sorter classes
    public class GroupMemberSorter : IComparer
    {
        public enum SortByColumn
        {
            Name,
            Title,
            LastOnline,
            Contribution
        }

        public enum SortOrder
        {
            Ascending,
            Descending
        }

        public SortOrder CurrentOrder = SortOrder.Ascending;
        public SortByColumn SortBy = SortByColumn.Name;

        public int Compare(object x, object y)
        {
            ListViewItem item1 = (ListViewItem)x;
            ListViewItem item2 = (ListViewItem)y;
            EnhancedGroupMember member1 = (EnhancedGroupMember)item1.Tag;
            EnhancedGroupMember member2 = (EnhancedGroupMember)item2.Tag;

            switch (SortBy)
            {
                case SortByColumn.Name:
                    if (CurrentOrder == SortOrder.Ascending)
                        return string.Compare(item1.Text, item2.Text);
                    else
                        return string.Compare(item2.Text, item1.Text);

                case SortByColumn.Title:
                    if (CurrentOrder == SortOrder.Ascending)
                        return string.Compare(member1.Base.Title, member2.Base.Title);
                    else
                        return string.Compare(member2.Base.Title, member1.Base.Title);

                case SortByColumn.LastOnline:
                    if (CurrentOrder == SortOrder.Ascending)
                        return DateTime.Compare(member1.LastOnline, member2.LastOnline);
                    else
                        return DateTime.Compare(member2.LastOnline, member1.LastOnline);

                case SortByColumn.Contribution:
                    if (member1.Base.Contribution < member2.Base.Contribution)
                        return CurrentOrder == SortOrder.Ascending ? -1 : 1;
                    else if (member1.Base.Contribution > member2.Base.Contribution)
                        return CurrentOrder == SortOrder.Ascending ? 1 : -1;
                    else
                        return 0;
            }

            return 0;
        }
    }


    public class GroupNoticeSorter : IComparer
    {
        public enum SortByColumn
        {
            Subject,
            Sender,
            Date
        }

        public enum SortOrder
        {
            Ascending,
            Descending
        }

        public SortOrder CurrentOrder = SortOrder.Descending;
        public SortByColumn SortBy = SortByColumn.Date;

        private int IntCompare(uint x, uint y)
        {
            if (x < y)
            {
                return -1;
            }
            else if (x > y)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int Compare(object x, object y)
        {
            ListViewItem item1 = (ListViewItem)x;
            ListViewItem item2 = (ListViewItem)y;
            GroupNoticesListEntry member1 = (GroupNoticesListEntry)item1.Tag;
            GroupNoticesListEntry member2 = (GroupNoticesListEntry)item2.Tag;

            switch (SortBy)
            {
                case SortByColumn.Subject:
                    if (CurrentOrder == SortOrder.Ascending)
                        return string.Compare(member1.Subject, member2.Subject);
                    else
                        return string.Compare(member2.Subject, member1.Subject);

                case SortByColumn.Sender:
                    if (CurrentOrder == SortOrder.Ascending)
                        return string.Compare(member1.FromName, member2.FromName);
                    else
                        return string.Compare(member2.FromName, member1.FromName);

                case SortByColumn.Date:
                    if (CurrentOrder == SortOrder.Ascending)
                        return IntCompare(member1.Timestamp, member2.Timestamp);
                    else
                        return IntCompare(member2.Timestamp, member1.Timestamp);
            }

            return 0;
        }
    }
    #endregion
}
