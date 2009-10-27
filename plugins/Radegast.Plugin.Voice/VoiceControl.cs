// 
// Radegast Metaverse Client Voice Interface
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
// $Id: PluginControl.cs 203 2009-09-07 19:26:02Z mojitotech@gmail.com $
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using Radegast;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast.Plugin.Voice
{
    public class VoiceControl : IRadegastPlugin
    {
        private const string VERSION = "0.1";
        internal RadegastInstance instance;
        internal ToolStripDropDownButton ToolsMenu;
        internal VoiceClient vClient;
        private ToolStripMenuItem VoiceButton;
        internal SettingsForm voiceConsole;

        /// <summary>
        /// Plugin start-up entry.
        /// </summary>
        /// <param name="inst"></param>
        /// <remarks>Called by Radegast at start-up</remarks>
        public void StartPlugin(RadegastInstance inst)
        {
            instance = inst;
            vClient = new VoiceClient(this);

            // Add our enable/disable item to the Plugin Menu.
            ToolsMenu = instance.MainForm.PluginsMenu;

            VoiceButton = new ToolStripMenuItem("Voice", null, OnVoiceMenuButtonClicked);
            ToolsMenu.DropDownItems.Add(VoiceButton);

            VoiceButton.Checked = instance.GlobalSettings["plugin.voice.enabled"].AsBoolean();

            // Watch for login and logout
            instance.Netcom.ClientLoginStatus +=
                new EventHandler<Radegast.Netcom.ClientLoginEventArgs>(Netcom_ClientLoginStatus);

            ToolsMenu.Visible = true;

            if (VoiceButton.Checked)
            {
                StartControls();
            }
        }

        void Netcom_ClientLoginStatus(object sender, Radegast.Netcom.ClientLoginEventArgs e)
        {
            if (e.Status == LoginStatus.Success)
            {
                if (VoiceButton.Checked)
                    StartControls();
            }
        }

        /// <summary>
        /// Start all Voice subsystems 
        /// </summary>
        private void StartControls()
        {
            // If we are not logged in, do not do this.
            if (!instance.Netcom.IsLoggedIn) return;

            instance.MainForm.TabConsole.DisplayNotificationInChat("Starting Voice service.");

            try
            {
                voiceConsole = new SettingsForm(this);
                voiceConsole.Show();
                vClient.Start();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Voice can not start.  See log.");
                Logger.Log("Voice can not start.", Helpers.LogLevel.Error, e);

                System.Console.WriteLine(e.StackTrace);
                MarkDisabled();
                return;
            }
        }

        void MarkDisabled()
        {
            VoiceButton.Checked = false;
        }

        /// <summary>
        /// Plugin shut-down entry
        /// </summary>
        /// <param name="inst"></param>
        /// <remarks>Called by Radegast at shut-down, or when Voice is switched off.
        /// We use this to release system resources.</remarks>
        public void StopPlugin(RadegastInstance inst)
        {
            if (!VoiceButton.Checked) return;

            try
            {
                vClient.Stop();
                voiceConsole.Hide();
            }
            catch (Exception e)
            {
                Logger.Log("Voice can not stop.", Helpers.LogLevel.Error, e);

                System.Console.WriteLine(e.StackTrace);
                MarkDisabled();
                return;
            }
        }

        /// <summary>
        /// Handle toggling of our enable flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVoiceMenuButtonClicked(object sender, EventArgs e)
        {
            VoiceButton.Checked = !VoiceButton.Checked;

            if (VoiceButton.Checked)
            {
                StartControls();
            }
            else
            {
                StopPlugin(instance);
            }

            // Save this into the config file.
            instance.GlobalSettings["plugin.voice.enabled"] = 
                OSD.FromBoolean(VoiceButton.Checked);
        }
    }
}

