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
using System.Drawing;
using OpenMetaverse;

namespace Radegast
{
    public partial class GroupsConsole : UserControl
    {
        GridClient client;
        RadegastInstance instance;
        UUID newGrpID;

        public GroupsConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(GroupsDialog_Disposed);
            this.client = instance.Client;
            this.instance = instance;
            client.Groups.CurrentGroups += new EventHandler<CurrentGroupsEventArgs>(Groups_CurrentGroups);
            client.Groups.GroupCreatedReply += new EventHandler<GroupCreatedReplyEventArgs>(Groups_GroupCreatedReply);
            client.Groups.GroupRoleDataReply += new EventHandler<GroupRolesDataReplyEventArgs>(Groups_GroupRoleDataReply);
            client.Self.MuteListUpdated += new EventHandler<EventArgs>(Self_MuteListUpdated);
            client.Groups.RequestCurrentGroups();
            UpdateDisplay();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void GroupsDialog_Disposed(object sender, EventArgs e)
        {
            client.Groups.CurrentGroups -= new EventHandler<CurrentGroupsEventArgs>(Groups_CurrentGroups);
            client.Groups.GroupCreatedReply -= new EventHandler<GroupCreatedReplyEventArgs>(Groups_GroupCreatedReply);
            client.Groups.GroupRoleDataReply += new EventHandler<GroupRolesDataReplyEventArgs>(Groups_GroupRoleDataReply);
            client.Self.MuteListUpdated -= new EventHandler<EventArgs>(Self_MuteListUpdated);
        }

        void Groups_GroupRoleDataReply(object sender, GroupRolesDataReplyEventArgs e)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Groups_GroupRoleDataReply(sender, e)));
                }
                return;
            }

            if (!(txtKeys.Tag is Group)) return;
            Group g = (Group)txtKeys.Tag;
            if (g.ID != e.GroupID) return;

            foreach (var r in e.Roles)
            {
                txtKeys.AppendText(string.Format("Role \"{0}\": {1}{2}", r.Value.Name, r.Key, Environment.NewLine));
            }
        }

        void Groups_GroupCreatedReply(object sender, GroupCreatedReplyEventArgs e)
        {
            if (e.Success)
            {
                newGrpID = e.GroupID;
                client.Groups.ActivateGroup(newGrpID);
                client.Groups.RequestCurrentGroups();
            }
            else
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    lblCreateStatus.Text = string.Format("Group creation failed: {0}", e.Message);
                }
                ));
            }
        }

        void Groups_CurrentGroups(object sender, CurrentGroupsEventArgs e)
        {
            BeginInvoke(new MethodInvoker(UpdateDisplay));
        }

        private object DisplaySyncRoot = new object();

        public void UpdateDisplay()
        {
            lock (DisplaySyncRoot)
            {

                Group none = new Group();
                none.Name = "(none)";
                none.ID = UUID.Zero;

                listBox1.Items.Clear();
                listBox1.Items.Add(none);

                foreach (Group g in instance.Groups.Values)
                {
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

                lblGroupNr.Text = string.Format("{0} groups", instance.Groups.Count);
                if (client.Network.MaxAgentGroups > 0)
                {
                    lblGrpMax.Text = string.Format("max {0} groups", client.Network.MaxAgentGroups);
                }
                else
                {
                    lblGrpMax.Text = string.Empty;
                }

                if (newGrpID != UUID.Zero)
                {
                    lblCreateStatus.Text = "Group created successfully";
                    btnCancel.PerformClick();
                    instance.MainForm.ShowGroupProfile(instance.Groups[newGrpID]);
                }
            }
            newGrpID = UUID.Zero;
        }

        private void activateBtn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            client.Groups.ActivateGroup(g.ID);
        }

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                return;
            }

            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero)
            {
                return;
            }

            if(MessageBox.Show(string.Format("Leave {0}?", g.Name), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                client.Groups.LeaveGroup(g.ID);
                listBox1.Items.Remove(g);
            }
        }

        private void imBtn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero) return;

            if (!instance.TabConsole.TabExists(g.ID.ToString()))
            {
                instance.MediaManager.PlayUISound(UISounds.IMWindow);
                instance.TabConsole.AddGroupIMTab(g.ID, g.Name);
                instance.TabConsole.Tabs[g.ID.ToString()].Highlight();
                instance.TabConsole.Tabs[g.ID.ToString()].Select();

            }
            else
            {
                RadegastTab t = instance.TabConsole.Tabs[g.ID.ToString()];
                t.Select();
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero) return;

            instance.MainForm.ShowGroupProfile(g);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            client.Groups.RequestCurrentGroups();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnInfo.PerformClick();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtNewGroupCharter.Text = txtNewGroupName.Text = lblCreateStatus.Text = string.Empty;
            pnlNewGroup.Visible = false;
        }

        private void btnNewGroup_Click(object sender, EventArgs e)
        {
            pnlNewGroup.Visible = true;
            txtNewGroupName.Focus();
        }

        private void txtNewGroupName_TextChanged(object sender, EventArgs e)
        {
            btnCreateGroup.Enabled = txtNewGroupName.Text.Length >= 4 && txtNewGroupName.Text.Length <= 35;
        }

        private void txtNewGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                if (btnCreateGroup.Enabled) btnCreateGroup.PerformClick();
            }
        }

        private void btnCreateGroup_Click(object sender, EventArgs e)
        {
            Group g = new Group();
            g.Name = txtNewGroupName.Text;
            g.Charter = txtNewGroupCharter.Text;
            g.FounderID = client.Self.AgentID;
            lblCreateStatus.Text = "Creating group...";
            client.Groups.RequestCreateGroup(g);
        }

        private void lblCreateStatus_TextChanged(object sender, EventArgs e)
        {
            instance.TabConsole.DisplayNotificationInChat(lblCreateStatus.Text, ChatBufferTextStyle.Invisible);
        }

        private void UpdateMuteButton()
        {
            btnMute.Text = "Mute";

            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero) return;

            if (null != client.Self.MuteList.Find(me => me.Type == MuteType.Group && me.ID == g.ID))
            {
                btnMute.Text = "Unmute";
            }
        }

        void Self_MuteListUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Self_MuteListUpdated(sender, e)));
                }
                return;
            }

            UpdateMuteButton();
            listBox1.Invalidate();
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero) return;

            if (btnMute.Text == "Mute")
            {
                client.Self.UpdateMuteListEntry(MuteType.Group, g.ID, g.Name);
            }
            else
            {
                client.Self.RemoveMuteListEntry(g.ID, g.Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMuteButton();
            pnlKeys.Visible = false;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            try
            {
                if (e.Index >= 0)
                {
                    var item = ((ListBox)sender).Items[e.Index];
                    string title = item.ToString();
                    using (var brush = new SolidBrush(e.ForeColor))
                    {
                        e.Graphics.DrawString(title, e.Font, brush, e.Bounds.X, e.Bounds.Y + 2);
                        if (item is Group)
                        {
                            UUID gid = ((Group)item).ID;
                            bool isMuted = client.Self.MuteList.Find(me => me.ID == gid && me.Type == MuteType.Group) != null;
                            if (isMuted)
                            {
                                var tsize = e.Graphics.MeasureString(title, e.Font);

                                using (var font = new Font(e.Font, FontStyle.Italic))
                                {
                                    e.Graphics.DrawString("(muted)", font, brush, e.Bounds.X + tsize.Width + 10, e.Bounds.Y + 2);
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            e.DrawFocusRectangle();
        }

        private void btnKeys_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Group g = (Group)listBox1.SelectedItem;
            if (g.ID == UUID.Zero) return;

            pnlKeys.Visible = !pnlKeys.Visible;
            if (pnlKeys.Visible)
            {
                txtKeys.Text = string.Empty;
                txtKeys.Tag = g;
                txtKeys.AppendText(string.Format("Group \"{0}\": {1}{2}", g.Name, g.ID, Environment.NewLine));
                client.Groups.RequestGroupRoles(g.ID);
            }
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            try
            {
                e.ItemHeight = Font.Height + 2;
                e.ItemWidth = listBox1.Width - 4;
            }
            catch { }
        }
    }
}