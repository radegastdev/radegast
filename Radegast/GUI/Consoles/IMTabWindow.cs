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
using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public partial class IMTabWindow : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private UUID target;
        private UUID session;
        private string toName;
        private IMTextManager textManager;
        private bool typing = false;
        private List<string> chatHistory = new List<string>();
        private int chatPointer;

        public IMTabWindow(RadegastInstance instance, UUID target, UUID session, string toName)
        {
            InitializeComponent();
            Disposed += new EventHandler(IMTabWindow_Disposed);
            
            this.instance = instance;

            this.target = target;
            this.session = session;
            this.toName = toName;

            textManager = new IMTextManager(this.instance, new RichTextBoxPrinter(rtbIMText), IMTextManagerType.Agent, this.session, toName);

            AddNetcomEvents();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        private void IMTabWindow_Disposed(object sender, EventArgs e)
        {
            CleanUp();
        }

        private void AddNetcomEvents()
        {
            netcom.ClientLoginStatus += new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(netcom_ClientDisconnected);
            instance.GlobalSettings.OnSettingChanged += new Settings.SettingChangedCallback(GlobalSettings_OnSettingChanged);
        }

        private void RemoveNetcomEvents()
        {
            netcom.ClientLoginStatus -= new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(netcom_ClientDisconnected);
            instance.GlobalSettings.OnSettingChanged -= new Settings.SettingChangedCallback(GlobalSettings_OnSettingChanged);
        }

        void GlobalSettings_OnSettingChanged(object sender, SettingsEventArgs e)
        {
        }

        private void netcom_ClientLoginStatus(object sender, LoginProgressEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            RefreshControls();
        }

        private void netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            RefreshControls();
        }

        public void CleanUp()
        {
            instance.TabConsole.RemoveTab(SessionId.ToString());
            textManager.CleanUp();
            textManager = null;
            RemoveNetcomEvents();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            ProcessInput();
        }

        private void cbxInput_TextChanged(object sender, EventArgs e)
        {
            RefreshControls();
        }

        private void RefreshControls()
        {
            if (!netcom.IsLoggedIn)
            {
                cbxInput.Enabled = false;
                btnSend.Enabled = false;
                return;
            }

            cbxInput.Enabled = true;

            if (cbxInput.Text.Length > 0)
            {
                btnSend.Enabled = true;

                if (!typing)
                {
                    netcom.SendIMStartTyping(target, session);
                    typing = true;
                }
            }
            else
            {
                btnSend.Enabled = false;
                netcom.SendIMStopTyping(target, session);
                typing = false;
            }
        }

        private void ProcessInput()
        {
            if (cbxInput.Text.Length == 0) return;

            chatHistory.Add(cbxInput.Text);
            chatPointer = chatHistory.Count;

            string msg = cbxInput.Text.Replace(ChatInputBox.NewlineMarker, "\n");

            if (instance.GlobalSettings["mu_emotes"].AsBoolean() && msg.StartsWith(":"))
            {
                msg = "/me " + msg.Substring(1);
            }

            if (instance.RLV.RestictionActive("sendim", target.ToString()))
                msg = "*** IM blocked by sender's viewer";

            netcom.SendInstantMessage(msg, target, session);
            this.ClearIMInput();
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
            ProcessInput();
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

        private void tbtnProfile_Click(object sender, EventArgs e)
        {
            instance.MainForm.ShowAgentProfile(toName, target);
        }

        private void btnOfferTeleport_Click(object sender, EventArgs e)
        {
            instance.MainForm.AddNotification(new ntfSendLureOffer(instance, target));
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            (new frmPay(instance, target, toName, false)).ShowDialog();
        }

        public UUID TargetId
        {
            get { return target; }
            set { target = value; }
        }

        public string TargetName
        {
            get { return toName; }
            set { toName = value; }
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

        private void cbxInput_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) cbxInput.Focus();
        }

        private void cbxInput_SizeChanged(object sender, EventArgs e)
        {
            pnlChatInput.Height = cbxInput.Height + 9;
        }

        private void cbAlwaysDing_CheckedChanged(object sender, EventArgs e)
        {
            textManager.DingOnAllIncoming = ((CheckBox)sender).Checked;
        }
    }
}
