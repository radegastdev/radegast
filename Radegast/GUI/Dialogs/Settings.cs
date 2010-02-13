
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public partial class frmSettings : RadegastForm
    {
        private Settings s;

        public frmSettings(RadegastInstance instance)
        {
            InitializeComponent();
            s = instance.GlobalSettings;

            cbChatTimestamps.Checked = s["chat_timestamps"].AsBoolean();

            if (s["im_timestamps"].Type == OSDType.Unknown)
                s["im_timestamps"] = OSD.FromBoolean(true);

            cbIMTimeStamps.Checked = s["im_timestamps"].AsBoolean();

            cbChatTimestamps.CheckedChanged += new EventHandler(cbChatTimestamps_CheckedChanged);
            cbIMTimeStamps.CheckedChanged += new EventHandler(cbIMTimeStamps_CheckedChanged);

            cbTrasactDialog.Checked = s["transaction_notification_dialog"].AsBoolean();
            cbTrasactChat.Checked = s["transaction_notification_chat"].AsBoolean();

            cbFriendsNotifications.Checked = s["show_friends_online_notifications"].AsBoolean();
            cbFriendsNotifications.CheckedChanged += new EventHandler(cbFriendsNotifications_CheckedChanged);

            cbAutoReconnect.Checked = s["auto_reconnect"].AsBoolean();
            cbAutoReconnect.CheckedChanged += new EventHandler(cbAutoReconnect_CheckedChanged);

            cbHideLoginGraphics.Checked = s["hide_login_graphics"].AsBoolean();
            cbHideLoginGraphics.CheckedChanged += new EventHandler(cbHideLoginGraphics_CheckedChanged);

            if (s["rlv_enabled"].Type == OSDType.Unknown)
            {
                s["rlv_enabled"] = new OSDBoolean(false);
            }

            cbRLV.Checked = s["rlv_enabled"].AsBoolean();
            cbRLV.CheckedChanged += (object sender, EventArgs e) =>
            {
                s["rlv_enabled"] = new OSDBoolean(cbRLV.Checked);
            };

            if (s["mu_emotes"].Type == OSDType.Unknown)
            {
                s["mu_emotes"] = new OSDBoolean(false);
            }

            cbMUEmotes.Checked = s["mu_emotes"].AsBoolean();
            cbMUEmotes.CheckedChanged += (object sender, EventArgs e) =>
            {
                s["mu_emotes"] = new OSDBoolean(cbMUEmotes.Checked);
            };


        }

        void cbHideLoginGraphics_CheckedChanged(object sender, EventArgs e)
        {
            s["hide_login_graphics"] = OSD.FromBoolean(cbHideLoginGraphics.Checked);
        }

        void cbAutoReconnect_CheckedChanged(object sender, EventArgs e)
        {
            s["auto_reconnect"] = OSD.FromBoolean(cbAutoReconnect.Checked);
        }

        void cbFriendsNotifications_CheckedChanged(object sender, EventArgs e)
        {
            s["show_friends_online_notifications"] = OSD.FromBoolean(cbFriendsNotifications.Checked);
        }

        void cbChatTimestamps_CheckedChanged(object sender, EventArgs e)
        {
            s["chat_timestamps"] = OSD.FromBoolean(cbChatTimestamps.Checked);
        }

        void cbIMTimeStamps_CheckedChanged(object sender, EventArgs e)
        {
            s["im_timestamps"] = OSD.FromBoolean(cbIMTimeStamps.Checked);
        }

        private void cbTrasactDialog_CheckedChanged(object sender, EventArgs e)
        {
            s["transaction_notification_dialog"] = OSD.FromBoolean(cbTrasactDialog.Checked);
        }

        private void cbTrasactChat_CheckedChanged(object sender, EventArgs e)
        {
            s["transaction_notification_chat"] = OSD.FromBoolean(cbTrasactChat.Checked);
        }
    }
}
