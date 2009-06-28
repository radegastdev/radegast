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
using System.IO;
using System.Text;
using System.Windows.Forms;
using RadegastNc;
using OpenMetaverse;

namespace Radegast
{
    public class RadegastInstance
    {
        private GridClient client;
        private RadegastNetcom netcom;

        private ImageCache imageCache;
        private StateManager state;
        private ConfigManager config;

        private frmMain mainForm;
        private TabsConsole tabsConsole;

        // Singleton, there can be only one instance
        private static RadegastInstance globalInstance = null;
        public static RadegastInstance GlobalInstance
        {
            get
            {
                if (globalInstance == null)
                {
                    globalInstance = new RadegastInstance();
                }
                return globalInstance;
            }
        }

        private string userDir;
        /// <summary>
        /// System (not grid!) user's dir
        /// </summary>
        public string UserDir { get { return userDir; } }

        private string clientDir;
        /// <summary>
        /// Grid client's user dir for settings and logs
        /// </summary>
        public string ClientDir { get { return clientDir; } }

        public string InventoryCacheFileName { get { return Path.Combine(ClientDir, "inventory.cache"); } }

        private string animCacheDir;
        public string AnimCacheDir { get { return animCacheDir; } }

        private string globalLogFile;
        public string GlobalLogFile { get { return globalLogFile; } }

        private bool monoRuntime;
        public bool MonoRuntime { get { return monoRuntime; } }

        public Dictionary<UUID, Group> groups;
        public Dictionary<UUID, string> nameCache = new Dictionary<UUID,string>();

        public delegate void OnAvatarNameCallBack(UUID agentID, string agentName);
        public event OnAvatarNameCallBack OnAvatarName;

        public readonly bool advancedDebugging = false;

        private RadegastInstance()
        {
            InitializeLoggingAndConfig();

            // Settings.PIPELINE_REFRESH_INTERVAL = 2000.0f;

            client = new GridClient();
            client.Settings.ALWAYS_REQUEST_OBJECTS = true;
            client.Settings.ALWAYS_DECODE_OBJECTS = true;
            client.Settings.OBJECT_TRACKING = true;
            client.Settings.ENABLE_SIMSTATS = true;
            client.Settings.FETCH_MISSING_INVENTORY = true;
            client.Settings.MULTIPLE_SIMS = false;
            client.Settings.SEND_AGENT_THROTTLE = true;
            client.Settings.SEND_AGENT_UPDATES = true;

            client.Settings.USE_TEXTURE_CACHE = true;
            client.Settings.TEXTURE_CACHE_DIR = Path.Combine(userDir,  "cache");
            client.Assets.Cache.AutoPruneEnabled = false;
    
            client.Throttle.Texture = 2446000.0f;
            client.Throttle.Asset = 2446000.0f;
            client.Settings.THROTTLE_OUTGOING_PACKETS = true;
            client.Settings.LOGIN_TIMEOUT = 120 * 1000;
            client.Settings.SIMULATOR_TIMEOUT = 120 * 1000;
            client.Settings.USE_INTERPOLATION_TIMER = false;
            client.Settings.MAX_CONCURRENT_TEXTURE_DOWNLOADS = 20;

            netcom = new RadegastNetcom(client);
            imageCache = new ImageCache();
            state = new StateManager(this);

            InitializeConfigLegacy();

            mainForm = new frmMain(this);
            mainForm.InitializeControls();
            tabsConsole = mainForm.TabConsole;

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            groups = new Dictionary<UUID, Group>();
         
            client.Groups.OnCurrentGroups += new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
            client.Groups.OnGroupLeft += new GroupManager.GroupLeftCallback(Groups_OnGroupLeft);
            client.Groups.OnGroupDropped += new GroupManager.GroupDroppedCallback(Groups_OnGroupDropped);
            client.Groups.OnGroupJoined += new GroupManager.GroupJoinedCallback(Groups_OnGroupJoined);
            client.Groups.OnGroupProfile += new GroupManager.GroupProfileCallback(Groups_OnGroupProfile);
            client.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client.Network.OnLogin += new NetworkManager.LoginCallback(Network_OnLogin);
            client.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
        }

        public void CleanUp()
        {
            if (client != null)
            {
                client.Groups.OnCurrentGroups -= new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
                client.Groups.OnGroupLeft -= new GroupManager.GroupLeftCallback(Groups_OnGroupLeft);
                client.Groups.OnGroupDropped -= new GroupManager.GroupDroppedCallback(Groups_OnGroupDropped);
                client.Groups.OnGroupJoined -= new GroupManager.GroupJoinedCallback(Groups_OnGroupJoined);
                client.Groups.OnGroupProfile -= new GroupManager.GroupProfileCallback(Groups_OnGroupProfile);
                client.Avatars.OnAvatarNames -= new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
                client.Network.OnLogin -= new NetworkManager.LoginCallback(Network_OnLogin);
                client.Network.OnDisconnected -= new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            }

            if (MonoRuntime)
            {
                Environment.Exit(0);
            }

        }

        void Avatars_OnAvatarNames(Dictionary<UUID, string> names)
        {
            lock (nameCache)
            {
                foreach (KeyValuePair<UUID, string> av in names)
                {
                    if (OnAvatarName != null) try { OnAvatarName(av.Key, av.Value); }
                        catch (Exception) { };

                    if (!nameCache.ContainsKey(av.Key))
                    {
                        nameCache.Add(av.Key, av.Value);
                    }
                }
            }
        }

        public string getAvatarName(UUID key)
        {
            if (nameCache.ContainsKey(key))
            {
                return nameCache[key];
            }
            else
            {
                client.Avatars.RequestAvatarName(key);
                return "Loading...";
            }
        }

        void Groups_OnGroupProfile(Group group)
        {
            if (groups.ContainsKey(group.ID))
            {
                groups[group.ID] = group;
            }
        }

        void Groups_OnGroupJoined(UUID groupID, bool success)
        {
            if (success && !groups.ContainsKey(groupID))
            {
                groups.Add(groupID, new Group());
                client.Groups.RequestGroupProfile(groupID);
            }
        }

        void Groups_OnGroupLeft(UUID groupID, bool success)
        {
            if (groups.ContainsKey(groupID))
            {
                groups.Remove(groupID);
            }
        }

        void Groups_OnGroupDropped(UUID groupID)
        {
            if (groups.ContainsKey(groupID))
            {
                groups.Remove(groupID);
            }
        }

        void Network_OnDisconnected(NetworkManager.DisconnectType reason, string message)
        {
            clientDir = null;
        }

        void Network_OnLogin(LoginStatus login, string message)
        {
            if (login != LoginStatus.Success)
                return;

            clientDir = Path.Combine(userDir, client.Self.Name);
            try
            {
                if (!Directory.Exists(clientDir))
                {
                    Directory.CreateDirectory(clientDir);
                }
            }
            catch (Exception)
            {
                clientDir = Directory.GetCurrentDirectory();
            }

        }

        public void LogClientMessage(string fileName, string message)
        {
            if (clientDir == null) return;

            lock (this)
            {
                try
                {
                    foreach (char lDisallowed in System.IO.Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(lDisallowed.ToString(), "_");
                    }

                    StreamWriter logfile = File.AppendText(Path.Combine(clientDir, fileName));
                    logfile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd [HH:mm:ss] ") + message);
                    logfile.Close();
                    logfile.Dispose();
                }
                catch (Exception) { }
            }
        }

        void Groups_OnCurrentGroups(Dictionary<UUID, Group> gr)
        {
            this.groups = gr;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            config.SaveCurrentConfig();
        }

        private void InitializeLoggingAndConfig()
        {
            // Are we running mono?
            if (null == Type.GetType("Mono.Runtime"))
            {
                monoRuntime = false;
            }
            else
            {
                monoRuntime = true;
            }

            try
            {
                userDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Properties.Resources.ProgramName);
                if (!Directory.Exists(userDir))
                {
                    Directory.CreateDirectory(userDir);
                }
            }
            catch (Exception)
            {
                userDir = System.Environment.CurrentDirectory;
            };

            animCacheDir = Path.Combine(userDir, @"anim_cache");
            globalLogFile = Path.Combine(userDir, Properties.Resources.ProgramName + ".log");
        }

        private void InitializeConfigLegacy()
        {
            config = new ConfigManager(this);
            config.ApplyDefault();

            netcom.LoginOptions.FirstName = config.CurrentConfig.FirstName;
            netcom.LoginOptions.LastName = config.CurrentConfig.LastName;
            netcom.LoginOptions.Password = config.CurrentConfig.PasswordMD5;
            netcom.LoginOptions.IsPasswordMD5 = true;
        }

        public GridClient Client
        {
            get { return client; }
        }

        public RadegastNetcom Netcom
        {
            get { return netcom; }
        }

        public ImageCache ImageCache
        {
            get { return imageCache; }
        }

        public StateManager State
        {
            get { return state; }
        }

        public ConfigManager Config
        {
            get { return config; }
        }

        public frmMain MainForm
        {
            get { return mainForm; }
        }

        public TabsConsole TabConsole
        {
            get { return tabsConsole; }
        }
    }
}
