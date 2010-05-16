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
        RadegastInstance instance;

        private List<MediaObject> sounds = new List<MediaObject>();

        public MediaManager(RadegastInstance instance)
            : base()
        {
            this.instance = instance;

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
        }

        private void CommandLoop()
        {
            SoundDelegate action = null;

            UpVector.x = 0.0f;
            UpVector.y = 1.0f;
            UpVector.z = 0.0f;
            ZeroVector.x = 0.0f;
            ZeroVector.y = 0.0f;
            ZeroVector.z = 0.0f;

            allSounds = new Dictionary<IntPtr,MediaObject>();
            allChannels = new Dictionary<IntPtr, MediaObject>();

            InitFMOD();
            if (!this.soundSystemAvailable) return;

            // Initialize the command queue.
            queue = new Queue<SoundDelegate>();

            try
            {
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
                    action();
                    action = null;
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Sound shutdown " + e.Message);
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
            Vector3 lastpos = new Vector3(0.0f, 0.0f, 0.0f );
            float lastface = 0.0f;

            while (true)
            {
                Thread.Sleep(500);

                if (system == null) continue;

                AgentManager my = instance.Client.Self;

                // If we are standing still, nothing to update now, but
                // FMOD needs a 'tick' anyway for callbacks, etc.  In looping
                // 'game' programs, the loop is the 'tick'.   Since Radegast
                // uses events and has no loop, we use this position update
                // thread to drive the tick.
                if (my.SimPosition.Equals(lastpos) &&
                    my.Movement.BodyRotation.W == lastface)
                {
                    invoke(new SoundDelegate(delegate
                    {
                        system.update();
                    }));
                    continue;
                }

                lastpos = my.SimPosition;
                lastface = my.Movement.BodyRotation.W;

                // Convert coordinate spaces.
                FMOD.VECTOR listenerpos = FromOMVSpace(my.SimPosition);

                // Get azimuth from the facing Quaternion.  Note we assume the
                // avatar is standing upright.  Avatars in unusual positions
                // hear things from unpredictable directions.
                // By definition, facing.W = Cos( angle/2 )
                double angle = 2.0 * Math.Acos(my.Movement.BodyRotation.W);
                FMOD.VECTOR forward = new FMOD.VECTOR();
                forward.x = (float)Math.Asin(angle);
                forward.y = 0.0f;
                forward.z = (float)Math.Acos(angle);

                // Tell FMOD the new orientation.
                invoke( new SoundDelegate( delegate
                {
                    FMODExec( system.set3DListenerAttributes(
                        0,
                        ref listenerpos,	// Position
                        ref ZeroVector,		// Velocity
                        ref forward,		// Facing direction
                        ref UpVector ));	// Top of head

                    system.update();
                }));
            }
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
