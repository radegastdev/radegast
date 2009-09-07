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

        private FMOD.Sound stream = null;
        private List<MediaObject> sounds = new List<MediaObject>();

        /// <summary>
        /// Parcel music stream player
        /// </summary>
        public Sound ParcelMusic { get { return parcelMusic; } }
        private Sound parcelMusic;

        public MediaManager()
            : base(null)
        {
            try
            {
                FMODExec(FMOD.Factory.System_Create(ref system));
                uint version = 0;
                FMODExec(system.getVersion(ref version));

                if (version < FMOD.VERSION.number)
                    throw new MediaException("You are using an old version of FMOD " + version.ToString("X") + ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");

                FMODExec(system.init(100, FMOD.INITFLAG.NORMAL, (IntPtr)null));
                FMODExec(system.setStreamBufferSize(64 * 1024, FMOD.TIMEUNIT.RAWBYTES));

                soundSystemAvailable = true;

                //parcelMusic = new Sound(system);
                //parcelMusic.PlayStream("http://scfire-ntc-aa06.stream.aol.com:80/stream/1065");

                Logger.Log("Initialized FMOD Ex", Helpers.LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to initialize the sound system", Helpers.LogLevel.Warning, ex);
            }
        }

        public override void Dispose()
        {
            if (parcelMusic != null)
            {
                if (!parcelMusic.Disposed)
                    parcelMusic.Dispose();
                parcelMusic = null;
            }

            lock(sounds)
            {
                for (int i=0; i<sounds.Count; i++)
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

        public static void FMODExec(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                throw new MediaException("FMOD error! " + result + " - " + FMOD.Error.String(result));
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
