// 
// Speech Input and Output for the Radegast Metaverse Client
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
// $Id: FmodSound.cs  $
//

// Define the following symbol to use the shared Radegast FMOD instance.
#define SHAREDFMOD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenMetaverse;
using System.Threading;
using Radegast.Media;

namespace RadegastSpeech.Sound
{
    class FmodSound : Control
    {
        Radegast.Media.Speech speechPlayer;
        private AutoResetEvent playing;

        internal FmodSound(PluginControl pc)
            : base(pc)
        {
            playing = new AutoResetEvent(false);
            speechPlayer = new Radegast.Media.Speech();
            speechPlayer.OnSpeechDone += new Speech.SpeechDoneCallback(SpeechDoneHandler);
        }

        internal override void Stop()
        {
            if (speechPlayer == null) return;
            speechPlayer.Stop();
        }

        /// <summary>
        /// Play a prerecorded sound
        /// </summary>
        /// <param name="filename">Name of the file to play</param>
        /// <param name="sps">Samples per second</param>
        /// <param name="worldPos">Position of the sound</param>
        /// <param name="deleteAfter">True if we should delete the file when done</param>
        /// <param name="global">True if position is in world coordinates
        /// instead of hed-relative</param>
        internal override void Play(string filename,
            int sps,
            OpenMetaverse.Vector3 worldPos,
            bool deleteAfter,
            bool global)
        {
            if (speechPlayer != null)
            {
                // Play this file at the designated position.  When it finishes, the
                // SpeechDoneHandler will be called.
                uint len = speechPlayer.Play(filename, global, worldPos);

                // Wait for it to finish. Max 2sec longer tha it is supposed to
                playing.WaitOne((int)len + 2000, false);
            }

            // Delete the WAV file if requested.
            if (deleteAfter)
            {
                File.Delete(filename);
            }
        }

        // Handler for event when speech is done playing.
        private void SpeechDoneHandler(object sender, EventArgs e)
        {
            // Poke the semaphore
            playing.Set();
        }

        // TODO do we need this?
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (speechPlayer != null)
                {
                    speechPlayer.Stop();
                    speechPlayer = null;
                }
            }

        }

    }
}
