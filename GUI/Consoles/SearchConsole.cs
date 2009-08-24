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
using System.Diagnostics;

namespace Radegast
{
    public partial class SearchConsole : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }

        private FindPeopleConsole console;

        private string lastQuery = string.Empty;
        private int startResult = 0;

        private int totalResults = 0;

        public SearchConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(SearchConsole_Disposed);

            this.instance = instance;

            // Callbacks
            client.Directory.OnDirPeopleReply += new DirectoryManager.DirPeopleReplyCallback(Directory_OnDirPeopleReply);
            client.Directory.OnDirPlacesReply += new DirectoryManager.DirPlacesReplyCallback(Directory_OnDirPlacesReply);
            console = new FindPeopleConsole(instance, UUID.Random());
            console.Dock = DockStyle.Fill;
            console.SelectedIndexChanged += new EventHandler(console_SelectedIndexChanged);
            pnlFindPeople.Controls.Add(console);
            lvwPlaces.ListViewItemSorter = new PlaceSorter();
        }

        void SearchConsole_Disposed(object sender, EventArgs e)
        {
            client.Directory.OnDirPeopleReply -= new DirectoryManager.DirPeopleReplyCallback(Directory_OnDirPeopleReply);
            client.Directory.OnDirPlacesReply -= new DirectoryManager.DirPlacesReplyCallback(Directory_OnDirPlacesReply);
        }

        private void console_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnNewIM.Enabled = btnProfile.Enabled = (console.SelectedName != null);
        }


        //Separate thread
        private void Directory_OnDirPeopleReply(UUID queryID, List<DirectoryManager.AgentSearchData> matchedPeople)
        {
            BeginInvoke(new DirectoryManager.DirPeopleReplyCallback(PeopleReply), new object[] { queryID, matchedPeople });
        }

        //UI thread
        private void PeopleReply(UUID queryID, List<DirectoryManager.AgentSearchData> matchedPeople)
        {
            if (console.QueryID != queryID) return;

            totalResults += matchedPeople.Count;
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
            console.QueryID = client.Directory.StartPeopleSearch(
                DirectoryManager.DirFindFlags.NameSort |
                DirectoryManager.DirFindFlags.SortAsc |
                DirectoryManager.DirFindFlags.People,
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
            (new frmProfile(instance, console.SelectedName, console.SelectedAgentUUID)).Show();
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

        #region Places search
        private UUID placeSearch;
        private int placeMatches = 0;
        private int placeStart = 0;

        void Directory_OnDirPlacesReply(UUID queryID, List<DirectoryManager.DirectoryParcel> matchedParcels)
        {
            if (queryID != placeSearch) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Directory_OnDirPlacesReply(queryID, matchedParcels)));
                return;
            }

            lvwPlaces.BeginUpdate();

            if (placeMatches == 0)
                lvwPlaces.Items.Clear();

            foreach (DirectoryManager.DirectoryParcel parcel in matchedParcels)
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

            placeMatches += matchedParcels.Count;

            btnNextPlace.Enabled = placeMatches > 100;
            btnPrevPlace.Enabled = placeStart != 0;

            if (matchedParcels.Count > 0 && matchedParcels[matchedParcels.Count - 1].ID == UUID.Zero)
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
            placeSearch = client.Directory.StartDirPlacesSearch(txtSearchPlace.Text.Trim(), 0);
        }

        private void btnNextPlace_Click(object sender, EventArgs e)
        {
            placeMatches = 0;
            placeStart += 100;
            placeSearch = client.Directory.StartDirPlacesSearch(txtSearchPlace.Text.Trim(), placeStart);
        }

        private void btnPrevPlace_Click(object sender, EventArgs e)
        {
            placeMatches = 0;
            placeStart -= 100;
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
    }
}
