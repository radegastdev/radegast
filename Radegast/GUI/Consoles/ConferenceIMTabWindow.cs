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
using System.Threading;
using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public partial class ConferenceIMTabWindow : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private UUID session;
        private IMTextManager textManager;
        private GridClient client;
        private List<UUID> participants = new List<UUID>();
        ManualResetEvent WaitForSessionStart = new ManualResetEvent(false);

        public ConferenceIMTabWindow(RadegastInstance instance, UUID session, string sessionName)
        {
            InitializeComponent();
            Disposed += new EventHandler(IMTabWindow_Disposed);

            this.instance = instance;
            this.client = instance.Client;
            netcom = this.instance.Netcom;

            this.session = session;

            textManager = new IMTextManager(this.instance, new RichTextBoxPrinter(rtbIMText), this.session, sessionName);
            
            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);

            this.client.Self.ChatterBoxAcceptInvite(session);
        }

        private void IMTabWindow_Disposed(object sender, EventArgs e)
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected -= new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            CleanUp();
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            RefreshControls();
        }

        private void netcom_ClientDisconnected(object sender, ClientDisconnectEventArgs e)
        {
            RefreshControls();
        }

        public void CleanUp()
        {
            textManager.CleanUp();
            textManager = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (cbxInput.Text.Length == 0) return;

            SendMessage(cbxInput.Text);
            this.ClearIMInput();
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

            if (cbxInput.Text.Length > 0)
            {
                btnSend.Enabled = true;
            }
            else
            {
                btnSend.Enabled = false;
            }
        }

        private void cbxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (cbxInput.Text.Length == 0) return;

            string message = cbxInput.Text;

            SendMessage(message);

            this.ClearIMInput();
        }

        private void SendMessage(string msg)
        {
            string message = msg.Replace(ChatInputBox.NewlineMarker, "\n");

            if (message.Length > 1023)
            {
                message = message.Remove(1023);
            }
            if (message.Length > 0)
            {
                client.Self.InstantMessageGroup(session, message);
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

        private void cbxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
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
    }
}
