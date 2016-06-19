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
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenMetaverse;
using Radegast.Netcom;

namespace Radegast
{
    public partial class FindPeopleConsole : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;

        private UUID queryID;
        private Dictionary<string, UUID> findPeopleResults;

        public event EventHandler SelectedIndexChanged;

        public FindPeopleConsole(RadegastInstance instance, UUID queryID)
        {
            InitializeComponent();
            Disposed += new EventHandler(FindPeopleConsole_Disposed);

            findPeopleResults = new Dictionary<string, UUID>();
            this.queryID = queryID;

            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;

            // Callbacks
            client.Directory.DirPeopleReply += new EventHandler<DirPeopleReplyEventArgs>(Directory_DirPeopleReply);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void FindPeopleConsole_Disposed(object sender, EventArgs e)
        {
            client.Directory.DirPeopleReply -= new EventHandler<DirPeopleReplyEventArgs>(Directory_DirPeopleReply);
        }

        void Directory_DirPeopleReply(object sender, DirPeopleReplyEventArgs e)
        {
            if (e.QueryID != this.queryID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Directory_DirPeopleReply(sender, e)));
                return;
            }

            lvwFindPeople.BeginUpdate();

            foreach (DirectoryManager.AgentSearchData person in e.MatchedPeople)
            {
                string fullName = person.FirstName + " " + person.LastName;
                findPeopleResults.Add(fullName, person.AgentID);

                ListViewItem item = lvwFindPeople.Items.Add(fullName);
                item.SubItems.Add(person.Online ? "Yes" : "No");
            }

            lvwFindPeople.Sort();
            lvwFindPeople.EndUpdate();
        }

        public void ClearResults()
        {
            findPeopleResults.Clear();
            lvwFindPeople.Items.Clear();
        }

        private void lvwFindPeople_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged(e);
        }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            if (SelectedIndexChanged != null) SelectedIndexChanged(this, e);
        }

        public Dictionary<string, UUID> LLUUIDs
        {
            get { return findPeopleResults; }
        }

        public UUID QueryID
        {
            get { return queryID; }
            set { queryID = value; }
        }

        public int SelectedIndex
        {
            get
            {
                if (lvwFindPeople.SelectedItems == null) return -1;
                if (lvwFindPeople.SelectedItems.Count == 0) return -1;

                return lvwFindPeople.SelectedIndices[0];
            }
        }

        public string SelectedName
        {
            get
            {
                if (lvwFindPeople.SelectedItems == null) return string.Empty;
                if (lvwFindPeople.SelectedItems.Count == 0) return string.Empty;

                return lvwFindPeople.SelectedItems[0].Text;
            }
        }

        public bool SelectedOnlineStatus
        {
            get
            {
                if (lvwFindPeople.SelectedItems == null) return false;
                if (lvwFindPeople.SelectedItems.Count == 0) return false;

                string yesNo = lvwFindPeople.SelectedItems[0].SubItems[0].Text;

                if (yesNo == "Yes")
                    return true;
                else if (yesNo == "No")
                    return false;
                else
                    return false;
            }
        }

        public UUID SelectedAgentUUID
        {
            get
            {
                if (lvwFindPeople.SelectedItems == null) return UUID.Zero;
                if (lvwFindPeople.SelectedItems.Count == 0) return UUID.Zero;

                string name = lvwFindPeople.SelectedItems[0].Text;
                return findPeopleResults[name];
            }
        }
    }
}
