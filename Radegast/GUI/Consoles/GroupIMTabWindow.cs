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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public partial class GroupIMTabWindow : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private UUID session;
        private IMTextManager textManager;
        private object AvatarListSyncRoot = new object();
        private List<string> chatHistory = new List<string>();
        private int chatPointer;

        ManualResetEvent WaitForSessionStart = new ManualResetEvent(false);

        public GroupIMTabWindow(RadegastInstance instance, UUID session, string sessionName)
        {
            InitializeComponent();
            Disposed += new EventHandler(IMTabWindow_Disposed);

            this.instance = instance;
            this.session = session;

            textManager = new IMTextManager(this.instance, new RichTextBoxPrinter(rtbIMText), IMTextManagerType.Group, this.session, sessionName);

            btnShow.Text = "Show";
            chatSplit.Panel2Collapsed = true;

            // Callbacks
            RegisterClientEvents(client);
            if (!client.Self.GroupChatSessions.ContainsKey(session))
            {
                client.Self.RequestJoinGroupChat(session);
            }
            Load += new EventHandler(GroupIMTabWindow_Load);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        private void RegisterClientEvents(GridClient client)
        {
            client.Self.GroupChatJoined += new EventHandler<GroupChatJoinedEventArgs>(Self_GroupChatJoined);
            client.Self.ChatSessionMemberAdded += new EventHandler<ChatSessionMemberAddedEventArgs>(Self_ChatSessionMemberAdded);
            client.Self.ChatSessionMemberLeft += new EventHandler<ChatSessionMemberLeftEventArgs>(Self_ChatSessionMemberLeft);
            instance.Names.NameUpdated += new EventHandler<UUIDNameReplyEventArgs>(Names_NameUpdated);
            instance.Netcom.ClientConnected += new EventHandler<EventArgs>(Netcom_Connected);
            instance.Netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(Netcom_Disconnected);
            instance.GlobalSettings.OnSettingChanged += new Settings.SettingChangedCallback(GlobalSettings_OnSettingChanged);
            instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
        }

        private void UnregisterClientEvents(GridClient client)
        {
            client.Self.GroupChatJoined -= new EventHandler<GroupChatJoinedEventArgs>(Self_GroupChatJoined);
            client.Self.ChatSessionMemberAdded -= new EventHandler<ChatSessionMemberAddedEventArgs>(Self_ChatSessionMemberAdded);
            client.Self.ChatSessionMemberLeft -= new EventHandler<ChatSessionMemberLeftEventArgs>(Self_ChatSessionMemberLeft);
            instance.Names.NameUpdated -= new EventHandler<UUIDNameReplyEventArgs>(Names_NameUpdated);
            instance.Netcom.ClientConnected -= new EventHandler<EventArgs>(Netcom_Connected);
            instance.Netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(Netcom_Disconnected);
            instance.GlobalSettings.OnSettingChanged -= new Settings.SettingChangedCallback(GlobalSettings_OnSettingChanged);
            instance.ClientChanged -= new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(client);
        }

        void GroupIMTabWindow_Load(object sender, EventArgs e)
        {
            UpdateParticipantList();
        }

        private void IMTabWindow_Disposed(object sender, EventArgs e)
        {
            if (instance.Netcom.IsLoggedIn)
            {
                client.Self.RequestLeaveGroupChat(session);
            }

            UnregisterClientEvents(client);
            CleanUp();
        }

        void GlobalSettings_OnSettingChanged(object sender, SettingsEventArgs e)
        {
            if (e.Key == "display_name_mode")
                UpdateParticipantList();
        }

        void Netcom_Disconnected(object sender, DisconnectedEventArgs e)
        {
            RefreshControls();
        }

        void Netcom_Connected(object sender, EventArgs e)
        {
            client.Self.RequestJoinGroupChat(session);
            RefreshControls();
        }

        void Names_NameUpdated(object sender, UUIDNameReplyEventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => Names_NameUpdated(sender, e)));
                return;
            }

            lock (AvatarListSyncRoot)
            {

                Participants.BeginUpdate();

                foreach (KeyValuePair<UUID, string> kvp in e.Names)
                {
                    if (Participants.Items.ContainsKey(kvp.Key.ToString()))
                        Participants.Items[kvp.Key.ToString()].Text = kvp.Value;
                }

                Participants.Sort();
                Participants.EndUpdate();
            }
        }

        void Self_ChatSessionMemberLeft(object sender, ChatSessionMemberLeftEventArgs e)
        {
            if (e.SessionID == session)
                UpdateParticipantList();
        }

        void Self_ChatSessionMemberAdded(object sender, ChatSessionMemberAddedEventArgs e)
        {
            if (e.SessionID == session)
                UpdateParticipantList();
        }

        void UpdateParticipantList()
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(UpdateParticipantList));
                return;
            }

            try
            {
                lock (AvatarListSyncRoot)
                {
                    Participants.BeginUpdate();
                    Participants.Items.Clear();

                    List<ChatSessionMember> participants;

                    if (client.Self.GroupChatSessions.TryGetValue(session, out participants))
                    {
                        ChatSessionMember[] members = participants.ToArray();
                        for (int i = 0; i < members.Length; i++)
                        {
                            ChatSessionMember participant = members[i];
                            ListViewItem item = new ListViewItem();
                            item.Name = participant.AvatarKey.ToString();
                            item.Text = instance.Names.Get(participant.AvatarKey);
                            item.Tag = participant.AvatarKey;

                            if (participant.IsModerator)
                                item.Font = new Font(item.Font, FontStyle.Bold);
                            Participants.Items.Add(item);
                        }
                    }

                    Participants.Sort();
                    Participants.EndUpdate();
                }
            }
            catch (Exception)
            {
                Participants.EndUpdate();
            }
        }

        void Self_GroupChatJoined(object sender, GroupChatJoinedEventArgs e)
        {
            if (e.SessionID != session && e.TmpSessionID != session)
            {
                return;
            }

            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    Invoke(new MethodInvoker(() => Self_GroupChatJoined(sender, e)));
                return;
            }

            if (e.Success)
            {
                textManager.TextPrinter.PrintTextLine("Join Group Chat Success!", Color.Green);
                WaitForSessionStart.Set();
            }
            else
            {
                textManager.TextPrinter.PrintTextLine("Join Group Chat failed.", Color.Red);
            }
        }

        public void CleanUp()
        {
            textManager.CleanUp();
            textManager = null;
            instance = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMsg();
            this.ClearIMInput();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (chatSplit.Panel2Collapsed)
            {
                chatSplit.Panel2Collapsed = false;
                btnShow.Text = "Hide";
            }
            else
            {
                chatSplit.Panel2Collapsed = true;
                btnShow.Text = "Show";
            }
        }

        private void cbxInput_TextChanged(object sender, EventArgs e)
        {
            RefreshControls();
        }

        private void RefreshControls()
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(RefreshControls));
                return;
            }

            if (!netcom.IsLoggedIn)
            {
                cbxInput.Enabled = false;
                btnSend.Enabled = false;
                btnShow.Enabled = false;
                btnShow.Text = "Show";
                chatSplit.Panel2Collapsed = true;
                return;
            }

            cbxInput.Enabled = true;
            btnShow.Enabled = true;

            if (cbxInput.Text.Length > 0)
            {
                btnSend.Enabled = true;
            }
            else
            {
                btnSend.Enabled = false;
            }
        }

        void ChatHistoryPrev()
        {
            if (chatPointer == 0) return;
            chatPointer--;
            if (chatHistory.Count > chatPointer)
            {
                cbxInput.Text = chatHistory[chatPointer];
                cbxInput.SelectionStart = cbxInput.Text.Length;
                cbxInput.SelectionLength = 0;
            }
        }

        void ChatHistoryNext()
        {
            if (chatPointer == chatHistory.Count) return;
            chatPointer++;
            if (chatPointer == chatHistory.Count)
            {
                cbxInput.Text = string.Empty;
                return;
            }
            cbxInput.Text = chatHistory[chatPointer];
            cbxInput.SelectionStart = cbxInput.Text.Length;
            cbxInput.SelectionLength = 0;
        }


        private void cbxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                ChatHistoryPrev();
                return;
            }

            if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                ChatHistoryNext();
                return;
            }

            if (e.KeyCode != Keys.Enter) return;
            e.Handled = e.SuppressKeyPress = true;

            SendMsg();
        }

        private void SendMsg()
        {
            if (cbxInput.Text.Length == 0) return;

            chatHistory.Add(cbxInput.Text);
            chatPointer = chatHistory.Count;

            string message = cbxInput.Text.Replace(ChatInputBox.NewlineMarker, "\n");

            if (instance.GlobalSettings["mu_emotes"].AsBoolean() && message.StartsWith(":"))
            {
                message = "/me " + message.Substring(1);
            }

            this.ClearIMInput();

            if (instance.RLV.RestictionActive("sendim")) return;

            if (message.Length > 1023) message = message.Remove(1023);

            if (!client.Self.GroupChatSessions.ContainsKey(session))
            {
                WaitForSessionStart.Reset();
                client.Self.RequestJoinGroupChat(session);
            }
            else
            {
                WaitForSessionStart.Set();
            }

            if (WaitForSessionStart.WaitOne(10000, false))
            {
                client.Self.InstantMessageGroup(session, message);
            }
            else
            {
                textManager.TextPrinter.PrintTextLine("Cannot send group IM.", Color.Red);
            }
        }

        private void ClearIMInput()
        {
            cbxInput.Text = string.Empty;
        }

        public void SelectIMInput()
        {
            cbxInput.Select();
        }

        private void rtbIMText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            instance.MainForm.ProcessLink(e.LinkText);
        }

        public UUID SessionId
        {
            get { return session; }
            set { session = value; }
        }

        public IMTextManager TextManager
        {
            get { return textManager; }
            set { textManager = value; }
        }

        private void Participants_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = Participants.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                try
                {
                    instance.MainForm.ShowAgentProfile(item.Text, new UUID(item.Name));
                }
                catch (Exception) { }
            }
        }

        private void cbxInput_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) cbxInput.Focus();
        }

        private void cbxInput_SizeChanged(object sender, EventArgs e)
        {
            pnlChatInput.Height = cbxInput.Height + 7;
        }

        private void avatarContext_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Participants.SelectedItems.Count != 1)
            {
                e.Cancel = true;
                return;
            }

            UUID av = (UUID)Participants.SelectedItems[0].Tag;

            if (av == client.Self.AgentID)
            {
                ctxMute.Enabled = ctxPay.Enabled = ctxStartIM.Enabled = false;
            }
            else
            {
                ctxMute.Enabled = ctxPay.Enabled = ctxStartIM.Enabled = true;

                bool isMuted = client.Self.MuteList.Find(me => me.Type == MuteType.Resident && me.ID == av) != null;
                ctxMute.Text = isMuted ? "Unmute" : "Mute";
            }
        }

        private void ctxProfile_Click(object sender, EventArgs e)
        {
            if (Participants.SelectedItems.Count != 1) return;
            UUID av = (UUID)Participants.SelectedItems[0].Tag;
            string name = instance.Names.Get(av);

            instance.MainForm.ShowAgentProfile(name, av);
        }

        private void ctxPay_Click(object sender, EventArgs e)
        {
            if (Participants.SelectedItems.Count != 1) return;
            UUID av = (UUID)Participants.SelectedItems[0].Tag;
            string name = instance.Names.Get(av);

            new frmPay(instance, av, name, false).ShowDialog();
        }

        private void ctxStartIM_Click(object sender, EventArgs e)
        {
            if (Participants.SelectedItems.Count != 1) return;
            UUID av = (UUID)Participants.SelectedItems[0].Tag;
            string name = instance.Names.Get(av);

            instance.TabConsole.ShowIMTab(av, name, true);
        }

        private void ctxOfferTP_Click(object sender, EventArgs e)
        {
            if (Participants.SelectedItems.Count != 1) return;
            UUID av = (UUID)Participants.SelectedItems[0].Tag;
            instance.MainForm.AddNotification(new ntfSendLureOffer(instance, av));
        }

        private void ctxReqestLure_Click(object sender, EventArgs e)
        {
            if (Participants.SelectedItems.Count != 1) return;
            UUID av = (UUID)Participants.SelectedItems[0].Tag;
            instance.MainForm.AddNotification(new ntfSendLureRequest(instance, av));
        }

        private void ctxEject_Click(object sender, EventArgs e)
        {
            if (Participants.SelectedItems.Count != 1) return;
            UUID av = (UUID)Participants.SelectedItems[0].Tag;
            instance.Client.Groups.EjectUser(session, av);
        }

        private void ctxBan_Click(object sender, EventArgs e)
        {
            if (Participants.SelectedItems.Count != 1) return;
            UUID av = (UUID)Participants.SelectedItems[0].Tag;
        }

        private void ctxMute_Click(object sender, EventArgs e)
        {
            if (Participants.SelectedItems.Count != 1) return;
            UUID av = (UUID)Participants.SelectedItems[0].Tag;
            if (av == client.Self.AgentID) return;

            if (ctxMute.Text == "Mute")
            {
                client.Self.UpdateMuteListEntry(MuteType.Resident, av, instance.Names.GetLegacyName(av));
            }
            else
            {
                client.Self.RemoveMuteListEntry(av, instance.Names.GetLegacyName(av));
            }
        }
    }
}
