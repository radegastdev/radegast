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
using Radegast.Core;
using OpenMetaverse;

namespace Radegast
{
    public partial class VoiceConsole : UserControl
    {
        // These enumerated values must match the sequence of icons in TalkStates.
        private enum TalkState
        {
            Idle = 0,
            Talking,
            Muted
        };

        private RadegastInstance instance;
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private GridClient client { get { return instance.Client; } }
        private TabsConsole tabConsole;

        private VoiceGateway gateway;
        private VoiceSession session;

        public VoiceConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(VoiceConsole_Disposed);

            this.instance = instance;

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);

            gateway = new VoiceGateway(this.instance.Client);

            this.instance.MainForm.Load += new EventHandler(MainForm_Load);

            chkVoiceEnable.Checked = instance.GlobalSettings["Voice.enabled"].AsBoolean();
            if (chkVoiceEnable.Checked)
                Start();
        }

        private void Start()
        {
            // Initialize progress bar
            progressBar1.Maximum = (int)VoiceGateway.ConnectionState.SessionRunning;
            SetProgress(0);
            RegisterClientEvents();

            gateway.Start();
        }

        private void Stop()
        {
            participants.Items.Clear();
            SetProgress(VoiceGateway.ConnectionState.None);
            gateway.Stop();
        }

        private void RegisterClientEvents()
        {
            gateway.OnSessionCreate +=
                new EventHandler(gateway_OnSessionCreate);
            gateway.OnSessionRemove +=
                new EventHandler(gateway_OnSessionRemove);
            gateway.OnVoiceConnectionChange +=
                new VoiceGateway.VoiceConnectionChangeCallback(gateway_OnVoiceConnectionChange);

            KeyDown += new KeyEventHandler(VoiceConsole_KeyDown);
            KeyUp += new KeyEventHandler(VoiceConsole_KeyUp);
            MouseDown += new MouseEventHandler(OnMouseDown);
            MouseUp += new MouseEventHandler(OnMouseUp);
        }

       void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            gateway.OnSessionCreate -=
                new EventHandler(gateway_OnSessionCreate);
            gateway.OnSessionRemove -=
                new EventHandler(gateway_OnSessionRemove);
            gateway.OnVoiceConnectionChange -=
                new VoiceGateway.VoiceConnectionChangeCallback(gateway_OnVoiceConnectionChange);

            MouseDown -= new MouseEventHandler(OnMouseDown);
            MouseUp -= new MouseEventHandler(OnMouseUp);
            
        }

        private void UnregisterClientEvents()
        {
        }

        #region Connection Status
        void gateway_OnVoiceConnectionChange(VoiceGateway.ConnectionState state)
        {
            BeginInvoke(new MethodInvoker(delegate()
            {
                if (state == VoiceGateway.ConnectionState.AccountLogin)
                {
                    LoadMicDevices( gateway.CaptureDevices );
                    LoadSpkrDevices( gateway.PlaybackDevices );
                }

                SetProgress(state);
            }));
        }

        void SetProgress(VoiceGateway.ConnectionState s)
        {
            int value = (int)s;
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
        void gateway_OnSessionCreate(object sender, EventArgs e)
        {
            VoiceSession s = sender as VoiceSession;

            BeginInvoke(new MethodInvoker(delegate()
             {
                 // There could theoretically be more than one session active
                 // at a time, but the current implementation in SL seems to be limited to one.
                 session = s;
                 session.OnParticipantAdded += new EventHandler(session_OnParticipantAdded);
                 session.OnParticipantRemoved += new EventHandler(session_OnParticipantRemoved);
                 session.OnParticipantUpdate += new EventHandler(session_OnParticipantUpdate);
                 participants.Items.Clear();

                 // Default Mic off and Spkr on
                 gateway.MicMute = true;
                 gateway.SpkrMute = false;
                 gateway.SpkrLevel = 64;
                 gateway.MicLevel = 64;

                 SetProgress(VoiceGateway.ConnectionState.SessionRunning);
             }));
        }

        void gateway_OnSessionRemove(object sender, EventArgs e)
        {
            VoiceSession s = sender as VoiceSession;
            if (session != s)
                return;

            BeginInvoke(new MethodInvoker(delegate()
             {
                 participants.Items.Clear();
                 session.OnParticipantAdded -= new EventHandler(session_OnParticipantAdded);
                 session.OnParticipantRemoved -= new EventHandler(session_OnParticipantRemoved);
                 session.OnParticipantUpdate -= new EventHandler(session_OnParticipantUpdate);

                 gateway.MicMute = true;
                 micMute.Checked = true;

                 session = null;
                 SetProgress(VoiceGateway.ConnectionState.AccountLogin);
             }));
        }


        #endregion
        #region Participants
        void session_OnParticipantAdded(object sender, EventArgs e)
        {
            VoiceParticipant p = sender as VoiceParticipant;
            BeginInvoke(new MethodInvoker(delegate()
            {
                // Supply the name based on the UUID.
                p.Name = instance.getAvatarName(p.ID);

                ListViewItem item = new ListViewItem(p.Name);
                item.Tag = p;
                item.StateImageIndex = (int)TalkState.Idle;

                lock (participants)
                    participants.Items.Add(item);
            }));
        }

        void session_OnParticipantRemoved(object sender, EventArgs e)
        {
            VoiceParticipant p = sender as VoiceParticipant;
            if (p.Name == null) return;
            BeginInvoke(new MethodInvoker(delegate()
            {
                lock (participants)
                {
                    ListViewItem item = FindParticipantItem(p);
                    if (item != null)
                        participants.Items.Remove(item);
                }
            }));
        }

        void session_OnParticipantUpdate(object sender, EventArgs e)
        {
            VoiceParticipant p = sender as VoiceParticipant;
            if (p.Name == null) return;
            BeginInvoke(new MethodInvoker(delegate()
            {
                lock (participants)
                {
                    ListViewItem item = FindParticipantItem(p);
                    if (item != null)
                    {
                        if (p.IsMuted)
                            item.StateImageIndex = (int)TalkState.Muted;
                        else if (p.IsSpeaking)
                            item.StateImageIndex = (int)TalkState.Talking;
                        else
                            item.StateImageIndex = (int)TalkState.Idle;
                    }
                }
            }));
        }

        ListViewItem FindParticipantItem(VoiceParticipant p)
        {
            foreach (ListViewItem item in participants.Items)
            {
                if (item.Tag == p)
                    return item;
            }
            return null;
        }

        #endregion

        void VoiceConsole_Disposed(object sender, EventArgs e)
        {

            netcom.ClientLoginStatus -= new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            UnregisterClientEvents();
            gateway.Stop();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tabConsole = instance.TabConsole;
        }

        private void netcom_ClientLoginStatus(object sender, LoginProgressEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            BeginInvoke(new MethodInvoker(delegate()
            {
                if (chkVoiceEnable.Checked)
                    gateway.Start();
            }));

        }

        #region Talk control
        void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                micMute.Checked = false;
//                gateway.MicMute = true;
            }
        }
        void VoiceConsole_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.RControlKey)
                micMute.Checked = false;
        }

        void VoiceConsole_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.RControlKey)
                micMute.Checked = true;
        }
        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, e);
        }

        private void splitContainer1_Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(sender, e);
        }

        void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                micMute.Checked = false;
//                gateway.MicMute = false;
            }
        }
        #endregion

        #region Audio Settings
 
        void LoadMicDevices(List<string> available)
        {
            foreach (string name in available)
                micDevice.Items.Add(name);
            micDevice.SelectedItem = instance.GlobalSettings["Voice.capture.device"].AsString();
        }

        void LoadSpkrDevices(List<string> available)
        {
            foreach (string name in available)
                spkrDevice.Items.Add(name);
            spkrDevice.SelectedItem = instance.GlobalSettings["Voice.playback.device"].AsString();
        }

        private void spkrDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            gateway.PlaybackDevice = (string)spkrDevice.SelectedItem;
            instance.GlobalSettings["Voice.playback.device"] =
                OpenMetaverse.StructuredData.OSD.FromString((string)spkrDevice.SelectedItem);
        }

        private void micDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            gateway.CaptureDevice = (string)micDevice.SelectedItem;
            instance.GlobalSettings["Voice.capture.device"] =
               OpenMetaverse.StructuredData.OSD.FromString((string)micDevice.SelectedItem);
        }

        private void micLevel_ValueChanged(object sender, EventArgs e)
        {
            gateway.MicLevel = micLevel.Value;
        }

        private void spkrLevel_ValueChanged(object sender, EventArgs e)
        {
            gateway.SpkrLevel = spkrLevel.Value;
        }
        #endregion

        /// <summary>
        /// Start and stop the voice functions.
        /// </summary>
       private void chkVoiceEnable_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                instance.GlobalSettings["Voice.enabled"] =
                    OpenMetaverse.StructuredData.OSD.FromBoolean(chkVoiceEnable.Checked);

                if (chkVoiceEnable.Checked)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }));
        }

       private void micMute_CheckedChanged(object sender, EventArgs e)
       {
           gateway.MicMute = micMute.Checked;
       }

    }
}

