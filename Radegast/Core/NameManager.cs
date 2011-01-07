// 
// Radegast Metaverse Client
// Copyright (c) 2009-2010, Radegast Development Team
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
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public enum NameMode : int
    {
        Standard,
        DisplayNameAndUserName,
        OnlyDisplayName
    }

    public class NameManager : IDisposable
    {
        public event EventHandler<UUIDNameReplyEventArgs> NameUpdated;

        public NameMode Mode
        {
            get
            {
                if (!UseDisplayNames)
                    return NameMode.Standard;

                return (NameMode)instance.GlobalSettings["display_name_mode"].AsInteger();
            }

            set
            {
                instance.GlobalSettings["display_name_mode"] = (int)value;
            }
        }

        RadegastInstance instance;
        GridClient client { get { return instance.Client; } }
        Timer requestTimer;
        Timer cacheTimer;

        Queue<UUID> requests = new Queue<UUID>();

        bool UseDisplayNames
        {
            get
            {
                return client.Avatars.DisplayNamesAvailable();
            }
        }

        int MaxNameRequests = 80;

        const int REQUEST_DELAY = 100;
        const int CACHE_DELAY = 5000;

        Dictionary<UUID, AgentDisplayName> names = new Dictionary<UUID, AgentDisplayName>();

        public NameManager(RadegastInstance instance)
        {
            this.instance = instance;

            Mode = NameMode.DisplayNameAndUserName;

            requestTimer = new Timer(MakeRequest, null, Timeout.Infinite, Timeout.Infinite);
            cacheTimer = new Timer(SaveCache, null, Timeout.Infinite, Timeout.Infinite);

            instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
            RegisterEvents(client);
        }

        public void Dispose()
        {
            if (client != null)
            {
                DeregisterEvents(client);
            }

            if (requestTimer != null)
            {
                requestTimer.Dispose();
                requestTimer = null;
            }

            if (cacheTimer != null)
            {
                cacheTimer.Dispose();
                cacheTimer = null;
            }
        }

        void RegisterEvents(GridClient c)
        {
            c.Avatars.UUIDNameReply += new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
        }

        void DeregisterEvents(GridClient c)
        {
            c.Avatars.UUIDNameReply -= new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
        }

        DateTime UUIDNameOnly = new DateTime(1970, 9, 4, 10, 0, 0, DateTimeKind.Utc);

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            DeregisterEvents(e.OldClient);
            RegisterEvents(e.Client);
        }

        void Avatars_UUIDNameReply(object sender, UUIDNameReplyEventArgs e)
        {
            Dictionary<UUID, string> ret = new Dictionary<UUID, string>();

            foreach (KeyValuePair<UUID, string> kvp in e.Names)
            {
                lock (names)
                {
                    if (!names.ContainsKey(kvp.Key))
                    {
                        names[kvp.Key] = new AgentDisplayName();
                        names[kvp.Key].ID = kvp.Key;
                        names[kvp.Key].NextUpdate = UUIDNameOnly;
                        names[kvp.Key].IsDefaultDisplayName = true;
                    }

                    string[] parts = kvp.Value.Split(' ');
                    if (parts.Length == 2)
                    {
                        if (InvalidName(names[kvp.Key].DisplayName))
                        {
                            names[kvp.Key].DisplayName = string.Format("{0} {1}", parts[0], parts[1]);
                        }

                        names[kvp.Key].LegacyFirstName = parts[0];
                        names[kvp.Key].LegacyLastName = parts[1];
                        if (names[kvp.Key].LegacyLastName == "Resident")
                        {
                            names[kvp.Key].UserName = names[kvp.Key].LegacyFirstName.ToLower();
                        }
                        else
                        {
                            names[kvp.Key].UserName = string.Format("{0}.{1}", parts[0], parts[1]).ToLower();
                        }

                        ret.Add(kvp.Key, FormatName(names[kvp.Key]));
                    }
                }
            }

            if (ret.Count > 0 && NameUpdated != null)
            {
                try
                {
                    NameUpdated(this, new UUIDNameReplyEventArgs(ret));
                }
                catch (Exception ex)
                {
                    Logger.Log("Failure in event handler: " + ex.Message, Helpers.LogLevel.Warning, client, ex);
                }
            }

            TriggerCacheSave();
        }

        string FormatName(AgentDisplayName n)
        {
            switch (Mode)
            {
                case NameMode.OnlyDisplayName:
                    return n.DisplayName;

                case NameMode.DisplayNameAndUserName:
                    if (n.IsDefaultDisplayName)
                        return n.DisplayName;
                    else
                        return string.Format("{0} ({1})", n.DisplayName, n.UserName);

                default:
                    return n.LegacyFullName;
            }
        }

        bool InvalidName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName) ||
                displayName == "???" ||
                displayName == RadegastInstance.INCOMPLETE_NAME)
            {
                return true;
            }
            return false;
        }

        void ProcessDisplayNames(AgentDisplayName[] names)
        {
            Dictionary<UUID, string> ret = new Dictionary<UUID, string>();

            foreach (var name in names)
            {
                if (InvalidName(name.DisplayName)) continue;

                ret.Add(name.ID, FormatName(name));

                lock (this.names)
                {
                    this.names[name.ID] = name;
                }
            }

            if (ret.Count > 0 && NameUpdated != null)
            {
                try
                {
                    NameUpdated(this, new UUIDNameReplyEventArgs(ret));
                }
                catch (Exception ex)
                {
                    Logger.Log("Failure in event handler: " + ex.Message, Helpers.LogLevel.Warning, client, ex);
                }
            }

            TriggerCacheSave();
        }

        void MakeRequest(object sync)
        {
            List<UUID> req = new List<UUID>();
            lock (requests)
            {
                while (requests.Count > 0)
                    req.Add(requests.Dequeue());
            }


            if (req.Count > 0)
            {
                if (!UseDisplayNames)
                {
                    client.Avatars.RequestAvatarNames(req);
                }
                else
                {
                    client.Avatars.GetDisplayNames(req, (bool success, AgentDisplayName[] names, UUID[] badIDs) =>
                        {
                            if (success)
                            {
                                ProcessDisplayNames(names);
                            }
                            else
                            {
                                Logger.Log("Failed fetching display names", Helpers.LogLevel.Warning, client);
                            }
                        }
                    );
                }
            }
        }

        void SaveCache(object sync)
        {
        }

        void TriggerNameRequest()
        {
            requestTimer.Change(REQUEST_DELAY, Timeout.Infinite);
        }

        void TriggerCacheSave()
        {
            cacheTimer.Change(CACHE_DELAY, Timeout.Infinite);
        }

        void QueueNameRequest(UUID agentID)
        {
            lock (requests)
            {
                if (!requests.Contains(agentID))
                {
                    requests.Enqueue(agentID);

                    if (requests.Count >= MaxNameRequests && UseDisplayNames)
                    {
                        requestTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        MakeRequest(this);
                    }
                    else
                    {
                        TriggerNameRequest();
                    }
                }
            }
        }

        public string Get(UUID agentID)
        {
            lock (names)
            {
                if (names.ContainsKey(agentID))
                {
                    if (Mode != NameMode.Standard && names[agentID].NextUpdate == UUIDNameOnly)
                    {
                        QueueNameRequest(agentID);
                    }
                    return FormatName(names[agentID]);
                }
            }

            QueueNameRequest(agentID);
            return RadegastInstance.INCOMPLETE_NAME;
        }

        public string Get(UUID agentID, string defaultValue)
        {
            if (Mode == NameMode.Standard)
                return defaultValue;

            string name = Get(agentID);
            if (name == RadegastInstance.INCOMPLETE_NAME)
                return defaultValue;

            return name;
        }
    }
}
