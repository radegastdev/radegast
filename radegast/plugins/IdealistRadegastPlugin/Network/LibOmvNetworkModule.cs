using System;
using System.Collections.Generic;
using System.Reflection;
using IdealistViewer;
using IdealistViewer.Network;
using OpenMetaverse;
using OpenMetaverse.Assets;
using OpenMetaverse.Imaging;
using OpenMetaverse.Packets;
using log4net;
using System.Drawing;
using IdealistViewer.Scene;
using System.Drawing.Imaging;
using Radegast;

namespace IdealistRadegastPlugin
{

    public class SimulatorCull
    {
        public Dictionary<uint, List<Primitive>> PrimsAwaitingParent = new Dictionary<uint, List<Primitive>>();
        public Dictionary<uint, Primitive> ParentsSent = new Dictionary<uint, Primitive>();
        public Dictionary<uint, List<Primitive>> ChildsSent = new Dictionary<uint, List<Primitive>>();
        public Dictionary<uint, Primitive> ParentsCulled = new Dictionary<uint, Primitive>();
        public Dictionary<uint, List<Primitive>> ChildsCulled = new Dictionary<uint, List<Primitive>>();
        public List<SimPatchInfo> PatchesSent = new List<SimPatchInfo>();
        public List<SimPatchInfo> PatchesCulled = new List<SimPatchInfo>();
        readonly public Simulator simulator;
        public SimulatorCull(Simulator sim)
        {
            simulator = sim;
        }
    }
    public class SimPatchInfo
    {
        public SimPatchInfo(Simulator simulator0, int x0, int y0, int width0, float[] data0)
        {
            simulator = simulator0;
            x = x0;
            y = y0;
            width = width0;
            data = data0;
            float w2 = (float) width/2;
            pos = RadegastNetworkModule.GlobalPos(simulator.Handle, new Vector3(width * x + w2, width * y + w2, data[0]));
        }
        public override int GetHashCode()
        {
            return ((int) simulator.Handle*width*width) + x + y*width;
        }
        public override bool Equals(object obj)
        {
            if (obj is SimPatchInfo)
            {
                SimPatchInfo other = (SimPatchInfo)obj;
                return simulator == other.simulator && x == other.x && y == other.y;
            }
            return false;
        }
        public Vector3d pos;
        public Simulator simulator;
        public int x;
        public int y;
        public int width;
        public float[] data;
    }

    public class RadegastNetworkModule : INetworkInterface
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<ulong, SimulatorCull> SimulatorCullings = new Dictionary<ulong, SimulatorCull>();
        /// <summary>
        /// Basically so we don't add 8 neighbors terrain into scene until we need it
        /// </summary>
        public double LandSightDistance = 96.0;
        /// <summary>
        /// When set true will try to limit the number of objects sent to the native layer based on avatars camera distance
        /// </summary>
        public bool ObjectsToManagedLayerIsRangeBased = true;
        /// <summary>
        /// The maximum prims at a time we send to the native layer. TODO: Implement!
        /// </summary>
        public int MAX_PRIMS = 8000;
        /// <summary>
        /// The Count of prims we've already sent to the native layer. TODO: Implement!
        /// </summary>
        public int CURRENT_PRIMS = 0;
        /// <summary>
        /// How far to look
        /// </summary>
        public double MAX_DIST
        {
            get
            {
                return m_user.Self.Movement.Camera.Far;
            }
        }
        public Vector3d CameraPosition
        {
            get
            {
                return m_user.Self.GlobalPosition;
            }
        }

        static internal Vector3d GlobalPos(ulong regionHandle, Vector3 pos)
        {
            uint globalX, globalY;
            Utils.LongToUInts(regionHandle, out globalX, out globalY);
            return new Vector3d(
                (double)globalX + (double)pos.X,
                (double)globalY + (double)pos.Y,
                (double)pos.Z);
        }

        private bool Sendable(Primitive primitive)
        {

            if (m_user.Network.CurrentSim == null) //;// || primitive.RegionHandle != m_user.Network.CurrentSim.Handle)
                return false;
            double d = Vector3d.Distance(CameraPosition, GlobalPos(primitive.RegionHandle, primitive.Position));
            return d < MAX_DIST;
        }

        public void Statistics()
        {
        }

        private Vector3 _lastKnownSimPostion = Vector3.Zero;
        private readonly object _lastKnownPosLock = new object();
        private void OnSelfUpdated(Simulator simulator, ObjectUpdate update, ulong regionhandle)
        {
            lock (_lastKnownPosLock)
            {
                if (_lastKnownSimPostion == Vector3.Zero)
                {
                    _lastKnownSimPostion = update.Position;
                    return;
                }
                if (Vector3.Distance(update.Position, _lastKnownSimPostion) < 8) return;
                _lastKnownSimPostion = update.Position;
            }

            List<SimulatorCull> simCulls = null;
            List<SimPatchInfo> sendpatches = new List<SimPatchInfo>();
            lock (SimulatorCullings)
            {
                simCulls = new List<SimulatorCull>(SimulatorCullings.Values);
            }


            foreach (SimulatorCull sc in simCulls)
            {
                lock (sc)
                {
                    foreach (var p in sc.PatchesCulled)
                    {

                        if (Vector3d.Distance(CameraPosition, p.pos) < LandSightDistance)
                        {
                            sendpatches.Add(p);
                        }
                    }
                    foreach (var p in sendpatches)
                    {
                        sc.PatchesCulled.Remove(p);
                        sc.PatchesSent.Add(p);
                    }
                }
            }
            foreach (var p in sendpatches)
            {
                landPatchCallback(p.simulator, p.x, p.y, p.width, p.data);
            }

            if (!ObjectsToManagedLayerIsRangeBased) return;

            foreach (SimulatorCull sc in simCulls)
            {
                lock (sc) OnSelfUpdatedObjects(sc);
            }
        }

        private void OnSelfUpdatedObjects(SimulatorCull sc)
        {
            List<Primitive> sendprims = new List<Primitive>();

            foreach (var p in sc.ParentsCulled.Values)
            {

                if (Vector3d.Distance(CameraPosition, GlobalPos(p.RegionHandle, p.Position)) <
                    MAX_DIST)
                {
                    sendprims.Add(p);
                }
            }
            foreach (var p in sendprims)
            {
                sc.ParentsCulled.Remove(p.LocalID);
                if (sc.ChildsCulled.ContainsKey(p.LocalID))
                {
                    sc.PrimsAwaitingParent[p.LocalID] = sc.ChildsCulled[p.LocalID];
                }
                newPrimCulled(sc.simulator, p, sc.simulator.Handle, 0);
            }
            sendprims.Clear();
            foreach (var p in sc.ParentsSent.Values)
            {
                if (Vector3d.Distance(CameraPosition, GlobalPos(p.RegionHandle, p.Position)) >
                    MAX_DIST)
                {
                    sendprims.Add(p);
                }
            }
            foreach (var p in sendprims)
            {
                sc.ParentsSent.Remove(p.LocalID);
                sc.ParentsCulled[p.LocalID] = p;
                if (sc.ChildsSent.ContainsKey(p.LocalID))
                {
                    sc.ChildsCulled[p.LocalID] = new List<Primitive>(sc.ChildsSent[p.LocalID]);
                    foreach (var pc in sc.ChildsSent[p.LocalID])
                    {
                        objectKilledCallback(sc.simulator, pc.LocalID);
                    }
                    sc.ChildsSent[p.LocalID].Clear();
                }
                objectKilledCallback(sc.simulator, p.LocalID);
            }
        }

        private void landPatchCallbackCulled(Simulator simulator, int x, int y, int width, float[] data)
        {
            SimulatorCull sc;
            ulong regionhandle = simulator.Handle;
            lock (SimulatorCullings)
                if (!SimulatorCullings.TryGetValue(regionhandle, out sc))
                {
                    SimulatorCullings[regionhandle] = sc = new SimulatorCull(simulator);
                }
            lock (sc)
            {
                SimPatchInfo p = new SimPatchInfo(simulator, x, y, width, data);
                Vector3d p3d = p.pos;
                p3d.Z = CameraPosition.Z;
                if (Vector3d.Distance(CameraPosition, p3d) > LandSightDistance)
                {
                    sc.PatchesCulled.Add(p);
                    return;
                }
                else
                {
                    if (sc.PatchesSent.Contains(p)) return;
                    sc.PatchesSent.Add(p);
                }
            }
            landPatchCallback(simulator, x, y, width, data);
        }

        private void objectKilledCallbackCulled(Simulator simulator, uint objectid)
        {
            if (!ObjectsToManagedLayerIsRangeBased)
            {
                objectKilledCallback(simulator, objectid);
                return;
            }
            SimulatorCull sc;
            ulong regionhandle = simulator.Handle;
            lock (SimulatorCullings) if (!SimulatorCullings.TryGetValue(regionhandle, out sc))
                {
                    SimulatorCullings[regionhandle] = sc = new SimulatorCull(simulator);
                }
            lock (sc)
            {

                List<Primitive> childs;
                if (sc.ChildsCulled.TryGetValue(objectid, out childs))
                {
                    childs.Clear();
                    sc.ChildsCulled.Remove(objectid);
                }
                if (sc.ChildsSent.TryGetValue(objectid, out childs))
                {
                    childs.Clear();
                    sc.ChildsSent.Remove(objectid);
                }
                if (sc.ParentsCulled.ContainsKey(objectid))
                {
                    sc.ParentsCulled.Remove(objectid);
                    return;
                }
                if (sc.ParentsSent.ContainsKey(objectid))
                {
                    sc.ParentsSent.Remove(objectid);
                    objectKilledCallback(simulator, objectid);
                    return;
                }
                // this is a child
                Primitive prim = null;
                foreach (List<Primitive> ch in sc.PrimsAwaitingParent.Values)
                {
                    foreach (var p in ch)
                    {
                        if (p.LocalID == objectid)
                        {
                            prim = p;
                            break;
                        }
                    }
                    if (prim == null) continue;
                    ch.Remove(prim);
                    break;
                }
                //if (prim != null) return;
                foreach (List<Primitive> ch in sc.ChildsCulled.Values)
                {
                    foreach (var p in ch)
                    {
                        if (p.LocalID == objectid)
                        {
                            prim = p;
                            break;
                        }
                    }
                    if (prim == null) continue;
                    ch.Remove(prim);
                    return;
                }
                foreach (List<Primitive> ch in sc.ChildsSent.Values)
                {
                    foreach (var p in ch)
                    {
                        if (p.LocalID == objectid)
                        {
                            prim = p;
                            break;
                        }
                    }
                    if (prim == null) continue;
                    ch.Remove(prim);
                    break;
                }
                objectKilledCallback(simulator, objectid);
            }

        }

        private void newPrimCulled(Simulator simulator, Primitive prim, ulong regionhandle, ushort timedilation)
        {
            if (prim.RegionHandle == 0)
            {
                prim.RegionHandle = regionhandle;
            }
            SimulatorCull sc;
            lock (SimulatorCullings) if (!SimulatorCullings.TryGetValue(regionhandle, out sc))
                {
                    SimulatorCullings[regionhandle] = sc = new SimulatorCull(simulator);
                }

            if (!ObjectsToManagedLayerIsRangeBased)
            {
                newPrim(sc, simulator, prim, regionhandle, timedilation);
                return;
            }
            lock (sc)
            {
                List<Primitive> prims;
                if (prim.ParentID == 0)
                {
                    if (sc.PrimsAwaitingParent.TryGetValue(prim.LocalID, out prims))
                    {
                        if (Sendable(prim))
                        {
                            //sc.PrimsAwaitingParent.Remove(prim.LocalID);
                            //// send the childs
                            //foreach (Primitive p in prims)
                            //{
                            //    newPrim(sc, simulator, p, regionhandle, timedilation);
                            //}
                            newPrim(sc, simulator, prim, regionhandle, timedilation);
                            return;
                        }
                        else
                        {
                            sc.PrimsAwaitingParent.Remove(prim.LocalID);
                            sc.ParentsCulled[prim.LocalID] = prim;
                            sc.ChildsCulled[prim.LocalID] = prims;
                            return;
                        }
                    }
                    else
                    {
                        if (Sendable(prim))
                        {
                            newPrim(sc, simulator, prim, regionhandle, timedilation);
                            return;
                        }
                        else
                        {
                            sc.ParentsCulled[prim.LocalID] = prim;
                            return;
                        }
                    }
                }

                if (sc.ParentsCulled.ContainsKey(prim.ParentID))
                {
                    if (!sc.ChildsCulled.TryGetValue(prim.ParentID, out prims))
                    {
                        sc.ChildsCulled[prim.ParentID] = prims = new List<Primitive>();
                        prims.Add(prim);
                        return;
                    }
                    else
                    {
                        if (!prims.Contains(prim))
                        {
                            prims.Add(prim);
                            return;
                        }
                    }
                }
                // child with parent already sent
                if (sc.ParentsSent.ContainsKey(prim.ParentID))
                {
                    //this is an update
                    newPrim(sc, simulator, prim, regionhandle, timedilation);
                    return;
                }
                // child with culled parent
                if (sc.ParentsCulled.ContainsKey(prim.ParentID))
                {
                    if (!sc.ChildsCulled.TryGetValue(prim.ParentID, out prims))
                    {
                        sc.ChildsCulled[prim.ParentID] = prims = new List<Primitive>();
                        prims.Add(prim);
                    }
                    else
                    {
                        if (!prims.Contains(prim))
                        {
                            prims.Add(prim);
                        }
                    }
                    return;
                }
                if (sc.ChildsSent.ContainsKey(prim.ParentID))
                {
                    //this is an update
                    newPrim(sc, simulator, prim, regionhandle, timedilation);
                    return;
                }
                if (sc.ParentsSent.ContainsKey(prim.ParentID))
                {
                    //this is an update
                    newPrim(sc, simulator, prim, regionhandle, timedilation);
                    return;
                }
                if (!sc.PrimsAwaitingParent.TryGetValue(prim.ParentID, out prims))
                {
                    sc.PrimsAwaitingParent[prim.ParentID] = prims = new List<Primitive>();
                    prims.Add(prim);
                    return;
                }
                else
                {
                    if (!prims.Contains(prim))
                    {
                        prims.Add(prim);
                        return;
                    }
                    return;
                }
            }
        }

        public event NetworkAvatarAddDelegate OnAvatarAdd;
        public event NetworkChatDelegate OnChat;
        public event NetworkConnectedDelegate OnConnected;
        public event NetworkSimulatorConnectedDelegate OnSimulatorConnected;
        public event NetworkLandUpdateDelegate OnLandUpdate;
        public event NetworkLoginDelegate OnLoggedIn;
        public event NetworkObjectAddDelegate OnObjectAdd;
        public event NetworkObjectRemoveDelegate OnObjectRemove;
        public event NeworkObjectUpdateDelegate OnObjectUpdate;
        public event NetworkTextureDownloadedDelegate OnTextureDownloaded;
        public event NetworkFriendsListUpdateDelegate OnFriendsListUpdate;

        private Vector3 lastCameraPos = Vector3.Zero;
        private Vector3 lastCameraTarget = Vector3.Zero;

        public string loginURI;
        public string LoginURI
        {
            get
            {
                return loginURI;
            }
        }
        public string firstName;
        public string FirstName
        {
            get
            {
                return firstName;
            }
        }
        public string lastName;
        public string LastName
        {
            get
            {
                return lastName;
            }
        }
        public string username;
        public string UserName
        {
            get
            {
                return username;
            }
        }
        public string password;
        public string Password
        {
            get
            {
                return password;
            }
        }
        public string startlocation;
        public string StartLocation
        {
            get
            {
                return startlocation;
            }
        }

        // received animations are stored here before being processed in the main frame loop
        public Dictionary<UUID, List<UUID>> avatarAnimations = new Dictionary<UUID,List<UUID>>();
        public Dictionary<UUID, List<UUID>> AvatarAnimations
        {
            get
            {
                return avatarAnimations;
            }
            set
            {
                avatarAnimations = value;
            }
        }

        public GridClient m_user;
        private RadegastInstance radegastInstance;

        public RadegastNetworkModule(RadegastInstance inst)
        {
            radegastInstance = inst;
            m_user = inst.Client;

            //dm m_user.Settings.USE_LLSD_LOGIN = true;

            //m_user.Settings.STORE_LAND_PATCHES = true;
            //m_user.Settings.MULTIPLE_SIMS = false;
            //m_user.Settings.OBJECT_TRACKING = true;
            //m_user.Settings.AVATAR_TRACKING = true;
            //dm  m_user.Settings.USE_TEXTURE_CACHE = false;
            //m_user.Settings.USE_TEXTURE_CACHE = true;
            //m_user.Settings.
            //dm   m_user.Settings.ALWAYS_DECODE_OBJECTS = false;

            //m_user.Settings.SEND_AGENT_UPDATES = true;
            //m_user.Settings.SEND_AGENT_THROTTLE = true;
            //m_user.Settings.SEND_PINGS = true;

            // m_user.Self.Movement.Camera.Far = 512.0f;

            //dm m_user.Settings.MAX_CONCURRENT_TEXTURE_DOWNLOADS = 2;
            //dm m_user.Settings.PIPELINE_REQUEST_TIMEOUT = 30 * 1000;

            m_user.Network.OnConnected += gridConnectedCallback;
            m_user.Network.OnDisconnected += disconnectedCallback;
            m_user.Network.OnSimConnected += simConnectedCallback;
            m_user.Network.OnLogin += loginStatusCallback;
            m_user.Terrain.OnLandPatch += landPatchCallbackCulled;
           // m_user.Self.OnChat += chatCallback;
            m_user.Objects.OnNewAvatar += newAvatarCallback;
            m_user.Objects.OnNewPrim += newPrimCulled;
            m_user.Objects.OnObjectKilled += objectKilledCallbackCulled;
            m_user.Network.OnLogin += loginCallback;
            m_user.Objects.OnObjectUpdated += objectUpdatedCallback;
            //m_user.Assets.OnImageReceived += imageReceivedCallback;
            //m_user.Friends.OnFriendNamesReceived += Friends_OnFriendNamesReceived;
            //m_user.Friends.OnFriendOnline += Friends_OnFriendOnline;
            //m_user.Friends.OnFriendOffline += Friends_OnFriendOffline;

            //m_user.Assets.RequestImage(
            //m_user.Assets.Cache..RequestImage(UUID.Zero, ImageType.Normal);
            
            m_user.Network.RegisterCallback(OpenMetaverse.Packets.PacketType.AvatarAnimation, AvatarAnimationHandler);

        }

        private void Friends_OnFriendOffline(FriendInfo friend)
        {
            if( OnFriendsListUpdate != null )
            {
                OnFriendsListUpdate();
            }
        }

        private void Friends_OnFriendOnline(FriendInfo friend)
        {
            if (OnFriendsListUpdate != null)
            {
                OnFriendsListUpdate();
            }
        }

        private void Friends_OnFriendNamesReceived(Dictionary<UUID, string> names)
        {
            if (OnFriendsListUpdate != null)
            {
                OnFriendsListUpdate();
            }
        }

        public InternalDictionary<UUID, FriendInfo> Friends
        {
            get
            {
                return m_user.Friends.FriendList;
                //return null;
            }
        }

        public void AvatarAnimationHandler(OpenMetaverse.Packets.Packet packet, Simulator sim)
        {
            // When animations for any avatar are received put them in the AvatarAnimations dictionary
            // in this module. They should be processed and deleted inbetween frames in the main frame loop
            // or deleted when an avatar is deleted from the scene.
            AvatarAnimationPacket animation = (AvatarAnimationPacket)packet;

            UUID avatarID = animation.Sender.ID;
            List<UUID> currentAnims = new List<UUID>();

            for (int i = 0; i < animation.AnimationList.Length; i++)
                currentAnims.Add(animation.AnimationList[i].AnimID);

            lock (AvatarAnimations)
            {
                if (AvatarAnimations.ContainsKey(avatarID))
                    AvatarAnimations[avatarID] = currentAnims;
                else
                    AvatarAnimations.Add(avatarID, currentAnims);
            }
        }

        public void loginStatusCallback(LoginStatus login, string message)
        {
            if (login == LoginStatus.Failed)
            {
                m_log.ErrorFormat("[CONNECTION]: Login Failed:{0}",message);
            }
        }

        private void imageReceivedCallback(TextureRequestState state, AssetTexture asset)
        {
            if (state == TextureRequestState.Timeout)
            {
                // need a re-request if a texture times out but doing it here borks libomv
                //m_user.Assets.RequestImage(asset.AssetID, ImageType.Normal, imageReceivedCallback);
                return;
            }

            if (state != TextureRequestState.Finished)
                return;

            if (OnTextureDownloaded != null)
            {

                VTexture texture = new VTexture();
                texture.TextureId = asset.AssetID;

                ManagedImage managedImage;
                Image tempImage;

                try
                {
                    if (OpenJPEG.DecodeToImage(asset.AssetData, out managedImage, out tempImage))
                    {
                        Bitmap textureBitmap = new Bitmap(tempImage.Width, tempImage.Height, PixelFormat.Format32bppArgb);
                        Graphics graphics = Graphics.FromImage(textureBitmap);
                        graphics.DrawImage(tempImage, 0, 0);
                        graphics.Flush();
                        graphics.Dispose();
                        texture.Image = textureBitmap;
                        OnTextureDownloaded(texture);
                    }
                }
                catch (Exception e)
                {
                    m_log.Error(":( :( :( :( got exception decoding image ): ): ): ):\nException: " + e.ToString());
                }
            }
        }
        private void objectKilledCallback(Simulator simulator, uint objectID)
        {
            if (OnObjectRemove != null)
            {
                OnObjectRemove(new VSimulator(simulator), objectID);
            }
        }

        public void Login(string loginURI, string username, string password, string startlocation)
        {

            string firstname;
            string lastname;

            this.loginURI = loginURI;
            this.username = username;
            this.password = password;
            this.startlocation = startlocation;

            Util.separateUsername(username, out firstname, out lastname);

            this.firstName = firstname;
            this.lastName = lastname;


            LoginParams loginParams = getLoginParams(loginURI, username, password, startlocation);

            m_user.Network.BeginLogin(loginParams);
        }

        private void gridConnectedCallback(object sender)
        {
           
            m_user.Appearance.SetPreviousAppearance(false);



            if (OnConnected != null)
            {
                OnConnected();
            }
        }
        private void simConnectedCallback(Simulator sender)
        {
            m_user.Throttle.Total = 600000;
            m_user.Throttle.Land = 80000;
            m_user.Throttle.Task = 200000;
            m_user.Throttle.Texture = 100000;
            m_user.Throttle.Wind = 10000;
            m_user.Throttle.Resend = 100000;
            m_user.Throttle.Asset = 100000;
            m_user.Throttle.Cloud = 10000;
            m_user.Self.Movement.Camera.Far = 64f;

            // this line creates a not found exception but for some unknown reason allows more prims 
            // to show up on the linden grid
            //m_user.Self.Movement.Camera.Position = m_user.Network.CurrentSim.AvatarPositions[m_user.Self.AgentID];
            
            // this line *should* work in all cases
            m_user.Self.Movement.Camera.Position = m_user.Self.SimPosition;

            SetHeightWidth(768, 1024);
            if (OnSimulatorConnected != null)
            {
                OnSimulatorConnected(new VSimulator(sender));
            }
        }
        private void loginCallback(LoginStatus status, string message)
        {
            if (OnLoggedIn != null)
            {
                OnLoggedIn((LoginStatus)status, message);
            }
        }
        public void Logout()
        {
            if (m_user.Network.Connected)
            {
                m_user.Network.Logout();
            }
        }
        public void disconnectedCallback(NetworkManager.DisconnectType reason, string message)
        {
            m_log.ErrorFormat("[CONNECTION]: Disconnected{0}: Message:{1}",reason.ToString(), message);
        }
        public bool Connected
        {
            get { return m_user.Network.Connected; }
        }
        public void Whisper(string message)
        {
            m_user.Self.Chat(message, 0, ChatType.Whisper);
        }

        public void Say(string message)
        {
            m_user.Self.Chat(message, 0, ChatType.Normal);
        }

        public void Shout(string message)
        {
            m_user.Self.Chat(message, 0, ChatType.Shout);
        }

        public void Teleport(string region, float x, float y, float z)
        {
            m_user.Self.Teleport(region, new Vector3(x, y, z));
        }

        private LoginParams getLoginParams(string loginURI, string username, string password, string startlocation)
        {
            string firstname;
            string lastname;

            Util.separateUsername(username, out firstname, out lastname);

            LoginParams loginParams = m_user.Network.DefaultLoginParams(
                firstname, lastname, password, "IdealistViewer", "0.0.0.1");//Constants.Version);


            loginURI = Util.getSaneLoginURI(loginURI);
            
            if (startlocation.Length == 0)
            {

                if (!loginURI.EndsWith("/"))
                    loginURI += "/";

                string[] locationparse = loginURI.Split('/');
                try
                {
                    startlocation = locationparse[locationparse.Length - 2];
                    if (startlocation == locationparse[2])
                    {
                        startlocation = "last";
                    }
                    else
                    {
                        loginURI = "";
                        for (int i = 0; i < locationparse.Length - 2; i++)
                        {
                            loginURI += locationparse[i] + "/";
                        }
                    }

                }
                catch (Exception)
                {
                    startlocation = "last";
                }

            }
            else
            {
                

                //if (!loginURI.EndsWith("/"))
                //    loginURI += "/";

               // string[] locationparse = loginURI.Split('/');
               // try
               // {
               //     string end = locationparse[locationparse.Length - 2];
               //     if (end != locationparse[2])
               //     {
               //         loginURI = "";
               //         for (int i = 0; i < 3; i++)
               //         {
               //             if (locationparse[i].Length != 0 || i==1)
               //                 loginURI += locationparse[i] + "/";
               //         }
               //     }

                //}
               // catch (Exception)
                //{
                    //startlocation = "last";
                //    m_log.Warn("[URLPARSING]: Unable to parse URL provided!");
                //}


            }

            loginParams.URI = loginURI;
           

            if (startlocation != "last" && startlocation != "home")
                startlocation = "uri:" + startlocation + "&128&128&20";

            loginParams.Start = startlocation;

            return loginParams;
        }
        
        private void chatCallback(string message, ChatAudibleLevel audible, ChatType type, ChatSourceType sourcetype,
                                  string fromName, UUID id, UUID ownerid, Vector3 position)
        {
            // This is weird -- we get start/stop typing chats from
            // other avatars, and we get messages back that we sent.
            // (Tested on OpenSim r3187)
            // So we explicitly check for those cases here.
            if ((int)type < 4 && id != m_user.Self.AgentID)
            {
                m_log.Debug("Chat: " + fromName + ": " + message);
                if (OnChat != null)
                {
                    OnChat(message, audible, type, sourcetype,
                                      fromName, id, ownerid, position);
                }
            }
        }
        
        

        private void newPrim(SimulatorCull sc, Simulator simulator, Primitive prim, ulong regionHandle,
                             ushort timeDilation)
        {
            lock (sc)
            {

                List<Primitive> prims;
                if (prim.ParentID == 0)
                {
                    sc.ParentsSent[prim.LocalID] = prim;
                    if (sc.ChildsCulled.TryGetValue(prim.LocalID, out prims))
                    {
                        sc.ChildsCulled.Remove(prim.LocalID);
                        foreach (var p in prims)
                        {
                            newPrim(sc, simulator, p, regionHandle, timeDilation);
                        }
                    }
                    if (sc.PrimsAwaitingParent.TryGetValue(prim.LocalID, out prims))
                    {
                        sc.PrimsAwaitingParent.Remove(prim.LocalID);
                        foreach (var p in prims)
                        {
                            newPrim(sc, simulator, p, regionHandle, timeDilation);
                        }
                    }
                }
                else
                {
                    if (!sc.ChildsSent.TryGetValue(prim.ParentID, out prims))
                    {
                        sc.ChildsSent[prim.ParentID] = prims = new List<Primitive>();
                        prims.Add(prim);
                    }
                    else
                    {
                        if (!prims.Contains(prim))
                        {
                            prims.Add(prim);
                        }
                    }
                }
            }
            if (OnObjectAdd != null)
            {
                OnObjectAdd(new VSimulator(simulator), prim, regionHandle, timeDilation);
            }
        }

        
        private void landPatchCallback(Simulator simulator, int x, int y, int width, float[] data)
        {
            if (OnLandUpdate != null)
            {
                OnLandUpdate(new VSimulator(simulator), x, y, width, data);
            }
        }
        private void newAvatarCallback(Simulator simulator, Avatar avatar, ulong regionHandle,
                                       ushort timeDilation)
        {
            if (OnAvatarAdd != null)
            {
               //avatar.Velocity
                OnAvatarAdd(new VSimulator(simulator), avatar, regionHandle, timeDilation);
            }
        }
        private void objectUpdatedCallback(Simulator simulator, ObjectUpdate update, ulong regionHandle,
                                          ushort timeDilation)
        {
            if (simulator == m_user.Network.CurrentSim)
            {
                if (m_user.Self.LocalID == update.LocalID)
                {
                    OnSelfUpdated(simulator, update, regionHandle);
                }
            }
            if (OnObjectUpdate != null)
            {
                OnObjectUpdate(new VSimulator(simulator), update, regionHandle, timeDilation);
            }
        }

        public void RequestTexture(UUID assetID)
        {
            m_user.Assets.RequestImage(assetID, ImageType.Normal, imageReceivedCallback);
        }

        public void SendCameraViewMatrix(Vector3[] camdata)
        {

            //for (int i=0;i<camdata.Length; i++)
            //    if (Single.IsNaN(camdata[i].X) || Single.IsNaN(camdata[i].Y) || Single.IsNaN(camdata[i].Z))
            //        return;

            //    //m_user.Self.Movement.Camera.Position = pPosition;
            //    //m_user.Self.Movement.Camera.LookAt(pPosition, pTarget);
            //m_user.Self.Movement.Camera.AtAxis = camdata[1];
            //m_user.Self.Movement.Camera.LeftAxis = camdata[0];
            //m_user.Self.Movement.Camera.LeftAxis = camdata[2];

            Quaternion q = m_user.Self.Movement.HeadRotation;
            
            Vector3 myPos;
            myPos = m_user.Self.SimPosition;
            //try
            //{
            //    myPos = m_user.Network.CurrentSim.AvatarPositions[m_user.Self.AgentID];
            //}
            //catch (Exception e)
            //{
            //    m_log.Warn("[CAMERA] - unable to determine agent location", e);
            //    myPos = m_user.Self.SimPosition;
            //}

            Vector3 currPos = myPos + new Vector3(-1.0f, 0.0f, 0.0f) * q;
            Vector3 currTarget = myPos + new Vector3(1.0f, 0.0f, 0.0f) * q;

            //m_log.Debug("[CAMERA UPDATE] - pos: " + myPos.ToString() + " q: " + q.ToString());
            //m_log.Debug("[CAMERA UPDATE] - pos: " + myPos.ToString() + " cam: " + currPos.ToString() + " tgt: " + currTarget.ToString());

            //Vector3 lastCameraPos = m_user.Self.Movement.Camera.Position;
            
            if (Vector3.Distance(lastCameraPos, myPos) > 1.0f || Vector3.Distance(lastCameraTarget, currTarget) > 1.0f)
            {
                
                m_user.Self.Movement.Camera.LookDirection(currTarget);
                m_user.Self.Movement.Camera.Position = currPos;

                lastCameraPos = currPos;
                lastCameraTarget = currTarget;
                //m_log.Debug("[CAMERA UPDATE] - " + m_user.Self.Movement.Camera.Position.ToString() + ", " + currTarget.ToString());
                m_log.Debug("[CAMERA UPDATE] - pos: " + myPos.ToString() + " cam: " + currPos.ToString() + " tgt: " + currTarget.ToString());

            }
            
        }
        
        public void SetHeightWidth(uint height, uint width)
        {
            m_user.Self.SetHeightWidth((ushort)height, (ushort)width);
        }

        public UUID GetSelfUUID
        {
            get { return m_user.Self.AgentID; }
        }

        public bool StraffLeft
        {
            set {m_user.Self.Movement.LeftPos = value;}
            get { return m_user.Self.Movement.LeftPos; }
        }
        public bool StraffRight
        {
            set { m_user.Self.Movement.LeftNeg = value; }
            get { return m_user.Self.Movement.LeftNeg; }
        }

        public void UpdateFromHeading(double heading)
        {
            m_user.Self.Movement.UpdateFromHeading(heading ,false);
        }

        public void TurnToward(Vector3 target)
        {
            m_user.Self.Movement.TurnToward(target);
        }

        public bool Forward
        {
            set {m_user.Self.Movement.AtPos = value;}
            get { return m_user.Self.Movement.AtPos; }
        }

        public bool Backward
        {
            set { m_user.Self.Movement.AtNeg = value; }
            get { return m_user.Self.Movement.AtNeg; }
        }

        public bool Jump
        {
            set { m_user.Self.Jump(value); }
        }

        public bool Flying
        {
            get { return m_user.Self.Movement.Fly; }
            set { m_user.Self.Movement.Fly = value; }
        }

        public bool Up
        {
            get { return m_user.Self.Movement.UpPos; }
            set { m_user.Self.Movement.UpPos = value; }
        }

        public bool Down
        {
            get { return m_user.Self.Movement.UpNeg; }
            set { m_user.Self.Movement.UpNeg = value; }
        }

        public bool MultipleSims
        {
            get
            {
                return m_user.Settings.MULTIPLE_SIMS;
            }
            set
            {
                m_user.Settings.MULTIPLE_SIMS=value;
            }
        }


        #region IProtocol Members


        public void Process()
        {
        }

        #endregion
    }
}
