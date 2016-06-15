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
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class ntfSendLureRequest : Notification
    {
        private RadegastInstance instance;
        private UUID agentID;
        private string agentName;

        public ntfSendLureRequest(RadegastInstance instance, UUID agentID)
            : base(NotificationType.SendLureRequest)
        {
            InitializeComponent();
            this.instance = instance;
            this.agentID = agentID;

            txtHead.BackColor = instance.MainForm.NotificationBackground;

            agentName = instance.Names.Get(agentID, true);
            txtHead.Text = String.Format("Request a teleport to {0}'s location with the following message:", agentName);
            txtMessage.BackColor = instance.MainForm.NotificationBackground;
            btnRequest.Focus();

            // Fire off event
            NotificationEventArgs args = new NotificationEventArgs(instance);
            args.Text = txtHead.Text + Environment.NewLine + txtMessage.Text;
            args.Buttons.Add(btnRequest);
            args.Buttons.Add(btnCancel);
            FireNotificationCallback(args);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        private void btnTeleport_Click(object sender, EventArgs e)
        {
            if (!instance.Client.Network.Connected) return;

            instance.Client.Self.InstantMessage(instance.Client.Self.Name, agentID, txtMessage.Text,
                instance.Client.Self.AgentID ^ agentID, InstantMessageDialog.RequestLure, InstantMessageOnline.Offline,
                instance.Client.Self.SimPosition, instance.Client.Network.CurrentSim.ID, null);
            instance.MainForm.RemoveNotification(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }
    }
}
