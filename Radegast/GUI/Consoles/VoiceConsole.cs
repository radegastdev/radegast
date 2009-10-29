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
// $Id: ChatConsole.cs 371 2009-10-26 10:26:04Z latifer $
//
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public partial class VoiceConsole : UserControl
    {
        // These enumerated values must match the sequence of icons in TalkStates.
        private enum State
        {
            Idle = 0,
            Talking,
            Muted
        };

        private RadegastInstance instance;
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private GridClient client { get { return instance.Client; } }
        private TabsConsole tabConsole;
        private Avatar currentAvatar;
        private Dictionary<uint, Avatar> avatars = new Dictionary<uint, Avatar>();
        private Dictionary<uint, bool> bots = new Dictionary<uint,bool>();
        private Radegast.Core.VoiceGateway gateway;
        private Radegast.Core.VoiceSession session;

        public VoiceConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(ChatConsole_Disposed);

            this.instance = instance;
            this.instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);

            gateway = new Radegast.Core.VoiceGateway(this.instance.Client);
            SetProgress(0);
            RegisterClientEvents(client);

            this.instance.MainForm.Load += new EventHandler(MainForm_Load);

            SorterClass sc = new SorterClass();
            participants.ListViewItemSorter = sc;


            chkVoiceEnable.Checked = instance.GlobalSettings["Voice.enabled"].AsBoolean();
//            if (chkVoiceEnable.Checked)
//                gateway.Start();
        }

        private void RegisterClientEvents(GridClient client)
        {
            gateway.OnSessionCreate += new EventHandler(gateway_OnSessionCreate);
        }

        void gateway_OnSessionCreate(object sender, EventArgs e)
        {
            session = sender as Radegast.Core.VoiceSession;
            participants.Items.Clear();

            // Default Mic off and Spkr on
            gateway.MicMute = true;
            gateway.SpkrMute = false;
            gateway.SpkrLevel = 64;
            gateway.MicLevel = 64;           
        }

        private void UnregisterClientEvents(GridClient client)
        {
        }

#region Connection Status
        void gateway_OnDaemonRunning()
        {
            SetProgress( 1 );
        }

        void SetProgress(int value)
        {
            if (value == progressBar1.Maximum)
                progressBar1.ForeColor = Color.Green;
            else if (value > (progressBar1.Maximum / 2))
                progressBar1.ForeColor = Color.Yellow;
            else
                progressBar1.ForeColor = Color.Red;

            progressBar1.Value = value;
        }
#endregion
        #region Sessions
        #endregion

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(client);
        }

        void ChatConsole_Disposed(object sender, EventArgs e)
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
            UnregisterClientEvents(client);
        }

       private void MainForm_Load(object sender, EventArgs e)
        {
            tabConsole = instance.TabConsole;
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
             participants.Items.Clear();
        }

        #region Talk control
        void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                micMute_Set(true);
                gateway.MicMute = true;
            }
        }

        void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                micMute_Set(false);
                gateway.MicMute = false;
            }
        }
        #endregion

        #region Audio Settings
        private void micMute_Set(bool on)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                micMute.Checked = on;
            }));
        }

        private void spkrDevice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void micDevice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void micLevel_ValueChanged(object sender, EventArgs e)
        {

        }

        private void spkrLevel_ValueChanged(object sender, EventArgs e)
        {

        }
#endregion

        private void chkVoiceEnable_Click(object sender, EventArgs e)
        {
            chkVoiceEnable.Checked = !chkVoiceEnable.Checked;
            instance.GlobalSettings["Voice.enabled"] =
                OpenMetaverse.StructuredData.OSD.FromBoolean(chkVoiceEnable.Checked);

            if (chkVoiceEnable.Checked)
            {
                gateway.Start();
            }
            else
            {
                gateway.Stop();
            }
        }
    }
}

