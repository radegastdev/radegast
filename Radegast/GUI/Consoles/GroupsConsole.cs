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
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class GroupsConsole : UserControl
    {
        GridClient client;
        RadegastInstance instance;
 
        public GroupsConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(GroupsDialog_Disposed);
            this.client = instance.Client;
            this.instance = instance;
            client.Groups.OnCurrentGroups += new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
            client.Groups.RequestCurrentGroups();
            UpdateDisplay();
        }

        void GroupsDialog_Disposed(object sender, EventArgs e)
        {
            client.Groups.OnCurrentGroups -= new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
        }

        void Groups_OnCurrentGroups(Dictionary<UUID, Group> groups)
        {
            BeginInvoke(new MethodInvoker(UpdateDisplay));
        }

        public void UpdateDisplay()
        {
            Group none = new Group();
            none.Name = "(none)";
            none.ID = UUID.Zero;

            listBox1.Items.Clear();
            listBox1.Items.Add(none);

            foreach (Group g in instance.Groups.Values) {
                listBox1.Items.Add(g);
            }

            foreach (Group g in listBox1.Items)
            {
                if (g.ID == client.Self.ActiveGroup)
                {
                    listBox1.SelectedItem = g;
                    break;
                }
            }
        }

        private void activateBtn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            client.Groups.ActivateGroup(g.ID);
        }

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero) return;

            client.Groups.LeaveGroup(g.ID);
            listBox1.Items.Remove(g);
        }

        private void imBtn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero) return;

            if (!instance.TabConsole.TabExists(g.ID.ToString()))
            {
                instance.TabConsole.AddGroupIMTab(g.ID, g.Name);
                instance.TabConsole.Tabs[g.ID.ToString()].Highlight();
                instance.TabConsole.Tabs[g.ID.ToString()].Select();

            }
            else
            {
                RadegastTab t = instance.TabConsole.Tabs[g.ID.ToString()];
                if (!t.Selected)
                {
                    t.Highlight();
                }
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero) return;

            (new frmGroupInfo(instance, g)).Show();
        }
    }
}