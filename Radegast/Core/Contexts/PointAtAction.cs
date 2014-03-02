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
// $Id: 
//
using System;
using System.Threading;
using OpenMetaverse;

namespace Radegast
{
    public class PointAtAction : ContextAction
    {
        public PointAtAction(RadegastInstance inst)
            : base(inst)
        {
            Label = "Point at";
            ContextType = typeof(Primitive);
        }
        public override string LabelFor(object target)
        {
            if (instance.State.IsPointing)
            {
                return "Unpoint";
            }
            return base.LabelFor(target);
        }

        public override bool IsEnabled(object target)
        {
            return true;
        }
        public override void OnInvoke(object sender, EventArgs e, object target)
        {
            if (instance.State.IsPointing)
            {
                instance.TabConsole.DisplayNotificationInChat("stopping pointing");
                instance.State.UnSetPointing();
                return;
            }
            string pname = instance.Names.Get(ToUUID(target));
            if (pname == "(???) (???)") pname = "" + target;
            Primitive prim = ToPrimitive(target);

            if (prim != null)
            {
                instance.TabConsole.DisplayNotificationInChat(string.Format("Pointing at {0}", pname));
                instance.State.SetPointing(prim, 5);
            }
            else
            {
                instance.TabConsole.DisplayNotificationInChat(string.Format("Could not locate {0}", target));
            }
        }
    }
}