// 
// Speech Input and Output for the Radegast Metaverse Client
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

namespace RadegastSpeech.Sound
{
    class FmodSound : Control
    {
        private FMOD.System  FmodSystem   =  null;
        private FMOD.Sound   sound1   = null;
        private FMOD.Channel channel1 = null;

        private FMOD.VECTOR listenerpos = new FMOD.VECTOR();
        private FMOD.VECTOR forward = new FMOD.VECTOR();
        private FMOD.VECTOR up = new FMOD.VECTOR();
        private FMOD.VECTOR ZeroVector = new FMOD.VECTOR();
        private bool busy = true;

       internal FmodSound( PluginControl pc ) : base(pc)
        {
           // Head always upright and we ignore doppler effects.
           up.x = 0.0f; up.y = 1.0f; up.z = 0.0f;
           forward.x = 0.0f; forward.y = 0.0f; forward.z = 1.0f;
           ZeroVector.x = 0.0f; ZeroVector.y = 0.0f; ZeroVector.z = 0.0f;
#if SHAREDFMOD
           FmodSystem = control.instance.MediaManager.FMODSystem;
#else
            FMOD.RESULT     result;
            // Create a FmodSystem object and initialize.
            result = FMOD.Factory.System_Create(ref FmodSystem);
            ERRCHECK(result);

           // Check that the FMOD library is not older than when we were compiled.
            uint version = 0;
            result = FmodSystem.getVersion(ref version);
            ERRCHECK(result);
            if (version < FMOD.VERSION.number)
            {
                System.Windows.Forms.MessageBox.Show(
                    "You are using an old version of FMOD " + version.ToString("X") +
                    ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");
                FmodSystem = null;
                return;
            }

            // Assume no special hardware capabilities except 5.1 surround sound.
            FMOD.CAPS caps = FMOD.CAPS.NONE;
			FMOD.SPEAKERMODE speakermode = FMOD.SPEAKERMODE.SURROUND; // = FMOD.SPEAKERMODE._5POINT1;
            
            // Get the capabilities of the driver.
			int	 minfrequency = 0, maxfrequency = 0;
			StringBuilder name = new StringBuilder(128);
			result = FmodSystem.getDriverCaps(0, ref caps,
                ref minfrequency, ref maxfrequency,
                ref speakermode);
			ERRCHECK(result);

            // Set FMOD speaker mode to what the driver supports.
			result = FmodSystem.setSpeakerMode(speakermode);
			ERRCHECK(result);

#if ALSA
           // Forcing ALSA make createSound fail.  Reason not understood yet.
            if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                result = FmodSystem.setOutput(FMOD.OUTPUTTYPE.ALSA);
            ERRCHECK(result);
#endif
#if ACC
            // The user has the 'Acceleration' slider set to off, which
            // is really bad for latency.  At 48khz, the latency between
            // issuing an fmod command and hearing it will now be about 213ms.
           // This code does not match the comment.  Not understood yet.
			if ((caps & FMOD.CAPS.HARDWARE_EMULATED) == FMOD.CAPS.HARDWARE_EMULATED)
			{                                                                        
				result = FmodSystem.setDSPBufferSize(1024, 10);	
				ERRCHECK(result);
			}
#endif
#if SIGMA
            // Get driver information so we can check for a wierd one.
            FMOD.GUID guid = new FMOD.GUID();
            result = FmodSystem.getDriverInfo(0, name, 128, ref guid);
            ERRCHECK(result);

			// Sigmatel sound devices crackle for some reason if the format is pcm 16bit.
            // pcm floating point output seems to solve it.
            if (name.ToString().IndexOf("SigmaTel") != -1)
			{
				result = FmodSystem.setSoftwareFormat(
                    48000,
                    FMOD.SOUND_FORMAT.PCMFLOAT,
                    0,0,
                    FMOD.DSP_RESAMPLER.LINEAR);
				ERRCHECK(result);
			}
#endif
            // Try to initialize with all those settings, and Max 32 channels.
            result = FmodSystem.init(32, FMOD.INITFLAG.NORMAL, (IntPtr)null);
		    if (result == FMOD.RESULT.ERR_OUTPUT_CREATEBUFFER)
			{
				System.Console.WriteLine("Retry INIT");
                // Can not handle surround sound - back to Stereo.
				result = FmodSystem.setSpeakerMode(FMOD.SPEAKERMODE.STEREO); 
				ERRCHECK(result);

                // And init again.
				result = FmodSystem.init(
                    32,
                    FMOD.INITFLAG.NORMAL,
                    (IntPtr)null);
				ERRCHECK(result);
			}

            // Set real-world effect scales.
            result = FmodSystem.set3DSettings(
                1.0f,   // Doppler scale
                1.0f,   // Distance scale is meters
                1.0f);  // Rolloff factor
            ERRCHECK(result);

//			System.Console.WriteLine("FMOD initialized");
#endif
       }

       internal override void Stop()
       {
           if (channel1 != null)
           {
               channel1.stop();
               channel1 = null;
           }
           if (sound1 != null)
           {
               sound1.release();
               sound1 = null;
           }

           busy = false;
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
#if SHAREDFMOD
            if (control.instance.MediaManager != null)
            {
                FmodSystem = control.instance.MediaManager.FMODSystem;
            }
            else
            {
                FmodSystem = null;
            }
#endif

            // If we can not play the sound, at least delete the file
            // if requested.
            if (FmodSystem == null)
            {
                if (deleteAfter)
                    File.Delete(filename);
                return;
            }

            FMOD.RESULT result;

            busy = true;

            // Set flags to determine how it will be played.
            FMOD.MODE mode = FMOD.MODE.SOFTWARE | FMOD.MODE._3D;

            // Set coordinate space interpretation.
            if (global)
                mode |= FMOD.MODE._3D_WORLDRELATIVE;
            else
                mode |= FMOD.MODE._3D_HEADRELATIVE;

			// Report if the file is missing.
			try
			{
                FileStream fs = File.OpenRead(filename);
				int b = fs.ReadByte();
				fs.Close();
			}
			catch (Exception)
			{
				System.Console.WriteLine("Can not read "+filename );
                return;
			}

            // Load the speech into an FMOD buffer.
			sound1 = null;
            result = FmodSystem.createSound(
                filename,       // File containing the sound
                mode,           // Options set above
                ref sound1);
			if (result != FMOD.RESULT.OK)
			{
				System.Console.WriteLine("Fmod fails creating sound. " + FMOD.Error.String(result));
				return;
			}
//            ERRCHECK(result);


			// Set attenuation limits.
            result = sound1.set3DMinMaxDistance(
                2.0f,       // Any closer than this gets no louder
                100.0f);     // Further than this gets no softer.
            ERRCHECK(result);

//            result = sound1.setMode(FMOD.MODE.LOOP_NORMAL);
//            ERRCHECK(result);
			
            // Start with our ears at the current avatar position
            OnListenerUpdate();

            // TODO wrap the call like this and enclose in try/catch
            // MediaManager.FMODExec(system.playSound(FMOD.CHANNELINDEX.FREE, sound, false, ref channel));
			
            // Start in paused mode so we get back the channel assignment.
            result = FmodSystem.playSound(
                FMOD.CHANNELINDEX.FREE, // Pick a free channel
                sound1,         // The sound buffer
                true,           // Initially paused
                ref channel1);
            ERRCHECK(result);

            // Set the sound point of origin.  We assume it is not moving because
            // most people can not move and chat at the same time, and the speeds
            // are too low for doppler to matter.
            FMOD.VECTOR pos = FromOMVSpace(worldPos);
            result = channel1.set3DAttributes(ref pos, ref ZeroVector);
            ERRCHECK(result);
			
            // Turn off pause mode.  The sound will start playing now.
            result = channel1.setPaused(false);
            ERRCHECK(result);

            // Wait for it to finish, while keeping our position current.
            // The actual playing goes on in the background, but we wait here
            // so subsequent speakers do not (1) mess up the context and
            // (2) confuse the user.
            // TODO manage multiple speeches at once.
            while (busy)
            {
                channel1.isPlaying(ref busy);
                if (busy)
                {
                    Thread.Sleep(500);  // Check twice per second.
                    OnListenerUpdate();
                }
            }

            // Release the buffer to avoid a big memory leak.
            if (sound1 != null)
            {
                sound1.release();
                sound1 = null;
            }
            channel1 = null;

            // Delete the WAV file if requested.
            if (deleteAfter)
            {
                File.Delete(filename);
            }

        }

        /// <summary>
        /// Convert OMV to FMOD coordinate space.
        /// </summary>
        /// <param name="omvV"></param>
        /// <returns></returns>
        private FMOD.VECTOR FromOMVSpace(OpenMetaverse.Vector3 omvV)
        {
            // OMV  X is forward, Y is left, Z is up
            // FMOD Z is forward, X is right, Y is up
            FMOD.VECTOR v = new FMOD.VECTOR();
            v.x = -omvV.Y;
            v.y =  omvV.Z;
            v.z =  omvV.X;
            return v;
        }

        /// <summary>
        /// Update listener position
        /// </summary>
        private void OnListenerUpdate()
        {
            AgentManager my = control.instance.Client.Self;
            // TODO we need an event to update this on movement.
            listenerpos = FromOMVSpace( my.SimPosition );

            // Get azimuth from the facing Quaternion
            // By definition, facing.W = Cos( angle/2 )
            double angle = 2.0 * Math.Acos( my.Movement.BodyRotation.W);
            forward.x = (float)Math.Asin(angle);
            forward.y = 0.0f;
            forward.z = (float)Math.Acos(angle);

            FMOD.RESULT result;

            if (FmodSystem != null)
            {
                result = FmodSystem.set3DListenerAttributes(
                    0,
                    ref listenerpos,	// Position
                    ref ZeroVector,		// Velocity
                    ref forward,		// Facing direction
                    ref up);			// Top of head
                ERRCHECK(result);
                FmodSystem.update();
            }
        }

        private void ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                System.Console.WriteLine("FMOD error! " + result + " - " + FMOD.Error.String(result));
            }
        }

        // TODO do we need this?
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                FMOD.RESULT result;

                /*
                    Shut down
                */
                if (sound1 != null)
                {
                    result = sound1.release();
                    ERRCHECK(result);
                }
                if (FmodSystem != null)
                {
                    result = FmodSystem.close();
                    ERRCHECK(result);
                    result = FmodSystem.release();
                    ERRCHECK(result);
                }
            }

        }

	}
}
