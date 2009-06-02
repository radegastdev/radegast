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
        public readonly string userDir;
        public readonly string animCacheDir;

        public Dictionary<UUID, Group> groups;
        public Dictionary<UUID, string> nameCache = new Dictionary<UUID,string>();

        public delegate void OnAvatarNameCallBack(UUID agentID, string agentName);
        public event OnAvatarNameCallBack OnAvatarName;

        public readonly bool advancedDebugging = false;

        public RadegastInstance()
        {
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

            Settings.PIPELINE_REFRESH_INTERVAL = 2000.0f;

            client = new GridClient();
            client.Settings.ALWAYS_REQUEST_OBJECTS = true;
            client.Settings.ALWAYS_DECODE_OBJECTS = true;
            client.Settings.OBJECT_TRACKING = true;
            client.Settings.ENABLE_SIMSTATS = true;
            client.Settings.FETCH_MISSING_INVENTORY = true;
            client.Settings.MULTIPLE_SIMS = true;
            client.Settings.SEND_AGENT_THROTTLE = true;
            client.Settings.SEND_AGENT_UPDATES = true;

            client.Settings.USE_TEXTURE_CACHE = true;
            client.Settings.TEXTURE_CACHE_DIR = Path.Combine(userDir,  "cache");
            client.Assets.Cache.AutoPruneEnabled = false;
    
            client.Throttle.Texture = 2446000.0f;
            client.Throttle.Asset = 2446000.0f;
            client.Settings.THROTTLE_OUTGOING_PACKETS = false;
            
            netcom = new RadegastNetcom(client);

            imageCache = new ImageCache();
            state = new StateManager(this);
            InitializeConfig();

            mainForm = new frmMain(this);
            mainForm.InitializeControls();
            tabsConsole = mainForm.TabConsole;

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            groups = new Dictionary<UUID, Group>();
         
            client.Groups.OnCurrentGroups += new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
            client.Groups.OnGroupLeft += new GroupManager.GroupLeftCallback(Groups_OnGroupLeft);
            client.Groups.OnGroupJoined += new GroupManager.GroupJoinedCallback(Groups_OnGroupJoined);
            client.Groups.OnGroupProfile += new GroupManager.GroupProfileCallback(Groups_OnGroupProfile);
            client.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
        }

        public void CleanUp()
        {
            if (client != null)
            {
                client.Groups.OnCurrentGroups -= new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
                client.Groups.OnGroupLeft -= new GroupManager.GroupLeftCallback(Groups_OnGroupLeft);
                client.Groups.OnGroupJoined -= new GroupManager.GroupJoinedCallback(Groups_OnGroupJoined);
                client.Groups.OnGroupProfile -= new GroupManager.GroupProfileCallback(Groups_OnGroupProfile);
                client.Avatars.OnAvatarNames -= new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);

                client = null;
            }

            if (netcom != null)
            {
                netcom = null;
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

        void Groups_OnCurrentGroups(Dictionary<UUID, Group> gr)
        {
            this.groups = gr;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            config.SaveCurrentConfig();
        }

        private void InitializeConfig()
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
