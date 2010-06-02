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
using FMOD;
using OpenMetaverse;

namespace Radegast.Media
{
    public class StreamInfoArgs : EventArgs
    {
        public string Key;
        public string Value;

        public StreamInfoArgs(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class Stream : MediaObject
    {
        /// <summary>
        /// Fired when a stream meta data is received
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Key, value are sent in e</param>
        public delegate void StreamInfoCallback(object sender, StreamInfoArgs e);

        /// <summary>
        /// Fired when a stream meta data is received
        /// </summary>
        public event StreamInfoCallback OnStreamInfo;

        /// <summary>
        /// Creates a new sound object
        /// </summary>
        /// <param name="system">Sound system</param>
        public Stream()
            :base()
        {
        }


        /// <summary>
        /// Releases resources of this sound object
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Plays audio stream
        /// </summary>
        /// <param name="url">URL of the stream</param>
        public void PlayStream(string url)
        {
            // Stop old stream first.
            if (channel != null)
            {
                invoke(new SoundDelegate(
                    delegate
                    {
                        FMODExec(channel.stop());
                        channel = null;
                        UnRegisterSound();
                        FMODExec(sound.release());
                        sound = null;
                    }));
            }

            extraInfo.nonblockcallback = loadCallback;
            extraInfo.format = SOUND_FORMAT.PCM16;

            invoke( new SoundDelegate(
                delegate {
                    FMODExec(
                        system.createSound(url,
                        (MODE.HARDWARE | MODE._2D | MODE.CREATESTREAM | MODE.NONBLOCKING),
                        ref extraInfo,
                        ref sound), "Stream load");
                    // Register for callbacks.
                    RegisterSound(sound);
                }));
        }

        /// <summary>
        /// Callback when a stream has been loaded
        /// </summary>
        /// <param name="instatus"></param>
        /// <returns></returns>
        protected override RESULT NonBlockCallbackHandler(RESULT instatus)
        {
            if (instatus != RESULT.OK)
            {
                Logger.Log("Error opening audio stream: " + instatus,
                    Helpers.LogLevel.Debug);
                return RESULT.OK;
            }

            invoke(new SoundDelegate(
                delegate
                {
                    try
                    {
                        // Allocate a channel and set initial volume.
                        FMODExec(system.playSound(
                            CHANNELINDEX.FREE,
                            sound,
                            false,
                            ref channel), "Stream channel");
                        volume = 0.5f;
//                        FMODExec(channel.setVolume(volume), "Stream volume");

                        // If this is not Unix, try to get MP3 tags.
                        // getTag seems to break on Unix.
#if NOT
                        // Seems to break on Windows too now
                        if (false) //Environment.OSVersion.Platform != PlatformID.Unix)
                        {
                            TAG tag = new TAG();

                            while (sound.getTag(null, -1, ref tag) == RESULT.OK)
                            {
                                if (tag.datatype != TAGDATATYPE.STRING) continue;

                                // Tell listeners about the Stream tag.  This can be
                                // displayed to the user.
                                if (OnStreamInfo != null)
                                    try
                                    {
                                        OnStreamInfo(this, new StreamInfoArgs(tag.name.ToLower(), Marshal.PtrToStringAnsi(tag.data)));
                                    }
                                    catch (Exception) { }
                            }
                        }
#endif

                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error playing stream: ", Helpers.LogLevel.Debug, ex);
                    }
                }));

            Logger.Log("Stream playing", Helpers.LogLevel.Info);
            return RESULT.OK;
        }
    }
}
