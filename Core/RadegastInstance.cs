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
using System.Reflection;
using System.Windows.Forms;
using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public class RadegastInstance
    {
        private GridClient client;
        private RadegastNetcom netcom;

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

        /// <summary>
        /// Grid client's user dir for settings and logs
        /// </summary>
        public string ClientDir
        {
            get
            {
                if (client != null && client.Self != null && !string.IsNullOrEmpty(client.Self.Name))
                {
                    return Path.Combine(userDir, client.Self.Name);
                }
                else
                {
                    return Environment.CurrentDirectory;
                }
            }
        }

        public string InventoryCacheFileName { get { return Path.Combine(ClientDir, "inventory.cache"); } }

        private string animCacheDir;
        public string AnimCacheDir { get { return animCacheDir; } }

        private string globalLogFile;
        public string GlobalLogFile { get { return globalLogFile; } }

        private bool monoRuntime;
        public bool MonoRuntime { get { return monoRuntime; } }

        private Dictionary<UUID, Group> groups;
        public Dictionary<UUID, Group> Groups { get { return groups; } }

        public Dictionary<UUID, string> nameCache = new Dictionary<UUID, string>();

        public const string INCOMPLETE_NAME = "Loading...";

        public readonly bool advancedDebugging = false;

        public readonly List<IRadegastPlugin> PluginsLoaded = new List<IRadegastPlugin>();

        private RadegastInstance()
        {
            InitializeLoggingAndConfig();

            client = new GridClient();
            client.Settings.USE_INTERPOLATION_TIMER = false;
            client.Settings.ALWAYS_REQUEST_OBJECTS = true;
            client.Settings.ALWAYS_DECODE_OBJECTS = true;
            client.Settings.OBJECT_TRACKING = true;
            client.Settings.ENABLE_SIMSTATS = true;
            client.Settings.FETCH_MISSING_INVENTORY = true;
            client.Settings.MULTIPLE_SIMS = true;
            client.Settings.SEND_AGENT_THROTTLE = true;
            client.Settings.SEND_AGENT_UPDATES = true;

            client.Settings.USE_ASSET_CACHE = true;
            client.Settings.ASSET_CACHE_DIR = Path.Combine(userDir, "cache");
            client.Assets.Cache.AutoPruneEnabled = false;

            client.Throttle.Texture = 2446000.0f;
            client.Throttle.Asset = 2446000.0f;
            client.Settings.THROTTLE_OUTGOING_PACKETS = true;
            client.Settings.LOGIN_TIMEOUT = 120 * 1000;
            client.Settings.SIMULATOR_TIMEOUT = 120 * 1000;
            client.Settings.MAX_CONCURRENT_TEXTURE_DOWNLOADS = 20;

            netcom = new RadegastNetcom(client);
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
            client.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);
            ScanAndLoadPlugins();
        }

        public void CleanUp()
        {
            if (client != null)
            {
                client.Groups.OnCurrentGroups -= new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
                client.Groups.OnGroupLeft -= new GroupManager.GroupLeftCallback(Groups_OnGroupLeft);
                client.Groups.OnGroupDropped -= new GroupManager.GroupDroppedCallback(Groups_OnGroupDropped);
                client.Groups.OnGroupJoined -= new GroupManager.GroupJoinedCallback(Groups_OnGroupJoined);
                client.Avatars.OnAvatarNames -= new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
                client.Network.OnConnected -= new NetworkManager.ConnectedCallback(Network_OnConnected);
            }

            lock (PluginsLoaded)
            {
                PluginsLoaded.ForEach(plug =>
                {
                    try
                    {
                        plug.StopPlugin(this);
                    }
                    catch (Exception) { }
                });
            }

            state.Dispose();
            state = null;
            netcom.Dispose();
            netcom = null;
            Logger.Log("RadegastInstance finished cleaning up.", Helpers.LogLevel.Debug);
        }

        private void ScanAndLoadPlugins()
        {
            string dirName = Application.StartupPath;

            if (!Directory.Exists(dirName)) return;

            foreach (string loadfilename in Directory.GetFiles(dirName))
            {
                if (loadfilename.ToLower().EndsWith(".dll") || loadfilename.ToLower().EndsWith(".exe"))
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(loadfilename);
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (typeof(IRadegastPlugin).IsAssignableFrom(type))
                            {
                                foreach (var ci in type.GetConstructors())
                                {
                                    if (ci.GetParameters().Length > 0) continue;
                                    try
                                    {
                                        IRadegastPlugin plug = (IRadegastPlugin)ci.Invoke(new object[0]);
                                        plug.StartPlugin(this);
                                        lock (PluginsLoaded) PluginsLoaded.Add(plug);
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log("ERROR in Radegast Plugin: " + ex.Message, Helpers.LogLevel.Debug);
                                    }
                                }
                            }
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        // non .NET .dlls
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Out of date or dlls missing sub dependencies
                    }
                }
            }
        }

        void Network_OnConnected(object sender)
        {
            try
            {
                if (!Directory.Exists(ClientDir))
                    Directory.CreateDirectory(ClientDir);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to create client directory", Helpers.LogLevel.Warning, ex);
            }
        }

        void Avatars_OnAvatarNames(Dictionary<UUID, string> names)
        {
            lock (nameCache)
            {
                foreach (KeyValuePair<UUID, string> av in names)
                {
                    if (!nameCache.ContainsKey(av.Key))
                    {
                        nameCache.Add(av.Key, av.Value);
                    }
                }
            }
        }

        public string getAvatarName(UUID key)
        {
            lock (nameCache)
            {
                if (key == UUID.Zero)
                {
                    return "(???) (???)";
                }
                if (nameCache.ContainsKey(key))
                {
                    return nameCache[key];
                }
                else
                {
                    client.Avatars.RequestAvatarName(key);
                    return INCOMPLETE_NAME;
                }
            }
        }

        public void getAvatarNames(List<UUID> keys)
        {
            lock (nameCache)
            {
                List<UUID> newNames = new List<UUID>();
                foreach (UUID key in keys)
                {
                    if (!nameCache.ContainsKey(key))
                    {
                        newNames.Add(key);
                    }
                }
                if (newNames.Count > 0)
                {
                    client.Avatars.RequestAvatarNames(newNames);
                }
            }
        }

        public bool haveAvatarName(UUID key)
        {
            lock (nameCache)
            {
                if (nameCache.ContainsKey(key))
                    return true;
                else
                    return false;
            }
        }

        void Groups_OnGroupJoined(UUID groupID, bool success)
        {
            client.Groups.RequestCurrentGroups();
        }

        void Groups_OnGroupLeft(UUID groupID, bool success)
        {
            client.Groups.RequestCurrentGroups();
        }

        void Groups_OnGroupDropped(UUID groupID)
        {
            client.Groups.RequestCurrentGroups();
        }

        public void LogClientMessage(string fileName, string message)
        {
            lock (this)
            {
                try
                {
                    foreach (char lDisallowed in System.IO.Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(lDisallowed.ToString(), "_");
                    }

                    StreamWriter logfile = File.AppendText(Path.Combine(ClientDir, fileName));
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
            monoRuntime = Type.GetType("Mono.Runtime") != null;

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
