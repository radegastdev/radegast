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
// $Id$
//
using System;
using System.Timers;
using OpenMetaverse;

namespace Radegast
{
    public class RadegastMovement : IDisposable
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private Timer timer;
        private Vector3 forward = new Vector3(1, 0, 0);
        private bool turningLeft = false;
        private bool turningRight = false;
        private bool movingForward = false;
        private bool movingBackward = false;

        public bool TurningLeft
        {
            get {
                return turningLeft;
            }
            set {
                turningLeft = value;
                if (value) {
                    timer_Elapsed(null, null);
                    timer.Enabled = true;
                } else {
                    timer.Enabled = false;
                    client.Self.Movement.TurnLeft = false;
                    client.Self.Movement.SendUpdate(true);
                }
            }
        }

        public bool TurningRight
        {
            get
            {
                return turningRight;
            }
            set
            {
                turningRight = value;
                if (value) {
                    timer_Elapsed(null, null);
                    timer.Enabled = true;
                } else {
                    timer.Enabled = false;
                    client.Self.Movement.TurnRight = false;
                    client.Self.Movement.SendUpdate(true);
                }
            }
        }

        public bool MovingForward
        {
            get
            {
                return movingForward;
            }
            set
            {
                movingForward = value;
                if (value) {
                    client.Self.Movement.AtPos = true;
                    client.Self.Movement.SendUpdate(true);
                } else {
                    client.Self.Movement.AtPos = false;
                    client.Self.Movement.SendUpdate(true);
                }
            }
        }

        public bool MovingBackward
        {
            get
            {
                return movingBackward;
            }
            set
            {
                movingBackward = value;
                if (value) {
                    client.Self.Movement.AtNeg = true;
                    client.Self.Movement.SendUpdate(true);
                } else {
                    client.Self.Movement.AtNeg = false;
                    client.Self.Movement.SendUpdate(true);
                    
                }
            }
        }

        public RadegastMovement(RadegastInstance instance)
        {
            this.instance = instance;
            timer = new System.Timers.Timer(100);
            timer.Elapsed +=new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
        }

        public void Dispose()
        {
            timer.Enabled = false;
            timer.Dispose();
            timer = null;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            float delta = (float)timer.Interval / 1000f;
            if (turningLeft) {
                client.Self.Movement.TurnLeft = true;
                client.Self.Movement.BodyRotation = client.Self.Movement.BodyRotation * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, delta);
                client.Self.Movement.SendUpdate(true);
            } else if (turningRight) {
                client.Self.Movement.TurnRight = true;
                client.Self.Movement.BodyRotation = client.Self.Movement.BodyRotation * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -delta);
                client.Self.Movement.SendUpdate(true);
            }
        }
    }
}
