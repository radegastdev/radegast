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
using System.Text;
using FMOD;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast.Media
{
    public class MediaManager : MediaObject
    {
        /// <summary>
        /// Indicated wheather spund sytem is ready for use
        /// </summary>
        public bool SoundSystemAvailable { get { return soundSystemAvailable; } }
        private bool soundSystemAvailable = false;
        private Thread soundThread;
        private Thread listenerThread;
        public RadegastInstance Instance;

        private List<MediaObject> sounds = new List<MediaObject>();

        public MediaManager(RadegastInstance instance)
            : base()
        {
            this.Instance = instance;
            manager = this;

            loadCallback = new FMOD.SOUND_NONBLOCKCALLBACK(DispatchNonBlockCallback);
            endCallback = new FMOD.CHANNEL_CALLBACK(DispatchEndCallback);
            allBuffers = new Dictionary<UUID,BufferSound>();

            // Start the background thread that does all the FMOD calls.
            soundThread = new Thread(new ThreadStart(CommandLoop));
            soundThread.IsBackground = true;
            soundThread.Name = "SoundThread";
            soundThread.Start();

            // Start the background thread that updates listerner position.
            listenerThread = new Thread(new ThreadStart(ListenerUpdate));
            listenerThread.IsBackground = true;
            listenerThread.Name = "ListenerThread";
            listenerThread.Start();

            // Initial inworld-sound setting comes from Config.
            ObjectEnable = true;
        }

        /// <summary>
        /// Thread that processes FMOD calls.
        /// </summary>
        private void CommandLoop()
        {
            SoundDelegate action = null;

            // Initialze a bunch of static values
            UpVector.x = 0.0f;
            UpVector.y = 1.0f;
            UpVector.z = 0.0f;
            ZeroVector.x = 0.0f;
            ZeroVector.y = 0.0f;
            ZeroVector.z = 0.0f;

            allSounds = new Dictionary<IntPtr,MediaObject>();
            allChannels = new Dictionary<IntPtr, MediaObject>();

            // Initialize the FMOD sound package
            InitFMOD();
            if (!this.soundSystemAvailable) return;

            // Initialize the command queue.
            queue = new Queue<SoundDelegate>();

            while (true)
            {
                // Wait for something to show up in the queue.
                lock (queue)
                {
                    while (queue.Count == 0)
                    {
                        Monitor.Wait(queue);
                    }
                    action = queue.Dequeue();
                }

                // We have an action, so call it.
                try
                {
                    action();
                    action = null;
                }
                catch (Exception e)
                {
                    Logger.Log("Error in sound action:\n    " + e.Message + "\n" + e.StackTrace,
                        Helpers.LogLevel.Error);
                }
            }
        }

        /// <summary>
        /// Initialize the FMOD sound system.
        /// </summary>
        private void InitFMOD()
        {
            try
            {
                FMODExec(FMOD.Factory.System_Create(ref system));
                uint version = 0;
                FMODExec(system.getVersion(ref version));

                if (version < FMOD.VERSION.number)
                    throw new MediaException("You are using an old version of FMOD " +
                        version.ToString("X") +
                        ".  This program requires " +
                        FMOD.VERSION.number.ToString("X") + ".");

                // Assume no special hardware capabilities except 5.1 surround sound.
                FMOD.CAPS caps = FMOD.CAPS.NONE;
                FMOD.SPEAKERMODE speakermode = FMOD.SPEAKERMODE._5POINT1;

                // Get the capabilities of the driver.
                int minfrequency = 0, maxfrequency = 0;
                StringBuilder name = new StringBuilder(128);
                FMODExec(system.getDriverCaps(0, ref caps,
                    ref minfrequency,
                    ref maxfrequency,
                    ref speakermode)
                );

                // Set FMOD speaker mode to what the driver supports.
                FMODExec(system.setSpeakerMode(speakermode));

                // Forcing the ALSA sound system on Linux seems to avoid a CPU loop
                if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                    FMODExec(system.setOutput(FMOD.OUTPUTTYPE.ALSA));

                // The user has the 'Acceleration' slider set to off, which
                // is really bad for latency.  At 48khz, the latency between
                // issuing an fmod command and hearing it will now be about 213ms.
                if ((caps & FMOD.CAPS.HARDWARE_EMULATED) == FMOD.CAPS.HARDWARE_EMULATED)
                {
                    FMODExec(system.setDSPBufferSize(1024, 10));
                }

                // Get driver information so we can check for a wierd one.
                FMOD.GUID guid = new FMOD.GUID();
                FMODExec(system.getDriverInfo(0, name, 128, ref guid));

                // Sigmatel sound devices crackle for some reason if the format is pcm 16bit.
                // pcm floating point output seems to solve it.
                if (name.ToString().IndexOf("SigmaTel") != -1)
                {
                    FMODExec(system.setSoftwareFormat(
                        48000,
                        FMOD.SOUND_FORMAT.PCMFLOAT,
                        0, 0,
                        FMOD.DSP_RESAMPLER.LINEAR)
                    );
                }

                // Try to initialize with all those settings, and Max 32 channels.
                FMOD.RESULT result = system.init(32, FMOD.INITFLAG.NORMAL, (IntPtr)null);
                if (result == FMOD.RESULT.ERR_OUTPUT_CREATEBUFFER)
                {
                    // Can not handle surround sound - back to Stereo.
                    FMODExec(system.setSpeakerMode(FMOD.SPEAKERMODE.STEREO));

                    // And init again.
                    FMODExec(system.init(
                        32,
                        FMOD.INITFLAG.NORMAL,
                        (IntPtr)null)
                    );
                }

                // Set real-world effect scales.
                FMODExec(system.set3DSettings(
                    1.0f,   // Doppler scale
                    1.0f,   // Distance scale is meters
                    1.0f)   // Rolloff factor
                );

                soundSystemAvailable = true;
                Logger.Log("Initialized FMOD Ex", Helpers.LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to initialize the sound system: ", Helpers.LogLevel.Warning, ex);
            }
        }

        public override void Dispose()
        {
            lock (sounds)
            {
                for (int i = 0; i < sounds.Count; i++)
                {
                    if (!sounds[i].Disposed)
                        sounds[i].Dispose();
                }
                sounds.Clear();
            }

            sounds = null;

            if (system != null)
            {
                Logger.Log("FMOD interface stopping", Helpers.LogLevel.Info);
                system.release();
                system = null;
            }

            base.Dispose();
        }

        /// <summary>
        /// Thread to update listener position and generally keep
        /// FMOD up to date.
        /// </summary>
        private void ListenerUpdate()
        {
            // Notice changes in position or direction.
            Vector3 lastpos = new Vector3(0.0f, 0.0f, 0.0f );
            float lastface = 0.0f;

            while (true)
            {
                // Two updates per second.
                Thread.Sleep(500);

                if (system == null) continue;

                AgentManager my = Instance.Client.Self;
                Vector3 newPosition = new Vector3(my.SimPosition);
                float newFace = my.SimRotation.W;

                // If we are standing still, nothing to update now, but
                // FMOD needs a 'tick' anyway for callbacks, etc.  In looping
                // 'game' programs, the loop is the 'tick'.   Since Radegast
                // uses events and has no loop, we use this position update
                // thread to drive the FMOD tick.  Have to move more than
                // 500mm or turn more than 10 desgrees to bother with.
                //
                if (newPosition.ApproxEquals(lastpos, 0.5f) &&
                    Math.Abs(newFace - lastface) < 0.2)
                {
                    invoke(new SoundDelegate(delegate
                    {
                        FMODExec(system.update());
                    }));
                    continue;
                }

                // We have moved or turned.  Remember new position.
                lastpos = newPosition;
                lastface = newFace;

                // Convert coordinate spaces.
                FMOD.VECTOR listenerpos = FromOMVSpace(newPosition);

                // Get azimuth from the facing Quaternion.  Note we assume the
                // avatar is standing upright.  Avatars in unusual positions
                // hear things from unpredictable directions.
                // By definition, facing.W = Cos( angle/2 )
                // With angle=0 meaning East.
                double angle = 2.0 * Math.Acos(newFace);

                // Construct facing unit vector in FMOD coordinates.
                // Z is East, X is South, Y is up.
                FMOD.VECTOR forward = new FMOD.VECTOR();
                forward.x = (float)Math.Sin(angle); // South
                forward.y = 0.0f;
                forward.z = (float)Math.Cos(angle); // East

                Logger.Log(
                    String.Format(
                        "Standing at <{0:0.0},{1:0.0},{2:0.0}> facing {3:d}",
                            listenerpos.x,
                            listenerpos.y,
                            listenerpos.z,
                            (int)(angle * 180.0 / 3.141592)),
                    Helpers.LogLevel.Debug);

                // Tell FMOD the new orientation.
                invoke( new SoundDelegate( delegate
                {
                    FMODExec( system.set3DListenerAttributes(
                        0,
                        ref listenerpos,	// Position
                        ref ZeroVector,		// Velocity
                        ref forward,		// Facing direction
                        ref UpVector ));	// Top of head

                    FMODExec(system.update());
                }));
            }
        }

        /// <summary>
        /// Handle request to play a sound, which might (or mioght not) have been preloaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sound_SoundTrigger(object sender, SoundTriggerEventArgs e)
        {
            if (e.SoundID == UUID.Zero) return;

            Logger.Log("Trigger sound " + e.SoundID.ToString() +
                " in object " + e.ObjectID.ToString(),
                Helpers.LogLevel.Debug);

            new BufferSound(
                e.ObjectID,
                e.SoundID,
                false,
                true,
                e.Position,
                e.Gain * ObjectVolume);
        }

        /// <summary>
        /// Handle sound attached to an object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sound_AttachedSound(object sender, AttachedSoundEventArgs e)
        {
            // We seem to get a lot of these zero sounds.
            if (e.SoundID == UUID.Zero) return;

            if ((e.Flags & SoundFlags.Stop) == SoundFlags.Stop)
            {
                BufferSound.Kill(e.SoundID);
                return;
            }

            // This event tells us the Object ID, but not the Prim info directly.
            // So we look it up in our internal Object memory.
            Simulator sim = Instance.Client.Network.CurrentSim;
            Primitive p = sim.ObjectsPrimitives.Find((Primitive p2) => { return p2.ID == e.ObjectID; });
            if (p==null) return;

            // If this is a child prim, its position is relative to the root.
            Vector3 fullPosition = p.Position;
            if (p.ParentID != 0)
            {
                Primitive parentP;
                sim.ObjectsPrimitives.TryGetValue(p.ParentID, out parentP);
                if (parentP == null) return;
                fullPosition += parentP.Position;
            }

            new BufferSound(
                e.ObjectID,
                e.SoundID,
                (e.Flags & SoundFlags.Loop) == SoundFlags.Loop,
                true,
                fullPosition,
                e.Gain * ObjectVolume);
        }

        
        /// <summary>
        /// Handle request to preload a sound for playing later.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sound_PreloadSound(object sender, PreloadSoundEventArgs e)
        {
            if (e.SoundID == UUID.Zero) return;

            new BufferSound( e.SoundID );
        }

        private void Objects_ObjectPropertiesUpdated(object sender, ObjectPropertiesUpdatedEventArgs e)
        {
            HandleObjectSound(e.Prim, e.Simulator );
        }
     
     /// <summary>
        /// Handle object updates, looking for sound events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Objects_ObjectUpdate(object sender, PrimEventArgs e)
        {
            HandleObjectSound(e.Prim, e.Simulator );
        }

        /// <summary>
        /// Handle deletion of a noise-making object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Objects_KillObject(object sender, KillObjectEventArgs e)
        {
            Primitive p = null;
            if (!e.Simulator.ObjectsPrimitives.TryGetValue(e.ObjectLocalID, out  p)) return;

            // Objects without sounds are not interesting.
            if (p.Sound == null) return;
            if (p.Sound == UUID.Zero) return;

            BufferSound.Kill(p.Sound);
        }

        /// <summary>
        /// Common object sound processing for various Update events
        /// </summary>
        /// <param name="p"></param>
        /// <param name="s"></param>
        private void HandleObjectSound(Primitive p, Simulator s)
        {
            // Objects without sounds are not interesting.
            if (p.Sound == null) return;
            if (p.Sound == UUID.Zero) return;
           
            if ((p.SoundFlags & SoundFlags.Stop) == SoundFlags.Stop)
            {
                BufferSound.Kill(p.ID);
                return;
            }

            // If this is a child prim, its position is relative to the root prim.
            Vector3 fullPosition = p.Position;
            if (p.ParentID != 0)
            {
                Primitive parentP;
                if (!s.ObjectsPrimitives.TryGetValue(p.ParentID, out parentP)) return;
                fullPosition += parentP.Position;
            }
  
            // See if this is an update to  something we already know about.
            if (allBuffers.ContainsKey(p.ID))
            {
                // Exists already, so modify existing sound.
                //TODO posible to change sound on the same object.  Key by Object?
                BufferSound snd = allBuffers[p.ID];
                snd.Volume = p.SoundGain * ObjectVolume;
                snd.Position = fullPosition;
            }
            else
            {
                // Does not exist, so create a new one.
                new BufferSound(
                    p.ID,
                    p.Sound,
                    (p.SoundFlags & SoundFlags.Loop) == SoundFlags.Loop,
                    true,
                    fullPosition, //Instance.State.GlobalPosition(e.Simulator, fullPosition),
                    p.SoundGain * ObjectVolume);
            }
        }

        /// <summary>
        /// Control the volume of all inworld sounds
        /// </summary>
        public float ObjectVolume
        {
            set
            {
                AllObjectVolume = value;
                BufferSound.AdjustVolumes();
            }
            get { return AllObjectVolume; }
        }

        private bool m_objectEnabled = true;
        /// <summary>
        /// Enable and Disable inworld sounds
        /// </summary>
        public bool ObjectEnable
        {
            set
            {
                try
                {
                    if (value)
                    {
                        // Subscribe to events about inworld sounds
                        Instance.Client.Sound.SoundTrigger += new EventHandler<SoundTriggerEventArgs>(Sound_SoundTrigger);
                        Instance.Client.Sound.AttachedSound += new EventHandler<AttachedSoundEventArgs>(Sound_AttachedSound);
                        Instance.Client.Sound.PreloadSound += new EventHandler<PreloadSoundEventArgs>(Sound_PreloadSound);
                        Instance.Client.Objects.ObjectUpdate += new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
                        Instance.Client.Objects.ObjectPropertiesUpdated += new EventHandler<ObjectPropertiesUpdatedEventArgs>(Objects_ObjectPropertiesUpdated);
                        Instance.Client.Objects.KillObject += new EventHandler<KillObjectEventArgs>(Objects_KillObject);
                        Instance.Client.Network.SimChanged += new EventHandler<SimChangedEventArgs>(Network_SimChanged);
                        Logger.Log("Inworld sound enabled", Helpers.LogLevel.Info);
                    }
                    else
                    {
                        // Subscribe to events about inworld sounds
                        Instance.Client.Sound.SoundTrigger -= new EventHandler<SoundTriggerEventArgs>(Sound_SoundTrigger);
                        Instance.Client.Sound.AttachedSound -= new EventHandler<AttachedSoundEventArgs>(Sound_AttachedSound);
                        Instance.Client.Sound.PreloadSound -= new EventHandler<PreloadSoundEventArgs>(Sound_PreloadSound);
                        Instance.Client.Objects.ObjectUpdate -= new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
                        Instance.Client.Objects.ObjectPropertiesUpdated -= new EventHandler<ObjectPropertiesUpdatedEventArgs>(Objects_ObjectPropertiesUpdated);
                        Instance.Client.Objects.KillObject -= new EventHandler<KillObjectEventArgs>(Objects_KillObject);
                        Instance.Client.Network.SimChanged -= new EventHandler<SimChangedEventArgs>(Network_SimChanged);

                        // Stop all running sounds
                        BufferSound.KillAll();

                        Logger.Log("Inworld sound disabled", Helpers.LogLevel.Info);
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error on enable/disable: "+e.Message);
                }

                m_objectEnabled = value;
            }
            get { return m_objectEnabled; }
        }

        /// <summary>
        /// Watch for Teleports to cancel all the old sounds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Network_SimChanged(object sender, SimChangedEventArgs e)
        {
            BufferSound.KillAll();
        }


    }

    public class MediaException : Exception
    {
        public MediaException(string msg)
            : base(msg)
        {
        }
    }
}
