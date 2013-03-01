// 
// Radegast Metaverse Client
//Copyright (c) 2009-2013, Radegast Development Team
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast.Automation
{
    public class LSLHelper : IDisposable
    {
        public bool Enabled;
        public UUID AllowedOwner;

        RadegastInstance instance;
        GridClient client
        {
            get { return instance.Client; }
        }

        public LSLHelper(RadegastInstance instance)
        {
            this.instance = instance;
        }

        public void Dispose()
        {
        }

        public void LoadSettings()
        {
            if (!client.Network.Connected) return;
            try
            {
                if (!(instance.ClientSettings["LSLHelper"] is OSDMap))
                    return;
                OSDMap map = (OSDMap)instance.ClientSettings["LSLHelper"];
                Enabled = map["enabled"];
                AllowedOwner = map["allowed_owner"];
            }
            catch { }
        }

        public void SaveSettings()
        {
            if (!client.Network.Connected) return;
            try
            {
                OSDMap map = new OSDMap(2);
                map["enabled"] = Enabled;
                map["allowed_owner"] = AllowedOwner;
                instance.ClientSettings["LSLHelper"] = map;
            }
            catch { }
        }

    }
}
