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
// $Id: Sound.cs 502 2010-03-14 23:13:46Z latifer $
//
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
        private Boolean prefetchOnly = false;
        private FMOD.MODE mode;
        public Sound Sound { get { return sound; } }
        private Boolean loopSound = false;

        /// <summary>
        /// Creates a new sound object
        /// </summary>
        /// <param name="system">Sound system</param>
        public BufferSound( UUID soundId, bool loop, bool global, Vector3 worldpos, float vol )
            :base()
        {
            InitBuffer(soundId, loop, global, worldpos, vol);
        }

        public BufferSound(UUID soundId, bool loop, bool global, Vector3d worldpos, float vol)
            : base()
        {
            InitBuffer(soundId, loop, global, new Vector3(worldpos), vol);
        }

        private void InitBuffer(UUID soundId, bool loop, bool global, Vector3 worldpos, float vol)
        {
            // Do not let this get garbage-collected.
            lock (allBuffers)
                allBuffers[soundId] =this;

            Id = soundId;
            position = FromOMVSpace(worldpos);
            volume = vol;
            loopSound = loop;

            // Set flags to determine how it will be played.
            mode = FMOD.MODE.SOFTWARE | // Need software processing for all the features
                FMOD.MODE._3D |         // Need 3D effects for placement
                FMOD.MODE.OPENMEMORY |  // Use sound data in memory
                FMOD.MODE.NONBLOCKING;

            // Set coordinate space interpretation.
            if (global)
                mode |= FMOD.MODE._3D_WORLDRELATIVE;
            else
                mode |= FMOD.MODE._3D_HEADRELATIVE;

            // Fetch the sound data.
            manager.Instance.Client.Assets.RequestAsset(
                Id,
                AssetType.Sound,
                false,
                new AssetManager.AssetReceivedCallback(Assets_OnSoundReceived));
        }

        public static void Kill( UUID id )
        {
            if (allBuffers.ContainsKey(id))
            {
                BufferSound bs = allBuffers[id];
                bs.StopSound();
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
        }

        // A simpler constructor used by PreFetchSound.
        public BufferSound( UUID soundId )
            : base()
        {
            lock (allBuffers)
                allBuffers[soundId] = this;

            prefetchOnly = true;
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
                    lock (allBuffers)
                        allBuffers.Remove(Id);
                    return;
                }

                Logger.Log("Opening sound " + Id.ToString(), Helpers.LogLevel.Debug);

                // Decode the Ogg Vorbis buffer.
                AssetSound s = asset as AssetSound;
                s.Decode();
                byte[] data = s.AssetData;

                // Describe the data to FMOD
                extraInfo.length = (uint)data.Length;
                extraInfo.nonblockcallback = loadCallback;

                invoke(new SoundDelegate(delegate
                {
                    // Create an FMOD sound of this Ogg data.
                    RESULT status = system.createSound(
                        data,
                        mode,
                        ref extraInfo,
                        ref sound);

                    // Register for callbacks.
                    RegisterSound(sound);
                    }));
            }
            else
            {
                Logger.Log("Failed to download sound: " + transfer.Status.ToString(),
                                        Helpers.LogLevel.Error);
            }
        }

        protected override RESULT NonBlockCallbackHandler(RESULT instatus)
        {
            if (instatus != RESULT.OK)
            {
                Logger.Log("Error opening sound: " + instatus, Helpers.LogLevel.Error);
                return RESULT.OK;
            }

            invoke(new SoundDelegate(delegate
            {
                try
                {
                    // If looping is requested, loop the entire thing.
                    if (loopSound)
                    {
                        uint soundlen = 0;
                        FMODExec(sound.getLength(ref soundlen, TIMEUNIT.PCM));
                        FMODExec( sound.setLoopPoints( 0, TIMEUNIT.PCM, soundlen-1, TIMEUNIT.PCM ));
                        FMODExec(sound.setLoopCount(-1));
                    }

                    // Allocate a channel and set initial volume.  Initially paused.
                    FMODExec(system.playSound(CHANNELINDEX.FREE, sound, true, ref channel));
                    FMODExec(channel.setVolume(volume));

                    // Take note of when the sound is finished playing.
                    FMODExec(channel.setCallback(endCallback));

                    // Set attenuation limits.
                    FMODExec(sound.set3DMinMaxDistance(
                                1.2f,       // Any closer than this gets no louder
                                100.0f));     // Further than this gets no softer.

                    // Set the sound point of origin.  This is in GLOBAL coordinates.
                    FMODExec(channel.set3DAttributes(ref position, ref ZeroVector));

                    // Turn off pause mode.  The sound will start playing now.
                    FMODExec(channel.setPaused(false));
                }
                catch (Exception ex)
                {
                    Logger.Log("Error starting sound: ", Helpers.LogLevel.Error, ex);
                }
            }));

            return RESULT.OK;
        }

        /// <summary>
        /// Callback handler for reaching the end of a sound.
        /// </summary>
        /// <param name="channelraw"></param>
        /// <param name="type"></param>
        /// <param name="commanddata1"></param>
        /// <param name="commanddata2"></param>
        /// <returns></returns>
        private RESULT EndCallback(
            IntPtr channelraw,
            CHANNEL_CALLBACKTYPE type,
            IntPtr commanddata1,
            IntPtr commanddata2)
        {
            // Ignore other callback types.
            if (type != CHANNEL_CALLBACKTYPE.END) return RESULT.OK;

            StopSound();

            return RESULT.OK;
        }

        protected void StopSound()
        {
            Logger.Log("Removing sound " + Id.ToString(), Helpers.LogLevel.Debug);

            // Release the buffer to avoid a big memory leak.
            if (channel != null)
            {
                channel.stop();
                channel = null;
            }
            if (sound != null)
            {
                sound.release();
                sound = null;
            }
            channel = null;

            lock (allBuffers)
                allBuffers.Remove(Id);
        }

    }
}
