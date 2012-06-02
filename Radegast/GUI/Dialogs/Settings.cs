﻿
// 
// Radegast Metaverse Client
// Copyright (c) 2009-2012, Radegast Development Team
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

using OpenMetaverse;
using OpenMetaverse.StructuredData;

using Radegast.Bot;

namespace Radegast
{
    public enum AutoResponseType : int
    {
        WhenBusy = 0,
        WhenFromNonFriend = 1,
        Always = 2
    }

    public partial class frmSettings : RadegastForm
    {
        private Settings s;
        private static bool settingInitialized = false;

        public static void InitSettigs(Settings s)
        {
            if (s["im_timestamps"].Type == OSDType.Unknown)
            {
                s["im_timestamps"] = OSD.FromBoolean(true);
            }

            if (s["rlv_enabled"].Type == OSDType.Unknown)
            {
                s["rlv_enabled"] = new OSDBoolean(false);
            }

            if (s["mu_emotes"].Type == OSDType.Unknown)
            {
                s["mu_emotes"] = new OSDBoolean(false);
            }

            if (s["friends_notification_highlight"].Type == OSDType.Unknown)
            {
                s["friends_notification_highlight"] = new OSDBoolean(true);
            }

            if (!s.ContainsKey("no_typing_anim")) s["no_typing_anim"] = OSD.FromBoolean(false);

            if (!s.ContainsKey("auto_response_type"))
            {
                s["auto_response_type"] = (int)AutoResponseType.WhenBusy;
                s["auto_response_text"] = "The Resident you messaged is in 'busy mode' which means they have requested not to be disturbed.  Your message will still be shown in their IM panel for later viewing.";
            }

            if (!s.ContainsKey("script_syntax_highlight")) s["script_syntax_highlight"] = OSD.FromBoolean(true);

            if (!s.ContainsKey("display_name_mode")) s["display_name_mode"] = (int)NameMode.Smart;

            // Convert legacy settings from first last name to username
            if (!s.ContainsKey("username") && (s.ContainsKey("first_name") && s.ContainsKey("last_name")))
            {
                s["username"] = s["first_name"] + " " + s["last_name"];
                s.Remove("first_name");
                s.Remove("last_name");
            }

            if (!s.ContainsKey("reconnect_time")) s["reconnect_time"] = 120;

            if (!s.ContainsKey("transaction_notification_chat")) s["transaction_notification_chat"] = true;

            if (!s.ContainsKey("transaction_notification_dialog")) s["transaction_notification_dialog"] = true;

            if (!s.ContainsKey("minimize_to_tray")) s["minimize_to_tray"] = false;

            if (!s.ContainsKey("scene_window_docked")) s["scene_window_docked"] = true;

            if (!s.ContainsKey("taskbar_highlight")) s["taskbar_highlight"] = true;

            if (!s.ContainsKey("rendering_occlusion_culling_enabled")) s["rendering_occlusion_culling_enabled"] = true;

            if (!s.ContainsKey("rendering_use_vbo")) s["rendering_use_vbo"] = true;

            if (!s.ContainsKey("send_rad_client_tag")) s["send_rad_client_tag"] = true;

            if (!s.ContainsKey("log_to_file")) s["log_to_file"] = true;

            if (!s.ContainsKey("disable_chat_im_log")) s["disable_chat_im_log"] = false;

            if (!s.ContainsKey("disable_look_at")) s["disable_look_at"] = false;

            if (!s.ContainsKey("highlight_on_chat")) s["highlight_on_chat"] = true;

            if (!s.ContainsKey("highlight_on_im")) s["highlight_on_im"] = true;

            if (!s.ContainsKey("highlight_on_group_im")) s["highlight_on_group_im"] = true;
        }

        public frmSettings(RadegastInstance instance)
            : base(instance)
        {
            if (settingInitialized)
            {
                frmSettings.InitSettigs(instance.GlobalSettings);
            }

            InitializeComponent();

            s = instance.GlobalSettings;
            tbpGraphics.Controls.Add(new Radegast.Rendering.GraphicsPreferences(instance));
            cbChatTimestamps.Checked = s["chat_timestamps"].AsBoolean();

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

            cbRLV.Checked = s["rlv_enabled"].AsBoolean();
            cbRLV.CheckedChanged += (object sender, EventArgs e) =>
            {
                s["rlv_enabled"] = new OSDBoolean(cbRLV.Checked);
            };

            cbMUEmotes.Checked = s["mu_emotes"].AsBoolean();
            cbMUEmotes.CheckedChanged += (object sender, EventArgs e) =>
            {
                s["mu_emotes"] = new OSDBoolean(cbMUEmotes.Checked);
            };

            if (s["chat_font_size"].Type != OSDType.Real)
            {
                s["chat_font_size"] = OSD.FromReal(((ChatConsole)instance.TabConsole.Tabs["chat"].Control).cbxInput.Font.Size);
            }

            cbFontSize.Text = s["chat_font_size"].AsReal().ToString(System.Globalization.CultureInfo.InvariantCulture);

            if (!s.ContainsKey("minimize_to_tray")) s["minimize_to_tray"] = OSD.FromBoolean(false);
            cbMinToTrey.Checked = s["minimize_to_tray"].AsBoolean();
            cbMinToTrey.CheckedChanged += (object sender, EventArgs e) =>
            {
                s["minimize_to_tray"] = OSD.FromBoolean(cbMinToTrey.Checked);
            };


            cbNoTyping.Checked = s["no_typing_anim"].AsBoolean();
            cbNoTyping.CheckedChanged += (object sender, EventArgs e) =>
            {
                s["no_typing_anim"] = OSD.FromBoolean(cbNoTyping.Checked);
            };

            txtAutoResponse.Text = s["auto_response_text"];
            txtAutoResponse.TextChanged += (object sender, EventArgs e) =>
            {
                s["auto_response_text"] = txtAutoResponse.Text;
            };
            AutoResponseType art = (AutoResponseType)s["auto_response_type"].AsInteger();
            switch (art)
            {
                case AutoResponseType.WhenBusy: rbAutobusy.Checked = true; break;
                case AutoResponseType.WhenFromNonFriend: rbAutoNonFriend.Checked = true; break;
                case AutoResponseType.Always: rbAutoAlways.Checked = true; break;
            }

            cbSyntaxHighlight.Checked = s["script_syntax_highlight"].AsBoolean();
            cbSyntaxHighlight.CheckedChanged += (object sender, EventArgs e) =>
            {
                s["script_syntax_highlight"] = OSD.FromBoolean(cbSyntaxHighlight.Checked);
            };

            switch ((NameMode)s["display_name_mode"].AsInteger())
            {
                case NameMode.Standard: rbDNOff.Checked = true; break;
                case NameMode.Smart: rbDNSmart.Checked = true; break;
                case NameMode.DisplayNameAndUserName: rbDNDandUsernme.Checked = true; break;
                case NameMode.OnlyDisplayName: rbDNOnlyDN.Checked = true; break;
            }

            txtReconnectTime.Text = s["reconnect_time"].AsInteger().ToString();

            cbRadegastClientTag.Checked = s["send_rad_client_tag"];
            cbRadegastClientTag.CheckedChanged += (sender, e) =>
            {
                s["send_rad_client_tag"] = cbRadegastClientTag.Checked;
                instance.SetClientTag();
            };

            cbOnInvOffer.SelectedIndex = s["inv_auto_accept_mode"].AsInteger();
            cbOnInvOffer.SelectedIndexChanged += (sender, e) =>
            {
                s["inv_auto_accept_mode"] = cbOnInvOffer.SelectedIndex;
            };

            cbRadegastLogToFile.Checked = s["log_to_file"];

            cbDisableChatIMLog.Checked = s["disable_chat_im_log"];
            cbDisableChatIMLog.CheckedChanged += (sender, e) =>
            {
                s["disable_chat_im_log"] = cbDisableChatIMLog.Checked;
            };

            cbDisableLookAt.Checked = s["disable_look_at"];
            cbDisableLookAt.CheckedChanged += (sender, e) =>
            {
                s["disable_look_at"] = cbDisableLookAt.Checked;
            };

            cbTaskBarHighLight.Checked = s["taskbar_highlight"];
            cbTaskBarHighLight.CheckedChanged += (sender, e) =>
            {
                s["taskbar_highlight"] = cbTaskBarHighLight.Checked;
                UpdateEnabled();
            };

            cbFriendsHighlight.Checked = s["friends_notification_highlight"].AsBoolean();
            cbFriendsHighlight.CheckedChanged += (object sender, EventArgs e) =>
            {
                s["friends_notification_highlight"] = new OSDBoolean(cbFriendsHighlight.Checked);
            };

            cbHighlightChat.Checked = s["highlight_on_chat"];
            cbHighlightChat.CheckedChanged += (sender, e) =>
            {
                s["highlight_on_chat"] = cbHighlightChat.Checked;
            };

            cbHighlightIM.Checked = s["highlight_on_im"];
            cbHighlightIM.CheckedChanged += (sender, e) =>
            {
                s["highlight_on_im"] = cbHighlightIM.Checked;
            };

            cbHighlightGroupIM.Checked = s["highlight_on_group_im"];
            cbHighlightGroupIM.CheckedChanged += (sender, e) =>
            {
                s["highlight_on_group_im"] = cbHighlightGroupIM.Checked;
            };

            autoSitPrefsUpdate();
            pseudoHomePrefsUpdated();

            UpdateEnabled();
        }

        void UpdateEnabled()
        {
            if (cbTaskBarHighLight.Checked)
            {
                cbFriendsHighlight.Enabled = cbHighlightChat.Enabled = cbHighlightGroupIM.Enabled = cbHighlightIM.Enabled = true;
            }
            else
            {
                cbFriendsHighlight.Enabled = cbHighlightChat.Enabled = cbHighlightGroupIM.Enabled = cbHighlightIM.Enabled = false;
            }
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

        private void UpdateFontSize()
        {
            double f = 8.25;
            double existing = s["chat_font_size"].AsReal();

            if (!double.TryParse(cbFontSize.Text, out f))
            {
                cbFontSize.Text = s["chat_font_size"].AsReal().ToString(System.Globalization.CultureInfo.InvariantCulture);
                return;
            }

            if (Math.Abs(existing - f) > 0.0001f)
                s["chat_font_size"] = OSD.FromReal(f);

        }

        private void cbFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFontSize();
        }

        private void cbFontSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateFontSize();
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void cbFontSize_Leave(object sender, EventArgs e)
        {
            UpdateFontSize();
        }

        private void rbAutobusy_CheckedChanged(object sender, EventArgs e)
        {
            s["auto_response_type"] = (int)AutoResponseType.WhenBusy;
        }

        private void rbAutoNonFriend_CheckedChanged(object sender, EventArgs e)
        {
            s["auto_response_type"] = (int)AutoResponseType.WhenFromNonFriend;
        }

        private void rbAutoAlways_CheckedChanged(object sender, EventArgs e)
        {
            s["auto_response_type"] = (int)AutoResponseType.Always;
        }

        private void rbDNOff_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDNOff.Checked)
                s["display_name_mode"] = (int)NameMode.Standard;
        }

        private void rbDNSmart_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDNSmart.Checked)
                s["display_name_mode"] = (int)NameMode.Smart;
        }

        private void rbDNDandUsernme_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDNDandUsernme.Checked)
                s["display_name_mode"] = (int)NameMode.DisplayNameAndUserName;
        }

        private void rbDNOnlyDN_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDNOnlyDN.Checked)
                s["display_name_mode"] = (int)NameMode.OnlyDisplayName;
        }

        private void txtReconnectTime_TextChanged(object sender, EventArgs e)
        {
            string input = System.Text.RegularExpressions.Regex.Replace(txtReconnectTime.Text, @"[^\d]", "");
            int t = 120;
            int.TryParse(input, out t);

            if (txtReconnectTime.Text != t.ToString())
            {
                txtReconnectTime.Text = t.ToString();
                txtReconnectTime.Select(txtReconnectTime.Text.Length, 0);
            }

            s["reconnect_time"] = t;
        }

        private void cbRadegastLogToFile_CheckedChanged(object sender, EventArgs e)
        {
            s["log_to_file"] = OSD.FromBoolean(cbRadegastLogToFile.Checked);
        }

        #region Auto-Sit

        private void autoSitPrefsUpdate()
        {
            autoSit.Enabled = (Instance.Client.Network.Connected && Instance.ClientSettings != null);
            if (!autoSit.Enabled)
            {
                return;
            }
            AutoSitPreferences prefs = Instance.State.AutoSit.Preferences;
            autoSitName.Text = prefs.PrimitiveName;
            autoSitUUID.Text = prefs.Primitive.ToString();
            autoSitSit.Enabled = prefs.Primitive != UUID.Zero;
            autoSitEnabled.Checked = prefs.Enabled;
        }

        private void autoSitClear_Click(object sender, EventArgs e)
        {
            Instance.State.AutoSit.Preferences = new AutoSitPreferences();
            autoSitPrefsUpdate();
        }

        private void autoSitNameLabel_Click(object sender, EventArgs e)
        {
            autoSitName.SelectAll();
        }

        private void autoSitUUIDLabel_Click(object sender, EventArgs e)
        {
            autoSitUUID.SelectAll();
        }

        private void autoSitSit_Click(object sender, EventArgs e)
        {
            Instance.State.AutoSit.TrySit();
        }

        private void autoSitEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Instance.State.AutoSit.Preferences = new AutoSitPreferences
            {
                Primitive = Instance.State.AutoSit.Preferences.Primitive,
                PrimitiveName = Instance.State.AutoSit.Preferences.PrimitiveName,
                Enabled = autoSitEnabled.Checked
            };
        }

        #endregion

        #region Pseudo Home

        private void pseudoHomePrefsUpdated()
        {
            pseudoHome.Enabled = (Instance.Client.Network.Connected && Instance.ClientSettings != null);
            if (!pseudoHome.Enabled)
            {
                return;
            }
            PseudoHomePreferences prefs = Instance.State.PseudoHome.Preferences;
            pseudoHomeLocation.Text = (prefs.Region != string.Empty) ? string.Format("{0} <{1}, {2}, {3}>", prefs.Region, (int)prefs.Position.X, (int)prefs.Position.Y, (int)prefs.Position.Z) : "";
            pseudoHomeEnabled.Checked = prefs.Enabled;
            pseudoHomeTP.Enabled = (prefs.Region.Trim() != string.Empty);
            pseudoHomeTolerance.Value = Math.Max(pseudoHomeTolerance.Minimum, Math.Min(pseudoHomeTolerance.Maximum, prefs.Tolerance));
        }

        private void pseudoHomeLabel_Click(object sender, EventArgs e)
        {
            pseudoHomeLocation.SelectAll();
        }

        private void pseudoHomeEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Instance.State.PseudoHome.Preferences = new PseudoHomePreferences
            {
                Enabled = pseudoHomeEnabled.Checked,
                Region = Instance.State.PseudoHome.Preferences.Region,
                Position = Instance.State.PseudoHome.Preferences.Position,
                Tolerance = Instance.State.PseudoHome.Preferences.Tolerance
            };
        }

        private void pseudoHomeTP_Click(object sender, EventArgs e)
        {
            Instance.State.PseudoHome.ETGoHome();
        }

        private void pseudoHomeSet_Click(object sender, EventArgs e)
        {
            Instance.State.PseudoHome.Preferences = new PseudoHomePreferences
            {
                Enabled = Instance.State.PseudoHome.Preferences.Enabled,
                Region = Instance.Client.Network.CurrentSim.Name,
                Position = Instance.Client.Self.SimPosition,
                Tolerance = Instance.State.PseudoHome.Preferences.Tolerance
            };
            pseudoHomePrefsUpdated();
        }

        private void pseudoHomeTolerance_ValueChanged(object sender, EventArgs e)
        {
            Instance.State.PseudoHome.Preferences = new PseudoHomePreferences
            {
                Enabled = Instance.State.PseudoHome.Preferences.Enabled,
                Region = Instance.State.PseudoHome.Preferences.Region,
                Position = Instance.State.PseudoHome.Preferences.Position,
                Tolerance = (uint)pseudoHomeTolerance.Value
            };
        }

        #endregion
    }
}
