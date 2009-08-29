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
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using OpenMetaverse;

namespace Radegast
{
    public partial class FriendsConsole : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private FriendInfo selectedFriend;

        private bool settingFriend = false;

        public FriendsConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(FriendsConsole_Disposed);

            this.instance = instance;

            // Callbacks
            client.Friends.OnFriendOffline += new FriendsManager.FriendOfflineEvent(Friends_OnFriendOffline);
            client.Friends.OnFriendOnline += new FriendsManager.FriendOnlineEvent(Friends_OnFriendOnline);
            client.Friends.OnFriendshipTerminated += new FriendsManager.FriendshipTerminatedEvent(Friends_OnFriendshipTerminated);

        }

        void FriendsConsole_Disposed(object sender, EventArgs e)
        {
            client.Friends.OnFriendOffline -= new FriendsManager.FriendOfflineEvent(Friends_OnFriendOffline);
            client.Friends.OnFriendOnline -= new FriendsManager.FriendOnlineEvent(Friends_OnFriendOnline);
            client.Friends.OnFriendshipTerminated -= new FriendsManager.FriendshipTerminatedEvent(Friends_OnFriendshipTerminated);
        }

        private void InitializeFriendsList()
        {
            List<FriendInfo> friends = client.Friends.FriendList.FindAll(delegate(FriendInfo f) { return true; });

            lbxFriends.BeginUpdate();
            lbxFriends.Items.Clear();
            foreach (FriendInfo friend in friends)
                lbxFriends.Items.Add(new FriendsListItem(friend));

            lbxFriends.EndUpdate();
        }

        private void RefreshFriendsList()
        {
            InitializeFriendsList();
            SetFriend(selectedFriend);
        }

        private void Friends_OnFriendOffline(FriendInfo friend)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Friends_OnFriendOffline(friend)));
                return;
            }

            instance.MainForm.TabConsole.DisplayNotificationInChat(friend.Name + " is offline");
            RefreshFriendsList();
        }

        private void Friends_OnFriendOnline(FriendInfo friend)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Friends_OnFriendOnline(friend)));
                return;
            }

            instance.MainForm.TabConsole.DisplayNotificationInChat(friend.Name + " is online");
            RefreshFriendsList();
        }

        void Friends_OnFriendshipTerminated(UUID agentID, string agentName)
        {
            if (agentName == string.Empty)
            {
                using (ManualResetEvent done = new ManualResetEvent(false))
                {

                    AvatarManager.AvatarNamesCallback callback = delegate(Dictionary<UUID, string> names)
                    {
                        if (names.ContainsKey(agentID))
                        {
                            agentName = names[agentID];
                            done.Set();
                        }
                    };

                    client.Avatars.OnAvatarNames += callback;
                    agentName = instance.getAvatarName(agentID);
                    if (agentName == RadegastInstance.INCOMPLETE_NAME)
                    {
                        done.WaitOne(3000, false);
                    }
                    client.Avatars.OnAvatarNames -= callback;
                }
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Friends_OnFriendshipTerminated(agentID, agentName)));
                return;
            }

            instance.MainForm.TabConsole.DisplayNotificationInChat(agentName + " is no longer on your friend list");
            RefreshFriendsList();
        }

        private void SetFriend(FriendInfo friend)
        {
            if (friend == null) return;
            selectedFriend = friend;

            lblFriendName.Text = friend.Name + (friend.IsOnline ? " (online)" : " (offline)");

            btnIM.Enabled = btnProfile.Enabled = btnOfferTeleport.Enabled = btnPay.Enabled = btnRemove.Enabled = true;
            chkSeeMeOnline.Enabled = chkSeeMeOnMap.Enabled = chkModifyMyObjects.Enabled = true;
            chkSeeMeOnMap.Enabled = friend.CanSeeMeOnline;

            settingFriend = true;
            chkSeeMeOnline.Checked = friend.CanSeeMeOnline;
            chkSeeMeOnMap.Checked = friend.CanSeeMeOnMap;
            chkModifyMyObjects.Checked = friend.CanModifyMyObjects;
            settingFriend = false;
        }

        private void lbxFriends_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index < 0) return;

            FriendsListItem itemToDraw = (FriendsListItem)lbxFriends.Items[e.Index];
            Brush textBrush = null;
            Font textFont = null;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                textBrush = new SolidBrush(Color.FromKnownColor(KnownColor.HighlightText));
                textFont = new Font(e.Font, FontStyle.Bold);
            }
            else
            {
                textBrush = new SolidBrush(Color.FromKnownColor(KnownColor.ControlText));
                textFont = new Font(e.Font, FontStyle.Regular);
            }

            SizeF stringSize = e.Graphics.MeasureString(itemToDraw.Friend.Name, textFont);
            float stringX = e.Bounds.Left + 4 + Properties.Resources.GreenOrb_16.Width;
            float stringY = e.Bounds.Top + 2 + ((Properties.Resources.GreenOrb_16.Height / 2) - (stringSize.Height / 2));

            if (itemToDraw.Friend.IsOnline)
                e.Graphics.DrawImage(Properties.Resources.GreenOrb_16, e.Bounds.Left + 2, e.Bounds.Top + 2);
            else
                e.Graphics.DrawImage(Properties.Resources.GreenOrbFaded_16, e.Bounds.Left + 2, e.Bounds.Top + 2);

            e.Graphics.DrawString(itemToDraw.Friend.Name, textFont, textBrush, stringX, stringY);

            e.DrawFocusRectangle();

            textFont.Dispose();
            textBrush.Dispose();
            textFont = null;
            textBrush = null;
        }

        private void lbxFriends_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxFriends.SelectedItem == null) return;

            FriendsListItem item = (FriendsListItem)lbxFriends.SelectedItem;
            SetFriend(item.Friend);
        }

        private void btnIM_Click(object sender, EventArgs e)
        {
            if (instance.TabConsole.TabExists((client.Self.AgentID ^ selectedFriend.UUID).ToString()))
            {
                instance.TabConsole.SelectTab((client.Self.AgentID ^ selectedFriend.UUID).ToString());
                return;
            }

            instance.TabConsole.AddIMTab(selectedFriend.UUID, client.Self.AgentID ^ selectedFriend.UUID, selectedFriend.Name);
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            (new frmProfile(instance, selectedFriend.Name, selectedFriend.UUID)).Show();
        }

        private void chkSeeMeOnline_CheckedChanged(object sender, EventArgs e)
        {
            if (settingFriend) return;

            selectedFriend.CanSeeMeOnline = chkSeeMeOnline.Checked;
            client.Friends.GrantRights(selectedFriend.UUID, selectedFriend.TheirFriendRights);
        }

        private void chkSeeMeOnMap_CheckedChanged(object sender, EventArgs e)
        {
            if (settingFriend) return;

            selectedFriend.CanSeeMeOnMap = chkSeeMeOnMap.Checked;
            client.Friends.GrantRights(selectedFriend.UUID, selectedFriend.TheirFriendRights);
        }

        private void chkModifyMyObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (settingFriend) return;

            selectedFriend.CanModifyMyObjects = chkModifyMyObjects.Checked;
            client.Friends.GrantRights(selectedFriend.UUID, selectedFriend.TheirFriendRights);
        }

        private void btnOfferTeleport_Click(object sender, EventArgs e)
        {
            client.Self.SendTeleportLure(selectedFriend.UUID, "Join me in " + client.Network.CurrentSim.Name + "!");
        }

        private void timInitDelay_Tick(object sender, EventArgs e)
        {
            timInitDelay.Enabled = false;
            lblFriendName.Text = "Please select a friend.";

            InitializeFriendsList();
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            (new frmPay(instance, selectedFriend.UUID, selectedFriend.Name, false)).ShowDialog();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            client.Friends.TerminateFriendship(selectedFriend.UUID);
            if (lbxFriends.Items.Count > 0)
            {
                lbxFriends.SelectedIndex = 0;
            }
        }
    }
}
