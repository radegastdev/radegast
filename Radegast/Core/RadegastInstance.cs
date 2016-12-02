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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Radegast.Commands;
using Radegast.Netcom;
using Radegast.Media;
using OpenMetaverse;

namespace Radegast
{
    public class RadegastInstance
    {
        #region OnRadegastFormCreated
        public event Action<RadegastForm> RadegastFormCreated;
        /// <summary>
        /// Triggers the RadegastFormCreated event.
        /// </summary>
        public virtual void OnRadegastFormCreated(RadegastForm radForm)
        {
            RadegastFormCreated?.Invoke(radForm);
        }
        #endregion

        // Singleton, there can be only one instance
        private static RadegastInstance globalInstance = null;
        public static RadegastInstance GlobalInstance => globalInstance ?? (globalInstance = new RadegastInstance(new GridClient()));

        /// <summary>
        /// Manages retrieving avatar names
        /// </summary>
        public NameManager Names { get; private set; }

        /// <summary>
        /// When was Radegast started (UTC)
        /// </summary>
        public readonly DateTime StartupTimeUTC;

        /// <summary>
        /// Time zone of the current world (currently hard coded to US Pacific time)
        /// </summary>
        public TimeZoneInfo WordTimeZone;

        /// <summary>
        /// System (not grid!) user's dir
        /// </summary>
        public string UserDir { get; private set; }

        /// <summary>
        /// Grid client's user dir for settings and logs
        /// </summary>
        public string ClientDir => !string.IsNullOrEmpty(Client?.Self?.Name) ? Path.Combine(UserDir, Client.Self.Name) : Environment.CurrentDirectory;

        public string InventoryCacheFileName => Path.Combine(ClientDir, "inventory.cache");

        public string GlobalLogFile { get; private set; }

        public bool MonoRuntime { get; }

        public Dictionary<UUID, Group> Groups { get; private set; } = new Dictionary<UUID, Group>();

        /// <summary>
        /// Global settings for the entire application
        /// </summary>
        public Settings GlobalSettings { get; private set; }

        /// <summary>
        /// Per client settings
        /// </summary>
        public Settings ClientSettings { get; private set; }

        public const string INCOMPLETE_NAME = "Loading...";

        public readonly bool advancedDebugging = false;

        /// <summary> Handles loading plugins and scripts</summary>
        public PluginManager PluginManager { get; private set; }

        /// <summary>
        /// Radegast media manager for playing streams and in world sounds
        /// </summary>
        public MediaManager MediaManager { get; private set; }


        /// <summary>
        /// Radegast command manager for executing textual console commands
        /// </summary>
        public CommandsManager CommandsManager { get; private set; }

        /// <summary>
        /// Radegast ContextAction manager for context sensitive actions
        /// </summary>
        public ContextActionsManager ContextActionManager { get; private set; }

        /// <summary>
        /// Allows key emulation for moving avatar around
        /// </summary>
        public RadegastMovement Movement { get; private set; }

        private InventoryClipboard inventoryClipboard;
        /// <summary>
        /// The last item that was cut or copied in the inventory, used for pasting
        /// in a different place on the inventory, or other places like profile
        /// that allow sending copied inventory items
        /// </summary>
        public InventoryClipboard InventoryClipboard
        {
            get { return inventoryClipboard; }
            set
            {
                inventoryClipboard = value;
                OnInventoryClipboardUpdated(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Manager for RLV functionality
        /// </summary>
        public RLVManager RLV { get; private set; }

        /// <summary>Manages default params for different grids</summary>
        public GridManager GridManger { get; private set; }

        /// <summary>
        /// Is system using plain color theme, with white background and dark text
        /// </summary>
        public bool PlainColors
        {
            get
            {
                // If windows background is whiteish, declare as standard color scheme
                var c = System.Drawing.SystemColors.Window;
                return c.R > 240 && c.G > 240 && c.B > 240;
            }
        }

        /// <summary>
        /// Keyaboard handling manager (used in 3D scene viewer)
        /// </summary>
        public Keyboard Keyboard;

        /// <summary>
        /// Current Outfit Folder (appearnce) manager
        /// </summary>
        public CurrentOutfitFolder COF;

        /// <summary>
        /// Did we report crash to the grid login service
        /// </summary>
        public bool ReportedCrash = false;

        private string CrashMarkerFileName => Path.Combine(UserDir, "crash_marker");

        #region Events

        #region ClientChanged event
        /// <summary>The event subscribers, null of no subscribers</summary>
        private EventHandler<ClientChangedEventArgs> m_ClientChanged;

        ///<summary>Raises the ClientChanged Event</summary>
        /// <param name="e">A ClientChangedEventArgs object containing
        /// the old and the new client</param>
        protected virtual void OnClientChanged(ClientChangedEventArgs e)
        {
            EventHandler<ClientChangedEventArgs> handler = m_ClientChanged;
            handler?.Invoke(this, e);
        }

        /// <summary>Thread sync lock object</summary>
        private readonly object m_ClientChangedLock = new object();

        /// <summary>Raised when the GridClient object in the main Radegast instance is changed</summary>
        public event EventHandler<ClientChangedEventArgs> ClientChanged
        {
            add { lock (m_ClientChangedLock) { m_ClientChanged += value; } }
            remove { lock (m_ClientChangedLock) { m_ClientChanged -= value; } }
        }
        #endregion ClientChanged event

        #region InventoryClipboardUpdated event
        /// <summary>The event subscribers, null of no subscribers</summary>
        private EventHandler<EventArgs> m_InventoryClipboardUpdated;

        ///<summary>Raises the InventoryClipboardUpdated Event</summary>
        /// <param name="e">A EventArgs object containing
        /// the old and the new client</param>
        protected virtual void OnInventoryClipboardUpdated(EventArgs e)
        {
            EventHandler<EventArgs> handler = m_InventoryClipboardUpdated;
            handler?.Invoke(this, e);
        }

        /// <summary>Thread sync lock object</summary>
        private readonly object m_InventoryClipboardUpdatedLock = new object();

        /// <summary>Raised when the GridClient object in the main Radegast instance is changed</summary>
        public event EventHandler<EventArgs> InventoryClipboardUpdated
        {
            add { lock (m_InventoryClipboardUpdatedLock) { m_InventoryClipboardUpdated += value; } }
            remove { lock (m_InventoryClipboardUpdatedLock) { m_InventoryClipboardUpdated -= value; } }
        }
        #endregion InventoryClipboardUpdated event


        #endregion Events

        public RadegastInstance(GridClient client0)
        {
            // incase something else calls GlobalInstance while we are loading
            globalInstance = this;

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += HandleThreadException;
            }

            Client = client0;

            // Initialize current time zone, and mark when we started
            GetWorldTimeZone();
            StartupTimeUTC = DateTime.UtcNow;

            // Are we running mono?
            MonoRuntime = Type.GetType("Mono.Runtime") != null;

            Keyboard = new Keyboard();
            Application.AddMessageFilter(Keyboard);

            Netcom = new RadegastNetcom(this);
            State = new StateManager(this);
            MediaManager = new MediaManager(this);
            CommandsManager = new CommandsManager(this);
            ContextActionManager = new ContextActionsManager(this);
            RegisterContextActions();
            Movement = new RadegastMovement(this);

            InitializeLoggingAndConfig();
            InitializeClient(Client);

            RLV = new RLVManager(this);
            GridManger = new GridManager();
            GridManger.LoadGrids();

            Names = new NameManager(this);
            COF = new CurrentOutfitFolder(this);

            MainForm = new frmMain(this);
            MainForm.InitializeControls();

            MainForm.Load += new EventHandler(mainForm_Load);
            PluginManager = new PluginManager(this);
            PluginManager.ScanAndLoadPlugins();
        }

        private void InitializeClient(GridClient client)
        {
            client.Settings.MULTIPLE_SIMS = false;

            client.Settings.USE_INTERPOLATION_TIMER = false;
            client.Settings.ALWAYS_REQUEST_OBJECTS = true;
            client.Settings.ALWAYS_DECODE_OBJECTS = true;
            client.Settings.OBJECT_TRACKING = true;
            client.Settings.ENABLE_SIMSTATS = true;
            client.Settings.FETCH_MISSING_INVENTORY = true;
            client.Settings.SEND_AGENT_THROTTLE = true;
            client.Settings.SEND_AGENT_UPDATES = true;
            client.Settings.STORE_LAND_PATCHES = true;

            client.Settings.USE_ASSET_CACHE = true;
            client.Settings.ASSET_CACHE_DIR = Path.Combine(UserDir, "cache");
            client.Assets.Cache.AutoPruneEnabled = false;
            client.Assets.Cache.ComputeAssetCacheFilename = ComputeCacheName;

            client.Throttle.Total = 5000000f;
            client.Settings.THROTTLE_OUTGOING_PACKETS = false;
            client.Settings.LOGIN_TIMEOUT = 120 * 1000;
            client.Settings.SIMULATOR_TIMEOUT = 180 * 1000;
            client.Settings.MAX_CONCURRENT_TEXTURE_DOWNLOADS = 20;

            client.Self.Movement.AutoResetControls = false;
            client.Self.Movement.UpdateInterval = 250;

            RegisterClientEvents(client);
            SetClientTag();
        }

        public string ComputeCacheName(string cacheDir, UUID assetID)
        {
            string fileName = assetID.ToString();
            string dir = cacheDir
                + Path.DirectorySeparatorChar + fileName.Substring(0, 1)
                + Path.DirectorySeparatorChar + fileName.Substring(1, 1);
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch
            {
                return Path.Combine(cacheDir, fileName);
            }
            return Path.Combine(dir, fileName);
        }

        private void RegisterClientEvents(GridClient client)
        {
            client.Groups.CurrentGroups += new EventHandler<CurrentGroupsEventArgs>(Groups_CurrentGroups);
            client.Groups.GroupLeaveReply += new EventHandler<GroupOperationEventArgs>(Groups_GroupsChanged);
            client.Groups.GroupDropped += new EventHandler<GroupDroppedEventArgs>(Groups_GroupsChanged);
            client.Groups.GroupJoinedReply += new EventHandler<GroupOperationEventArgs>(Groups_GroupsChanged);
            if (Netcom != null)
                Netcom.ClientConnected += new EventHandler<EventArgs>(netcom_ClientConnected);
            client.Network.LoginProgress += new EventHandler<LoginProgressEventArgs>(Network_LoginProgress);
        }

        private void UnregisterClientEvents(GridClient client)
        {
            client.Groups.CurrentGroups -= new EventHandler<CurrentGroupsEventArgs>(Groups_CurrentGroups);
            client.Groups.GroupLeaveReply -= new EventHandler<GroupOperationEventArgs>(Groups_GroupsChanged);
            client.Groups.GroupDropped -= new EventHandler<GroupDroppedEventArgs>(Groups_GroupsChanged);
            client.Groups.GroupJoinedReply -= new EventHandler<GroupOperationEventArgs>(Groups_GroupsChanged);
            if (Netcom != null)
                Netcom.ClientConnected -= new EventHandler<EventArgs>(netcom_ClientConnected);
            client.Network.LoginProgress -= new EventHandler<LoginProgressEventArgs>(Network_LoginProgress);
        }

        public void SetClientTag()
        {
            Client.Settings.CLIENT_IDENTIFICATION_TAG = GlobalSettings["send_rad_client_tag"] ? new UUID("b748af88-58e2-995b-cf26-9486dea8e830") : UUID.Zero;
        }
        private void GetWorldTimeZone()
        {
            try
            {
                foreach (TimeZoneInfo tz in TimeZoneInfo.GetSystemTimeZones())
                {
                    if (tz.Id == "Pacific Standard Time" || tz.Id == "America/Los_Angeles")
                    {
                        WordTimeZone = tz;
                        break;
                    }
                }
            }
            catch (Exception) { }
        }

        public DateTime GetWorldTime()
        {
            DateTime now;

            try
            {
                now = WordTimeZone != null ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, WordTimeZone) : DateTime.UtcNow.AddHours(-7);
            }
            catch (Exception)
            {
                now = DateTime.UtcNow.AddHours(-7);
            }

            return now;
        }


        public void Reconnect()
        {
            TabConsole.DisplayNotificationInChat("Attempting to reconnect...", ChatBufferTextStyle.StatusDarkBlue);
            Logger.Log("Attemting to reconnect", Helpers.LogLevel.Info, Client);
            GridClient oldClient = Client;
            Client = new GridClient();
            UnregisterClientEvents(oldClient);
            InitializeClient(Client);
            OnClientChanged(new ClientChangedEventArgs(oldClient, Client));
            Netcom.Login();
        }

        public void CleanUp()
        {
            MarkEndExecution();

            if (COF != null)
            {
                COF.Dispose();
                COF = null;
            }

            if (Names != null)
            {
                Names.Dispose();
                Names = null;
            }

            if (GridManger != null)
            {
                GridManger.Dispose();
                GridManger = null;
            }

            if (RLV != null)
            {
                RLV.Dispose();
                RLV = null;
            }

            if (Client != null)
            {
                UnregisterClientEvents(Client);
            }

            if (PluginManager != null)
            {
                PluginManager.Dispose();
                PluginManager = null;
            }

            if (Movement != null)
            {
                Movement.Dispose();
                Movement = null;
            }
            if (CommandsManager != null)
            {
                CommandsManager.Dispose();
                CommandsManager = null;
            }
            if (ContextActionManager != null)
            {
                ContextActionManager.Dispose();
                ContextActionManager = null;
            }
            if (MediaManager != null)
            {
                MediaManager.Dispose();
                MediaManager = null;
            }
            if (State != null)
            {
                State.Dispose();
                State = null;
            }
            if (Netcom != null)
            {
                Netcom.Dispose();
                Netcom = null;
            }
            if (MainForm != null)
            {
                MainForm.Load -= new EventHandler(mainForm_Load);
            }
            Logger.Log("RadegastInstance finished cleaning up.", Helpers.LogLevel.Debug);
        }

        void mainForm_Load(object sender, EventArgs e)
        {
            PluginManager.StartPlugins();
        }

        void netcom_ClientConnected(object sender, EventArgs e)
        {
            Client.Self.RequestMuteList();
        }

        void Network_LoginProgress(object sender, LoginProgressEventArgs e)
        {
            if (e.Status != LoginStatus.ConnectingToSim) return;
            try
            {
                if (!Directory.Exists(ClientDir))
                {
                    Directory.CreateDirectory(ClientDir);
                }
                ClientSettings = new Settings(Path.Combine(ClientDir, "client_settings.xml"));
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to create client directory", Helpers.LogLevel.Warning, ex);
            }
        }


        /// <summary>
        /// Fetches avatar name
        /// </summary>
        /// <param name="key">Avatar UUID</param>
        /// <param name="blocking">Should we wait until the name is retrieved</param>
        /// <returns>Avatar name</returns>
        [Obsolete("Use Instance.Names.Get() instead")]
        public string getAvatarName(UUID key, bool blocking)
        {
            return Names.Get(key, blocking);
        }

        /// <summary>
        /// Fetches avatar name from cache, if not in cache will requst name from the server
        /// </summary>
        /// <param name="key">Avatar UUID</param>
        /// <returns>Avatar name</returns>
        [Obsolete("Use Instance.Names.Get() instead")]
        public string getAvatarName(UUID key)
        {
            return Names.Get(key);
        }

        void Groups_GroupsChanged(object sender, EventArgs e)
        {
            Client.Groups.RequestCurrentGroups();
        }

        public static string SafeFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, lDisallowed) => current.Replace(lDisallowed.ToString(), "_"));
        }

        public string ChatFileName(string session)
        {
            string fileName = System.IO.Path.GetInvalidFileNameChars().Aggregate(session, (current, lDisallowed) => current.Replace(lDisallowed.ToString(), "_"));
            return Path.Combine(ClientDir, fileName);
        }

        public void LogClientMessage(string sessioName, string message)
        {
            if (GlobalSettings["disable_chat_im_log"]) return;

            lock (this)
            {
                try
                {
                    File.AppendAllText(ChatFileName(sessioName),
                        DateTime.Now.ToString("yyyy-MM-dd [HH:mm:ss] ") + message + Environment.NewLine);
                }
                catch (Exception) { }
            }
        }

        void Groups_CurrentGroups(object sender, CurrentGroupsEventArgs e)
        {
            this.Groups = e.Groups;
        }

        private void InitializeLoggingAndConfig()
        {
            try
            {
                UserDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Properties.Resources.ProgramName);
                if (!Directory.Exists(UserDir))
                {
                    Directory.CreateDirectory(UserDir);
                }
            }
            catch (Exception)
            {
                UserDir = System.Environment.CurrentDirectory;
            };

            GlobalLogFile = Path.Combine(UserDir, Properties.Resources.ProgramName + ".log");
            GlobalSettings = new Settings(Path.Combine(UserDir, "settings.xml"));
            frmSettings.InitSettigs(GlobalSettings, MonoRuntime);
        }

        public GridClient Client { get; private set; }
        public RadegastNetcom Netcom { get; private set; }
        public StateManager State { get; private set; }
        public frmMain MainForm { get; }
        public TabsConsole TabConsole => MainForm.TabConsole;

        public void HandleThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Log("Unhandled thread exception: "
                + e.Exception.Message + Environment.NewLine
                + e.Exception.StackTrace + Environment.NewLine,
                Helpers.LogLevel.Error,
                Client);
        }

        #region Crash reporting
        FileStream MarkerLock = null;

        public bool AnotherInstanceRunning()
        {
            // We have successfuly obtained lock
            if (MarkerLock != null && MarkerLock.CanWrite)
            {
                Logger.Log("No other instances detected, marker file already locked", Helpers.LogLevel.Debug);
                return false || MonoRuntime;
            }

            try
            {
                MarkerLock = new FileStream(CrashMarkerFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                Logger.Log($"Successfully created and locked marker file {CrashMarkerFileName}", Helpers.LogLevel.Debug);
                return false || MonoRuntime;
            }
            catch
            {
                MarkerLock = null;
                Logger.Log($"Another instance detected, marker fils {CrashMarkerFileName} locked", Helpers.LogLevel.Debug);
                return true;
            }
        }

        public LastExecStatus GetLastExecStatus()
        {
            // Crash marker file found and is not locked by us
            if (File.Exists(CrashMarkerFileName) && MarkerLock == null)
            {
                Logger.Log($"Found crash marker file {CrashMarkerFileName}", Helpers.LogLevel.Debug);
                return LastExecStatus.OtherCrash;
            }
            else
            {
                Logger.Log($"No crash marker file {CrashMarkerFileName} found", Helpers.LogLevel.Debug);
                return LastExecStatus.Normal;
            }
        }

        public void MarkStartExecution()
        {
            Logger.Log($"Marking start of execution run, creating file: {CrashMarkerFileName}", Helpers.LogLevel.Debug);
            try
            {
                File.Create(CrashMarkerFileName).Dispose();
            }
            catch { }
        }

        public void MarkEndExecution()
        {
            Logger.Log($"Marking end of execution run, deleting file: {CrashMarkerFileName}", Helpers.LogLevel.Debug);
            try
            {
                if (MarkerLock != null)
                {
                    MarkerLock.Close();
                    MarkerLock.Dispose();
                    MarkerLock = null;
                }

                File.Delete(CrashMarkerFileName);
            }
            catch { }
        }

        #endregion Crash reporting

        #region Context Actions
        void RegisterContextActions()
        {
            ContextActionManager.RegisterContextAction(typeof(Primitive), "Save as DAE...", ExportDAEHander);
            ContextActionManager.RegisterContextAction(typeof(Primitive),"Copy UUID to clipboard", CopyObjectUUIDHandler);
        }

        void DeregisterContextActions()
        {
            ContextActionManager.DeregisterContextAction(typeof(Primitive), "Save as DAE...");
            ContextActionManager.DeregisterContextAction(typeof(Primitive), "Copy UUID to clipboard");
        }

        void ExportDAEHander(object sender, EventArgs e)
        {
            MainForm.DisplayColladaConsole((Primitive)sender);
        }

        void CopyObjectUUIDHandler(object sender, EventArgs e)
        {
            if (MainForm.InvokeRequired)
            {
                if (MainForm.IsHandleCreated || !MonoRuntime)
                {
                    MainForm.Invoke(new MethodInvoker(() => CopyObjectUUIDHandler(sender, e)));
                }
                return;
            }

            Clipboard.SetText(((Primitive)sender).ID.ToString());
        }

        #endregion Context Actions

    }

    #region Event classes
    public class ClientChangedEventArgs : EventArgs
    {
        public GridClient OldClient { get; }
        public GridClient Client { get; }

        public ClientChangedEventArgs(GridClient OldClient, GridClient Client)
        {
            this.OldClient = OldClient;
            this.Client = Client;
        }
    }
    #endregion Event classes
}
