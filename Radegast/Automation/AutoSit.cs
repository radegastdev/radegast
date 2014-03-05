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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast.Automation
{
    public class AutoSitPreferences
    {
        public UUID Primitive { get; set; }
        public string PrimitiveName { get; set; }
        public bool Enabled { get; set; }

        public static explicit operator AutoSitPreferences(OSD osd){
            AutoSitPreferences prefs = new AutoSitPreferences
            {
                Primitive = UUID.Zero,
                PrimitiveName = ""
            };

            if (osd != null && osd.Type == OSDType.Map)
            {
                OSDMap map = (OSDMap)osd;
                prefs.Primitive = map.ContainsKey("Primitive") ? map["Primitive"].AsUUID() : UUID.Zero;
                prefs.PrimitiveName = prefs.Primitive != UUID.Zero && map.ContainsKey("PrimitiveName") ? map["PrimitiveName"].AsString() : "";
                prefs.Enabled = map.ContainsKey("Enabled") ? map["Enabled"].AsBoolean() : false;
            }

            return prefs;
        }

        public static implicit operator OSD(AutoSitPreferences prefs){
            return (OSDMap)prefs;
        }

        public static explicit operator OSDMap(AutoSitPreferences prefs)
        {
            OSDMap map = new OSDMap(3);

            map["Primitive"] = prefs.Primitive;
            map["PrimitiveName"] = prefs.PrimitiveName;
            map["Enabled"] = prefs.Enabled;

            return map;
        }

        public static explicit operator AutoSitPreferences(Settings s){
            return (s != null && s.ContainsKey("AutoSit")) ? (AutoSitPreferences)s["AutoSit"] : new AutoSitPreferences();
        }
    }

    public class AutoSit : IDisposable
    {
        private const string c_label = "Use as Auto-Sit target";

        private RadegastInstance m_instance;
        private Timer m_Timer;

        public AutoSit(RadegastInstance instance)
        {
            m_instance = instance;
            m_Timer = new Timer(10 * 1000);
            m_Timer.Elapsed += new ElapsedEventHandler((sender, args) => {
                TrySit();
            });
            m_Timer.Enabled = false;
        }

        public void Dispose()
        {
            if (m_Timer != null)
            {
                m_Timer.Enabled = false;
                m_Timer.Dispose();
                m_Timer = null;
            }
        }

        public AutoSitPreferences Preferences
        {
            get { return !m_instance.Client.Network.Connected ? null : (AutoSitPreferences)m_instance.ClientSettings; }

            set {
                m_instance.ClientSettings["AutoSit"] = value;
                if (Preferences.Enabled)
                {
                    m_instance.ContextActionManager.RegisterContextAction(typeof(Primitive), c_label, PrimitiveContextAction);
                }
                else
                {
                    m_instance.ContextActionManager.DeregisterContextAction(typeof(Primitive), c_label);
                }
            }
        }

        public void PrimitiveContextAction(object sender, EventArgs e)
        {
            Primitive prim = (Primitive)sender;
            Preferences = new AutoSitPreferences
            {
                Primitive = prim.ID,
                PrimitiveName = prim.Properties != null ? prim.Properties.Name : "",
                Enabled = Preferences.Enabled
            };
            if (prim.Properties == null)
            {
                m_instance.Client.Objects.ObjectProperties += Objects_ObjectProperties;
                m_instance.Client.Objects.ObjectPropertiesUpdated += Objects_ObjectProperties;
            }
        }

        public void Objects_ObjectProperties(object sender, ObjectPropertiesEventArgs e)
        {
            if (e.Properties.ObjectID == Preferences.Primitive)
            {
                Preferences = new AutoSitPreferences
                {
                    Primitive = Preferences.Primitive,
                    PrimitiveName = e.Properties.Name,
                    Enabled = Preferences.Enabled
                };

                m_instance.Client.Objects.ObjectProperties -= Objects_ObjectProperties;
            }
        }

        public void TrySit()
        {
            if (Preferences != null && m_instance.Client.Network.Connected)
            {
                if (Preferences.Enabled && Preferences.Primitive != UUID.Zero)
                {
                    if (!m_instance.State.IsSitting)
                    {
                        m_instance.State.SetSitting(true, Preferences.Primitive);
                        m_Timer.Enabled = true;
                    }
                    else
                    {
                        if (!m_instance.Client.Network.CurrentSim.ObjectsPrimitives.ContainsKey(m_instance.Client.Self.SittingOn))
                        {
                            m_instance.State.SetSitting(false, UUID.Zero);
                        }
                    }
                }
                else
                {
                    m_Timer.Enabled = false;
                }
            }
            else
            {
                m_Timer.Enabled = false; // being lazy here, just letting timer elapse rather than disabling on client disconnect
            }
        }
    }
}
