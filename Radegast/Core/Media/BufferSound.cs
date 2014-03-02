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
// $Id: Sound.cs 502 2010-03-14 23:13:46Z latifer $
//
// Uncomment this to get lots more logging
//#define TRACE_SOUND
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using FMOD;
using OpenMetaverse;
using OpenMetaverse.Assets;
using System.Threading;

namespace Radegast.Media
{

    public class BufferSound : MediaObject
    {
        private UUID Id;
        private UUID ContainerId;
        private Boolean prefetchOnly = false;
        private FMOD.MODE mode;
        public Sound Sound { get { return sound; } }
        private Boolean loopSound = false;
        /// <summary>
        /// The individual volume setting for THIS object
        /// </summary>
        private float volumeSetting = 0.5f;

        /// <summary>
        /// Creates a new sound object
        /// </summary>
        /// <param name="system">Sound system</param>
        public BufferSound(UUID objectId, UUID soundId, bool loop, bool global, Vector3 worldpos, float vol)
            : base()
        {
            InitBuffer(objectId, soundId, loop, global, worldpos, vol);
        }

        public BufferSound(UUID objectId, UUID soundId, bool loop, bool global, Vector3d worldpos, float vol)
            : base()
        {
            InitBuffer(objectId, soundId, loop, global, new Vector3(worldpos), vol);
        }

        private void InitBuffer(UUID objectId, UUID soundId, bool loop, bool global, Vector3 worldpos, float vol)
        {
            if (manager == null || !manager.SoundSystemAvailable) return;

            // Do not let this get garbage-collected.
            lock (allBuffers)
                allBuffers[objectId] = this;

            ContainerId = objectId;
            Id = soundId;
            position = FromOMVSpace(worldpos);
            volumeSetting = vol;
            loopSound = loop;

            Logger.Log(
                String.Format(
                    "Playing sound at <{0:0.0},{1:0.0},{2:0.0}> ID {3}",
                    position.x,
                    position.y,
                    position.z,
                    Id.ToString()),
                Helpers.LogLevel.Debug);

            // Set flags to determine how it will be played.
            mode = FMOD.MODE.SOFTWARE | // Need software processing for all the features
                FMOD.MODE._3D |         // Need 3D effects for placement
                FMOD.MODE.OPENMEMORY;   // Use sound data in memory

            // Set coordinate space interpretation.
            if (global)
                mode |= FMOD.MODE._3D_WORLDRELATIVE;
            else
                mode |= FMOD.MODE._3D_HEADRELATIVE;

            if (loopSound)
                mode |= FMOD.MODE.LOOP_NORMAL;

            // Fetch the sound data.
            manager.Instance.Client.Assets.RequestAsset(
                Id,
                AssetType.Sound,
                false,
                new AssetManager.AssetReceivedCallback(Assets_OnSoundReceived));
        }

        public static void Kill(UUID id)
        {
            if (allBuffers.ContainsKey(id))
            {
                BufferSound bs = allBuffers[id];
                bs.StopSound(true);
            }
        }

        /// <summary>
        /// Stop all playing sounds in the environment
        /// </summary>
        public static void KillAll()
        {
            // Make a list from the dictionary so we do not get a deadlock
            // on it when removing entries.
            List<BufferSound> list = new List<BufferSound>(allBuffers.Values);

            foreach (BufferSound s in list)
            {
                s.StopSound();
            }

            List<MediaObject> objs = new List<MediaObject>(allChannels.Values);
            foreach (MediaObject obj in objs)
            {
                if (obj is BufferSound)
                    ((BufferSound)obj).StopSound();
            }
        }

        /// <summary>
        /// Adjust volumes of all playing sounds to observe the new global sound volume
        /// </summary>
        public static void AdjustVolumes()
        {
            // Make a list from the dictionary so we do not get a deadlock
            List<BufferSound> list = new List<BufferSound>(allBuffers.Values);

            foreach (BufferSound s in list)
            {
                s.AdjustVolume();
            }
        }

        /// <summary>
        /// Adjust the volume of THIS sound when all are being adjusted.
        /// </summary>
        private void AdjustVolume()
        {
            Volume = volumeSetting * AllObjectVolume;
        }

        // A simpler constructor used by PreFetchSound.
        public BufferSound(UUID soundId)
            : base()
        {
            prefetchOnly = true;
            ContainerId = UUID.Zero;
            Id = soundId;

            manager.Instance.Client.Assets.RequestAsset(
                Id,
                AssetType.Sound,
                false,
                new AssetManager.AssetReceivedCallback(Assets_OnSoundReceived));
        }

        /// <summary>
        /// Releases resources of this sound object
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /**
         * Handle arrival of a sound resource.
         */
        void Assets_OnSoundReceived(AssetDownload transfer, Asset asset)
        {
            if (transfer.Success)
            {
                // If this was a Prefetch, just stop here.
                if (prefetchOnly)
                {
                    return;
                }

                //                Logger.Log("Opening sound " + Id.ToString(), Helpers.LogLevel.Debug);

                // Decode the Ogg Vorbis buffer.
                AssetSound s = asset as AssetSound;
                s.Decode();
                byte[] data = s.AssetData;

                // Describe the data to FMOD
                extraInfo.length = (uint)data.Length;
                extraInfo.cbsize = Marshal.SizeOf(extraInfo);

                invoke(new SoundDelegate(delegate
                {
                    try
                    {
                        // Create an FMOD sound of this Ogg data.
                        FMODExec(system.createSound(
                            data,
                            mode,
                            ref extraInfo,
                            ref sound));

                        // Register for callbacks.
                        RegisterSound(sound);


                        // If looping is requested, loop the entire thing.
                        if (loopSound)
                        {
                            uint soundlen = 0;
                            FMODExec(sound.getLength(ref soundlen, TIMEUNIT.PCM));
                            FMODExec(sound.setLoopPoints(0, TIMEUNIT.PCM, soundlen - 1, TIMEUNIT.PCM));
                            FMODExec(sound.setLoopCount(-1));
                        }

                        // Allocate a channel and set initial volume.  Initially paused.
                        FMODExec(system.playSound(CHANNELINDEX.FREE, sound, true, ref channel));
#if TRACE_SOUND
                    Logger.Log(
                        String.Format("Channel {0} for {1} assigned to {2}",
                             channel.getRaw().ToString("X"),
                             sound.getRaw().ToString("X"),
                             Id),
                        Helpers.LogLevel.Debug);
#endif
                        RegisterChannel(channel);

                        FMODExec(channel.setVolume(volumeSetting * AllObjectVolume));

                        // Take note of when the sound is finished playing.
                        FMODExec(channel.setCallback(endCallback));

                        // Set attenuation limits.
                        FMODExec(sound.set3DMinMaxDistance(
                                    1.2f,       // Any closer than this gets no louder
                                    100.0f));     // Further than this gets no softer.

                        // Set the sound point of origin.  This is in SIM coordinates.
                        FMODExec(channel.set3DAttributes(ref position, ref ZeroVector));

                        // Turn off pause mode.  The sound will start playing now.
                        FMODExec(channel.setPaused(false));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error playing sound: ", Helpers.LogLevel.Error, ex);
                    }
                }));
            }
            else
            {
                Logger.Log("Failed to download sound: " + transfer.Status.ToString(),
                                        Helpers.LogLevel.Error);
            }
        }

        /// <summary>
        /// Handles stop sound even from FMOD
        /// </summary>
        /// <returns>RESULT.OK</returns>
        protected override RESULT EndCallbackHandler()
        {
            StopSound();
            return RESULT.OK;
        }

        protected void StopSound()
        {
            StopSound(false);
        }

        protected void StopSound(bool blocking)
        {
            ManualResetEvent stopped = null;
            if (blocking)
                stopped = new ManualResetEvent(false);

            finished = true;

            invoke(new SoundDelegate(delegate
            {
                string chanStr = "none";
                string soundStr = "none";

                // Release the buffer to avoid a big memory leak.
                if (channel != null)
                {
                    lock (allChannels)
                        allChannels.Remove(channel.getRaw());
                    chanStr = channel.getRaw().ToString("X");
                    channel.stop();
                    channel = null;
                }

                if (sound != null)
                {
                    soundStr = sound.getRaw().ToString("X");
                    sound.release();
                    sound = null;
                }
#if TRACE_SOUND
                Logger.Log(String.Format("Removing channel {0} sound {1} ID {2}",
                    chanStr,
                    soundStr,
                    Id.ToString()),
                    Helpers.LogLevel.Debug);
#endif
                lock (allBuffers)
                    allBuffers.Remove(ContainerId);

                if (blocking)
                    stopped.Set();
            }));

            if (blocking)
                stopped.WaitOne();

        }

    }
}
