// 
// Radegast Metaverse Client
// Copyright (c) 2009-2012, Radegast Development Team
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
// $Id: 
//
using System;
using OpenMetaverse;

namespace Radegast
{
    public class TurnToAction : ContextAction
    {
        public TurnToAction(RadegastInstance inst)
            : base(inst)
        {
            Label = "Turn To";
            ContextType = typeof(Primitive);
        }
        public override bool IsEnabled(object target)
        {
            return true;
        }
        public override bool Contributes(object o, Type type)
        {
            return type == typeof(Vector3) || type == typeof(Vector3d) || base.Contributes(o, type);
        }
        public override void OnInvoke(object sender, EventArgs e, object target)
        {
            string pname = instance.Names.Get(ToUUID(target));
            if (pname == "(???) (???)") pname = "" + target;

            Simulator sim = null;
            Vector3 pos;

            if (base.TryFindPos(target, out sim, out pos))
            {
                instance.TabConsole.DisplayNotificationInChat(string.Format("Facing {0}", pname));
                Client.Self.Movement.TurnToward(instance.State.ToLocalPosition(sim.Handle, pos));
            }
            else
            {
                instance.TabConsole.DisplayNotificationInChat(string.Format("Could not locate {0}", target));
            }
        }
    }
}