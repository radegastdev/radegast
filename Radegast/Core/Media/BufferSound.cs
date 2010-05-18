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

namespace Radegast.Media
{

    public class BufferSound : MediaObject
    {
        private FMOD.CREATESOUNDEXINFO extendedInfo;
        private UUID Id;
        private Boolean prefetchOnly = false;
        private FMOD.MODE mode;
        public Sound Sound { get { return sound; } }

        /// <summary>
        /// Creates a new sound object
        /// </summary>
        /// <param name="system">Sound system</param>
        public BufferSound( UUID soundId, bool loop, bool global, Vector3 worldpos, float vol )
            :base()
        {
            allBuffers.AddLast(this);
            position = FromOMVSpace(worldpos);
            volume = vol;

            // Set flags to determine how it will be played.
            FMOD.MODE mode = FMOD.MODE.SOFTWARE | FMOD.MODE._3D | FMOD.MODE.NONBLOCKING;

            // Set coordinate space interpretation.
            if (global)
                mode |= FMOD.MODE._3D_WORLDRELATIVE;
            else
                mode |= FMOD.MODE._3D_HEADRELATIVE;

            manager.Instance.Client.Assets.RequestAsset(
                Id,
                AssetType.Sound,
                false,
                new AssetManager.AssetReceivedCallback(Assets_OnSoundReceived));
        }

        public BufferSound( UUID soundId )
            : base()
        {
            allBuffers.AddLast(this);
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
                    allBuffers.Remove(this);
                    return;
                }

                AssetSound s = asset as AssetSound;
                s.Decode();

                invoke(new SoundDelegate(delegate
                {
                    FMODExec(system.createSound(
                        s.AssetData,
                        mode,
                        ref extendedInfo,
                        ref sound));

                    // Register for callbacks.
                    RegisterSound(sound);

//                    if (loop)
//                        FMODExec(sound.setLoopCount(-1));
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
                Logger.Log("Error opening sound: ", Helpers.LogLevel.Error);
                return RESULT.OK;
            }

            try
            {
                // Allocate a channel and set initial volume.  Initially paused.
                FMODExec(system.playSound(CHANNELINDEX.FREE, sound, true, ref channel));
                FMODExec(channel.setVolume(volume));

                // Take note of when the sound is finished playing.
                FMODExec(channel.setCallback(EndCallback));

                // Set attenuation limits.
                FMODExec(sound.set3DMinMaxDistance(
                    1.2f,       // Any closer than this gets no louder
                    20.0f));     // Further than this gets no softer.

                // Set the sound point of origin.
                FMODExec(channel.set3DAttributes(ref position, ref ZeroVector));

                // Turn off pause mode.  The sound will start playing now.
                FMODExec(channel.setPaused(false));
            }
            catch (Exception ex)
            {
                Logger.Log("Error starting sound: ", Helpers.LogLevel.Debug, ex);
            }

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

            // Release the buffer to avoid a big memory leak.

            if (sound != null)
            {
                sound.release();
                sound = null;
            }
            channel = null;

            allBuffers.Remove(this);

            return RESULT.OK;
        }

    }
}
