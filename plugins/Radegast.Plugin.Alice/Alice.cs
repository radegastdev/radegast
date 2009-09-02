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
// $Id: Map.cs 152 2009-08-24 14:19:58Z latifer@gmail.com $
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Radegast;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using AIMLbot;

namespace Radegast.Plugin.Alice
{
    public class DemoPlugin : IRadegastPlugin
    {
        private RadegastInstance Instance;
        private GridClient Client { get { return Instance.Client; } }

        private bool Enabled = false;
        private Avatar.AvatarProperties MyProfile;
        private AIMLbot.Bot Alice;
        private Hashtable AliceUsers = new Hashtable();
        private ToolStripMenuItem MenuButton;

        public void StartPlugin(RadegastInstance inst)
        {
            Instance = inst;

            Alice = new AIMLbot.Bot();
            Alice.isAcceptingUserInput = false;

            if (!Instance.GlobalSettings.Keys.Contains("plugin.alice.enabled"))
            {
                Instance.GlobalSettings["plugin.alice.enabled"] = OSD.FromBoolean(Enabled);
            }
            else
            {
                Enabled = Instance.GlobalSettings["plugin.alice.enabled"].AsBoolean();
            }

            MenuButton = new ToolStripMenuItem("A.L.I.C.E. chatbot", null, OnALICEMenuButtonClicked);
            Instance.MainForm.ToolsMenu.DropDownItems.Add(MenuButton);
            MenuButton.Checked = Enabled;

            try
            {
                Alice.loadSettings();
                AIMLbot.Utils.AIMLLoader loader = new AIMLbot.Utils.AIMLLoader(Alice);
                Alice.isAcceptingUserInput = false;
                loader.loadAIML(Alice.PathToAIML);
                Alice.isAcceptingUserInput = true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Failed loading ALICE: " + ex.Message);
            }

            // Events
            Client.Self.OnChat += new AgentManager.ChatCallback(Self_OnChat);
            Client.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            Client.Avatars.OnAvatarProperties += new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
            Client.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);

        }

        public void StopPlugin(RadegastInstance Instance)
        {
            // Remove the menu button
            Instance.MainForm.ToolsMenu.DropDownItems.Remove(MenuButton);

            // Unregister events
            Client.Self.OnChat -= new AgentManager.ChatCallback(Self_OnChat);
            Client.Self.OnInstantMessage -= new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            Client.Avatars.OnAvatarProperties -= new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
            Client.Network.OnConnected -= new NetworkManager.ConnectedCallback(Network_OnConnected);
        }

        void Network_OnConnected(object sender)
        {
            Alice.GlobalSettings.updateSetting("name", FirstName(Client.Self.Name));
        }

        void Avatars_OnAvatarProperties(UUID avatarID, Avatar.AvatarProperties properties)
        {
            if (avatarID == Client.Self.AgentID)
            {
                MyProfile = properties;
                Alice.GlobalSettings.updateSetting("birthday", MyProfile.BornOn);
                DateTime bd;
                if (DateTime.TryParse(MyProfile.BornOn, Utils.EnUsCulture, System.Globalization.DateTimeStyles.None, out bd))
                {
                    DateTime now = DateTime.Today;
                    int age = now.Year - bd.Year;
                    if (now.Month < bd.Month || (now.Month == bd.Month && now.Day < bd.Day))
                    {
                        --age;
                    }
                    Alice.GlobalSettings.updateSetting("age", age.ToString());
                    Alice.GlobalSettings.updateSetting("birthday", bd.ToLongDateString());

                }
            }
        }

        void Self_OnChat(string message, ChatAudibleLevel audible, ChatType type, ChatSourceType sourceType, string fromName, UUID id, UUID ownerid, Vector3 position)
        {
            // We ignore everything except normal chat from other avatars
            if (sourceType != ChatSourceType.Agent || fromName == Client.Self.Name) return;

            if (Alice.isAcceptingUserInput && message.ToLower().Contains(FirstName(Client.Self.Name).ToLower()) && Enabled)
            {
                Alice.GlobalSettings.updateSetting("location", "region " + Client.Network.CurrentSim.Name);
                string msg = message.ToLower();
                msg = msg.Replace(FirstName(Client.Self.Name).ToLower(), "");
                AIMLbot.User user;
                if (AliceUsers.ContainsKey(fromName))
                {
                    user = (AIMLbot.User)AliceUsers[fromName];
                }
                else
                {
                    user = new User(fromName, Alice);
                    user.Predicates.removeSetting("name");
                    user.Predicates.addSetting("name", FirstName(fromName));
                    AliceUsers[fromName] = user;
                }
                Client.Self.Movement.TurnToward(position);
                if (!Instance.State.IsTyping)
                {
                    Instance.State.SetTyping(true);
                }
                System.Threading.Thread.Sleep(1000);
                Instance.State.SetTyping(false);
                AIMLbot.Request req = new Request(msg, user, Alice);
                AIMLbot.Result res = Alice.Chat(req);
                string outp = res.Output;
                if (outp.Length > 1000)
                {
                    outp = outp.Substring(0, 1000);
                }

                Client.Self.Chat(outp, 0, ChatType.Normal);
            }
        }

        void Self_OnInstantMessage(InstantMessage im, Simulator simulator)
        {
            // Every event coming from a different thread (almost all of them, most certanly those
            // from libomv) needs to be executed on the GUI thread. This code can be basically
            // copy-pasted on the begining of each libomv event handler that results in update 
            // of any GUI element
            //
            // In this case the IM we sent back as a reply is also displayed in the corresponding IM tab
            if (Instance.MainForm.InvokeRequired)
            {
                Instance.MainForm.BeginInvoke(
                    new MethodInvoker(
                        delegate()
                        {
                            Self_OnInstantMessage(im, simulator);
                        }
                        ));
                return;
            }

            // We need to filter out all sorts of things that come in as a instante message
            if (im.Dialog == InstantMessageDialog.MessageFromAgent // Message is not notice, inv. offer, etc etc
                && !Instance.Groups.ContainsKey(im.IMSessionID)  // Message is not group IM (sessionID == groupID)
                && im.BinaryBucket.Length < 2                    // Session is not ad-hoc friends conference
                && im.FromAgentName != "Second Life"             // Not a system message
                && Alice.isAcceptingUserInput                    // Alice bot loaded successfully
                && Enabled                                       // Alice bot is enabled
                )
            {
                Alice.GlobalSettings.updateSetting("location", "region " + Client.Network.CurrentSim.Name);
                AIMLbot.User user;
                if (AliceUsers.ContainsKey(im.FromAgentName))
                {
                    user = (AIMLbot.User)AliceUsers[im.FromAgentName];
                }
                else
                {
                    user = new User(im.FromAgentName, Alice);
                    user.Predicates.removeSetting("name");
                    user.Predicates.addSetting("name", FirstName(im.FromAgentName));
                    AliceUsers[im.FromAgentName] = user;
                }
                AIMLbot.Request req = new Request(im.Message, user, Alice);
                AIMLbot.Result res = Alice.Chat(req);
                string msg = res.Output;
                if (msg.Length > 1000)
                {
                    msg = msg.Substring(0, 1000);
                }
                Instance.Netcom.SendInstantMessage(msg, im.FromAgentID, im.IMSessionID);

            }
        }

        private string FirstName(string name)
        {
            return name.Split(' ')[0];
        }

        private void OnALICEMenuButtonClicked(object sender, EventArgs e)
        {
            Enabled = MenuButton.Checked = !Enabled;
            Instance.GlobalSettings["plugin.alice.enabled"] = OSD.FromBoolean(Enabled);
        }
    }
}
