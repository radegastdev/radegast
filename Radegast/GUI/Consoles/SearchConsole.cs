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
using Radegast.Netcom;
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
