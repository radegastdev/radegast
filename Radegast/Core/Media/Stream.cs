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
        /// Returns current position of the sound played in ms
        /// Do not confuse with the spatial Position on other suonds.
        /// </summary>
        public uint Position { get { return position; } }
        private uint position = 0;

        /// <summary>
        /// Is sound currently playing
        /// </summary>
        public bool Playing { get { return playing; } }
        private bool playing = false;

        /// <summary>
        /// Is sound currently paused
        /// </summary>
        public bool Paused { get { return paused; } }
        private bool paused = false;

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

        private System.Timers.Timer timer;

        /// <summary>
        /// Creates a new sound object
        /// </summary>
        /// <param name="system">Sound system</param>
        public Stream()
            :base()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 50d;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
        }


        /// <summary>
        /// Releases resources of this sound object
        /// </summary>
        public override void Dispose()
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Dispose();
                timer = null;
            }

            base.Dispose();
        }

        /// <summary>
        /// Plays audio stream
        /// </summary>
        /// <param name="url">URL of the stream</param>
        public void PlayStream(string url)
        {
            if (!Active)
            {
                invoke( new SoundDelegate(
                    delegate {
                        MediaManager.FMODExec(system.createSound(url,
                            (MODE.HARDWARE | MODE._2D | MODE.CREATESTREAM | MODE.NONBLOCKING), ref sound));
  
                        timer.Enabled = true;
                        timer_Elapsed(null, null);
                    }));
            }
        }

        /// <summary>
        /// Toggles sound pause
        /// </summary>
        public void TogglePaused()
        {
            invoke(new SoundDelegate(delegate
              {
                  if (channel != null)
                  {
                      channel.getPaused(ref paused);
                      channel.setPaused(!paused);
                  }
              }));
        }

         void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OPENSTATE openstate = 0;
            uint percentbuffered = 0;
            bool starving = false;

            try
            {
                if (Active)
                {
                    // Query what the sound is doing.
                    MediaManager.FMODExec(sound.getOpenState(ref openstate, ref percentbuffered, ref starving));

                    // If a channel is not allocated yet, ask for one.
                    if (openstate == OPENSTATE.READY && channel == null)
                    {
                        // Allocate a channel and set initial volume.
                        MediaManager.FMODExec(system.playSound( CHANNELINDEX.FREE, sound, false, ref channel));
                        volume = 0.5f;
                        MediaManager.FMODExec(channel.setVolume(volume));
                    }
                }

                if (channel != null)
                {
                    // Do not try to get MP3 tags om Unix.  It breaks.
                    if (Environment.OSVersion.Platform != PlatformID.Unix)
                    {
                        for (; ; )
                        {
                            TAG tag = new TAG();
                            if (sound.getTag(null, -1, ref tag) != RESULT.OK)
                            {
                                break;
                            }
                            if (tag.datatype != TAGDATATYPE.STRING)
                            {
                                break;
                            }
                            else
                            {
                                if (OnStreamInfo != null)
                                    try { OnStreamInfo(this, new StreamInfoArgs(tag.name.ToLower(), Marshal.PtrToStringAnsi(tag.data))); }
                                    catch (Exception) { }
                            }
                        }
                    }

                    // Get pause/play status
                    MediaManager.FMODExec(channel.getPaused(ref paused));
                    MediaManager.FMODExec(channel.isPlaying(ref playing));
                    MediaManager.FMODExec(channel.getPosition(ref position, TIMEUNIT.MS));
                }

                if (system != null)
                {
                    system.update();
                }
            }
            catch (Exception ex)
            {
                playing = paused = false;
                timer.Enabled = false;
                Logger.Log("Error playing stream: ", Helpers.LogLevel.Debug, ex);
            }
        }
    }
}
