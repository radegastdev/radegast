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
// $Id: Map.cs 152 2009-08-24 14:19:58Z latifer@gmail.com $
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Radegast;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using AIMLbot;

namespace Radegast.Plugin.Alice
{
    [Radegast.Plugin(Name = "ALICE Chatbot", Description = "A.L.I.C.E. based AI chat bot", Version = "1.1")]
    public class AliceAI : IRadegastPlugin
    {
        private RadegastInstance Instance;
        private GridClient Client { get { return Instance.Client; } }

        private bool Enabled = false;
        private Avatar.AvatarProperties MyProfile;
        private AIMLbot.Bot Alice;
        private Hashtable AliceUsers = new Hashtable();
        private ToolStripMenuItem MenuButton, EnabledButton;
        private TalkToAvatar talkToAvatar;
        private bool respondWithoutName = false;
        private int respondRange = -1;
        private bool shout2shout = false;
        private bool whisper2whisper = false;
        private ToolStripMenuItem respondWithoutNameButton, distance_5m, distance_10m, distance_15m, distance_20m, btn_shout2shout, btn_whisper2whisper, btn_enableDelay;
        private bool DisableOnStart = false;
        private ToolStripMenuItem btn_DisableOnStart;
        private bool EnableRandomDelay = false;
        private bool AimlLoaded = false;

        public void StartPlugin(RadegastInstance inst)
        {
            Instance = inst;
            Instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(Instance_ClientChanged);

            if (!Instance.GlobalSettings.Keys.Contains("plugin.alice.enabled"))
            {
                Instance.GlobalSettings["plugin.alice.enabled"] = OSD.FromBoolean(Enabled);
            }
            else
            {
                Enabled = Instance.GlobalSettings["plugin.alice.enabled"].AsBoolean();
            }

            if (!Instance.GlobalSettings.Keys.Contains("plugin.alice.disableOnStart"))
            {
                Instance.GlobalSettings["plugin.alice.disableOnStart"] = OSD.FromBoolean(DisableOnStart);
            }
            else
            {
                DisableOnStart = Instance.GlobalSettings["plugin.alice.disableOnStart"].AsBoolean();
            }
            if (DisableOnStart)
            {
                Enabled = false;
            }
            btn_DisableOnStart = new ToolStripMenuItem("Disable on start", null, (object sender, EventArgs e) =>
            {
                DisableOnStart = btn_DisableOnStart.Checked = !DisableOnStart;
                Instance.GlobalSettings["plugin.alice.disableOnStart"] = OSD.FromBoolean(DisableOnStart);
            });
            btn_DisableOnStart.Checked = DisableOnStart;

            EnabledButton = new ToolStripMenuItem("Enabled", null, (object sender, EventArgs e) =>
            {
                Enabled = SetEnabled(!Enabled);
                EnabledButton.Checked = MenuButton.Checked = Enabled;
                Instance.GlobalSettings["plugin.alice.enabled"] = OSD.FromBoolean(Enabled);
            });

            if (!Instance.GlobalSettings.Keys.Contains("plugin.alice.respondWithoutName"))
            {
                Instance.GlobalSettings["plugin.alice.respondWithoutName"] = OSD.FromBoolean(respondWithoutName);
            }
            else
            {
                respondWithoutName = Instance.GlobalSettings["plugin.alice.respondWithoutName"].AsBoolean();
            }

            respondWithoutNameButton = new ToolStripMenuItem("Respond without name", null, (object sender, EventArgs e) =>
            {
                respondWithoutName = respondWithoutNameButton.Checked = !respondWithoutName;
                Instance.GlobalSettings["plugin.alice.respondWithoutName"] = OSD.FromBoolean(respondWithoutName);
            });

            if (!Instance.GlobalSettings.Keys.Contains("plugin.alice.respondRange"))
            {
                Instance.GlobalSettings["plugin.alice.respondRange"] = respondRange;
            }
            else
            {
                respondRange = Instance.GlobalSettings["plugin.alice.respondRange"];
            }

            distance_5m = new ToolStripMenuItem("5m range", null, (object sender, EventArgs e) =>
            {
                distance_5m.Checked = !distance_5m.Checked;
                if (distance_5m.Checked)
                {
                    respondRange = 5;
                    distance_10m.Checked = false;
                    distance_15m.Checked = false;
                    distance_20m.Checked = false;
                    Instance.GlobalSettings["plugin.alice.respondRange"] = OSD.FromReal(respondRange);
                }
                else if (!distance_10m.Checked && !distance_15m.Checked && !distance_20m.Checked)
                {
                    respondRange = -1;
                    Instance.GlobalSettings["plugin.alice.respondRange"] = OSD.FromReal(respondRange);
                }
            });

            distance_10m = new ToolStripMenuItem("10m range", null, (object sender, EventArgs e) =>
            {
                distance_10m.Checked = !distance_10m.Checked;
                if (distance_10m.Checked)
                {
                    respondRange = 10;
                    distance_5m.Checked = false;
                    distance_15m.Checked = false;
                    distance_20m.Checked = false;
                    Instance.GlobalSettings["plugin.alice.respondRange"] = OSD.FromReal(respondRange);
                }
                else if (!distance_5m.Checked && !distance_15m.Checked && !distance_20m.Checked)
                {
                    respondRange = -1;
                    Instance.GlobalSettings["plugin.alice.respondRange"] = OSD.FromReal(respondRange);
                }
            });

            distance_15m = new ToolStripMenuItem("15m range", null, (object sender, EventArgs e) =>
            {
                distance_15m.Checked = !distance_15m.Checked;
                if (distance_15m.Checked)
                {
                    respondRange = 15;
                    distance_5m.Checked = false;
                    distance_10m.Checked = false;
                    distance_20m.Checked = false;
                    Instance.GlobalSettings["plugin.alice.respondRange"] = OSD.FromReal(respondRange);
                }
                else if (!distance_5m.Checked && !distance_10m.Checked && !distance_20m.Checked)
                {
                    respondRange = -1;
                    Instance.GlobalSettings["plugin.alice.respondRange"] = OSD.FromReal(respondRange);
                }
            });

            distance_20m = new ToolStripMenuItem("20m range", null, (object sender, EventArgs e) =>
            {
                distance_20m.Checked = !distance_20m.Checked;
                if (distance_20m.Checked)
                {
                    respondRange = 20;
                    distance_5m.Checked = false;
                    distance_10m.Checked = false;
                    distance_15m.Checked = false;
                    Instance.GlobalSettings["plugin.alice.respondRange"] = OSD.FromReal(respondRange);
                }
                else if (!distance_5m.Checked && !distance_10m.Checked && !distance_15m.Checked)
                {
                    respondRange = -1;
                    Instance.GlobalSettings["plugin.alice.respondRange"] = OSD.FromReal(respondRange);
                }
            });

            if (!Instance.GlobalSettings.ContainsKey("plugin.alice.shout2shout"))
            {
                Instance.GlobalSettings["plugin.alice.shout2shout"] = OSD.FromBoolean(shout2shout);
            }
            else
            {
                shout2shout = Instance.GlobalSettings["plugin.alice.shout2shout"].AsBoolean();
            }

            btn_shout2shout = new ToolStripMenuItem("Shout response to Shout", null, (object sender, EventArgs e) =>
            {
                shout2shout = btn_shout2shout.Checked = !shout2shout;
                Instance.GlobalSettings["plugin.alice.shout2shout"] = OSD.FromBoolean(shout2shout);
            });

            if (!Instance.GlobalSettings.ContainsKey("plugin.alice.whisper2whisper"))
            {
                Instance.GlobalSettings["plugin.alice.whisper2whisper"] = OSD.FromBoolean(whisper2whisper);
            }
            else
            {
                whisper2whisper = Instance.GlobalSettings["plugin.alice.whisper2whisper"].AsBoolean();
            }

            btn_whisper2whisper = new ToolStripMenuItem("Whisper response to Whisper", null, (object sender, EventArgs e) =>
            {
                whisper2whisper = btn_whisper2whisper.Checked = !whisper2whisper;
                Instance.GlobalSettings["plugin.alice.whisper2whisper"] = OSD.FromBoolean(whisper2whisper);
            });

            MenuButton = new ToolStripMenuItem("ALICE chatbot", null, (object sender, EventArgs e) =>
            {
                Enabled = SetEnabled(!Enabled);
                EnabledButton.Checked = MenuButton.Checked = Enabled;
                Instance.GlobalSettings["plugin.alice.enabled"] = OSD.FromBoolean(Enabled);
            });

            btn_enableDelay = new ToolStripMenuItem("Enable random delay", null, (sender, e) =>
            {
                btn_enableDelay.Checked = !btn_enableDelay.Checked;
                Instance.GlobalSettings["plugin.alice.enable_delay"] = EnableRandomDelay = btn_enableDelay.Checked;
            });
            btn_enableDelay.Checked = EnableRandomDelay = Instance.GlobalSettings["plugin.alice.enable_delay"];

            Instance.MainForm.PluginsMenu.DropDownItems.Add(MenuButton);
            Instance.MainForm.PluginsMenu.Visible = true;
            MenuButton.DropDownItems.Add(EnabledButton);
            MenuButton.Checked = EnabledButton.Checked = Enabled;

            MenuButton.DropDownItems.Add(respondWithoutNameButton);
            MenuButton.DropDownItems.Add(distance_5m);
            MenuButton.DropDownItems.Add(distance_10m);
            MenuButton.DropDownItems.Add(distance_15m);
            MenuButton.DropDownItems.Add(distance_20m);
            MenuButton.DropDownItems.Add(btn_shout2shout);
            MenuButton.DropDownItems.Add(btn_whisper2whisper);

            respondWithoutNameButton.Checked = respondWithoutName;
            if (respondRange == 5.0)
            {
                distance_5m.Checked = true;
            }
            else if (respondRange == 10.0)
            {
                distance_10m.Checked = true;
            }
            else if (respondRange == 15.0)
            {
                distance_15m.Checked = true;
            }
            else if (respondRange == 20.0)
            {
                distance_20m.Checked = true;
            }
            btn_shout2shout.Checked = shout2shout;
            btn_whisper2whisper.Checked = whisper2whisper;

            MenuButton.DropDownItems.Add(btn_enableDelay);
            MenuButton.DropDownItems.Add(btn_DisableOnStart);
            MenuButton.DropDownItems.Add("Reload AIML", null, (object sender, EventArgs e) =>
            {
                Alice = null;
                GC.Collect();
                LoadALICE();
            });

            SetEnabled(Enabled);

            // Events
            RegisterClientEvents(Client);
        }

        private bool SetEnabled(bool e)
        {
            if (!e || AimlLoaded) return e;
            if (!LoadALICE()) return false;
            if (Client.Network.Connected)
            {
                Alice.GlobalSettings.updateSetting("name", FirstName(Client.Self.Name));
            }
            AimlLoaded = true;
            talkToAvatar = new TalkToAvatar(Instance, Alice);
            Instance.ContextActionManager.RegisterContextAction(talkToAvatar);
            return true;
        }

        private bool LoadALICE()
        {
            try
            {
                Alice = new AIMLbot.Bot();
                Alice.isAcceptingUserInput = false;
                Alice.loadSettings();
                AIMLbot.Utils.AIMLLoader loader = new AIMLbot.Utils.AIMLLoader(Alice);
                Alice.isAcceptingUserInput = false;
                loader.loadAIML(Alice.PathToAIML);
                Alice.isAcceptingUserInput = true;
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Failed loading ALICE: " + ex.Message);
                return false;
            }
        }


        public void StopPlugin(RadegastInstance Instance)
        {
            // Remove the menu buttons
            EnabledButton.Dispose();
            MenuButton.Dispose();

            if (talkToAvatar != null)
            {
                Instance.ContextActionManager.DeregisterContextAction(talkToAvatar);
            }
            // Unregister events
            UnregisterClientEvents(Client);
        }

        private void RegisterClientEvents(GridClient client)
        {
            Client.Self.ChatFromSimulator += new EventHandler<ChatEventArgs>(Self_ChatFromSimulator);
            Client.Self.IM += new EventHandler<InstantMessageEventArgs>(Self_IM);
            Client.Avatars.AvatarPropertiesReply += new EventHandler<AvatarPropertiesReplyEventArgs>(Avatars_AvatarPropertiesReply);
            Instance.Netcom.ClientConnected += new EventHandler<EventArgs>(Netcom_ClientConnected);
        }

        private void UnregisterClientEvents(GridClient client)
        {
            Client.Self.ChatFromSimulator -= new EventHandler<ChatEventArgs>(Self_ChatFromSimulator);
            Client.Self.IM -= new EventHandler<InstantMessageEventArgs>(Self_IM);
            Client.Avatars.AvatarPropertiesReply -= new EventHandler<AvatarPropertiesReplyEventArgs>(Avatars_AvatarPropertiesReply);
            Instance.Netcom.ClientConnected -= new EventHandler<EventArgs>(Netcom_ClientConnected);
        }

        void Instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(Client);
        }

        void Netcom_ClientConnected(object sender, EventArgs e)
        {
            if (AimlLoaded)
            {
                Alice.GlobalSettings.updateSetting("name", FirstName(Client.Self.Name));
            }
        }

        void Avatars_AvatarPropertiesReply(object sender, AvatarPropertiesReplyEventArgs e)
        {
            if (e.AvatarID == Client.Self.AgentID && AimlLoaded)
            {
                MyProfile = e.Properties;
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

        private object syncChat = new object();
        private Random rand = new Random();

        void Self_ChatFromSimulator(object sender, ChatEventArgs e)
        {
            // We ignore everything except normal chat from other avatars
            if (!Enabled || e.SourceType != ChatSourceType.Agent || e.FromName == Client.Self.Name || e.Message.Trim().Length == 0) return;

            bool parseForResponse = Alice != null && Alice.isAcceptingUserInput && Enabled;
            if (parseForResponse && respondRange >= 0)
            {
                parseForResponse = Vector3.Distance(Client.Self.SimPosition, e.Position) <= respondRange;
            }
            if (parseForResponse)
            {
                parseForResponse = respondWithoutName || e.Message.ToLower().Contains(FirstName(Client.Self.Name).ToLower());
            }


            if (parseForResponse)
            {
                WorkPool.QueueUserWorkItem(sync =>
                {
                    lock (syncChat)
                    {
                        Alice.GlobalSettings.updateSetting("location", "region " + Client.Network.CurrentSim.Name);
                        string msg = e.Message.ToLower();
                        msg = msg.Replace(FirstName(Client.Self.Name).ToLower(), "");
                        AIMLbot.User user;
                        if (AliceUsers.ContainsKey(e.FromName))
                        {
                            user = (AIMLbot.User)AliceUsers[e.FromName];
                        }
                        else
                        {
                            user = new User(e.FromName, Alice);
                            user.Predicates.removeSetting("name");
                            user.Predicates.addSetting("name", FirstName(e.FromName));
                            AliceUsers[e.FromName] = user;
                        }

                        Client.Self.Movement.TurnToward(e.Position);
                        if (EnableRandomDelay) System.Threading.Thread.Sleep(1000 + 1000 * rand.Next(2));
                        if (!Instance.State.IsTyping)
                        {
                            Instance.State.SetTyping(true);
                        }
                        if (EnableRandomDelay)
                        {
                            System.Threading.Thread.Sleep(2000 + 1000 * rand.Next(5));
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                        Instance.State.SetTyping(false);
                        AIMLbot.Request req = new Request(msg, user, Alice);
                        AIMLbot.Result res = Alice.Chat(req);
                        string outp = res.Output;
                        if (outp.Length > 1000)
                        {
                            outp = outp.Substring(0, 1000);
                        }

                        ChatType useChatType = ChatType.Normal;
                        if (shout2shout && e.Type == ChatType.Shout)
                        {
                            useChatType = ChatType.Shout;
                        }
                        else if (whisper2whisper && e.Type == ChatType.Whisper)
                        {
                            useChatType = ChatType.Whisper;
                        }
                        Client.Self.Chat(outp, 0, useChatType);
                    }
                });
            }
        }

        void Self_IM(object sender, InstantMessageEventArgs e)
        {
            if (!Enabled) return;
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
                            Self_IM(sender, e);
                        }
                        ));
                return;
            }

            // We need to filter out all sorts of things that come in as a instante message
            if (e.IM.Dialog == InstantMessageDialog.MessageFromAgent // Message is not notice, inv. offer, etc etc
                && !Instance.Groups.ContainsKey(e.IM.IMSessionID)  // Message is not group IM (sessionID == groupID)
                && e.IM.BinaryBucket.Length < 2                    // Session is not ad-hoc friends conference
                && e.IM.FromAgentName != "Second Life"             // Not a system message
                && Alice.isAcceptingUserInput                    // Alice bot loaded successfully
                )
            {
                WorkPool.QueueUserWorkItem(sync =>
                {
                    lock (syncChat)
                    {
                        Alice.GlobalSettings.updateSetting("location", "region " + Client.Network.CurrentSim.Name);
                        AIMLbot.User user;
                        if (AliceUsers.ContainsKey(e.IM.FromAgentName))
                        {
                            user = (AIMLbot.User)AliceUsers[e.IM.FromAgentName];
                        }
                        else
                        {
                            user = new User(e.IM.FromAgentName, Alice);
                            user.Predicates.removeSetting("name");
                            user.Predicates.addSetting("name", FirstName(e.IM.FromAgentName));
                            AliceUsers[e.IM.FromAgentName] = user;
                        }
                        AIMLbot.Request req = new Request(e.IM.Message, user, Alice);
                        AIMLbot.Result res = Alice.Chat(req);
                        string msg = res.Output;
                        if (msg.Length > 1000)
                        {
                            msg = msg.Substring(0, 1000);
                        }
                        if (EnableRandomDelay) System.Threading.Thread.Sleep(2000 + 1000 * rand.Next(3));
                        Instance.Netcom.SendIMStartTyping(e.IM.FromAgentID, e.IM.IMSessionID);
                        if (EnableRandomDelay)
                        {
                            System.Threading.Thread.Sleep(2000 + 1000 * rand.Next(5));
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                        Instance.Netcom.SendIMStopTyping(e.IM.FromAgentID, e.IM.IMSessionID);
                        if (Instance.MainForm.InvokeRequired)
                        {
                            Instance.MainForm.BeginInvoke(new MethodInvoker(() => Instance.Netcom.SendInstantMessage(msg, e.IM.FromAgentID, e.IM.IMSessionID)));
                        }
                        else
                        {
                            Instance.Netcom.SendInstantMessage(msg, e.IM.FromAgentID, e.IM.IMSessionID);
                        }
                    }
                });
            }
        }

        private string FirstName(string name)
        {
            return name.Split(' ')[0];
        }
    }
}
