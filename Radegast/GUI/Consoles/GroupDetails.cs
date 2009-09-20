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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class GroupDetails : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private Group group;
        private Dictionary<UUID, GroupTitle> titles;
        private Dictionary<UUID, Group> myGroups { get { return instance.Groups; } }

        private UUID groupTitlesRequest;
        private UUID groupMembersRequest;

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

            lvwNoticeArchive.SmallImageList = frmMain.ResourceImages;
            lvwNoticeArchive.ListViewItemSorter = new GroupNoticeSorter();

            // Callbacks
            client.Groups.OnGroupTitles += new GroupManager.GroupTitlesCallback(Groups_OnGroupTitles);
            client.Groups.OnGroupMembers += new GroupManager.GroupMembersCallback(Groups_OnGroupMembers);
            client.Groups.OnGroupProfile += new GroupManager.GroupProfileCallback(Groups_OnGroupProfile);
            client.Groups.OnCurrentGroups += new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
            client.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client.Groups.OnGroupNoticesList += new GroupManager.GroupNoticesListCallback(Groups_OnGroupNoticesList);
            client.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            RefreshControlsAvailability();
            RefreshGroupInfo();
        }

        void GroupDetails_Disposed(object sender, EventArgs e)
        {
            client.Groups.OnGroupTitles -= new GroupManager.GroupTitlesCallback(Groups_OnGroupTitles);
            client.Groups.OnGroupMembers -= new GroupManager.GroupMembersCallback(Groups_OnGroupMembers);
            client.Groups.OnGroupProfile -= new GroupManager.GroupProfileCallback(Groups_OnGroupProfile);
            client.Groups.OnCurrentGroups -= new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
            client.Avatars.OnAvatarNames -= new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client.Groups.OnGroupNoticesList -= new GroupManager.GroupNoticesListCallback(Groups_OnGroupNoticesList);
            client.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
        }

        #region Network callbacks
        UUID destinationFolderID;

        void Self_OnInstantMessage(InstantMessage im, Simulator simulator)
        {
            if (im.Dialog != InstantMessageDialog.GroupNoticeRequested || im.FromAgentID != group.ID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Self_OnInstantMessage(im, simulator)));
                return;
            }

            InstantMessage msg = im;
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

        void Groups_OnGroupNoticesList(UUID groupID, GroupNoticeList notice)
        {
            if (groupID != group.ID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Groups_OnGroupNoticesList(groupID, notice)));
                return;
            }
            
            lvwNoticeArchive.BeginUpdate();

            ListViewItem item = new ListViewItem();
            item.SubItems.Add(notice.Subject);
            item.SubItems.Add(notice.FromName);
            item.SubItems.Add(Utils.UnixTimeToDateTime(notice.Timestamp).ToShortDateString());

            if (notice.HasAttachment)
            {
                item.ImageIndex = InventoryConsole.GetItemImageIndex(notice.AssetType.ToString().ToLower());
            }

            item.Tag = notice;

            lvwNoticeArchive.Items.Add(item);

            lvwNoticeArchive.EndUpdate();
        }

        void Groups_OnCurrentGroups(Dictionary<UUID, Group> groups)
        {
            BeginInvoke(new MethodInvoker(RefreshControlsAvailability));
        }

        void Groups_OnGroupProfile(Group group)
        {
            if (group.ID != this.group.ID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Groups_OnGroupProfile(group);
                    }
                ));
                return;
            }

            this.group = group;

            tbxCharter.Text = group.Charter;
            lblFounded.Text = "Founded by: " + instance.getAvatarName(group.FounderID);
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

            if (myGroups.ContainsKey(group.ID))
            {
                cbxReceiveNotices.Checked = myGroups[group.ID].AcceptNotices;
                cbxListInProfile.Checked = myGroups[group.ID].ListInProfile;
                cbxReceiveNotices.CheckedChanged += new EventHandler(cbxListInProfile_CheckedChanged);
                cbxListInProfile.CheckedChanged += new EventHandler(cbxListInProfile_CheckedChanged);
            }
        }

        void Avatars_OnAvatarNames(Dictionary<UUID, string> names)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Avatars_OnAvatarNames(names);
                    }
                ));
                return;
            }

            if (names.ContainsKey(group.FounderID))
            {
                lblFounded.Text = "Founded by: " + names[group.FounderID];
            }

            lvwGeneralMembers.BeginUpdate();
            bool modified = false;
            foreach (KeyValuePair<UUID, string> name in names)
            {
                if (lvwGeneralMembers.Items.ContainsKey(name.Key.ToString()))
                {
                    lvwGeneralMembers.Items[name.Key.ToString()].Text = name.Value;
                    modified = true;
                }
            }
            if (modified)
                lvwGeneralMembers.Sort();
            lvwGeneralMembers.EndUpdate();
        }

        void Groups_OnGroupTitles(UUID requestID, UUID groupID, Dictionary<UUID, GroupTitle> titles)
        {
            if (groupTitlesRequest != requestID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Groups_OnGroupTitles(requestID, groupID, titles);
                    }
                ));
                return;
            }

            this.titles = titles;

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

        void Groups_OnGroupMembers(UUID requestID, UUID groupID, Dictionary<UUID, GroupMember> members)
        {
            if (groupMembersRequest != requestID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Groups_OnGroupMembers(requestID, groupID, members);
                    }
                ));
                return;
            }

            lvwGeneralMembers.BeginUpdate();
            List<ListViewItem> newItems = new List<ListViewItem>();
            List<UUID> unknownNames = new List<UUID>();

            foreach (GroupMember baseMember in members.Values)
            {
                EnhancedGroupMember member = new EnhancedGroupMember(baseMember);
                string name;
                
                if (instance.haveAvatarName(member.Base.ID))
                {
                    name = instance.getAvatarName(member.Base.ID);
                }
                else
                {
                    name = "Loading...";
                    unknownNames.Add(member.Base.ID);
                }

                ListViewItem item = new ListViewItem(name);
                item.Tag = member;
                item.Name = member.Base.ID.ToString();
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, member.Base.Title));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, member.Base.OnlineStatus));

                newItems.Add(item);
            }

            if (unknownNames.Count > 0)
            {
                instance.getAvatarNames(unknownNames);
            }

            lvwGeneralMembers.Items.AddRange(newItems.ToArray());
            lvwGeneralMembers.Sort();
            lvwGeneralMembers.EndUpdate();
        }
        #endregion

        #region Privatate methods
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
            client.Groups.RequestGroupNoticeList(group.ID);
        }

        private void RefreshGroupInfo()
        {
            lvwGeneralMembers.Items.Clear();
            cbxActiveTitle.SelectedIndexChanged -= cbxActiveTitle_SelectedIndexChanged;
            cbxReceiveNotices.CheckedChanged -= new EventHandler(cbxListInProfile_CheckedChanged);
            cbxListInProfile.CheckedChanged -= new EventHandler(cbxListInProfile_CheckedChanged);

            cbxActiveTitle.Items.Clear();

            // Request group info
            client.Groups.RequestGroupProfile(group.ID);
            groupTitlesRequest = client.Groups.RequestGroupTitles(group.ID);
            groupMembersRequest = client.Groups.RequestGroupMembers(group.ID);
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
            }
        }
        #endregion

        void lvwGeneralMembers_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            GroupMemberSorter sorter = (GroupMemberSorter)lvwGeneralMembers.ListViewItemSorter;
            switch (e.Column)
            {
                case 0:
                    sorter.SortBy = GroupMemberSorter.SortByColumn.Name;
                    break;

                case 1:
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

            lvwGeneralMembers.Sort();
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
            }
        }

        private void lvwNoticeArchive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwNoticeArchive.SelectedItems.Count == 1)
            {
                if (lvwNoticeArchive.SelectedItems[0].Tag is GroupNoticeList)
                {
                    GroupNoticeList notice = (GroupNoticeList)lvwNoticeArchive.SelectedItems[0].Tag;
                    lblSentBy.Text = "Sent by " + notice.FromName;
                    lblTitle.Text = notice.Subject;
                    txtNotice.Text = string.Empty;
                    btnSave.Enabled = btnSave.Visible = icnItem.Visible = txtItemName.Visible = false;
                    client.Groups.RequestGroupNotice(notice.NoticeID);
                }
            }
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
            else
            {
                LastOnline = Convert.ToDateTime(baseMember.OnlineStatus, Utils.EnUsCulture);
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
            LastOnline
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
            GroupNoticeList member1 = (GroupNoticeList)item1.Tag;
            GroupNoticeList member2 = (GroupNoticeList)item2.Tag;

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
