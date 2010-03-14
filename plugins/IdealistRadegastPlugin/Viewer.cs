//#define DebugObjectPipeline
#define DebugTexturePipeline

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using System.Threading;
using IdealistViewer;
using log4net;
using IrrlichtNETCP;
using IrrlichtNETCP.Extensions;
using OpenMetaverse;
using Radegast;
using IConfig = Nini.Config.IConfig;
using IConfigSource = Nini.Config.IConfigSource;
using IniConfigSource = Nini.Config.IniConfigSource;
using Console = IdealistViewer.Console;
using MainConsole=IdealistViewer.MainConsole;
using Renderer = IdealistRadegastPlugin.RaegastRenderer;

namespace IdealistRadegastPlugin
{
    /// <summary>
    /// Viewer binds the managers together through centralized main loop and event handling.
    /// </summary>
    public class RadegastViewer : IdealistViewer.Viewer
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Viewer Components

        //public Renderer Renderer;
        //public UserInterfaceManager UserInterface;
        //public INetworkInterface NetworkInterface;
        //public Console Console;

        //public TextureManager TextureManager;
        //public MeshManager MeshManager;
        //public AnimationManager AnimationManager;
        //public VSceneGraph SceneGraph;
        //public ITerrainManager TerrainManager;

        //public AvatarController AvatarController;
        //public CameraController CameraController;

        private Thread m_consoleThread;
        private Thread m_mainThread;
        private Thread m_formsThread;

        #endregion

        #region Configuration Fields

        //public IdealistViewerConfigSource m_configSource = null;

        ///// <summary>
        ///// Flag to allow viewer to run. Set to false with File->Quit
        ///// </summary>
        //public bool Running = true;
        ///// <summary>
        ///// Configuration option to control backface culling.
        ///// </summary>
        //public bool BackFaceCulling = true;
        ///// <summary>
        ///// Configuration option to control foliage processing.
        ///// </summary>
        //public bool ProcessFoliage = true;
        ///// <summary>
        ///// Configuration option to load the textures
        ///// </summary>
        //public bool LoadTextures = true;
        ///// <summary>
        ///// Configuration option to enable meshing sculpted prims
        ///// </summary>
        //public bool MeshSculpties = true;
        ///// <summary>
        ///// Configuration option to represent the avatar mesh.
        ///// </summary>
        //public string AvatarMesh = "sydney.md2";
        ///// <summary>
        ///// Configuration option.  Texture to apply to avatarMesh
        ///// </summary>
        //public string AvatarMaterial = "sydney.BMP";
        ///// <summary>
        ///// Configuration option. Scale factor for the avatar mesh
        ///// </summary>
        //public float AvatarScale = 0.035f;
        ///// <summary>
        ///// If the neighbor returned is a 0 ulong region handle, use this one for testing
        ///// </summary> 
        //public ulong TestNeighbor = 1099511628032256;
        /// <summary>
        /// Process avatar modifications every second scene update.
        /// </summary>
        private const int m_avatarModificationProcessingRate = 2;
        /// <summary>
        /// Process object modifications every second scene update.
        /// </summary>
        private int m_objectModificationProcessingRate = 2; 
        /// <summary>
        /// The base time period for scene updates.
        /// </summary>
        private const int m_baseSceneUpdateTimePeriod = 100;
        /// <summary>
        /// Maximum frames per second to limit CPU usage.
        /// </summary>
        private int m_maximumFramesPerSecond = 60;
        /// <summary>
        /// limit FPS to m_maximumFramesPerSecond if true
        /// </summary>
        private bool m_limitFramesPerSecond = true;

        #endregion

        #region Viewer Internal State Fields

        /// <summary>
        /// Server version information.  Usually VersionInfo + information about svn revision, operating system, etc.
        /// </summary>
     //   public string Version;
        /// <summary>
        /// Time at which this server was started
        /// </summary>
    //    public DateTime StartupTime;
        /// <summary>
        /// Record the initial startup directory for info purposes
        /// </summary>
    //    public string StartupDirectory = Environment.CurrentDirectory;

        /// <summary>
        /// Held Controls
        /// </summary>
        private bool m_ctrlPressed = false;
        private bool m_shiftPressed = false;
     //   private bool m_appPressed = false;
        private bool m_leftMousePressed = false;
        private bool m_rightMousePressed = false;
        //private bool m_middleMousePressed = false;

        /// <summary>
        /// Last window or screen size.
        /// </summary>
        private Dimension2D m_lastScreenSize;
        
        /// <summary>
        /// Stored Mouse cordinates used to determine change.
        /// </summary>
        private int m_oldMouseX = 0;
        private int m_oldMouseY = 0;

        /// <summary>
        /// Window size.
        /// </summary>
        //public int WindowWidth = 1024;
        //public int WindowHeight = 768;
        //public float WindowAspect = 1024 / (float)768;

        /// <summary>
        /// Target Position of the camera
        /// </summary>
        private Vector3 m_lastTargetPos = Vector3.Zero;
        /// <summary>
        /// List of held keys.  Used to process multiple keypresses simulataniously
        /// </summary>
        private List<KeyCode> m_pressedKeys = new List<KeyCode>();
        /// <summary>
        /// System tick count at the beginning of the loop.
        /// </summary>
        private int m_lastTickCount = 0;
        /// <summary>
        /// The time in milliseconds since last scene update occured.
        /// </summary>
        private int m_timeSinceLastSceneUpdate = 0;
        /// <summary>
        /// Number of scene updates occured since startup.
        /// </summary>
        private int m_sceneUpdateCounter = 0;
        /// <summary>
        /// Time dilation between client and server.
        /// </summary>
        private float m_timeDilation = 0;

        readonly private RadegastInstance RInstance;
        readonly public IdealistUserControl ViewerControl;
        private IrrlichtDevice Device;

        #endregion

        #region Startup

        public RadegastViewer(RadegastInstance inst, IConfigSource iconfig, IdealistUserControl control):base(iconfig)
        
        {
            RInstance = inst;
            m_configSource = new IdealistViewerConfigSource();
            m_configSource.Source = new IniConfigSource();
            ViewerControl = control;


            string iniconfig = Path.Combine(Util.configDir(), "IdealistViewer.ini");
            if (File.Exists(iniconfig))
            {
                m_configSource.Source.Merge(new IniConfigSource(iniconfig));
            }
            m_configSource.Source.Merge(iconfig);
            StartupTime = DateTime.UtcNow;
        }

        private GUISkin skin;
        private Color skincolor;

        /// <summary>
        /// Performs initialisation of the scene, such as loading configuration from disk.
        /// </summary>
        public override void Startup()
        {
            m_consoleThread = Thread.CurrentThread;
            if (true)
            {
                m_mainThread = new Thread(new ThreadStart(() =>
                {
                    ViewerControl.InitializeComponent();
                    Device = new IrrlichtDevice(DriverType.OpenGL,
                                                new Dimension2D(WindowWidth,
                                                                WindowHeight),
                                                32, false, true, true, true
                        );

                    StartupComponents();
                    MainLoop(skin, skincolor);
                }));
            }
            else
            {
                m_mainThread = new Thread(new ThreadStart(() =>
                {
                    ViewerControl.InitializeComponent();
                    Device = new IrrlichtDevice(DriverType.OpenGL,
                                                new Dimension2D(WindowWidth,
                                                                WindowHeight),
                                                32, false, true, true, true,

                        ViewerControl.IdealistView.Handle
                        );

                    StartupComponents();
                    MainLoop(skin, skincolor);
                }));

            }


            m_mainThread.Start();
        }

        /// <summary>
        /// Must be overriden by child classes for their own server specific startup behaviour.
        /// </summary>
        protected override void StartupComponents()
        {
            m_log.Info("[STARTUP]: Beginning startup processing");

            Version = Util.EnhanceVersionInformation();

            m_log.Info("[STARTUP]: Version: " + Version + "\n");

            Console = new Console(this,"Region", this);
            IConfig cnf = m_configSource.Source.Configs["Startup"];
            string loginURI = "http://127.0.0.1:9000/";
            string firstName = string.Empty;
            string lastName = string.Empty;
            string password = string.Empty;
            string startlocation = "";
            bool loadtextures = true;
            bool multipleSims = false;

            if (cnf != null)
            {
                loginURI = cnf.GetString("login_uri", "");
                firstName = cnf.GetString("first_name", "test");
                lastName = cnf.GetString("last_name", "user");
                password = cnf.GetString("pass_word", "nopassword");
                loadtextures = cnf.GetBoolean("load_textures", true);
                MeshSculpties = cnf.GetBoolean("mesh_sculpties", MeshSculpties);
                BackFaceCulling = cnf.GetBoolean("backface_culling", BackFaceCulling);
                AvatarMesh = cnf.GetString("avatar_mesh", AvatarMesh);
                AvatarMaterial = cnf.GetString("avatar_material", AvatarMaterial);
                AvatarScale = cnf.GetFloat("avatar_scale", AvatarScale);
                startlocation = cnf.GetString("start_location", "");
                multipleSims = cnf.GetBoolean("multiple_sims", multipleSims);
                ProcessFoliage = cnf.GetBoolean("process_foliage", ProcessFoliage);
                m_limitFramesPerSecond = cnf.GetBoolean("limitfps", m_limitFramesPerSecond);
            }
            LoadTextures = loadtextures;
            MainConsole.Instance = Console;

            // Initialize LibOMV
            if (NetworkInterface == null) NetworkInterface = new RadegastNetworkModule(RInstance);
            NetworkInterface.MultipleSims = multipleSims;
            NetworkInterface.OnLandUpdate += OnNetworkLandUpdate;
            NetworkInterface.OnConnected += OnNetworkConnected;
            NetworkInterface.OnObjectAdd += OnNetworkObjectAdd;
            NetworkInterface.OnSimulatorConnected += OnNetworkSimulatorConnected;
            NetworkInterface.OnObjectUpdate += OnNetworkObjectUpdate;
            NetworkInterface.OnObjectRemove += OnNetworkObjectRemove;
            NetworkInterface.OnAvatarAdd += OnNetworkAvatarAdd;
            //NetworkInterface.OnChat +=new NetworkChatDelegate(OnNetworkChat);
            //NetworkInterface.OnFriendsListUpdate +=new NetworkFriendsListUpdateDelegate(OnNetworkFriendsListChange);

            //NetworkInterface.Login(loginURI, firstName + " " + lastName, password, startlocation);

            // Startup the GUI
            Renderer = new RaegastRenderer(this,Device);
            Renderer.Startup();

            GUIFont defaultFont = Renderer.GuiEnvironment.GetFont("defaultfont.png");

            skin = Renderer.GuiEnvironment.Skin;
            skin.Font = defaultFont;
            skincolor = skin.GetColor(GuiDefaultColor.Face3D);
            skincolor.A = 255;
            skin.SetColor(GuiDefaultColor.Face3D, skincolor);
            skincolor = skin.GetColor(GuiDefaultColor.Shadow3D);
            skincolor.A = 255;
            skin.SetColor(GuiDefaultColor.Shadow3D, skincolor);
            // Set up event handler for the GUI window events.
            Renderer.Device.OnEvent += new OnEventDelegate(OnDeviceEvent);

            Renderer.Device.Resizeable = true;

            MeshManager = new MeshManager(Renderer.SceneManager.MeshManipulator, Renderer.Device);

            SceneGraph = new VSceneGraph(this);

            // Set up the picker.
            SceneGraph.TrianglePicker = new TrianglePickerMapper(Renderer.SceneManager.CollisionManager);
            SceneGraph.TriangleSelector = Renderer.SceneManager.CreateMetaTriangleSelector();

            // Only create a texture manager if the user configuration option is enabled for downloading textures
            if (LoadTextures)
            {
                TextureManager = new TextureManager(this,Renderer.Device, Renderer.Driver, SceneGraph.TrianglePicker, SceneGraph.TriangleSelector, "IdealistCache", NetworkInterface);
                TextureManager.OnTextureLoaded += OnNetworkTextureDownloaded;
            }

            AvatarController = new AvatarController(NetworkInterface, null);

            TerrainManager = ModuleManager.GetTerrainManager(this, m_configSource);

            Renderer.SceneManager.SetAmbientLight(new Colorf(1f, 0.2f, 0.2f, 0.2f));

            // This light simulates the sun
            //SceneNode light2 = Renderer.SceneManager.AddLightSceneNode(Renderer.SceneManager.RootSceneNode, new Vector3D(0, 255, 0), new Colorf(0f, 0.5f, 0.5f, 0.5f), 250, -1);
            SceneNode light2 = Renderer.SceneManager.AddLightSceneNode(Renderer.SceneManager.RootSceneNode, new Vector3D(0, 255, 0), new Colorf(0f, 0.6f, 0.6f, 0.6f), 512, -1);

            // Fog is on by default, this line disables it.
            //Renderer.SceneManager.VideoDriver.SetFog(new Color(0, 255, 255, 255), false, 9999, 9999, 0, false, false);
            float fogBrightness = 0.8f;
            Renderer.SceneManager.VideoDriver.SetFog(new Color(0, (int)(0.5f * 255 * fogBrightness), (int)(0.5f * 255 * fogBrightness), (int)(1.0f * 255 * fogBrightness)), true, 50, 100, 0, true, true);

            //ATMOSkySceneNode skynode = new ATMOSkySceneNode(Renderer.Driver.GetTexture("irrlicht2_up.jpg"), null, Renderer.SceneManager, 100, -1);
            
            //ATMOSphere atmosphere = new ATMOSphere(Renderer.Device.Timer, null, Renderer.SceneManager, -1);
            //atmosphere.SkyTexture = Renderer.Driver.GetTexture("irrlicht2_up.jpg");

            
            Renderer.Driver.SetTextureFlag(TextureCreationFlag.CreateMipMaps, false);
            bool Broken = false;
            if (!Broken) Renderer.SceneManager.AddSkyBoxSceneNode(null, new Texture[] {
                Renderer.Driver.GetTexture("topax2.jpg"),
                Renderer.Driver.GetTexture("irrlicht2_dn.jpg"),
                Renderer.Driver.GetTexture("rightax2.jpg"),
				Renderer.Driver.GetTexture("leftax2.jpg"),
                Renderer.Driver.GetTexture("frontax2.jpg"),
                Renderer.Driver.GetTexture("backax2.jpg")}, 
0);

            Renderer.Driver.SetTextureFlag(TextureCreationFlag.CreateMipMaps, true);
            

            CameraController = new CameraController(this,Renderer.SceneManager);

            SceneGraph.WaterNode = new WaterSceneNode(null, Renderer.SceneManager, new Dimension2Df(180, 180), new Dimension2D(100, 100), new Dimension2D(512, 512));
            SceneGraph.WaterNode.Position = new Vector3D(0, 30, 0);
            //SceneGraph.WaterNode.WaveHeight *= .4f;
            SceneGraph.WaterNode.RefractionFactor = 0.3f;
            SceneGraph.WaterNode.WaveDisplacement = 2f;
            SceneGraph.WaterNode.WaveHeight = 2f;
            SceneGraph.WaterNode.WaveLength = 2f;
            SceneGraph.WaterNode.WaveSpeed = 5f;
            SceneGraph.WaterNode.WaveRepetition = 20f;
            SceneGraph.WaterNode.Scale = new Vector3D(0.2f,0.2f,0.2f);
            SceneGraph.WaterNode.MultiColor = new Colorf(0.9f, 0.7f, 0.7f, 1.0f);

            UserInterface = new UserInterfaceManager(this, Renderer.Driver, Renderer.SceneManager, Renderer.GuiEnvironment, CameraController, AvatarController);
            UserInterface.DefaultFont = defaultFont;
             
            XmlReader xml = Broken? null: XmlReader.Create(new StreamReader("../../../media/About.xml"));
            while (xml != null && xml.Read())
            {
                switch (xml.NodeType)
                {
                    case XmlNodeType.Text:
                        UserInterface.AboutText = xml.ReadContentAsString();
                        break;
                    case XmlNodeType.Element:
                        if (xml.Name.Equals("startUpModel"))
                        {
                        }
                        else if (xml.Name.Equals("messageText"))
                            UserInterface.AboutCaption = xml.GetAttribute("caption");
                        break;
                }
            }

            string formsUiConfigurationOption = m_configSource.Source.Configs["Startup"].GetString("forms", "true");
            if (formsUiConfigurationOption == "true")
            {
                frmCommunications f = new frmCommunications(NetworkInterface);
                f.Visible = true;
                this.m_formsThread = new Thread(delegate() { Application.DoEvents(); Thread.Sleep(50); });
                m_formsThread.Start();
            }

            AnimationManager = new AnimationManager(this);

            TimeSpan timeTaken = DateTime.UtcNow - StartupTime;

            m_log.InfoFormat("[STARTUP]: Startup took {0}m {1}s", timeTaken.Minutes, timeTaken.Seconds);

        }

        #endregion

        #region Shutdown
        /// <summary>
        /// Should be overriden and referenced by descendents if they need to perform extra shutdown processing
        /// </summary>      
        public override void Shutdown()
        {
            Renderer.Device.Close();
            Renderer.Device.Dispose();
            NetworkInterface.Logout();

            m_log.Info("[SHUTDOWN]: Shutdown processing on main thread complete.  Exiting...");

            Environment.Exit(0);
        }

        #endregion

        #region Main Loop

        public void MainLoop(GUISkin skin, Color skincolor)
        {
            // Main Render Loop
            int minFrameTime = (int) (1000/((float) m_maximumFramesPerSecond));

            m_lastScreenSize = Renderer.Driver.ScreenSize;

            while (Running)
            {
                try
                {
                    NetworkInterface.Process();
                }
                catch (Exception e)
                {
                    m_log.Error("Error processing network messages: " + e);
                }

                try
                {

                    // If you close the gui window, device.Run returns false.
                    Running = Renderer.Device.Run();
                }
                catch (AccessViolationException e)
                {
                    m_log.Error("[VIDEO]: Error in device" + e.ToString());
                }
                catch (OutOfMemoryException e)
                {
                    m_log.Error("[VIDEO]: Error in device" + e.ToString());
                    continue;
                }
                if (!Running)
                    break;
                try
                {
                    //  SceneGraph.WaterNode.Update();
                }
                catch ( /*AccessViolation*/ Exception)
                {
                    m_log.Warn("[water]: Unable to update this round");
                }

                Renderer.Driver.BeginScene(true, true, new Color(0, 255, 255, 255));
                try
                {
                    Renderer.SceneManager.DrawAll();
                }
                catch (Exception e)
                {

                    System.Console.WriteLine("" + e);
                }
                try
                {
                    Renderer.GuiEnvironment.DrawAll();
                }
                catch (Exception e)
                {

                    System.Console.WriteLine("" + e);
                }
                Renderer.Driver.EndScene();

                m_timeSinceLastSceneUpdate += System.Environment.TickCount - m_lastTickCount;
                int frameTime = System.Environment.TickCount - m_lastTickCount;
                m_lastTickCount = System.Environment.TickCount;

                // Update Interpolation targets
                SceneGraph.UpdateMovingObjects();
                // Update camera position and rotation.
                CameraController.CheckTarget();

                if (m_timeSinceLastSceneUpdate > m_baseSceneUpdateTimePeriod)
                {
                    // Repeat any held keys
                    UpdatePressedKeys();

                    m_timeSinceLastSceneUpdate = 0;
                    m_sceneUpdateCounter++;

                    if (m_sceneUpdateCounter == int.MaxValue)
                        m_sceneUpdateCounter = 0;
                }

                if ((m_sceneUpdateCounter%m_avatarModificationProcessingRate) == 0)
                {
                    AvatarController.UpdateRemote();
                    NetworkInterface.SendCameraViewMatrix(CameraController.GetCameraViewMatrix());
                    // process avatar animation changes
                    SceneGraph.ProcessAnimations();
                    // Process Avatar Mod Queue
                    SceneGraph.ProcessObjectModifications(20, ref SceneGraph.AvatarModifications);
                }

                try
                {
                    TerrainManager.Process();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("" + e);
                }

                if ((m_sceneUpdateCounter%m_objectModificationProcessingRate) == 0)
                {
                    // Process Object Mod Queue.  Parameter is 'Items'
                    try
                    {
                        SceneGraph.ProcessObjectModifications(20, ref SceneGraph.ObjectModifications);
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("MAINLOOP: ProcessObjectModifications" + ex);
                    }
                    // Process Mesh Queue.  Parameter is 'Items'
                    try
                    {
                        SceneGraph.ProcessMeshModifications(20);
                    }
                    catch (System.Exception ex)
                    {

                        System.Console.WriteLine("MAINLOOP: ProcessMeshModifications" + ex);
                    }
                    // Check the UnAssigned Child Queue for parents that have since rezed
                    try
                    {
                        SceneGraph.ProcessParentWaitingObjects(5);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine("MAINLOOP: ProcessParentWaitingObjects" + ex);
                    }
                    // Apply textures
                    try
                    {
                        SceneGraph.ProcessTextureModifications(10);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine("MAINLOOP: ProcessTextureModifications" + ex);
                    }
                    // Update foliage.
                    try
                    {
                        SceneGraph.ProcessFoliageMeshModifications(3);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine("MAINLOOP: ProcessFoliageMeshModifications" + ex);
                    }
                    // Set the FPS in the window title.
                    Renderer.Device.WindowCaption = "IdealistViewer 0.001, FPS:" + Renderer.Driver.FPS.ToString();

                }

                // process chat
                if (UserInterface.OutboundChatMessages.Count > 0)
                    lock (UserInterface.OutboundChatMessages)
                        for (int i = 0; i < UserInterface.OutboundChatMessages.Count; i++)
                            NetworkInterface.Say(UserInterface.OutboundChatMessages.Dequeue());

                UserInterface.UpdateChatWindow();

                // Sleep until full frame time has been used.                
                if (m_limitFramesPerSecond && frameTime < minFrameTime)
                    Thread.Sleep(minFrameTime - frameTime);

            }
            //In the end, delete the Irrlicht device.
            Shutdown();
        }

        #endregion

        #region Console Event Processing

        /// <summary>
        /// Runs commands issued by the server console from the operator
        /// </summary>
        /// <param name="command">The first argument of the parameter (the command)</param>
        /// <param name="cmdparams">Additional arguments passed to the command</param>
        public override void OnConsoleCommand(string command, string[] cmdparams)
        {
            switch (command)
            {
                case "a":  // experimental for animation debugging
                    try
                    {
                        int.TryParse(cmdparams[0], out AnimationManager.StartFrame);
                        int.TryParse(cmdparams[1], out AnimationManager.StopFrame);
                        AnimationManager.FramesDirty = true;
                    }
                    catch
                    {
                        m_log.Warn("usage: a <startFrame> <endFrame> - where startFrame and endFrame are integers");
                    }
                    break;
                case "fly":
                    AvatarController.Fly = !AvatarController.Fly;
                    break;
                case "goto":
                    float x = 128f;
                    float y = 128f;
                    float z = 22f;
                    string cmdStr = "";
                    foreach (string arg in cmdparams)
                        cmdStr += arg + " ";
                    cmdStr = cmdStr.Trim();
                    string[] dest = cmdStr.Split(new char[] { '/' });
                    if (float.TryParse(dest[1], out x) &&
                        float.TryParse(dest[2], out y) &&
                        float.TryParse(dest[3], out z))
                    {
                        NetworkInterface.Teleport(dest[0], x, y, z);
                    }
                    else
                        m_log.Warn("Usage: goto simname x y z");
                    break;
                case "help":
                    Console.ShowHelp(cmdparams);
                    Console.Notice("");
                    break;
                case "q":
                    string qMsg = " ***************QUEUE STATUS*****************";
                    qMsg += "\nfoliageObjectQueue.........................: " + SceneGraph.FoliageMeshModifications.Count.ToString();
                    qMsg += "\nobjectModQueue.............................: " + SceneGraph.ObjectModifications.Count.ToString();
                    qMsg += "\nobjectMeshQueue............................: " + SceneGraph.ObjectMeshModifications.Count.ToString();
                    qMsg += "\nUnAssignedChildObjectModQueue..............: " + SceneGraph.ParentWaitingObjects.Count.ToString();
                    qMsg += "\noutgoingChatQueue..........................: " + UserInterface.OutboundChatMessages.Count.ToString();

                    m_log.Debug(qMsg);
                    break;

                case "cq":
                    m_log.Debug("*************** UnAssignedChildObjectModQueue info ************");
                    lock (SceneGraph.ParentWaitingObjects)
                    {
                        foreach (VObject v in SceneGraph.ParentWaitingObjects)
                        {
                            Primitive prim = v.Primitive;

                            m_log.Debug(
                                 " UUID: " + prim.ID.ToString()
                                + " parentID: " + prim.ParentID.ToString()
                                + " localID: " + prim.LocalID.ToString()
                                );
                        }
                    }
                    break;

                case "relog":
                    NetworkInterface.Login(NetworkInterface.LoginURI, NetworkInterface.FirstName + " " + NetworkInterface.LastName, NetworkInterface.Password, NetworkInterface.StartLocation);
                    break;
                case "say":
                    string message = "";
                    foreach (string word in cmdparams)
                        message += word + " ";
                    NetworkInterface.Say(message);
                    break;
                case "set":
                    if (cmdparams.Length < 2)
                        return;

                    if (cmdparams[0] == "log" && cmdparams[1] == "level")
                    {
                        string[] setParams = new string[cmdparams.Length - 2];
                        Array.Copy(cmdparams, 2, setParams, 0, cmdparams.Length - 2);

                        Console.SetConsoleLogLevel(setParams);
                    }
                    break;

                case "show":
                    if (cmdparams.Length > 0)
                    {
                        Console.Show(cmdparams);
                    }
                    break;

                case "quit":
                case "shutdown":
                    Shutdown();
                    break;
            }
        }

        #endregion

        #region Network Event Processing

        protected void OnNetworkConnected()
        {
            UserInterface.UpdateFriendsList();
        }

        protected void OnNetworkSimulatorConnected(VSimulator sim)
        {
            SceneGraph.OnNetworkSimulatorConnected(sim);
        }

        private void OnNetworkFriendsListChange()
        {
            UserInterface.UpdateFriendsList();
        }

        private void OnNetworkChat(string message, ChatAudibleLevel audible, ChatType type, ChatSourceType sourcetype, string fromName, UUID id, UUID ownerid, Vector3 position)
        {
            UserInterface.OnNetworkChat(message, audible, type, sourcetype, fromName, id, ownerid, position);
        }

        private void OnNetworkAvatarAdd(VSimulator sim, Avatar avatar, ulong regionHandle, ushort timeDilation)
        {
            m_timeDilation = (float)(timeDilation / ushort.MaxValue);
            SceneGraph.OnNetworkAvatarAdd(sim, avatar, regionHandle, timeDilation);
        }

        public void OnNetworkObjectAdd(VSimulator sim, Primitive prim, ulong regionHandle, ushort timeDilation)
        {
            m_timeDilation = (float)(timeDilation / ushort.MaxValue);

            SceneGraph.OnNetworkObjectAdd(sim, prim, regionHandle, timeDilation);

        }

        private void OnNetworkObjectUpdate(VSimulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation)
        {
            m_timeDilation = (float)(timeDilation / ushort.MaxValue);
            SceneGraph.OnNetworkObjectUpdate(simulator, update, regionHandle, timeDilation);
        }

        private void OnNetworkObjectRemove(VSimulator psim, uint pLocalID)
        {
            SceneGraph.OnNetworkObjectRemove(psim, pLocalID);
        }

        private void OnNetworkLandUpdate(VSimulator sim, int x, int y, int width, float[] data)
        {
            TerrainManager.OnNetworkLandUpdate(sim, x, y, width, data);
            SceneGraph.WaterNode.Position = new Vector3D(0, sim.WaterHeight - 0.5f, 0);
        }

        public void OnNetworkTextureDownloaded(string tex, VObject vObj, UUID AssetID)
        {
            SceneGraph.OnNetworkTextureDownloaded(tex, vObj, AssetID);
        }

        #endregion

        #region Key Input Processing

        /// <summary>
        /// Processes held keys.  This allows us to do multiple keypresses.
        /// </summary>
        private void UpdatePressedKeys()
        {
            lock (m_pressedKeys)
            {
                foreach (KeyCode ky in m_pressedKeys)
                {
                    ProcessPressedKey(ky);
                }

            }
        }

        /// <summary>
        /// Manage held keys
        /// </summary>
        /// <param name="ky"></param>
        /// <param name="held"></param>
        public override void UpdateKeyPressState(KeyCode ky, bool held)
        {
            lock (m_pressedKeys)
            {
                if (held)
                {
                    if (!m_pressedKeys.Contains(ky))
                    {
                        m_pressedKeys.Add(ky);
                    }
                }
                else
                {
                    if (m_pressedKeys.Contains(ky))
                    {
                        m_pressedKeys.Remove(ky);
                    }
                }

            }
        }

        /// <summary>
        /// Does an avatar movement based on provided key
        /// </summary>
        /// <param name="ky"></param>
        private void ProcessPressedKey(KeyCode ky, bool kydown, bool held)
        {
            //m_log.DebugFormat("{0},{1},{2}", ky.ToString(), kydown, held);
            switch (ky)
            {
                case KeyCode.Key_W:
                case KeyCode.Up:

                    AvatarController.Forward = kydown;
                    break;

                case KeyCode.Key_S:
                case KeyCode.Down:
                    AvatarController.Back = kydown;
                    break;

                case KeyCode.Key_A:
                case KeyCode.Left:
                    CameraController.SetRotationDelta(-1, 0);
                    break;

                case KeyCode.Key_D:
                case KeyCode.Right:
                    CameraController.SetRotationDelta(1, 0);
                    break;

                case KeyCode.Prior:
                    if (AvatarController.Fly)
                        AvatarController.Up = kydown;
                    else
                        if (kydown)
                            AvatarController.Jump = true;
                        else
                            AvatarController.Jump = false;

                    break;

                case KeyCode.Next:
                    AvatarController.Down = kydown;
                    break;

                case KeyCode.Home:
                case KeyCode.Key_F:
                    if (!held && !kydown)
                        AvatarController.Fly = !AvatarController.Fly;
                    break;

            }
        }

        /// <summary>
        /// does an action for a held key
        /// </summary>
        /// <param name="ky"></param>
        private void ProcessPressedKey(KeyCode ky)
        {
            switch (ky)
            {
                case KeyCode.Up:
                    if (!m_shiftPressed && !m_ctrlPressed)
                    {
                        if (CameraController.CameraMode == ECameraMode.Build)
                        {
                            if (SceneGraph.m_avatarObject != null)
                            {
                                CameraController.SetTarget(SceneGraph.m_avatarObject.SceneNode);
                                CameraController.SwitchMode(ECameraMode.Third);
                            }
                        }

                        ProcessPressedKey(ky, true, true);
                    }
                    else
                    {
                        if (m_ctrlPressed)
                        {
                            CameraController.DoKeyAction(ky);
                        }
                    }
                    break;

                case KeyCode.Down:
                    if (!m_shiftPressed && !m_ctrlPressed)
                    {
                        if (CameraController.CameraMode == ECameraMode.Build)
                        {
                            if (SceneGraph.m_avatarObject != null)
                            {
                                CameraController.SetTarget(SceneGraph.m_avatarObject.SceneNode);
                                CameraController.SwitchMode(ECameraMode.Third);
                            }
                        }

                        ProcessPressedKey(ky, true, true);
                    }
                    else
                    {
                        if (m_ctrlPressed)
                        {
                            CameraController.SwitchMode(ECameraMode.Build);
                            CameraController.DoKeyAction(ky);
                        }

                    }
                    break;

                case KeyCode.Left:
                    if (!m_shiftPressed && !m_ctrlPressed)
                    {
                        if (CameraController.CameraMode == ECameraMode.Build)
                        {
                            if (SceneGraph.m_avatarObject != null)
                            {
                                CameraController.SetTarget(SceneGraph.m_avatarObject.SceneNode);
                                CameraController.SwitchMode(ECameraMode.Third);
                            }
                        }

                        ProcessPressedKey(ky, true, true);
                    }
                    else
                    {
                        if (m_ctrlPressed)
                        {
                            //vOrbit.X -= 2f;
                            CameraController.SwitchMode(ECameraMode.Build);
                            CameraController.DoKeyAction(ky);
                        }

                    }
                    break;

                case KeyCode.Right:
                    if (!m_shiftPressed && !m_ctrlPressed)
                    {
                        if (CameraController.CameraMode == ECameraMode.Build)
                        {
                            if (SceneGraph.m_avatarObject != null)
                            {
                                CameraController.SetTarget(SceneGraph.m_avatarObject.SceneNode);
                                CameraController.SwitchMode(ECameraMode.Third);
                            }
                        }

                        ProcessPressedKey(ky, true, true);
                    }
                    else
                    {
                        if (m_ctrlPressed)
                        {
                            //vOrbit.X += 2f;
                            //vOrbit.X -= 2f;
                            CameraController.SwitchMode(ECameraMode.Build);
                            CameraController.DoKeyAction(ky);
                        }

                    }
                    break;
                case KeyCode.Prior:
                    if (!m_shiftPressed && !m_ctrlPressed)
                    {
                        if (CameraController.CameraMode == ECameraMode.Build)
                        {
                            if (SceneGraph.m_avatarObject != null)
                            {
                                CameraController.SetTarget(SceneGraph.m_avatarObject.SceneNode);
                                CameraController.SwitchMode(ECameraMode.Third);
                            }
                        }

                        ProcessPressedKey(ky, true, true);
                    }
                    else
                    {
                        if (m_ctrlPressed)
                        {
                            //vOrbit.Y -= 2f;
                            CameraController.SwitchMode(ECameraMode.Build);
                            CameraController.DoKeyAction(ky);
                        }

                    }
                    break;

                case KeyCode.Next:
                    if (!m_shiftPressed && !m_ctrlPressed)
                    {
                        if (CameraController.CameraMode == ECameraMode.Build)
                        {
                            if (SceneGraph.m_avatarObject != null)
                            {
                                CameraController.SetTarget(SceneGraph.m_avatarObject.SceneNode);
                                CameraController.SwitchMode(ECameraMode.Third);
                            }
                        }

                        ProcessPressedKey(ky, true, true);
                    }
                    else
                    {
                        if (m_ctrlPressed)
                        {
                            CameraController.SwitchMode(ECameraMode.Build);
                            CameraController.DoKeyAction(ky);
                        }

                    }
                    break;

            }

        }

        #endregion

        #region Device Event Processing
        /// <summary>
        /// The Irrlicht window has had an event.
        /// </summary>
        /// <param name="p_event"></param>
        /// <returns></returns>
        public override bool OnDeviceEvent(Event p_event)
        {

            //m_log.Warn(p_event.Type.ToString());
            if (p_event.Type == EventType.LogTextEvent)
            {
                string eventype = p_event.LogText;
                if (eventype.Contains("Resizing window"))
                {
                    int pos = eventype.IndexOf('(');
                    int pos2 = eventype.IndexOf(')');
                    //
                    string resizeto = eventype.Substring(pos + 1, pos2 - (pos + 1));
                    string[] xy = resizeto.Split(' ');

                    WindowWidth = Convert.ToInt32(xy[0]);
                    WindowHeight = Convert.ToInt32(xy[1]);
                    WindowAspect = (float)WindowWidth / WindowHeight;
                }
            }

            if (p_event.Type == EventType.GUIEvent)
            {
                int id = p_event.Caller.ID;
                GUIEnvironment env = Renderer.Device.GUIEnvironment;

               

                //m_log.InfoFormat("{0} {1} {2} {3}", p_event.Type, p_event.Caller, p_event.GUIEvent, p_event.Caller.ID);
                switch (p_event.GUIEvent)
                {
                    case GUIEventType.ElementFocused:
                        this.UserInterface.FocusedElement = p_event.Caller as GUIElement;
                        break;
                    case GUIEventType.ElementFocusLost:
                        this.UserInterface.FocusedElement = null;
                        break;
                    case GUIEventType.MenuItemSelected:
                        // a menu item was clicked
                        GUIContextMenu menu = ((GUIContextMenu)p_event.Caller);
                        id = menu.GetItemCommandID(menu.SelectedItem);
                        switch (id)
                        {
                            case (int)MenuItems.FileOpen:
                                env.AddFileOpenDialog("Please select a model file to open", false, Renderer.Device.GUIEnvironment.RootElement, 0);
                                break;
                            case (int)MenuItems.FileQuit: // File -> Quit
                                Shutdown();
                                break;
                            case (int)MenuItems.ShowChat:
                                UserInterface.ShowChatWindow();
                                break;
                            case (int)MenuItems.ShowPrimcount:
                                {
                                    uint texcount = 0;
                                    if (TextureManager != null)
                                        texcount = TextureManager.TextureCacheCount;
                                    m_log.DebugFormat("FullUpdateCount:{0}, PrimCount:{1}, TextureCount:{2}, UniquePrim:{3}", SceneGraph.PrimitiveCount, SceneGraph.Objects.Count, texcount, MeshManager.UniqueObjects);
                                }
                                break;
                            case (int)MenuItems.ClearCache:
                                {
                                    if (TextureManager != null)
                                    {
                                        TextureManager.ClearMemoryCache();

                                    }
                                }
                                break;
                            case (int)MenuItems.ViewModeOne: // View -> Material -> Solid
                                //CFK nothing yet, circa Thanksgiving eve 08
                                //CFK                                if (Model != null)
                                //CFK                                    Model.SetMaterialType(MaterialType.Solid);
                                break;
                            case (int)MenuItems.ViewModeTwo: // View -> Material -> Transparent
                                //CFK nothing yet, circa Thanksgiving eve 08
                                //CFK                                if (Model != null)
                                //CFK                                    Model.SetMaterialType(MaterialType.TransparentAddColor);
                                break;
                            case (int)MenuItems.ViewModeThree: // View -> Material -> Reflection
                                //CFK nothing yet, circa Thanksgiving eve 08
                                //CFK                                if (Model != null)
                                //CFK                                    Model.SetMaterialType(MaterialType.SphereMap);
                                break;
                            default:
                                UserInterface.HandleMenuAction(id);
                                break;
                        }
                        break;//case GUIEventType.MenuItemSelected:

                    case GUIEventType.FileSelected:
                        //CFK nothing yet, circa Thanksgiving eve 08
                        // load the model file, selected in the file open dialog
                        //CFK GUIFileOpenDialog dialog = ((GUIFileOpenDialog)p_event.Caller);
                        //CFK                        loadModel(dialog.Filename);
                        break;
                    case GUIEventType.EditBoxEnter:
                        if (p_event.Caller == UserInterface.ChatBoxInput)
                        {
                            lock (UserInterface.OutboundChatMessages)
                                UserInterface.OutboundChatMessages.Enqueue(UserInterface.ChatBoxInput.Text);

                            lock (UserInterface.MessageHistory)
                            {
                                UserInterface.MessageHistory.Add("You: " + UserInterface.ChatBoxInput.Text);
                                UserInterface.NewChat = true;
                            }

                            UserInterface.ChatBoxInput.Text = "";
                        }
                        break;
                }
            }


            // !Mouse event  (we do this so that we don't process the rest of this each mouse move
            if (p_event.Type != EventType.MouseInputEvent)
            {
                
                //Keyboard event
                if (p_event.Type == EventType.KeyInputEvent)
                {
                    //If we have focus on the 3d screen and not a control
                    if (UserInterface.FocusedElement == null)
                    {
                        
                        switch (p_event.KeyCode)
                        {

                            case KeyCode.Control:
                                m_ctrlPressed = p_event.KeyPressedDown;
                                if (m_ctrlPressed)
                                {

                                    CameraController.ResetMouseOffsets();
                                }
                                else
                                {
                                    CameraController.ApplyMouseOffsets();
                                }
                                break;
                            case KeyCode.Shift:
                                m_shiftPressed = p_event.KeyPressedDown;
                                break;
                            case KeyCode.Up:
                            case KeyCode.Down:
                            case KeyCode.Left:
                            case KeyCode.Right:
                            case KeyCode.Prior:
                            case KeyCode.Next:
                            case KeyCode.Key_W:
                            case KeyCode.Key_S:
                            case KeyCode.Key_A:
                            case KeyCode.Key_D:
                            case KeyCode.Home:
                            case KeyCode.Key_F:
                                if (!m_ctrlPressed)
                                    ProcessPressedKey(p_event.KeyCode, p_event.KeyPressedDown, false);

                                UpdateKeyPressState(p_event.KeyCode, p_event.KeyPressedDown);
                                break;

                            case KeyCode.Key_P:
                                if (p_event.KeyPressedDown)
                                {
                                    uint texcount = 0;
                                    if (TextureManager != null)
                                        texcount = TextureManager.TextureCacheCount;
                                    m_log.DebugFormat("FullUpdateCount:{0}, PrimCount:{1}, TextureCount:{2}, UniquePrim:{3}", SceneGraph.PrimitiveCount, SceneGraph.Objects.Count, texcount, MeshManager.UniqueObjects);
                                }
                                break;

                            case KeyCode.Key_C:
                                if (p_event.KeyPressedDown)
                                {
                                    if (TextureManager != null)
                                    {
                                        TextureManager.ClearMemoryCache();

                                    }
                                }
                                break;
                        }  
                    }
                }
                UpdatePressedKeys();
            }

            if (p_event.Type == EventType.MouseInputEvent)
            {
                return OnMouseEvent(p_event);
            }

            return false;
        }
        #endregion

        #region Mouse Event Processing
        public override bool OnMouseEvent(Event p_event)
        {
            if (p_event.MouseInputEvent == MouseInputEvent.MouseWheel)
            {
                CameraController.MouseWheelAction(p_event.MouseWheelDelta);
            }
            if (p_event.MouseInputEvent == MouseInputEvent.LMouseLeftUp)
            {
                if (m_ctrlPressed)
                {
                    CameraController.ApplyMouseOffsets();
                }
                m_leftMousePressed = false;
            }
            if (p_event.MouseInputEvent == MouseInputEvent.LMousePressedDown)
            {

                m_leftMousePressed = true;
                if (m_ctrlPressed)
                {
                    CameraController.SwitchMode(ECameraMode.Build);
                    // Pick!

                    CameraController.ResetMouseOffsets();
                    Vector3D[] projection = CameraController.ProjectRayPoints(p_event.MousePosition, WindowWidth/2, WindowHeight/2, WindowAspect);
                    Line3D projectedray = new Line3D(projection[0], projection[1]);

                    Vector3D collisionpoint = new Vector3D(0, 0, 0);
                    Triangle3D tri = new Triangle3D(0, 0, 0, 0, 0, 0, 0, 0, 0);

                    // Check if we have a node under the mouse
                    SceneNode node = SceneGraph.TrianglePicker.GetSceneNodeFromRay(projectedray, 0x0128, true, CameraController.CameraNode.Position); //smgr.CollisionManager.GetSceneNodeFromScreenCoordinates(new Position2D(p_event.MousePosition.X, p_event.MousePosition.Y), 0, false);
                    if (node == null)
                    {
                        if (SceneGraph.TriangleSelector != null)
                        {
                            // Collide test against the terrain
                            if (Renderer.SceneManager.CollisionManager.GetCollisionPoint(projectedray, SceneGraph.TriangleSelector, out collisionpoint, out tri))
                            {
                                if (CameraController.CameraMode == ECameraMode.Build)
                                {
                                    CameraController.SetTarget(collisionpoint);
                                    CameraController.TargetNode = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Sometimes the terrain picker returns weird values.
                        // If it's weird try the general 'everything' triangle picker.
                        m_log.WarnFormat("[PICK]: Picked <{0},{1},{2}>", node.Position.X, node.Position.Y, node.Position.Z);
                        if (node.Position.X == 0 && node.Position.Z == 0)
                        {
                            if (SceneGraph.TriangleSelector != null)
                            {
                                if (Renderer.SceneManager.CollisionManager.GetCollisionPoint(projectedray, SceneGraph.TriangleSelector, out collisionpoint, out tri))
                                {
                                    if (CameraController.CameraMode == ECameraMode.Build)
                                    {
                                        CameraController.SetTarget(collisionpoint);
                                        CameraController.TargetNode = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Target the node
                            if (CameraController.CameraMode == ECameraMode.Build)
                            {
                                CameraController.SetTarget(node.Position);
                                CameraController.TargetNode = node;
                            }
                        }
                    }
                }
            }
            if (p_event.MouseInputEvent == MouseInputEvent.RMouseLeftUp)
            {
                m_rightMousePressed = false;
            }
            if (p_event.MouseInputEvent == MouseInputEvent.RMousePressedDown)
            {
                m_rightMousePressed = true;
            }

            if (p_event.MouseInputEvent == MouseInputEvent.MouseMoved)
            {
                if (m_leftMousePressed && m_ctrlPressed)
                {
                    int deltaX = p_event.MousePosition.X - m_oldMouseX;
                    int deltaY = p_event.MousePosition.Y - m_oldMouseY;

                    CameraController.SetDeltaFromMouse(deltaX, deltaY);
                }

                if (m_rightMousePressed)
                {

                    int deltaX = p_event.MousePosition.X - m_oldMouseX;
                    int deltaY = p_event.MousePosition.Y - m_oldMouseY;

                    CameraController.SetRotationDelta(deltaX, deltaY);
                }

                m_oldMouseX = p_event.MousePosition.X;
                m_oldMouseY = p_event.MousePosition.Y;
            }
            return false;
        }

        #endregion

        #region Helper Methods
        //public void SetViewerAvatar(VObject self)
        //{
        //    if (SceneGraph.m_avatarObject == null)
        //    {
        //        SceneGraph.m_avatarObject = self;
        //        AvatarController.AvatarNode = SceneGraph.m_avatarObject.SceneNode;
        //        CameraController.SetTarget(SceneGraph.m_avatarObject.SceneNode);
        //        CameraController.SwitchMode(ECameraMode.Third);
        //    }
        //}
        #endregion
    
    }

}
