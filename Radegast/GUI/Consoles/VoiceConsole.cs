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

        public VoiceConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(ChatConsole_Disposed);

            this.instance = instance;
            this.instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            RegisterClientEvents(client);

            this.instance.MainForm.Load += new EventHandler(MainForm_Load);

            SorterClass sc = new SorterClass();
            participants.ListViewItemSorter = sc;
        }

        private void RegisterClientEvents(GridClient client)
        {
        }

        private void UnregisterClientEvents(GridClient client)
        {
        }

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

            client.Avatars.RequestAvatarProperties(client.Self.AgentID);

         }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
             participants.Items.Clear();
        }

        private void lvwObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (participants.SelectedItems.Count == 0)
            {
                currentAvatar = null;
            }
            else
            {
                currentAvatar = client.Network.CurrentSim.ObjectsAvatars.Find(delegate(Avatar a)
                {
                    return a.ID == (UUID)participants.SelectedItems[0].Tag;
                });

            }
        }

        private void avatarContext_Opening(object sender, CancelEventArgs e)
        {
            if (participants.SelectedItems.Count == 0 && !instance.State.IsPointing)
            {
                e.Cancel = true;
                return;
            }
            else if (instance.State.IsPointing)
            {
                ctxPoint.Enabled = true;
                ctxPoint.Text = "Unpoint";
            }
            instance.ContextActionManager.AddContributions(
                avatarContext, typeof(Avatar), participants.SelectedItems[0]);
        }

        private void ctxSource_Click(object sender, EventArgs e)
        {
            if (participants.SelectedItems.Count != 1) return;

            instance.State.EffectSource = (UUID)participants.SelectedItems[0].Tag;
        }

        private void ctxPay_Click(object sender, EventArgs e)
        {
            if (participants.SelectedItems.Count != 1) return;
            (new frmPay(instance, (UUID)participants.SelectedItems[0].Tag, instance.getAvatarName((UUID)participants.SelectedItems[0].Tag), false)).ShowDialog();
        }

        private void rtbChat_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button!=MouseButtons.Right) return;
            RadegastContextMenuStrip cms = new RadegastContextMenuStrip();
            instance.ContextActionManager.AddContributions(cms,instance.Client);
            cms.Show((Control)sender,new Point(e.X,e.Y));
        }

    }
}

