// 
// Radegast Metaverse Client
//Copyright (c) 2009-2014, Radegast Development Team
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
// Implements RAD-354

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast.Automation
{
    public class PseudoHomePreferences
    {
        public bool Enabled { get; set; }
        public string Region { get; set; }
        public Vector3 Position { get; set; }
        public uint Tolerance { get; set; }

        public PseudoHomePreferences()
        {
            Tolerance = 256;
        }

        public static explicit operator PseudoHomePreferences(OSD osd)
        {
            PseudoHomePreferences prefs = new PseudoHomePreferences();

            if (osd != null && osd.Type == OSDType.Map)
            {
                OSDMap map = (OSDMap)osd;
                prefs.Enabled = map.ContainsKey("Enabled") ? map["Enabled"].AsBoolean() : false;
                prefs.Region = map.ContainsKey("Region") ? map["Region"].AsString().Trim() : "";
                prefs.Position = map.ContainsKey("Position") ? map["Position"].AsVector3() : new Vector3();
                prefs.Tolerance = map.ContainsKey("Tolerance") ? Math.Min(256, Math.Max(1, map["Tolerance"].AsUInteger())) : 256;
            }

            return prefs;
        }

        public static implicit operator OSD(PseudoHomePreferences prefs)
        {
            return (OSDMap)prefs;
        }

        public static explicit operator OSDMap(PseudoHomePreferences prefs)
        {
            OSDMap map = new OSDMap(4);

            map["Enabled"] = prefs != null ? prefs.Enabled : false;
            map["Region"] = prefs != null && prefs.Region != null ? prefs.Region.Trim() : string.Empty;
            map["Position"] = prefs != null ? prefs.Position : Vector3.Zero;
            map["Tolerance"] = prefs != null ? prefs.Tolerance : 256;

            return map;
        }

        public static explicit operator PseudoHomePreferences(Settings s)
        {
            return (s != null && s.ContainsKey("PseudoHome")) ? (PseudoHomePreferences)s["PseudoHome"] : new PseudoHomePreferences
            {
                Enabled = false,
                Region = "",
                Position = Vector3.Zero,
                Tolerance = 256
            };
        }
    }

    public class PseudoHome
    {
        private RadegastInstance m_instance;
        private Timer m_Timer;

        public PseudoHome(RadegastInstance instance)
        {
            m_instance = instance;
            m_Timer = new Timer(5000);
            m_Timer.Elapsed += new ElapsedEventHandler((sender, args) => {
                ETGoHome();
            });
            m_Timer.Enabled = false;
        }

        public PseudoHomePreferences Preferences
        {
            get { return !m_instance.Client.Network.Connected ? null : (PseudoHomePreferences)m_instance.ClientSettings; }

            set
            {
                m_instance.ClientSettings["PseudoHome"] = value;
            }
        }

        public void ETGoHome()
        {
            if (Preferences != null && m_instance.Client.Network.Connected && Preferences.Region.Trim() != string.Empty)
            {
                if (Preferences.Enabled && (m_instance.Client.Network.CurrentSim.Name != Preferences.Region || Vector3.Distance(m_instance.Client.Self.SimPosition, Preferences.Position) > Preferences.Tolerance))
                {
                    m_instance.Client.Self.Teleport(Preferences.Region, Preferences.Position);
                    m_Timer.Enabled = true;
                }
                else
                {
                    m_Timer.Enabled = false;
                }
            }
            else
            {
                m_Timer.Enabled = false;
            }
        }
    }
}
