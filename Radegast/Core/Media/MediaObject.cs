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
using System.Threading;
using FMOD;

namespace Radegast.Media
{
    public abstract class MediaObject : Object, IDisposable
    {
        /// <summary>
        /// Indicates if this object's resources have already been disposed
        /// </summary>
        public bool Disposed { get { return disposed; } }
        private bool disposed = false;
 
        /// All commands are made through queued delegate calls, so they
        /// are guaranteed to take place in the same thread.  FMOD requires this.
        public delegate void SoundDelegate();

        /// Queue of sound commands
        /// 
        /// </summary>
        protected static Queue<SoundDelegate> queue;

        /// <summary>
        /// FMOD channel controller, should not be used directly, add methods to Radegast.Media.Sound
        /// </summary>
        public Channel FMODChannel { get { return channel; } }
        protected Channel channel = null;

        /// <summary>
        /// FMOD sound object, should not be used directly, add methods to Radegast.Media.Sound
        /// </summary>
        public FMOD.Sound FMODSound { get { return sound; } }
        protected FMOD.Sound sound = null;

        public FMOD.System FMODSystem { get { return system; } }
        /// <summary>
        /// Base FMOD system object, of which there is only one.
        /// </summary>
        protected static FMOD.System system = null;

        public MediaObject()
        {
        }

        public virtual void Dispose()
        {
            if (sound != null)
            {
                sound.release();
                sound = null;
            }

            disposed = true;
        }

        public bool Active { get { return (sound != null); } }

        /// <summary>
        /// Put a delegate call on the command queue.
        /// </summary>
        /// <param name="action"></param>
        protected void invoke(SoundDelegate action)
        {
            // Do nothing if queue not ready yet.
            if (queue == null) return;

            // Put that on the queue and wake up the background thread.
            lock (queue)
            {
                queue.Enqueue( action );
                Monitor.Pulse(queue);
            }

        }

        /// <summary>
        ///  Common actgions for all sound types.
        /// </summary>
        protected float volume = 0.5f;
        public float Volume {
            get
            {
                return volume;
            }
            set
            {
                invoke(new SoundDelegate(delegate
                {
                    MediaManager.FMODExec(channel.setVolume(value));
                    volume = value;
                }));
            }
        }
    }
}
