using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
using RadegastNc;
using System.Diagnostics;
using OpenMetaverse.Packets;

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

            console = new FindPeopleConsole(instance, UUID.Random());
            console.Dock = DockStyle.Fill;
            console.SelectedIndexChanged += new EventHandler(console_SelectedIndexChanged);
            pnlFindPeople.Controls.Add(console);
        }

        void SearchConsole_Disposed(object sender, EventArgs e)
        {
            client.Directory.OnDirPeopleReply -= new DirectoryManager.DirPeopleReplyCallback(Directory_OnDirPeopleReply);
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

    }
}
