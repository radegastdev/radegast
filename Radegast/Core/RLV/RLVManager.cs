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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public class RLVManager : IDisposable
    {
        #region Events
        /// <summary>The event subscribers. null if no subcribers</summary>
        private EventHandler<RLVEventArgs> m_RLVRuleChanged;

        /// <summary>Raises the RLVRuleChanged event</summary>
        /// <param name="e">An RLVRuleChangedEventArgs object containing the
        /// data returned from the data server</param>
        protected virtual void OnRLVRuleChanged(RLVEventArgs e)
        {
            EventHandler<RLVEventArgs> handler = m_RLVRuleChanged;
            try
            {
                if (handler != null)
                    handler(this, e);
            }
            catch (Exception) { }
            
        }

        /// <summary>Thread sync lock object</summary>
        private readonly object m_RLVRuleChangedLock = new object();

        /// <summary>Triggered when an RLVRuleChangedUpdate packet is received,
        /// telling us what our avatar is currently wearing
        /// <see cref="RequestRLVRuleChanged"/> request.</summary>
        public event EventHandler<RLVEventArgs> RLVRuleChanged
        {
            add { lock (m_RLVRuleChangedLock) { m_RLVRuleChanged += value; } }
            remove { lock (m_RLVRuleChangedLock) { m_RLVRuleChanged -= value; } }
        }
        #endregion

        public bool Enabled
        {

            get
            {
                if (instance.GlobalSettings["rlv_enabled"].Type == OSDType.Unknown)
                {
                    instance.GlobalSettings["rlv_enabled"] = new OSDBoolean(false);
                }

                return instance.GlobalSettings["rlv_enabled"].AsBoolean();
            }

            set
            {
                if (Enabled != instance.GlobalSettings["rlv_enabled"].AsBoolean())
                {
                    instance.GlobalSettings["rlv_enabled"] = new OSDBoolean(value);
                    OnRLVRuleChanged(new RLVEventArgs(null));
                }

                if (value)
                    StartTimer();
                else
                    StopTimer();
            }
        }

        RadegastInstance instance;
        Regex rlv_regex = new Regex(@"(?<behaviour>[^:=]+)(:(?<option>[^=]+))?=(?<param>\w+)", RegexOptions.Compiled);
        List<RLVRule> rules = new List<RLVRule>();
        System.Timers.Timer CleanupTimer;

        public RLVManager(RadegastInstance instance)
        {
            this.instance = instance;

            if (Enabled)
            {
                StartTimer();
            }
        }

        public void Dispose()
        {
            StopTimer();
        }

        void StartTimer()
        {
            StopTimer();
            CleanupTimer = new System.Timers.Timer()
            {
                Enabled = true,
                Interval = 120 * 1000 // two minutes
            };

            CleanupTimer.Elapsed += new System.Timers.ElapsedEventHandler(CleanupTimer_Elapsed);
        }

        void CleanupTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<UUID> objecs = new List<UUID>();
            lock (rules)
            {
                foreach (var rule in rules)
                {
                    if (!objecs.Contains(rule.Sender))
                        objecs.Add(rule.Sender);
                }
            }

            foreach (UUID obj in objecs)
            {
                if (instance.Client.Network.CurrentSim.ObjectsPrimitives.Find((Primitive p) => { return p.ID == obj; }) == null)
                {
                    Clear(obj);
                }
            }
        }

        void StopTimer()
        {
            if (CleanupTimer != null)
            {
                CleanupTimer.Enabled = false;
                CleanupTimer.Dispose();
                CleanupTimer = null;
            }
        }

        public bool TryProcessCMD(ChatEventArgs e)
        {
            if (!Enabled || !e.Message.StartsWith("@")) return false;

            if (e.Message == "@clear")
            {
                Clear(e.SourceID);
                return true;
            }

            foreach (var cmd in e.Message.Substring(1).Split(','))
            {
                var m = rlv_regex.Match(cmd);
                if (!m.Success) continue;
                
                var rule = new RLVRule();
                rule.Behaviour = m.Groups["behaviour"].ToString().ToLower(); ;
                rule.Option = m.Groups["option"].ToString().ToLower();
                rule.Param = m.Groups["param"].ToString().ToLower();
                rule.Sender = e.SourceID;
                rule.SenderName = e.FromName;

                Logger.DebugLog(rule.ToString());

                if (rule.Param == "rem") rule.Param = "y";
                if (rule.Param == "add") rule.Param = "n";

                if (rule.Param == "n")
                {
                    lock (rules) rules.Add(rule);
                    OnRLVRuleChanged(new RLVEventArgs(rule));
                    continue;
                }

                if (rule.Param == "y")
                {
                    lock (rules)
                    {
                        if (rule.Option == "")
                        {
                            rules.RemoveAll((RLVRule r) =>
                                {
                                    return r.Behaviour == rule.Behaviour && r.Sender == rule.Sender;
                                });
                        }
                        else
                        {
                            rules.RemoveAll((RLVRule r) =>
                            {
                                return r.Behaviour == rule.Behaviour && r.Sender == rule.Sender && r.Option == rule.Option;
                            });
                        }
                    }

                    OnRLVRuleChanged(new RLVEventArgs(rule));
                    continue;
                }


                switch (rule.Behaviour)
                {
                    case "version":
                        int chan = 0;
                        if (int.TryParse(rule.Param, out chan) && chan > 0)
                        {
                            instance.Client.Self.Chat("RestrainedLife viewer v1.20c (" + Properties.Resources.RadegastTitle + "." + RadegastBuild.CurrentRev + ")", chan, ChatType.Normal);
                        }
                        break;

                    case "versionnum":
                        if (int.TryParse(rule.Param, out chan) && chan > 0)
                        {
                            instance.Client.Self.Chat("1210100", chan, ChatType.Normal);
                        }
                        break;

                    case "sit":
                        UUID sitTarget = UUID.Zero;

                        if (rule.Param == "force" && UUID.TryParse(rule.Option, out sitTarget) && sitTarget != UUID.Zero)
                        {
                            instance.Client.Self.RequestSit(sitTarget, Vector3.Zero);
                            instance.Client.Self.Sit();
                        }
                        break;

                    case "unsit":
                        if (rule.Param == "force")
                        {
                            instance.Client.Self.Stand();
                        }
                        break;

                    case "setrot":
                        double rot = 0.0;

                        if (rule.Param == "force" && double.TryParse(rule.Option, System.Globalization.NumberStyles.Float, Utils.EnUsCulture, out rot))
                        {
                            instance.Client.Self.Movement.UpdateFromHeading(rot, true);
                        }
                        break;

                    case "tpto":
                        var coord = rule.Option.Split('/');
                        
                        try
                        {
                            float gx = float.Parse(coord[0], Utils.EnUsCulture);
                            float gy = float.Parse(coord[1], Utils.EnUsCulture);
                            float z = float.Parse(coord[2], Utils.EnUsCulture);
                            float x = 0, y = 0;

                            instance.TabConsole.DisplayNotificationInChat("Starting teleport...");
                            ulong h = Helpers.GlobalPosToRegionHandle(gx, gy, out x, out y);
                            instance.Client.Self.RequestTeleport(h, new Vector3(x, y, z));
                        }
                        catch (Exception) { }

                        break;

                }
            }


            return true;
        }

        public void Clear(UUID id)
        {
            lock (rules)
            {
                rules.RemoveAll((RLVRule r) => { return r.Sender == id; });
            }
        }

        public bool RestictionActive(string behaviour)
        {
            if (!Enabled) return false;

            if (rules.FindAll((RLVRule r) => { return r.Behaviour == behaviour && string.IsNullOrEmpty(r.Option); }).Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool RestictionActive(string behaviour, string exception)
        {
            if (!Enabled) return false;
            var set = rules.FindAll((RLVRule r) => { return r.Behaviour == behaviour; });

            if (set.Count > 0 && 
                set.FindAll((RLVRule r) => { return r.Option == exception; }).Count == 0 &&
                set.FindAll((RLVRule r) => { return string.IsNullOrEmpty(r.Option) ; }).Count > 0
                )
            {
                return true;
            }

            return false;
        }

        public List<string> GetOptions(string behaviour)
        {
            List<string> ret = new List<string>();

            foreach (var rule in rules.FindAll((RLVRule r) => { return r.Behaviour == behaviour && !string.IsNullOrEmpty(r.Option); }))
            {
                if (!ret.Contains(rule.Option)) ret.Add(rule.Option);
            }

            return ret;
        }

        public bool AllowDetach(AttachmentInfo a)
        {
            if (!Enabled || a == null) return true;

            if (rules.FindAll((RLVRule r) => { return r.Behaviour == "detach" && r.Sender == a.Prim.ID; }).Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool AutoAcceptTP(UUID agent)
        {
            if (!Enabled || agent == UUID.Zero) return false;

            if (rules.FindAll((RLVRule r) => { return r.Behaviour == "accepttp" && (r.Option == "" || r.Option == agent.ToString()); }).Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
