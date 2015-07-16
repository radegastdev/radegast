using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using OpenMetaverse;

namespace Radegast
{
    public class SlUriParser
    {
        private enum ResolveType
        {
            /// <summary>
            /// Client specified name format
            /// </summary>
            AgentDefaultName,
            /// <summary>
            /// Display
            /// </summary>
            AgentDisplayName,
            /// <summary>
            /// first.last
            /// </summary>
            AgentUsername,
            /// <summary>
            /// Group name
            /// </summary>
            Group,
            /// <summary>
            /// Parcel name
            /// </summary>
            Parcel
        };

        // Regular expression created by following the majority of http://wiki.secondlife.com/wiki/Viewer_URI_Name_Space (excluding support for secondlife:///app/login).
        //  This is a nasty one and should really only be used on single links to minimize processing time.
        private readonly Regex patternUri = new Regex(
            @"(?<startingbrace>\[)?(" +
                @"(?<regionuri>secondlife://(?<region_name>[^\]/ ]+)(/(?<local_x>[0-9]+))?(/(?<local_y>[0-9]+))?(/(?<local_z>[0-9]+))?)|" +
                @"(?<appuri>secondlife:///app/(" +
                    @"(?<appcommand>agent)/(?<agent_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})/(?<action>[a-z]+)|" +
                    @"(?<appcommand>apperance)/show|" +
                    @"(?<appcommand>balance)/request|" +
                    @"(?<appcommand>chat)/(?<channel>\d+)/(?<text>[^\] ]+)|" + 
                    @"(?<appcommand>classified)/(?<classified_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})/about|" +
                    @"(?<appcommand>event)/(?<event_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})/about|" +
                    @"(?<appcommand>group)/(" +
                        @"(?<group_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})/(?<action>[a-z]+)|" +
                        @"(?<action>create)|" +
                        @"(?<action>list/show))|" +
                    @"(?<appcommand>help)/?<help_query>([^\] ]+)|" +
                    @"(?<appcommand>inventory)/(" +
                        @"(?<inventory_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})/(?<action>select)/?" +
                            @"([?&](" +
                                @"name=(?<name>[^& ]+)" +
                            @"))*|" +
                        @"(?<action>show))|" +
                    @"(?<appcommand>maptrackavatar)/(?<friend_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})|" +
                    @"(?<appcommand>objectim)/(?<object_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})/?" +
                        @"([?&](" +
                            @"name=(?<name>[^& ]+)|" +
                            @"owner=(?<owner>[^& ]+)|" +
                            @"groupowned=(?<groupowned>true)|" +
                            @"slurl=(?<region_name>[^\]/ ]+)(/(?<x>[0-9]+\.?[0-9]*))?(/(?<y>[0-9]+\.?[0-9]*))?(/(?<z>[0-9]+\.?[0-9]*))?" +
                        @"))*|" +
                    @"(?<appcommand>parcel)/(?<parcel_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})/about|" +
                    @"(?<appcommand>search)/(?<category>[a-z]+)/(?<search_term>[^\]/ ]+)|" +
                    @"(?<appcommand>sharewithavatar)/(?<agent_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})|" +
                    @"(?<appcommand>teleport)/(?<region_name>[^\]/ ]+)(/(?<local_x>[0-9]+))?(/(?<local_y>[0-9]+))?(/(?<local_z>[0-9]+))?|" +
                    @"(?<appcommand>voicecallavatar)/(?<agent_id>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})|" +
                    @"(?<appcommand>wear_folder)/?folder_id=(?<inventory_folder_uuid>[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})|" +
                    @"(?<appcommand>worldmap)/(?<region_name>[^\]/ ]+)(/(?<local_x>[0-9]+))?(/(?<local_y>[0-9]+))?(/(?<local_z>[0-9]+))?)))" +
            @"( (?<endingbrace>[^\]]*)\])?", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        /// <summary>
        /// Gets the display text for the specified URI
        /// </summary>
        /// <param name="uri">URI to get the display text of</param>
        /// <returns>Display text for URI</returns>
        public string GetLinkName(string uri)
        {
            if (!RadegastInstance.GlobalInstance.GlobalSettings["resolve_uris"])
            {
                return uri;
            }

            Match match = patternUri.Match(uri);
            if (!match.Success)
            {
                return uri;
            }

            // Custom named links in the form of [secondlife://<truncated> Custom%20Link%20Name] will
            //   result in a link named 'Custom Link Name' regardless of the previous secondlife URI.
            if (match.Groups["startingbrace"].Success && match.Groups["endingbrace"].Length > 0)
            {
                return HttpUtility.UrlDecode(match.Groups["endingbrace"].Value);
            }

            if (match.Groups["regionuri"].Success)
            {
                return GetLinkNameRegionUri(match);
            }

            if (match.Groups["appuri"].Success)
            {
                string appcommand = match.Groups["appcommand"].Value;

                switch (appcommand)
                {
                    case "agent":
                        return GetLinkNameAgent(match);
                    case "appearance":
                        return match.ToString();
                    case "balance":
                        return match.ToString();
                    case "chat":
                        return GetLinkNameChat(match);
                    case "classified":
                        return GetLinkNameClassified(match);
                    case "event":
                        return GetLinkNameEvent(match);
                    case "group":
                        return GetLinkNameGroup(match);
                    case "help":
                        return GetLinkNameHelp(match);
                    case "inventory":
                        return GetLinkNameInventory(match);
                    case "maptrackavatar":
                        return GetLinkNameTrackAvatar(match);
                    case "objectim":
                        return GetLinkNameObjectIm(match);
                    case "parcel":
                        return GetLinkNameParcel(match);
                    case "search":
                        return GetLinkNameSearch(match);
                    case "sharewithavatar":
                        return GetLinkNameShareWithAvatar(match);
                    case "teleport":
                        return GetLinkNameTeleport(match);
                    case "voicecallavatar":
                        return GetLinkNameVoiceCallAvatar(match);
                    case "wear_folder":
                        return GetLinkNameWearFolder(match);
                    case "worldmap":
                        return GetLinkNameWorldMap(match);
                    default:
                        return match.ToString();
                }
            }

            return match.ToString();
        }

        /// <summary>
        /// Parses and executes the specified SecondLife URI if valid
        /// </summary>
        /// <param name="uri">URI to parse and execute</param>
        public void ExecuteLink(string uri)
        {
            Match match = patternUri.Match(uri);
            if (!match.Success)
            {
                return;
            }

            if (match.Groups["regionuri"].Success)
            {
                ExecuteLinkRegionUri(match);
            }
            else if (match.Groups["appuri"].Success)
            {
                string appcommand = match.Groups["appcommand"].Value;

                switch (appcommand)
                {
                    case "agent":
                        ExecuteLinkAgent(match);
                        return;
                    case "appearance":
                        ExecuteLinkShowApperance();
                        return;
                    case "balance":
                        ExecuteLinkShowBalance();
                        return;
                    case "chat":
                        ExecuteLinkChat(match);
                        return;
                    case "classified":
                        ExecuteLinkClassified(match);
                        return;
                    case "event":
                        ExecuteLinkEvent(match);
                        return;
                    case "group":
                        ExecuteLinkGroup(match);
                        return;
                    case "help":
                        ExecuteLinkHelp(match);
                        return;
                    case "inventory":
                        ExecuteLinkInventory(match);
                        return;
                    case "maptrackavatar":
                        ExecuteLinkTrackAvatar(match);
                        return;
                    case "objectim":
                        ExecuteLinkObjectIm(match);
                        return;
                    case "parcel":
                        ExecuteLinkParcel(match);
                        return;
                    case "search":
                        ExecuteLinkSearch(match);
                        return;
                    case "sharewithavatar":
                        ExecuteLinkShareWithAvatar(match);
                        return;
                    case "teleport":
                        ExecuteLinkTeleport(match);
                        return;
                    case "voicecallavatar":
                        ExecuteLinkVoiceCallAvatar(match);
                        return;
                    case "wear_folder":
                        ExecuteLinkWearFolder(match);
                        return;
                    case "worldmap":
                        ExecuteLinkWorldMap(match);
                        return;
                }
            }
        }

        #region Name Resolution

        /// <summary>
        /// Gets the name of an agent by UUID. Will block for a short period of time to allow for name resolution.
        /// </summary>
        /// <param name="agentID">Agent UUID</param>
        /// <param name="nameType">Type of name resolution. See ResolveType</param>
        /// <returns>Name of agent on success, INCOMPLETE_NAME on failure or timeout</returns>
        private string GetAgentName(UUID agentID, ResolveType nameType)
        {
            RadegastInstance instance = RadegastInstance.GlobalInstance;
            string name = RadegastInstance.INCOMPLETE_NAME;

            using (ManualResetEvent gotName = new ManualResetEvent(false))
            {
                EventHandler<UUIDNameReplyEventArgs> handler = (object sender, UUIDNameReplyEventArgs e) =>
                {
                    if (e.Names.ContainsKey(agentID))
                    {
                        name = e.Names[agentID];
                        try
                        {
                            gotName.Set();
                        }
                        catch (ObjectDisposedException) { }
                    }
                };

                instance.Names.NameUpdated += handler;

                if (nameType == ResolveType.AgentDefaultName)
                {
                    name = instance.Names.Get(agentID);
                }
                else if (nameType == ResolveType.AgentUsername)
                {
                    name = instance.Names.GetUserName(agentID);
                }
                else if (nameType == ResolveType.AgentDisplayName)
                {
                    name = instance.Names.GetDisplayName(agentID);
                }
                else
                {
                    instance.Names.NameUpdated -= handler;
                    return agentID.ToString();
                }

                if (name == RadegastInstance.INCOMPLETE_NAME)
                {
                    gotName.WaitOne(instance.GlobalSettings["resolve_uri_time"], false);
                }

                instance.Names.NameUpdated -= handler;
            }

            return name;
        }

        /// <summary>
        /// Gets the name of a group by UUID. Will block for a short period of time to allow for name resolution.
        /// </summary>
        /// <param name="groupID">Group UUID</param>
        /// <returns>Name of the group on success, INCOMPLETE_NAME on failure or timeout</returns>
        private string GetGroupName(UUID groupID)
        {
            RadegastInstance instance = RadegastInstance.GlobalInstance;
            string name = RadegastInstance.INCOMPLETE_NAME;

            using (ManualResetEvent gotName = new ManualResetEvent(false))
            {
                EventHandler<GroupNamesEventArgs> handler = (object sender, GroupNamesEventArgs e) =>
                {
                    if (e.GroupNames.ContainsKey(groupID))
                    {
                        name = e.GroupNames[groupID];
                        try
                        {
                            gotName.Set();
                        }
                        catch (ObjectDisposedException) { }
                    }
                };

                instance.Client.Groups.GroupNamesReply += handler;
                instance.Client.Groups.RequestGroupName(groupID);
                if (name == RadegastInstance.INCOMPLETE_NAME)
                {
                    gotName.WaitOne(instance.GlobalSettings["resolve_uri_time"], false);
                }

                instance.Client.Groups.GroupNamesReply -= handler;
            }

            return name;
        }

        /// <summary>
        /// Gets the name of a parcel by UUID. Will block for a short period of time to allow for name resolution.
        /// </summary>
        /// <param name="parcelID">Parcel UUID</param>
        /// <returns>Name of the parcel on success, INCOMPLETE_NAME on failure or timeout</returns>
        private string GetParcelName(UUID parcelID)
        {
            RadegastInstance instance = RadegastInstance.GlobalInstance;
            string name = RadegastInstance.INCOMPLETE_NAME;
            
            using (ManualResetEvent gotName = new ManualResetEvent(false))
            {
                EventHandler<ParcelInfoReplyEventArgs> handler = (object sender, ParcelInfoReplyEventArgs e) =>
                {
                    if (e.Parcel.ID == parcelID)
                    {
                        name = e.Parcel.Name;
                        try
                        {
                            gotName.Set();
                        }
                        catch (ObjectDisposedException) { }
                    }
                };

                instance.Client.Parcels.ParcelInfoReply += handler;
                instance.Client.Parcels.RequestParcelInfo(parcelID);
                if (name == RadegastInstance.INCOMPLETE_NAME)
                {
                    gotName.WaitOne(instance.GlobalSettings["resolve_uri_time"], false);
                }

                instance.Client.Parcels.ParcelInfoReply -= handler;
            }

            return name;
        }
        #endregion

        /// <summary>
        /// Attempts to resolve the name of a given key by type (Agent, Group, Parce, etc)
        /// </summary>
        /// <param name="id">UUID of object to resolve</param>
        /// <param name="type">Type of object</param>
        /// <returns>Revoled name</returns>
        private string Resolve(UUID id, ResolveType type)
        {
            switch (type)
            {
                case ResolveType.AgentDefaultName:
                case ResolveType.AgentDisplayName:
                case ResolveType.AgentUsername:
                    return GetAgentName(id, type);
                case ResolveType.Group:
                    return GetGroupName(id);
                case ResolveType.Parcel:
                    return GetParcelName(id);
                default:
                    return id.ToString();
            }
        }

        #region Link name resolution

        private string GetLinkNameRegionUri(Match match)
        {
            string name = HttpUtility.UrlDecode(match.Groups["region_name"].Value);

            string coordinateString = "";
            if (match.Groups["local_x"].Success)
            {
                coordinateString += " (" + match.Groups["local_x"].Value;
            }
            if (match.Groups["local_y"].Success)
            {
                coordinateString += "," + match.Groups["local_y"].Value;
            }
            if (match.Groups["local_z"].Success)
            {
                coordinateString += "," + match.Groups["local_z"].Value;
            }
            if (coordinateString != "")
            {
                coordinateString += ")";
            }

            return string.Format("{0}{1}", name, coordinateString);
        }

        private string GetLinkNameAgent(Match match)
        {
            UUID agentID = new UUID(match.Groups["agent_id"].Value);
            string action = match.Groups["action"].Value;

            switch (action)
            {
                case "about":
                case "inspect":
                case "completename":
                    return Resolve(agentID, ResolveType.AgentDefaultName);
                case "displayname":
                    return Resolve(agentID, ResolveType.AgentDisplayName);
                case "username":
                    return Resolve(agentID, ResolveType.AgentUsername);
                case "im":
                    return "IM " + Resolve(agentID, ResolveType.AgentDefaultName);
                case "offerteleport":
                    return "Offer Teleport to " + Resolve(agentID, ResolveType.AgentDefaultName);
                case "pay":
                    return "Pay " + Resolve(agentID, ResolveType.AgentDefaultName);
                case "requestfriend":
                    return "Friend Request " + Resolve(agentID, ResolveType.AgentDefaultName);
                case "mute":
                    return "Mute " + Resolve(agentID, ResolveType.AgentDefaultName);
                case "unmute":
                    return "Unmute " + Resolve(agentID, ResolveType.AgentDefaultName);
                default:
                    return match.ToString();
            }
        }

        private string GetLinkNameChat(Match match)
        {
            //string channel = match.Groups["channel"].Value;
            //string text = System.Web.HttpUtility.UrlDecode(match.Groups["text"].Value);

            return match.ToString();
        }

        private string GetLinkNameClassified(Match match)
        {
            //UUID classifiedID = new UUID(match.Groups["classified_id"].Value);

            return match.ToString();
        }

        private string GetLinkNameEvent(Match match)
        {
            //UUID eventID = new UUID(match.Groups["event_id"].Value);

            return match.ToString();
        }

        private string GetLinkNameGroup(Match match)
        {
            string action = match.Groups["action"].Value;

            switch (action)
            {
                case "about":
                case "inspect":
                {
                    UUID groupID = new UUID(match.Groups["group_id"].Value);
                    return Resolve(groupID, ResolveType.Group);
                }
                case "create":
                case "list/show":
                    return match.ToString();
            }

            return match.ToString();
        }

        private string GetLinkNameHelp(Match match)
        {
            //string helpQuery = HttpUtility.UrlDecode(match.Groups["help_query"].Value);

            return match.ToString();
        }

        private string GetLinkNameInventory(Match match)
        {
            //UUID inventoryID = new UUID(match.Groups["agent_id"].Value);
            string action = match.Groups["action"].Value;

            if (action == "select" && match.Groups["name"].Success)
            {
                return HttpUtility.UrlDecode(match.Groups["name"].Value);
            }

            return match.ToString();
        }

        private string GetLinkNameTrackAvatar(Match match)
        {
            //UUID agentID = new UUID(match.Groups["friend_id"].Value);

            return match.ToString();
        }

        private string GetLinkNameObjectIm(Match match)
        {
            //UUID objectID = new UUID(match.Groups["object_id"].Value);
            string name = HttpUtility.UrlDecode(match.Groups["name"].Value);
            //UUID ownerID = new UUID(match.Groups["owner"].Value);
            //string groupowned = match.Groups["groupowned"].Value;
            //string slurl = match.Groups["slurl"].Value;

            if (name != string.Empty)
            {
                return name;
            }

            return match.ToString();
        }

        private string GetLinkNameParcel(Match match)
        {
            UUID parcelID = new UUID(match.Groups["parcel_id"].Value);
            return Resolve(parcelID, ResolveType.Parcel);
        }

        private string GetLinkNameSearch(Match match)
        {
            //string category = match.Groups["category"].Value;
            //string searchTerm = HttpUtility.UrlDecode(match.Groups["search_term"].Value);

            return match.ToString();
        }

        private string GetLinkNameShareWithAvatar(Match match)
        {
            //UUID agentID = new UUID(match.Groups["agent_id"].Value);

            return match.ToString();
        }

        private string GetLinkNameTeleport(Match match)
        {
            string name = HttpUtility.UrlDecode(match.Groups["region_name"].Value);

            string coordinateString = "";
            if (match.Groups["local_x"].Success)
            {
                coordinateString += " (" + match.Groups["local_x"].Value;
            }
            if (match.Groups["local_y"].Success)
            {
                coordinateString += "," + match.Groups["local_y"].Value;
            }
            if (match.Groups["local_z"].Success)
            {
                coordinateString += "," + match.Groups["local_z"].Value;
            }
            if (coordinateString != "")
            {
                coordinateString += ")";
            }

            return string.Format("Teleport to {0}{1}", name, coordinateString);
        }

        private string GetLinkNameVoiceCallAvatar(Match match)
        {
            //UUID agentID = new UUID(match.Groups["agent_id"].Value);

            return match.ToString();
        }

        private string GetLinkNameWearFolder(Match match)
        {
            //UUID folderID = new UUID(match.Groups["inventory_folder_uuid"].Value);

            return match.ToString();
        }

        private string GetLinkNameWorldMap(Match match)
        {
            string name = HttpUtility.UrlDecode(match.Groups["region_name"].Value);
            string x = match.Groups["local_x"].Success ? match.Groups["local_x"].Value : "128";
            string y = match.Groups["local_y"].Success ? match.Groups["local_y"].Value : "128";
            string z = match.Groups["local_z"].Success ? match.Groups["local_z"].Value : "0";

            return string.Format("Show Map for {0} ({1},{2},{3})", name, x, y, z);
        }
        #endregion

        #region Link Execution
        private void ExecuteLinkRegionUri(Match match)
        {
            RadegastInstance instance = RadegastInstance.GlobalInstance;

            string name = HttpUtility.UrlDecode(match.Groups["region_name"].Value);
            int x = match.Groups["local_x"].Success ? int.Parse(match.Groups["local_x"].Value) : 128;
            int y = match.Groups["local_y"].Success ? int.Parse(match.Groups["local_y"].Value) : 128;
            int z = match.Groups["local_z"].Success ? int.Parse(match.Groups["local_z"].Value) : 0;

            instance.MainForm.MapTab.Select();
            instance.MainForm.WorldMap.DisplayLocation(name, x, y, z);
        }

        private void ExecuteLinkAgent(Match match)
        {
            RadegastInstance instance = RadegastInstance.GlobalInstance;
            UUID agentID = new UUID(match.Groups["agent_id"].Value);
            //string action = match.Groups["action"].Value;

            RadegastInstance.GlobalInstance.MainForm.ShowAgentProfile(instance.Names.Get(agentID), agentID);
        }

        private void ExecuteLinkShowApperance()
        {

        }

        private void ExecuteLinkShowBalance()
        {

        }

        private void ExecuteLinkChat(Match match)
        {
            //string channel = match.Groups["channel"].Value;
            //string text = System.Web.HttpUtility.UrlDecode(match.Groups["text"].Value);
        }

        private void ExecuteLinkClassified(Match match)
        {
            //UUID classifiedID = new UUID(match.Groups["classified_id"].Value);
        }

        private void ExecuteLinkEvent(Match match)
        {
            //UUID eventID = new UUID(match.Groups["event_id"].Value);
        }

        private void ExecuteLinkGroup(Match match)
        {
            RadegastInstance instance = RadegastInstance.GlobalInstance;
            string action = match.Groups["action"].Value;

            switch (action)
            {
                case "about":
                case "inspect":
                {
                    UUID groupID = new UUID(match.Groups["group_id"].Value);
                    instance.MainForm.ShowGroupProfile(groupID);
                    return;
                }
                case "create":
                    return;
                case "list/show":
                    return;
            }
        }

        private void ExecuteLinkHelp(Match match)
        {
            //string helpQuery = HttpUtility.UrlDecode(match.Groups["help_query"].Value);
        }

        private void ExecuteLinkInventory(Match match)
        {
            //UUID inventoryID = new UUID(match.Groups["agent_id"].Value);
            //string action = match.Groups["action"].Value;
        }

        private void ExecuteLinkTrackAvatar(Match match)
        {
            //UUID agentID = new UUID(match.Groups["friend_id"].Value);
        }

        private void ExecuteLinkObjectIm(Match match)
        {
            //UUID objectID = new UUID(match.Groups["object_id"].Value);
            //string name = HttpUtility.UrlDecode(match.Groups["name"].Value);
            //UUID ownerID = new UUID(match.Groups["owner"].Value);
            //string groupowned = match.Groups["groupowned"].Value;
            //string slurl = match.Groups["slurl"].Value;
        }

        private void ExecuteLinkParcel(Match match)
        {
            //UUID parcelID = new UUID(match.Groups["parcel_id"].Value);
        }

        private void ExecuteLinkSearch(Match match)
        {
            //string category = match.Groups["category"].Value;
            //string searchTerm = HttpUtility.UrlDecode(match.Groups["search_term"].Value);
        }

        private void ExecuteLinkShareWithAvatar(Match match)
        {
            //UUID agentID = new UUID(match.Groups["agent_id"].Value);
        }

        private void ExecuteLinkTeleport(Match match)
        {
            //string name = HttpUtility.UrlDecode(match.Groups["region_name"].Value);
            //string x = match.Groups["local_x"].Success ? match.Groups["local_x"].Value : "128";
            //string y = match.Groups["local_y"].Success ? match.Groups["local_y"].Value : "128";
            //string z = match.Groups["local_z"].Success ? match.Groups["local_z"].Value : "0";
        }

        private void ExecuteLinkVoiceCallAvatar(Match match)
        {
            //UUID agentID = new UUID(match.Groups["agent_id"].Value);
        }

        private void ExecuteLinkWearFolder(Match match)
        {
            //UUID folderID = new UUID(match.Groups["inventory_folder_uuid"].Value);
        }

        private void ExecuteLinkWorldMap(Match match)
        {
            RadegastInstance instance = RadegastInstance.GlobalInstance;

            string name = HttpUtility.UrlDecode(match.Groups["region_name"].Value);
            int x = match.Groups["local_x"].Success ? int.Parse(match.Groups["local_x"].Value) : 128;
            int y = match.Groups["local_y"].Success ? int.Parse(match.Groups["local_y"].Value) : 128;
            int z = match.Groups["local_z"].Success ? int.Parse(match.Groups["local_z"].Value) : 0;

            instance.MainForm.MapTab.Select();
            instance.MainForm.WorldMap.DisplayLocation(name, x, y, z);
        }
        #endregion
    }
}
