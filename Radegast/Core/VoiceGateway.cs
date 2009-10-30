// This class is temporary.  It contains functions that will eventually
// move into OpenMetaverse.Voice.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Voice;
using OpenMetaverse.StructuredData;

namespace Radegast.Core
{
    public class VoiceGateway : IDisposable
    {
        // These states should be in increasing order of 'completeness'
        // so that the (int) values can drive a progress bar.
        public enum ConnectionState
        {
            None = 0,
            Provisioned,
            DaemonStarted,
            DaemonConnected,
            ConnectorConnected,
            AccountLogin,
            RegionCapAvailable,
            SessionRunning
        }

        private OpenMetaverse.Voice.VoiceGateway connector;

        internal string sipServer = "";
        private string acctServer = "https://www.bhr.vivox.com/api2/";
        private string connectionHandle;
        private string accountHandle;
        private string sessionHandle;

        // Parameters to Vivox daemon
        private string slvoicePath = "";
        private string slvoiceArgs = "-ll -1";
        private string daemonNode = "127.0.0.1";
        private int daemonPort = 44124;

        private string voiceUser;
        private string voicePassword;
        private string spatialUri;
        private string spatialCredentials;

        // Session management
        private Dictionary<string, VoiceSession> sessions;
        private VoiceSession spatialSession;
        private Uri currentParcelCap;
        private Uri nextParcelCap;
        private string regionName;

        // Position update thread
        private Thread posThread;
        private ManualResetEvent posRestart;
        public GridClient Client;
        private OpenMetaverse.Voice.VoiceGateway.VoicePosition position;
        private Vector3d oldPosition;
        private Vector3d oldAt;

        // Audio interfaces
        private List<string> inputDevices;
        /// <summary>
        /// List of audio input devices
        /// </summary>
        public List<string> CaptureDevices { get { return inputDevices; } }
        private List<string> outputDevices;
        /// <summary>
        /// List of audio output devices
        /// </summary>
        public List<string> PlaybackDevices { get { return outputDevices; } }
        private string currentCaptureDevice;
        private string currentPlaybackDevice;
        private bool testing = false;

        public event EventHandler OnSessionCreate;
        public event EventHandler OnSessionRemove;
        public delegate void VoiceConnectionChangeCallback( ConnectionState state );
        public event VoiceConnectionChangeCallback OnVoiceConnectionChange;
        public delegate void VoiceMicTestCallback(float level);
        public event VoiceMicTestCallback OnVoiceMicTest;
        public delegate void VoiceDevicesAvailableEvent(List<string> capture, List<string> playback);
        public event VoiceDevicesAvailableEvent OnVoiceDevicesAvailable;

        internal VoiceGateway( GridClient c )
        {
            Client = c;

            sessions = new Dictionary<string, VoiceSession>();
            position = new OpenMetaverse.Voice.VoiceGateway.VoicePosition();
            position.UpOrientation = new Vector3d(0.0, 1.0, 0.0);
            position.Velocity = new Vector3d(0.0, 0.0, 0.0);
            oldPosition = new Vector3d(0, 0, 0);
            oldAt = new Vector3d(1, 0, 0);

            slvoiceArgs = "-c";         // Cleanup old instances
            slvoiceArgs += " -ll -1";    // Min logging
            slvoiceArgs += " -i 0.0.0.0:" + daemonPort.ToString();
//            slvoiceArgs += " -lf " + control.instance.ClientDir;
        }

        public int Request(string action, string requestXML)
        {
            return connector.Request(action, requestXML);
        }

        /// <summary>
        /// Start up the Voice service.
        /// </summary>
        public void Start()
        {
            // Start the background thread
            if (posThread != null && posThread.IsAlive)
                posThread.Abort();
            posThread = new Thread(new ThreadStart(PositionThreadBody));
            posThread.Name = "VoicePositionUpdate";
            posThread.IsBackground = true;
            posRestart = new ManualResetEvent(false);
            posThread.Start();

            connector = new OpenMetaverse.Voice.VoiceGateway();

            Client.Network.EventQueueRunning += new EventHandler<EventQueueRunningEventArgs>(Network_EventQueueRunning);

            // Connection events
            connector.OnDaemonRunning +=
                 new OpenMetaverse.Voice.VoiceGateway.DaemonRunningCallback(connector_OnDaemonRunning);
            connector.OnDaemonCouldntRun +=
                new OpenMetaverse.Voice.VoiceGateway.DaemonCouldntRunCallback(connector_OnDaemonCouldntRun);
            connector.OnConnectorCreateResponse +=
                new OpenMetaverse.Voice.VoiceGateway.ConnectorCreateResponseCallback(connector_OnConnectorCreateResponse);
            connector.OnDaemonConnected +=
                new OpenMetaverse.Voice.VoiceGateway.DaemonConnectedCallback(connector_OnDaemonConnected);
            connector.OnDaemonCouldntConnect +=
                new OpenMetaverse.Voice.VoiceGateway.DaemonCouldntConnectCallback(connector_OnDaemonCouldntConnect);
            connector.OnAuxAudioPropertiesEvent +=
                new OpenMetaverse.Voice.VoiceGateway.AuxAudioPropertiesEventCallback(connector_OnAuxAudioPropertiesEvent);

            // Session events
           connector.OnSessionCreateResponse +=
                new OpenMetaverse.Voice.VoiceGateway.SessionCreateResponseCallback(connector_OnSessionCreateResponse);
            connector.OnSessionStateChangeEvent +=
                new OpenMetaverse.Voice.VoiceGateway.SessionStateChangeEventCallback(connector_OnSessionStateChangeEvent);
            connector.OnSessionAddedEvent +=
                new OpenMetaverse.Voice.VoiceGateway.SessionAddedEventCallback(connector_OnSessionAddedEvent);

            // Session Participants events
            connector.OnSessionParticipantStateChangeEvent +=
                new OpenMetaverse.Voice.VoiceGateway.SessionParticipantStateChangeEventCallback(connector_OnSessionParticipantStateChangeEvent);
            connector.OnSessionParticipantUpdatedEvent +=
                new OpenMetaverse.Voice.VoiceGateway.SessionParticipantUpdatedEventCallback(connector_OnSessionParticipantUpdatedEvent);
            connector.OnSessionParticipantAddedEvent +=
                new OpenMetaverse.Voice.VoiceGateway.SessionParticipantAddedEventCallback(connector_OnSessionParticipantAddedEvent);

            // Tuning events
            connector.OnAuxCaptureAudioStartResponse +=
                new OpenMetaverse.Voice.VoiceGateway.AuxCaptureAudioStartResponseCallback(connector_OnAuxCaptureAudioStartResponse);
            connector.OnAuxGetCaptureDevicesResponse +=
                new OpenMetaverse.Voice.VoiceGateway.AuxGetCaptureDevicesResponseCallback(connector_OnAuxGetCaptureDevicesResponse);
            connector.OnAuxGetRenderDevicesResponse +=
                new OpenMetaverse.Voice.VoiceGateway.AuxGetRenderDevicesResponseCallback(connector_OnAuxGetRenderDevicesResponse);

            // Account events
            connector.OnAccountLoginResponse +=
                new OpenMetaverse.Voice.VoiceGateway.AccountLoginResponseCallback(connector_OnAccountLoginResponse);

            Logger.Log("Voice initialized", Helpers.LogLevel.Info);

            // If voice provisioning capability is already available,
            // proceed with voice startup.   Otherwise the EventQueueRunning
            // event will do it.
            System.Uri vCap =
                 Client.Network.CurrentSim.Caps.CapabilityURI("ProvisionVoiceAccountRequest");
            if (vCap != null)
                RequestVoiceProvision(vCap);

        }

        internal void Stop()
        {
            Client.Network.EventQueueRunning -= new EventHandler<EventQueueRunningEventArgs>(Network_EventQueueRunning);

            if (connector != null)
            {
                // Connection events
                connector.OnDaemonRunning -=
                     new OpenMetaverse.Voice.VoiceGateway.DaemonRunningCallback(connector_OnDaemonRunning);
                connector.OnDaemonCouldntRun -=
                    new OpenMetaverse.Voice.VoiceGateway.DaemonCouldntRunCallback(connector_OnDaemonCouldntRun);
                connector.OnConnectorCreateResponse -=
                    new OpenMetaverse.Voice.VoiceGateway.ConnectorCreateResponseCallback(connector_OnConnectorCreateResponse);
                connector.OnDaemonConnected -=
                    new OpenMetaverse.Voice.VoiceGateway.DaemonConnectedCallback(connector_OnDaemonConnected);
                connector.OnDaemonCouldntConnect -=
                    new OpenMetaverse.Voice.VoiceGateway.DaemonCouldntConnectCallback(connector_OnDaemonCouldntConnect);
                connector.OnAuxAudioPropertiesEvent -=
                    new OpenMetaverse.Voice.VoiceGateway.AuxAudioPropertiesEventCallback(connector_OnAuxAudioPropertiesEvent);

                // Session events
                connector.OnSessionCreateResponse -=
                    new OpenMetaverse.Voice.VoiceGateway.SessionCreateResponseCallback(connector_OnSessionCreateResponse);
                connector.OnSessionStateChangeEvent -=
                    new OpenMetaverse.Voice.VoiceGateway.SessionStateChangeEventCallback(connector_OnSessionStateChangeEvent);
                connector.OnSessionAddedEvent -=
                    new OpenMetaverse.Voice.VoiceGateway.SessionAddedEventCallback(connector_OnSessionAddedEvent);

                // Session Participants events
               connector.OnSessionParticipantStateChangeEvent -=
                    new OpenMetaverse.Voice.VoiceGateway.SessionParticipantStateChangeEventCallback(connector_OnSessionParticipantStateChangeEvent);
                connector.OnSessionParticipantUpdatedEvent -=
                    new OpenMetaverse.Voice.VoiceGateway.SessionParticipantUpdatedEventCallback(connector_OnSessionParticipantUpdatedEvent);
                connector.OnSessionParticipantAddedEvent -=
                    new OpenMetaverse.Voice.VoiceGateway.SessionParticipantAddedEventCallback(connector_OnSessionParticipantAddedEvent);

                // Tuning events
                connector.OnAuxCaptureAudioStartResponse -=
                    new OpenMetaverse.Voice.VoiceGateway.AuxCaptureAudioStartResponseCallback(connector_OnAuxCaptureAudioStartResponse);
                connector.OnAuxGetCaptureDevicesResponse -=
                    new OpenMetaverse.Voice.VoiceGateway.AuxGetCaptureDevicesResponseCallback(connector_OnAuxGetCaptureDevicesResponse);
                connector.OnAuxGetRenderDevicesResponse -=
                    new OpenMetaverse.Voice.VoiceGateway.AuxGetRenderDevicesResponseCallback(connector_OnAuxGetRenderDevicesResponse);

                // Account events
                connector.OnAccountLoginResponse -=
                    new OpenMetaverse.Voice.VoiceGateway.AccountLoginResponseCallback(connector_OnAccountLoginResponse);
            }

            // Stop the background thread
            if (posThread != null)
            {
                PosUpdating(false);

                if (posThread.IsAlive)
                    posThread.Abort();
                posThread = null;
            }

            // Close all sessions
            foreach (VoiceSession s in sessions.Values)
            {
                s.Close();
            }

            if (connector != null)
            {
                connector.ConnectorInitiateShutdown(connectionHandle);
                connector.StopDaemon();
            }
        }

        /// <summary>
        /// Cleanup oject resources
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        internal string GetVoiceDaemonPath()
        {
            if (Environment.OSVersion.Platform != PlatformID.MacOSX &&
                Environment.OSVersion.Platform != PlatformID.Unix)
            {
                string progFiles;
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles(x86)")))
                {
                    progFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                }
                else
                {
                    progFiles = Environment.GetEnvironmentVariable("ProgramFiles");
                }

                return Path.Combine(progFiles, @"SecondLife\SLVoice.exe");
            }
            else
            {
                //TODO return Path.Combine(progFiles, @"SecondLife\SLVoice");
            }

            return string.Empty;
        }

        void RequestVoiceProvision( System.Uri cap )
        {
            OpenMetaverse.Http.CapsClient capClient =
                new OpenMetaverse.Http.CapsClient(cap);
            capClient.OnComplete +=
                new OpenMetaverse.Http.CapsClient.CompleteCallback(cClient_OnComplete);
            OSD postData = new OSD();

            // STEP 0
            Logger.Log("Requesting voice capability", Helpers.LogLevel.Info);
            capClient.BeginGetResponse(postData, OSDFormat.Xml, 10000);
        }

        /// <summary>
        /// Request voice cap when changeing regions
        /// </summary>
        /// <param name="simulator"></param>
        void Network_EventQueueRunning(object sender, EventQueueRunningEventArgs e)
        {
            // We only care about the sim we are in.
            if (e.Simulator != Client.Network.CurrentSim)
                return;

            // Did we provision voice login info?
            if (string.IsNullOrEmpty(voiceUser))
            {
                // The startup steps are
                //  0. Get voice account info
                //  1. Start Daemon
                //  2. Create TCP connection
                //  3. Create Connector
                //  4. Account login
                //  5. Create session

                // Get the voice provisioning data
                System.Uri vCap =
                    Client.Network.CurrentSim.Caps.CapabilityURI("ProvisionVoiceAccountRequest");

                // Do we have voice capability?
                if (vCap == null)
                {
                    Logger.Log("Null voice capability after event queue running", Helpers.LogLevel.Warning);
                }
                else
                {
                    RequestVoiceProvision(vCap);
                }

                return;
            }
            else
            {
                // Change voice session for this region.
                ParcelChanged();
            }
        }


       #region Partcipants
        void connector_OnSessionParticipantStateChangeEvent(
            string SessionHandle,
            int StatusCode,
            string StatusString,
            OpenMetaverse.Voice.VoiceGateway.ParticipantState State,
            string ParticipantURI,
            string AccountName,
            string DisplayName,
            OpenMetaverse.Voice.VoiceGateway.ParticipantType ParticipantType)
        {

        }

        void connector_OnSessionParticipantUpdatedEvent(string sessionHandle,
            string URI,
            bool isMuted,
            bool isSpeaking,
            int volume,
            float energy)
        {
            VoiceSession s = FindSession(sessionHandle, false);
            if (s == null) return;
            s.ParticipantUpdate(URI, isMuted, isSpeaking, volume, energy);
        }

        public string SIPFromUUID(UUID id)
        {
            return "sip:" +
                nameFromID(id) +
                "@" +
                sipServer;
        }

        private static string nameFromID(UUID id)
        {
            string result = null;

            if (id==UUID.Zero)
                return result;

            // Prepending this apparently prevents conflicts with reserved names inside the vivox and diamondware code.
	        result = "x";
	
	        // Base64 encode and replace the pieces of base64 that are less compatible 
	        // with e-mail local-parts.
	        // See RFC-4648 "Base 64 Encoding with URL and Filename Safe Alphabet"
            byte[] encbuff = id.GetBytes();
            result += Convert.ToBase64String(encbuff);
            result = result.Replace('+', '-');
            result = result.Replace('/', '_');
		
        	return result;
       }

        void connector_OnSessionParticipantAddedEvent(
                string SessionGroupHandle,
                string SessionHandle,
                string ParticipantUri,
                string AccountName,
                string DisplayName,
                OpenMetaverse.Voice.VoiceGateway.ParticipantType type,
                string Application )
        {
            VoiceSession s = FindSession(sessionHandle, false);
            if (s == null) return;
            s.AddParticipant( ParticipantUri );
        }

        void connector_OnSessionParticipantRemovedEvent(
                string SessionGroupHandle,
                string SessionHandle,
                string ParticipantUri,
                string AccountName,
                string Reason )
        {
            VoiceSession s = FindSession(sessionHandle, false);
            if (s == null) return;
            s.RemoveParticipant(ParticipantUri);
        }
       #endregion

        #region Sessions
        void connector_OnSessionAddedEvent(string sessionGroupHandle,
            string newSessionHandle,
            string URI,
            bool isChannel,
            bool isIncoming)
        {
           sessionHandle = newSessionHandle;

            // Create our session context.
            VoiceSession s = FindSession(sessionHandle, true);
            s.RegionName = regionName;

            Logger.Log("Added voice session in " + regionName, Helpers.LogLevel.Info);
            spatialSession = s;

            // Tell any user-facing code.
            if (OnSessionCreate != null)
                OnSessionCreate(s, null);
        }

        /// <summary>
        /// Handle session creation
        /// </summary>
        void connector_OnSessionCreateResponse(int ReturnCode,
            int StatusCode, string StatusString,
            string SessionHandle,
            OpenMetaverse.Voice.VoiceGateway.VoiceRequest Request)
        {
            if (StatusCode == 0) return;
        }

        /// <summary>
        /// Handle a change in session state
        /// </summary>
        /// <param name="SessionHandle"></param>
        /// <param name="StatusCode"></param>
        /// <param name="StatusString"></param>
        /// <param name="State"></param>
        /// <param name="URI"></param>
        /// <param name="IsChannel"></param>
        /// <param name="ChannelName"></param>
        void connector_OnSessionStateChangeEvent(string SessionHandle,
            int StatusCode,
            string StatusString,
            OpenMetaverse.Voice.VoiceGateway.SessionState State,
            string URI,
            bool IsChannel,
            string ChannelName)
        {
            VoiceSession s;

            switch (State)
            {
                case OpenMetaverse.Voice.VoiceGateway.SessionState.Connected:
                    s = FindSession(SessionHandle, true);
                    sessionHandle = SessionHandle;
                    s.RegionName = regionName;
                    spatialSession = s;

                    Logger.Log("Voice connected in " + regionName, Helpers.LogLevel.Info);
                    // Tell any user-facing code.
                    if (OnSessionCreate != null)
                        OnSessionCreate(s, null);
                    break;

                case OpenMetaverse.Voice.VoiceGateway.SessionState.Disconnected:
                    s = FindSession(sessionHandle, false);
                    sessions.Remove(sessionHandle);

                    if (s != null)
                    {
                        Logger.Log("Voice disconnected in " + s.RegionName, Helpers.LogLevel.Info);

                        // Inform interested parties
                        if (OnSessionRemove != null)
                            OnSessionRemove(s, null);

                        if (s == spatialSession)
                            spatialSession = null;
                    }

                    // The previous session is now ended.  Check for a new one and
                    // start it going.
                    if (nextParcelCap != null)
                    {
                        currentParcelCap = nextParcelCap;
                        nextParcelCap = null;
                        RequestParcelInfo(currentParcelCap);
                    }
                    break;
            }
            

        }
        
        /// <summary>
        /// Close a voice session
        /// </summary>
        /// <param name="sessionHandle"></param>
        internal void CloseSession(string sessionHandle)
        {
            if (!sessions.ContainsKey(sessionHandle))
                return;

            ReportConnectionState(ConnectionState.AccountLogin);

            // Clean up spatial pointers.
            VoiceSession s = sessions[sessionHandle];
            if (s.IsSpatial)
            {
                spatialSession = null;
                currentParcelCap = null;
            }

            // Remove this session from the master session list
            sessions.Remove(sessionHandle);

            // Let any user-facing code clean up.
            if (OnSessionRemove != null)
                OnSessionRemove(s, null);

            // Tell SLVoice to clean it up as well.
            connector.SessionTerminate(sessionHandle);
        }

        /// <summary>
        /// Locate a Session context from its handle
        /// </summary>
        /// <param name="sessionHandle"></param>
        /// <returns></returns>
        /// <remarks>Creates the session context if it does not exist.</remarks>
        VoiceSession FindSession(string sessionHandle, bool make)
        {
            if (sessions.ContainsKey(sessionHandle))
                return sessions[sessionHandle];

            if (!make) return null;

            // Create a new session and add it to the sessions list.
            VoiceSession s = new VoiceSession( this, sessionHandle);
            sessions.Add(sessionHandle, s);
            return s;
        }

#endregion

        #region MinorResponses

        void connector_OnAuxCaptureAudioStartResponse(int ReturnCode, int StatusCode,
            string StatusString, OpenMetaverse.Voice.VoiceGateway.VoiceRequest Request)
        {

        }

        void connector_OnAuxAudioPropertiesEvent(bool MicIsActive,
            float MicEnergy, float MicVolume, float SpeakerVolume)
        {
            if (OnVoiceMicTest != null)
                OnVoiceMicTest(MicEnergy);
        }

        #endregion

        private void ReportConnectionState(ConnectionState s)
        {
            if (OnVoiceConnectionChange == null) return;

            OnVoiceConnectionChange(s);
        }

        /// <summary>
        /// Handle completion of main voice cap request.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="result"></param>
        /// <param name="error"></param>
        void cClient_OnComplete(OpenMetaverse.Http.CapsClient client,
            OpenMetaverse.StructuredData.OSD result,
            Exception error)
        {
            if (error != null)
            {
                Logger.Log("Voice cap error "+error.Message, Helpers.LogLevel.Error);
                return;
            }

            Logger.Log("Voice provisioned", Helpers.LogLevel.Info);
            ReportConnectionState(ConnectionState.Provisioned);

            OpenMetaverse.StructuredData.OSDMap pMap = result as OpenMetaverse.StructuredData.OSDMap;

            // We can get back 4 interesting values:
            //      voice_sip_uri_hostname
            //      voice_account_server_name   (actually a full URI)
            //      username
            //      password
            if (pMap.ContainsKey("voice_sip_uri_hostname"))
                sipServer = pMap["voice_sip_uri_hostname"].AsString();
            if (pMap.ContainsKey("voice_account_server_name"))
                acctServer = pMap["voice_account_server_name"].AsString();
            voiceUser = pMap["username"].AsString();
            voicePassword = pMap["password"].AsString();

            // Start the SLVoice daemon
            slvoicePath = GetVoiceDaemonPath();

            // Test if the executable exists
            if (!System.IO.File.Exists(slvoicePath))
            {
                Logger.Log("SLVoice is missing", Helpers.LogLevel.Error);
                return;
            }

            // STEP 1
            connector.StartDaemon(slvoicePath, slvoiceArgs);
        }

        #region Daemon
        void connector_OnDaemonCouldntConnect()
        {
            Logger.Log("No voice daemon connect", Helpers.LogLevel.Error);
        }

        void connector_OnDaemonCouldntRun()
        {
            Logger.Log("Daemon not started", Helpers.LogLevel.Error);
        }

        /// <summary>
        /// Daemon has started so connect to it.
        /// </summary>
        void connector_OnDaemonRunning()
        {
            connector.OnDaemonRunning -=
                new OpenMetaverse.Voice.VoiceGateway.DaemonRunningCallback(connector_OnDaemonRunning);

            Logger.Log("Daemon started", Helpers.LogLevel.Info);
            ReportConnectionState(ConnectionState.DaemonStarted);

            // STEP 2
            connector.ConnectToDaemon(daemonNode, daemonPort);
        }
 
        /// <summary>
        /// The daemon TCP connection is open.
        /// </summary>
        /// <param name="ReturnCode"></param>
        /// <param name="VersionID"></param>
        /// <param name="StatusCode"></param>
        /// <param name="StatusString"></param>
        /// <param name="ConnectorHandle"></param>
        /// <param name="Request"></param>
        void connector_OnDaemonConnected()
        {
            Logger.Log("Daemon connected", Helpers.LogLevel.Info);
            ReportConnectionState(ConnectionState.DaemonConnected);

            // The connector is what does the logging.
            OpenMetaverse.Voice.VoiceGateway.VoiceLoggingSettings vLog =
                new OpenMetaverse.Voice.VoiceGateway.VoiceLoggingSettings();
//TODO            vLog.Folder = control.instance.ClientDir;
            vLog.Enabled = true;
            vLog.FileNamePrefix = "RadegastVoice";
            vLog.FileNameSuffix = ".log";
            vLog.LogLevel = 5;

            // STEP 3
            int reqId = connector.ConnectorCreate(
                "V2 SDK",       // Magic value keeps SLVoice happy
                acctServer,     // Account manager server
                30000, 30099,   // port range
                vLog);
            if (reqId < 0)
            {
                Logger.Log("No voice connector request", Helpers.LogLevel.Error);
            }
        }

        /// <summary>
        /// Handle creation of the Connector.
        /// </summary>
        /// <param name="ReturnCode"></param>
        /// <param name="VersionID"></param>
        /// <param name="StatusCode"></param>
        /// <param name="StatusString"></param>
        /// <param name="ConnectorHandle"></param>
        /// <param name="Request"></param>
        void connector_OnConnectorCreateResponse(int ReturnCode,
            string VersionID, int StatusCode,
            string StatusString, string ConnectorHandle,
            OpenMetaverse.Voice.VoiceGateway.VoiceRequest Request)
        {
            Logger.Log("Voice daemon protocol started "+StatusString, Helpers.LogLevel.Info);

            connectionHandle = ConnectorHandle;

            if (StatusCode != 0)
                return;

            // Request the available devices, to populate the GUI for choosing.
            connector.AuxGetCaptureDevices();
            connector.AuxGetRenderDevices();

            // STEP 4
            connector.AccountLogin(
                connectionHandle,
                voiceUser,
                voicePassword,
                "VerifyAnswer",   // This can also be "AutoAnswer"
                "",             // Default account management server URI
                10,            // Throttle state changes
                true);          // Enable buddies and presence
        }
        #endregion

        void connector_OnAccountLoginResponse(int ReturnCode, int StatusCode, string StatusString,
            string AccountHandle,
            OpenMetaverse.Voice.VoiceGateway.VoiceRequest Request)
        {
            Logger.Log("Account Login "+StatusString, Helpers.LogLevel.Info );
            accountHandle = AccountHandle;
            ReportConnectionState(ConnectionState.AccountLogin);
            ParcelChanged();
        }

        #region Audio devices
        /// <summary>
        /// Handle response to audio output device query
        /// </summary>
        /// <param name="ReturnCode"></param>
        /// <param name="StatusCode"></param>
        /// <param name="StatusString"></param>
        /// <param name="RenderDevices"></param>
        /// <param name="CurrentRenderDevice"></param>
        /// <param name="Request"></param>
        void connector_OnAuxGetRenderDevicesResponse(int ReturnCode,
           int StatusCode,
           string StatusString,
           List<string> RenderDevices,
           string CurrentRenderDevice,
           OpenMetaverse.Voice.VoiceGateway.VoiceRequest Request)
        {
            outputDevices = RenderDevices;
            if (inputDevices != null && OnVoiceDevicesAvailable != null)
                OnVoiceDevicesAvailable(inputDevices, outputDevices);
        }

        /// <summary>
        /// Handle response to audio input device query
        /// </summary>
        /// <param name="ReturnCode"></param>
        /// <param name="StatusCode"></param>
        /// <param name="StatusString"></param>
        /// <param name="CaptureDevices"></param>
        /// <param name="CurrentCaptureDevice"></param>
        /// <param name="Request"></param>
        void connector_OnAuxGetCaptureDevicesResponse(int ReturnCode,
            int StatusCode, string StatusString,
            List<string> CaptureDevices,
            string CurrentCaptureDevice,
            OpenMetaverse.Voice.VoiceGateway.VoiceRequest Request)
        {
            inputDevices = CaptureDevices;
            if (outputDevices != null && OnVoiceDevicesAvailable != null)
                OnVoiceDevicesAvailable(inputDevices, outputDevices);
        }

        public string CaptureDevice
        {
            get { return currentCaptureDevice; }
            set
            {
                currentCaptureDevice = value;
                connector.AuxSetCaptureDevice(value);
            }
        }
        public string PlaybackDevice
        {
            get { return currentPlaybackDevice; }
            set
            {
                currentPlaybackDevice = value;
                connector.AuxSetRenderDevice(value);
            }
        }

                internal int MicLevel
        {
            set
            {
                connector.ConnectorSetLocalMicVolume(connectionHandle, value);
            }
        }
        internal int SpkrLevel
        {
            set
            {
                connector.ConnectorSetLocalSpeakerVolume(connectionHandle, value);
            }
        }

        internal bool MicMute
        {
            set
            {
                connector.ConnectorMuteLocalMic(connectionHandle, value);
            }
        }

        internal bool SpkrMute
        {
            set
            {
                connector.ConnectorMuteLocalSpeaker(connectionHandle, value);
            }
        }

        /// <summary>
        /// Set audio test mode
        /// </summary>
        /// <param name="on"></param>
        public bool TestMode
        {
            get { return testing;  }
            set
            {
                testing = value;
                if (testing)
                {
                    if (spatialSession != null)
                    {
                        spatialSession.Close();
                        spatialSession = null;
                    }
                    connector.AuxCaptureAudioStart(0);
                }
                else
                {
                    connector.AuxCaptureAudioStop();
                    ParcelChanged();
                }
            }
        }
#endregion




        /// <summary>
        /// Set voice channel for new parcel
        /// </summary>
        ///
        internal void ParcelChanged()
        {
            // Get the capability for this parcel.
            Caps c = Client.Network.CurrentSim.Caps;
            System.Uri pCap = c.CapabilityURI("ParcelVoiceInfoRequest");

            if (pCap == null)
            {
                Logger.Log("Null voice capability", Helpers.LogLevel.Error);
                return;
            }
            
            if (pCap == currentParcelCap)
            {
                // Parcel has not changed, so nothing to do.
                return;
            }

            // Parcel has changed.  If we were already in a spatial session, we have to close it first.
            if (spatialSession != null)
            {
                nextParcelCap = pCap;
                CloseSession( spatialSession.SessionHandle );
            }

            // Not already in a session, so can start the new one.
            RequestParcelInfo(pCap);
        }

        private OpenMetaverse.Http.CapsClient parcelCap;

        /// <summary>
        /// Request info from a parcel capability Uri.
        /// </summary>
        /// <param name="cap"></param>
        
        void RequestParcelInfo(Uri cap)
        {
            Logger.Log("Requesting region voice info", Helpers.LogLevel.Info);

            parcelCap = new OpenMetaverse.Http.CapsClient(cap);
            parcelCap.OnComplete +=
                new OpenMetaverse.Http.CapsClient.CompleteCallback(pCap_OnComplete);
            OSD postData = new OSD();

            currentParcelCap = cap;
            parcelCap.BeginGetResponse(postData, OSDFormat.Xml, 10000);
        }

        /// <summary>
        /// Receive parcel voice cap
        /// </summary>
        /// <param name="client"></param>
        /// <param name="result"></param>
        /// <param name="error"></param>
        void pCap_OnComplete(OpenMetaverse.Http.CapsClient client,
            OpenMetaverse.StructuredData.OSD result,
            Exception error)
        {
            parcelCap.OnComplete -=
                new OpenMetaverse.Http.CapsClient.CompleteCallback(pCap_OnComplete);
            parcelCap = null;

            if (error != null)
            {
                Logger.Log("Region voice cap "+error.Message, Helpers.LogLevel.Error);
                return;
            }

            OpenMetaverse.StructuredData.OSDMap pMap = result as OpenMetaverse.StructuredData.OSDMap;

            regionName = pMap["region_name"].AsString();
            ReportConnectionState(ConnectionState.RegionCapAvailable);

            if (pMap.ContainsKey("voice_credentials"))
            {
                OpenMetaverse.StructuredData.OSDMap cred =
                    pMap["voice_credentials"] as OpenMetaverse.StructuredData.OSDMap;

                if (cred.ContainsKey("channel_uri"))
                    spatialUri = cred["channel_uri"].AsString();
                if (cred.ContainsKey("channel_credentials"))
                    spatialCredentials = cred["channel_credentials"].AsString();
            }

            if (spatialUri == null || spatialUri == "")
            {
                // "No voice chat allowed here");
                return;
            }

            Logger.Log("Voice connecting for region " + regionName, Helpers.LogLevel.Info);

            // STEP 5
            int reqId = connector.SessionCreate(
                accountHandle,
                spatialUri, // uri
                "", // Channel name seems to be always null
                spatialCredentials, // spatialCredentials, // session password
                true,   // Join Audio
                false,   // Join Text
                "");
            if (reqId < 0)
            {
                Logger.Log("Voice Session ReqID " + reqId.ToString(), Helpers.LogLevel.Error);
            }
        }


        internal class SessionListItem : System.Windows.Forms.ListViewItem
        {
//            OpenMetaverse.Voice.VoiceParticipant participant;
        }

#region Location Update
        /// <summary>
        /// Tell Vivox where we are standing
        /// </summary>
        /// <remarks>This has to be called when we move or turn.</remarks>
        internal void UpdatePosition(AgentManager self)
        {
            // Get position in Global coordinates
            Vector3d OMVpos = new Vector3d(self.GlobalPosition);

            // Do not send trivial updates.
            if (OMVpos.ApproxEquals(oldPosition, 1.0))
                return;

            oldPosition = OMVpos;

            // Convert to the coordinate space that Vivox uses
            // OMV X is East, Y is North, Z is up
            // VVX X is East, Y is up, Z is South
            position.Position = new Vector3d(OMVpos.X, OMVpos.Z, -OMVpos.Y);

            // TODO Rotate these two vectors

            // Get azimuth from the facing Quaternion.
            // By definition, facing.W = Cos( angle/2 )
            double angle = 2.0 * Math.Acos(self.Movement.BodyRotation.W);

            position.LeftOrientation = new Vector3d(-1.0, 0.0, 0.0);
            position.AtOrientation = new Vector3d((float)Math.Acos(angle), 0.0, -(float)Math.Asin(angle));

            connector.SessionSet3DPosition(
                sessionHandle,
                position,
                position);
        }

        /// <summary>
        /// Start and stop updating out position.
        /// </summary>
        /// <param name="go"></param>
        internal void PosUpdating( bool go )
        {
            if (go)
                posRestart.Set();
            else
                posRestart.Reset();
        }

        private void PositionThreadBody()
        {
            while (true)
            {
                posRestart.WaitOne();
                Thread.Sleep(1500);
                UpdatePosition(Client.Self);
            }
        }
#endregion

    }
}
