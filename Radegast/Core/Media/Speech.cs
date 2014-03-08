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
using System;
using System.Runtime.InteropServices;
using FMOD;
using OpenMetaverse;

namespace Radegast.Media
{
    public class Speech : MediaObject
    {
        /// <summary>
        /// Fired when a stream meta data is received
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Key, value are sent in e</param>
        public delegate void SpeechDoneCallback(object sender, EventArgs e);
        /// <summary>
        /// Fired when a stream meta data is received
        /// </summary>
        public event SpeechDoneCallback OnSpeechDone;
        private String filename;
        private Vector3 speakerPos;

        /// <summary>
        /// If true, decrease distance falloff effect in volume.
        /// </summary>
        private static Boolean compress = true;
        public static Boolean Compress
        {
            set { compress = value; }
            get { return compress; }
        }

        /// <summary>
        /// If truee use FMODEx 3D sound
        /// </summary>
        private static bool surround = false;
        public static bool Surround
        {
            set { surround = value; }
            get { return surround; }
        }

        /// <summary>
        /// Creates a new sound object
        /// </summary>
        /// <param name="system">Sound system</param>
        public Speech()
            : base()
        {
            extraInfo.format = SOUND_FORMAT.PCM16;
        }


        /// <summary>
        /// Releases resources of this sound object
        /// </summary>
        public override void Dispose()
        {
            if (sound != null)
            {
                sound.release();
                sound = null;
            }
            base.Dispose();
        }

        /// <summary>
        /// Plays audio from a file, as created by an external speech synthesizer.
        /// </summary>
        /// <param name="filename">Name of a WAV file</param>
        /// <param name="global"></param>
        /// <returns>Length of the sound in ms</returns>
        public uint Play(string speakfile, bool global, Vector3 pos)
        {
            uint len = 0;

            speakerPos = pos;
            filename = speakfile;

            // Set flags to determine how it will be played.
            FMOD.MODE mode = FMOD.MODE.SOFTWARE;

            if (Surround)
            {
                mode |= FMOD.MODE._3D;

                // Set coordinate space interpretation.
                if (global)
                    mode |= FMOD.MODE._3D_WORLDRELATIVE;
                else
                    mode |= FMOD.MODE._3D_HEADRELATIVE;
            }


            System.Threading.AutoResetEvent done = new System.Threading.AutoResetEvent(false);

            invoke(new SoundDelegate(
                delegate
                {
                    try
                    {
                        FMODExec(
                        system.createSound(filename,
                        mode,
                        ref extraInfo,
                        ref sound));

                        // Register for callbacks.
                        RegisterSound(sound);

                        // Find out how long the sound should play
                        FMODExec(sound.getLength(ref len, TIMEUNIT.MS));

                        // Allocate a channel, initially paused.
                        FMODExec(system.playSound(CHANNELINDEX.FREE, sound, true, ref channel));

                        // Set general Speech volume.
                        //TODO Set this in the GUI
                        volume = 1.0f;
                        FMODExec(channel.setVolume(volume));

                        if (Surround)
                        {
                            // Set attenuation limits so distant people get a little softer,
                            // but not TOO soft
                            if (compress)
                                FMODExec(sound.set3DMinMaxDistance(
                                        2f,       // Any closer than this gets no louder
                                        3.5f));     // Further than this gets no softer.
                            else
                                FMODExec(sound.set3DMinMaxDistance(
                                        2f,       // Any closer than this gets no louder
                                        10f));     // Further than this gets no softer.

                            // Set speaker position.
                            position = FromOMVSpace(speakerPos);
                            FMODExec(channel.set3DAttributes(
                               ref position,
                               ref ZeroVector));

                            Logger.Log(
                                String.Format(
                                    "Speech at <{0:0.0},{1:0.0},{2:0.0}>",
                                    position.x,
                                    position.y,
                                    position.z),
                                Helpers.LogLevel.Debug);
                        }

                        // SET a handler for when it finishes.
                        FMODExec(channel.setCallback(endCallback));
                        RegisterChannel(channel);

                        // Un-pause the sound.
                        FMODExec(channel.setPaused(false));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error playing speech: ", Helpers.LogLevel.Error, ex);
                    }
                    finally
                    {
                        done.Set();
                    }
                }));

            done.WaitOne(30 * 1000, false);
            return len;
        }

        /// <summary>
        /// Handler for reaching the end of playback of a speech.
        /// </summary>
        /// <returns></returns>
        protected override RESULT EndCallbackHandler()
        {
            invoke(new SoundDelegate(
                 delegate
                 {
                     UnRegisterChannel();
                     channel = null;
                     UnRegisterSound();
                     FMODExec(sound.release());
                     sound = null;

                     // Tell speech control the file has been played.  Note
                     // the event is dispatched on FMOD's thread, to make sure
                     // the event handler does not start a new sound before the
                     // old one is cleaned up.
                     if (OnSpeechDone != null)
                         try
                         {
                             OnSpeechDone(this, new EventArgs());
                         }
                         catch (Exception) { }
                 }));


            return RESULT.OK;
        }
    }
}
