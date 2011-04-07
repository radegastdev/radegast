// 
// Radegast Metaverse Client
// Copyright (c) 2009-2011, Radegast Development Team
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
    #region enums
    /// <summary>
    /// Enum representing different modes of handling display names
    /// </summary>
    public enum NameMode : int
    {
        /// <summary> No display names </summary>
        Standard,
        /// <summary> Display name followed by (username) if display name is not default  </summary>
        Smart,
        /// <summary> Display name followed by (username) </summary>
        OnlyDisplayName,
        /// <summary> Only display </summary>
        DisplayNameAndUserName,
    }
    #endregion enums

    /// <summary>
    /// Manager for looking up avatar names and their caching 
    /// </summary>
    public class NameManager : IDisposable
    {
        #region public fields and properties
        public event EventHandler<UUIDNameReplyEventArgs> NameUpdated;

        public NameMode Mode
        {
            get
            {
                if (!client.Avatars.DisplayNamesAvailable())
                    return NameMode.Standard;

                return (NameMode)instance.GlobalSettings["display_name_mode"].AsInteger();
            }

            set
            {
                instance.GlobalSettings["display_name_mode"] = (int)value;
            }
        }
        #endregion public fields and properties

        #region private fields and properties
        RadegastInstance instance;
        GridClient client { get { return instance.Client; } }
        Timer requestTimer;
        Timer cacheTimer;

        Queue<UUID> requests = new Queue<UUID>();

        int MaxNameRequests = 80;

        // Queue up name request for this many ms, and send a batch requsr
        const int REQUEST_DELAY = 100;
        // Save name cache after change to names after this many ms
        const int CACHE_DELAY = 5000;
        // Consider request failed after this many ms
        const int MAX_REQ_AGE = 15000;

        Dictionary<UUID, AgentDisplayName> names = new Dictionary<UUID, AgentDisplayName>();
        Dictionary<UUID, int> activeRequests = new Dictionary<UUID, int>();

        #endregion private fields and properties

        #region construction and disposal
        public NameManager(RadegastInstance instance)
        {
            this.instance = instance;

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
        #endregion construction and disposal

        #region private methods
        void RegisterEvents(GridClient c)
        {
            c.Avatars.UUIDNameReply += new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
            c.Avatars.DisplayNameUpdate += new EventHandler<DisplayNameUpdateEventArgs>(Avatars_DisplayNameUpdate);
        }

        void DeregisterEvents(GridClient c)
        {
            c.Avatars.UUIDNameReply -= new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
            c.Avatars.DisplayNameUpdate -= new EventHandler<DisplayNameUpdateEventArgs>(Avatars_DisplayNameUpdate);
        }

        DateTime UUIDNameOnly = new DateTime(1970, 9, 4, 10, 0, 0, DateTimeKind.Utc);

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            DeregisterEvents(e.OldClient);
            RegisterEvents(e.Client);
        }

        void Avatars_DisplayNameUpdate(object sender, DisplayNameUpdateEventArgs e)
        {
            lock (names)
            {
                names[e.DisplayName.ID] = e.DisplayName;
            }
            Dictionary<UUID, string> ret = new Dictionary<UUID, string>();
            ret.Add(e.DisplayName.ID, FormatName(e.DisplayName));
            TriggerEvent(ret);
        }

        void Avatars_UUIDNameReply(object sender, UUIDNameReplyEventArgs e)
        {
            Dictionary<UUID, string> ret = new Dictionary<UUID, string>();

            foreach (KeyValuePair<UUID, string> kvp in e.Names)
            {
                // Remove from the list of active requests if in UUID only (standard mode)
                if (Mode == NameMode.Standard)
                {
                    lock (activeRequests)
                    {
                        activeRequests.Remove(kvp.Key);
                    }
                }

                lock (names)
                {
                    if (!names.ContainsKey(kvp.Key))
                    {
                        names[kvp.Key] = new AgentDisplayName();
                        names[kvp.Key].ID = kvp.Key;
                        names[kvp.Key].NextUpdate = UUIDNameOnly;
                        names[kvp.Key].IsDefaultDisplayName = true;
                    }

                    string[] parts = kvp.Value.Trim().Split(' ');
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

            TriggerEvent(ret);
            TriggerCacheSave();
        }

        string FormatName(AgentDisplayName n)
        {
            switch (Mode)
            {
                case NameMode.OnlyDisplayName:
                    return n.DisplayName;

                case NameMode.Smart:
                    if (n.IsDefaultDisplayName)
                        return n.DisplayName;
                    else
                        return string.Format("{0} ({1})", n.DisplayName, n.UserName);

                case NameMode.DisplayNameAndUserName:
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
                // Remove from the list of active requests
                lock (activeRequests)
                {
                    activeRequests.Remove(name.ID);
                }

                if (InvalidName(name.DisplayName)) continue;

                ret.Add(name.ID, FormatName(name));

                lock (this.names)
                {
                    this.names[name.ID] = name;
                }
            }

            TriggerEvent(ret);
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
                // Request standard name anyway
                client.Avatars.RequestAvatarNames(req);

                if (Mode != NameMode.Standard) // Use display names
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

        void TriggerEvent(Dictionary<UUID, string> ret)
        {
            if (NameUpdated == null || ret.Count == 0) return;

            try
            {
                NameUpdated(this, new UUIDNameReplyEventArgs(ret));
            }
            catch (Exception ex)
            {
                Logger.Log("Failure in event handler: " + ex.Message, Helpers.LogLevel.Warning, client, ex);
            }
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
            // Check if we're already waiting for an answer on this one
            lock (activeRequests)
            {
                if (activeRequests.ContainsKey(agentID))
                {
                    // Logger.Log("Exiting is active " + agentID.ToString(), Helpers.LogLevel.Error);
                    if (Environment.TickCount - activeRequests[agentID] < MAX_REQ_AGE) // Not timeout yet
                    {
                        // Logger.Log("Exiting is active " + agentID.ToString(), Helpers.LogLevel.Error);
                        return;
                    }
                    else
                    {
                        // Logger.Log("Continuing, present but expired " + agentID.ToString(), Helpers.LogLevel.Error);
                    }
                }
                else
                {
                    // Logger.Log("Not present " + agentID.ToString(), Helpers.LogLevel.Error);
                }

                // Record time of when we're making this request
                activeRequests[agentID] = Environment.TickCount;
            }


            lock (requests)
            {
                if (!requests.Contains(agentID))
                {
                    // Logger.Log("Enqueueing " + agentID.ToString(), Helpers.LogLevel.Error);
                    requests.Enqueue(agentID);

                    if (requests.Count >= MaxNameRequests && Mode != NameMode.Standard)
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
        #endregion private methods

        #region public methods
        /// <summary>
        /// Gets legacy First Last name
        /// </summary>
        /// <param name="agentID">UUID of the agent</param>
        /// <returns></returns>
        public string GetLegacyName(UUID agentID)
        {
            if (agentID == UUID.Zero) return "(???) (???)";

            lock (names)
            {
                if (names.ContainsKey(agentID))
                {
                    return names[agentID].LegacyFullName;
                }
            }

            QueueNameRequest(agentID);
            return RadegastInstance.INCOMPLETE_NAME;
        }

        /// <summary>
        /// Get avatar display name, or queue fetching of the name
        /// </summary>
        /// <param name="agentID">UUID of avatar to lookup</param>
        /// <returns>Avatar display name or "Loading..." if not in cache</returns>
        public string Get(UUID agentID)
        {
            if (agentID == UUID.Zero) return "(???) (???)";

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

        /// <summary>
        /// Get avatar display name, or queue fetching of the name
        /// </summary>
        /// <param name="agentID">UUID of avatar to lookup</param>
        /// <param name="blocking">If true, wait until name is recieved, otherwise return immediately</param>
        /// <returns>Avatar display name or "Loading..." if not in cache</returns>
        public string Get(UUID agentID, bool blocking)
        {
            if (!blocking)
                Get(agentID);

            string name = null;

            using (ManualResetEvent gotName = new ManualResetEvent(false))
            {

                EventHandler<UUIDNameReplyEventArgs> handler = (object sender, UUIDNameReplyEventArgs e) =>
                {
                    if (e.Names.ContainsKey(agentID))
                    {
                        name = e.Names[agentID];
                        gotName.Set();
                    }
                };

                NameUpdated += handler;
                name = Get(agentID);

                if (name == RadegastInstance.INCOMPLETE_NAME)
                {
                    gotName.WaitOne(10 * 1000, false);
                }

                NameUpdated -= handler;
            }
            return name;
        }

        /// <summary>
        /// Get avatar display name, or queue fetching of the name
        /// </summary>
        /// <param name="agentID">UUID of avatar to lookup</param>
        /// <param name="defaultValue">If name failed to retrieve, use this</param>
        /// <returns>Avatar display name or the default value if not in cache</returns>
        public string Get(UUID agentID, string defaultValue)
        {
            if (Mode == NameMode.Standard)
                return defaultValue;

            string name = Get(agentID);
            if (name == RadegastInstance.INCOMPLETE_NAME)
                return defaultValue;

            return name;
        }

        /// <summary>
        /// Get avatar display name, or queue fetching of the name
        /// </summary>
        /// <param name="agentID">UUID of avatar to lookup</param>
        /// <param name="blocking">If true, wait until name is recieved, otherwise return immediately</param>
        /// <param name="defaultValue">If name failed to retrieve, use this</param>
        /// <returns></returns>
        public string Get(UUID agentID, bool blocking, string defaultValue)
        {
            if (Mode == NameMode.Standard)
                return defaultValue;

            string name = Get(agentID, blocking);
            if (name == RadegastInstance.INCOMPLETE_NAME)
                return defaultValue;

            return name;

        }

        #endregion public methods
    }
}
