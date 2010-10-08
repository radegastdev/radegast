// 
// Radegast Metaverse Client
// Copyright (c) 2009-2010, Radegast Development Team
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
using System.Windows.Forms;
using OpenMetaverse;
using System.Diagnostics;

namespace Radegast
{
    public partial class SearchConsole : UserControl
    {
        #region Private members
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }

        private FindPeopleConsole console;

        private string lastQuery = string.Empty;
        private int startResult = 0;

        private int totalResults = 0;
        #endregion Private members

        #region Construction and disposal
        public SearchConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(SearchConsole_Disposed);

            this.instance = instance;

            // Callbacks
            client.Directory.DirPeopleReply += new EventHandler<DirPeopleReplyEventArgs>(Directory_DirPeopleReply);
            client.Directory.DirPlacesReply += new EventHandler<DirPlacesReplyEventArgs>(Directory_DirPlacesReply);
            client.Directory.DirGroupsReply += new EventHandler<DirGroupsReplyEventArgs>(Directory_DirGroupsReply);
            console = new FindPeopleConsole(instance, UUID.Random());
            console.Dock = DockStyle.Fill;
            console.SelectedIndexChanged += new EventHandler(console_SelectedIndexChanged);
            pnlFindPeople.Controls.Add(console);
            lvwPlaces.ListViewItemSorter = new PlaceSorter();
            lvwGroups.ListViewItemSorter = new GroupSorter();
        }

        void SearchConsole_Disposed(object sender, EventArgs e)
        {
            client.Directory.DirPeopleReply -= new EventHandler<DirPeopleReplyEventArgs>(Directory_DirPeopleReply);
            client.Directory.DirPlacesReply -= new EventHandler<DirPlacesReplyEventArgs>(Directory_DirPlacesReply);
            client.Directory.DirGroupsReply -= new EventHandler<DirGroupsReplyEventArgs>(Directory_DirGroupsReply);
        }

        #endregion Construction and disposal

        #region People search
        private void console_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnNewIM.Enabled = btnProfile.Enabled = (console.SelectedName != null);
        }


        void Directory_DirPeopleReply(object sender, DirPeopleReplyEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Directory_DirPeopleReply(sender, e)));
                return;
            }

            if (console.QueryID != e.QueryID) return;

            totalResults += e.MatchedPeople.Count;
            lblResultCount.Text = totalResults.ToString() + " people found";

            txtPersonName.Enabled = true;
            btnFind.Enabled = true;

            btnNext.Enabled = (totalResults > 100);
            btnPrevious.Enabled = (startResult > 0);
        }

        private void txtPersonName_TextChanged(object sender, EventArgs e)
        {
            btnFind.Enabled = (txtPersonName.Text.Trim().Length > 2);
        }

        private void btnNewIM_Click(object sender, EventArgs e)
        {
            if (instance.TabConsole.TabExists((client.Self.AgentID ^ console.SelectedAgentUUID).ToString()))
            {
                instance.TabConsole.SelectTab((client.Self.AgentID ^ console.SelectedAgentUUID).ToString());
                return;
            }

            instance.TabConsole.AddIMTab(console.SelectedAgentUUID, client.Self.AgentID ^ console.SelectedAgentUUID, console.SelectedName);
            instance.TabConsole.SelectTab((client.Self.AgentID ^ console.SelectedAgentUUID).ToString());
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            lastQuery = txtPersonName.Text;
            startResult = 0;
            StartFinding();
        }

        private void txtPersonName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (txtPersonName.Text.Trim().Length < 3) return;

            lastQuery = txtPersonName.Text;
            startResult = 0;
            StartFinding();
        }

        private void StartFinding()
        {
            totalResults = 0;
            lblResultCount.Text = "Searching for " + lastQuery;

            txtPersonName.Enabled = false;
            btnFind.Enabled = false;
            btnNewIM.Enabled = false;
            btnProfile.Enabled = false;
            btnPrevious.Enabled = false;
            btnNext.Enabled = false;

            console.ClearResults();
            console.QueryID = client.Directory.StartPeopleSearch(/*
                DirectoryManager.DirFindFlags.NameSort |
                DirectoryManager.DirFindFlags.SortAsc |
                DirectoryManager.DirFindFlags.People,*/
                lastQuery, startResult);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            startResult += 100;
            StartFinding();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            startResult -= 100;
            StartFinding();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            instance.MainForm.ShowAgentProfile(console.SelectedName, console.SelectedAgentUUID);
        }

        private void txtPersonName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(((Button)sender).Tag.ToString());
            Process.Start(sInfo);
        }
        #endregion People search

        #region Places search
        private UUID placeSearch;
        private int placeMatches = 0;
        private int placeStart = 0;

        void Directory_DirPlacesReply(object sender, DirPlacesReplyEventArgs e)
        {
            if (e.QueryID != placeSearch) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Directory_DirPlacesReply(sender, e)));
                return;
            }

            lvwPlaces.BeginUpdate();

            if (e.MatchedParcels.Count == 0)
                lvwPlaces.Items.Clear();

            foreach (DirectoryManager.DirectoryParcel parcel in e.MatchedParcels)
            {
                if (parcel.ID == UUID.Zero) continue;

                ListViewItem item = new ListViewItem();
                item.Name = parcel.ID.ToString();
                item.Text = parcel.Name;
                item.Tag = parcel;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, parcel.Dwell.ToString()));
                lvwPlaces.Items.Add(item);
            }
            lvwPlaces.Sort();
            lvwPlaces.EndUpdate();

            placeMatches += e.MatchedParcels.Count;

            btnNextPlace.Enabled = placeMatches > 100;
            btnPrevPlace.Enabled = placeStart != 0;

            if (e.MatchedParcels.Count > 0 && e.MatchedParcels[e.MatchedParcels.Count - 1].ID == UUID.Zero)
                placeMatches -= 1;

            lblNrPlaces.Visible = true;
            lblNrPlaces.Text = string.Format("{0} places found", placeMatches > 100 ? "More than " + (placeStart + 100).ToString() : (placeStart + placeMatches).ToString());


        }

        private void txtSearchPlace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                e.SuppressKeyPress = true;
        }

        private void txtSearchPlace_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (btnSearchPlace.Enabled)
                {
                    btnSearchPlace_Click(null, EventArgs.Empty);
                }
            }
        }

        private void txtSearchPlace_TextChanged(object sender, EventArgs e)
        {
            if (txtSearchPlace.Text.Length > 0)
                btnSearchPlace.Enabled = true;
            else
                btnSearchPlace.Enabled = false;
        }

        // StartPlacesSearch(DirFindFlags findFlags, ParcelCategory searchCategory, string searchText, string simulatorName, UUID groupID, UUID transactionID)
        private void btnSearchPlace_Click(object sender, EventArgs e)
        {
            placeMatches = 0;
            placeStart = 0;
            lvwPlaces.Items.Clear();
            placeSearch = client.Directory.StartDirPlacesSearch(txtSearchPlace.Text.Trim(), 0);
        }

        private void btnNextPlace_Click(object sender, EventArgs e)
        {
            placeMatches = 0;
            placeStart += 100;
            lvwPlaces.Items.Clear();
            placeSearch = client.Directory.StartDirPlacesSearch(txtSearchPlace.Text.Trim(), placeStart);
        }

        private void btnPrevPlace_Click(object sender, EventArgs e)
        {
            placeMatches = 0;
            placeStart -= 100;
            lvwPlaces.Items.Clear();
            placeSearch = client.Directory.StartDirPlacesSearch(txtSearchPlace.Text.Trim(), placeStart);
        }

        public class PlaceSorter : System.Collections.IComparer
        {
            public enum SortByColumn
            {
                Name,
                Traffic
            }

            public enum SortOrder
            {
                Ascending,
                Descending
            }

            public SortOrder CurrentOrder = SortOrder.Descending;
            public SortByColumn SortBy = SortByColumn.Traffic;

            public int Compare(object x, object y)
            {
                ListViewItem item1 = (ListViewItem)x;
                ListViewItem item2 = (ListViewItem)y;
                DirectoryManager.DirectoryParcel member1 = (DirectoryManager.DirectoryParcel)item1.Tag;
                DirectoryManager.DirectoryParcel member2 = (DirectoryManager.DirectoryParcel)item2.Tag;

                switch (SortBy)
                {
                    case SortByColumn.Name:
                        if (CurrentOrder == SortOrder.Ascending)
                            return string.Compare(item1.Text, item2.Text);
                        else
                            return string.Compare(item2.Text, item1.Text);

                    case SortByColumn.Traffic:
                        if (CurrentOrder == SortOrder.Ascending)
                        {
                            if (member1.Dwell > member2.Dwell)
                                return 1;
                            else if (member1.Dwell < member2.Dwell)
                                return -1;
                        }
                        else
                        {
                            if (member1.Dwell > member2.Dwell)
                                return -1;
                            else if (member1.Dwell < member2.Dwell)
                                return 1;
                        }
                        break;
                }

                return 0;
            }
        }

        private void lvwPlaces_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            PlaceSorter sorter = (PlaceSorter)lvwPlaces.ListViewItemSorter;
            switch (e.Column)
            {
                case 0:
                    sorter.SortBy = PlaceSorter.SortByColumn.Name;
                    break;

                case 1:
                    sorter.SortBy = PlaceSorter.SortByColumn.Traffic;
                    break;
            }

            if (sorter.CurrentOrder == PlaceSorter.SortOrder.Ascending)
                sorter.CurrentOrder = PlaceSorter.SortOrder.Descending;
            else
                sorter.CurrentOrder = PlaceSorter.SortOrder.Ascending;

            lvwPlaces.Sort();

        }

        private void lvwPlaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < pnlPlaceDetail.Controls.Count; i++)
            {
                pnlPlaceDetail.Controls[i].Dispose();
            }
            pnlPlaceDetail.Controls.Clear();

            if (lvwPlaces.SelectedItems.Count == 1)
            {
                Landmark l = new Landmark(instance, ((DirectoryManager.DirectoryParcel)lvwPlaces.SelectedItems[0].Tag).ID);
                l.Dock = DockStyle.Fill;
                pnlPlaceDetail.Controls.Add(l);
            }
        }
        #endregion Places search

        #region Groups search
        private UUID groupSearch;
        private int groupMatches = 0;
        private int groupStart = 0;

        void Directory_DirGroupsReply(object sender, DirGroupsReplyEventArgs e)
        {
            if (e.QueryID != groupSearch) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Directory_DirGroupsReply(sender, e)));
                return;
            }

            lvwGroups.BeginUpdate();

            if (e.MatchedGroups.Count == 0)
                lvwGroups.Items.Clear();

            foreach (DirectoryManager.GroupSearchData group in e.MatchedGroups)
            {
                if (group.GroupID == UUID.Zero) continue;

                ListViewItem item = new ListViewItem();
                item.Name = group.GroupID.ToString();
                item.Text = group.GroupName;
                item.Tag = group;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, group.Members.ToString()));

                lvwGroups.Items.Add(item);
            }
            lvwGroups.Sort();
            lvwGroups.EndUpdate();

            groupMatches += e.MatchedGroups.Count;
            btnNextGroup.Enabled = groupMatches > 100;
            btnPrevGroup.Enabled = placeStart != 0;

            if (e.MatchedGroups.Count > 0 && e.MatchedGroups[e.MatchedGroups.Count - 1].GroupID == UUID.Zero)
                groupMatches -= 1;

            lblNrGroups.Visible = true;
            lblNrGroups.Text = string.Format("{0} groups found", groupMatches > 100 ? "More than " + (groupStart + 100).ToString() : (groupStart + groupMatches).ToString());
        }

        private void btnSearchGroup_Click(object sender, EventArgs e)
        {
            groupMatches = 0;
            groupStart = 0;
            lvwGroups.Items.Clear();
            groupSearch = client.Directory.StartGroupSearch(txtSearchGroup.Text.Trim(), 0);
        }

        private void txtSearchGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = e.Handled = true;
                btnSearchGroup.PerformClick();
            }
        }

        private void txtSearchGroup_TextChanged(object sender, EventArgs e)
        {
            if (txtSearchGroup.Text.Length > 1)
            {
                btnSearchGroup.Enabled = true;
            }
            else
            {
                btnSearchGroup.Enabled = false;
            }
        }

        private void btnPrevGroup_Click(object sender, EventArgs e)
        {
            groupMatches = 0;
            groupStart -= 100;
            lvwGroups.Items.Clear();
            groupSearch = client.Directory.StartGroupSearch(txtSearchGroup.Text.Trim(), groupStart);
        }

        private void btnNextGroup_Click(object sender, EventArgs e)
        {
            groupMatches = 0;
            groupStart += 100;
            lvwGroups.Items.Clear();
            groupSearch = client.Directory.StartGroupSearch(txtSearchGroup.Text.Trim(), groupStart);
        }

        private void lvwGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Control c in pnlGroupDetail.Controls)
            {
                c.Dispose();
            }
            pnlGroupDetail.Controls.Clear();

            if (lvwGroups.SelectedItems.Count == 1)
            {
                try
                {
                    DirectoryManager.GroupSearchData g = (DirectoryManager.GroupSearchData)lvwGroups.SelectedItems[0].Tag;
                    GroupDetails grpPanel = new GroupDetails(instance, new Group() { ID = g.GroupID, Name = g.GroupName });
                    grpPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    grpPanel.Region = new System.Drawing.Region(
                        new System.Drawing.RectangleF(
                            grpPanel.tpGeneral.Left, grpPanel.tpGeneral.Top, grpPanel.tpGeneral.Width, grpPanel.tpGeneral.Height));
                    pnlGroupDetail.Controls.Add(grpPanel);
                }
                catch { }
            }
        }

        private void lvwGroups_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            GroupSorter sorter = (GroupSorter)lvwGroups.ListViewItemSorter;
            switch (e.Column)
            {
                case 0:
                    sorter.SortBy = GroupSorter.SortByColumn.Name;
                    break;

                case 1:
                    sorter.SortBy = GroupSorter.SortByColumn.Members;
                    break;
            }

            if (sorter.CurrentOrder == GroupSorter.SortOrder.Ascending)
                sorter.CurrentOrder = GroupSorter.SortOrder.Descending;
            else
                sorter.CurrentOrder = GroupSorter.SortOrder.Ascending;

            lvwGroups.Sort();
        }

        public class GroupSorter : System.Collections.IComparer
        {
            public enum SortByColumn
            {
                Name,
                Members
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
                DirectoryManager.GroupSearchData group1 = (DirectoryManager.GroupSearchData)item1.Tag;
                DirectoryManager.GroupSearchData group2 = (DirectoryManager.GroupSearchData)item2.Tag;

                switch (SortBy)
                {
                    case SortByColumn.Name:
                        if (CurrentOrder == SortOrder.Ascending)
                            return string.Compare(item1.Text, item2.Text);
                        else
                            return string.Compare(item2.Text, item1.Text);

                    case SortByColumn.Members:
                        if (CurrentOrder == SortOrder.Ascending)
                        {
                            if (group1.Members > group2.Members)
                                return 1;
                            else if (group1.Members < group2.Members)
                                return -1;
                        }
                        else
                        {
                            if (group1.Members > group2.Members)
                                return -1;
                            else if (group1.Members < group2.Members)
                                return 1;
                        }
                        break;
                }

                return 0;
            }
        }
        #endregion Groups search
    }
}
