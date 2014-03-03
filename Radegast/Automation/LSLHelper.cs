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
using System.Windows.Forms;
using System.Threading;

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

        /// <summary>
        /// Dispatcher for incoming IM automation
        /// </summary>
        /// <param name="e">Incoming message</param>
        /// <returns>If message processed correctly, should GUI processing be halted</returns>
        public bool ProcessIM(InstantMessageEventArgs e)
        {
            LoadSettings();

            if (!Enabled)
            {
                return false;
            }

            switch (e.IM.Dialog)
            {
                case InstantMessageDialog.MessageFromObject:
                    {
                        if (e.IM.FromAgentID != AllowedOwner)
                        {
                            return true;
                        }
                        string[] args = e.IM.Message.Trim().Split('^');
                        if (args.Length < 1) return false;

                        switch (args[0].Trim())
                        {
                            case "group_invite":
                                {
                                    if (args.Length < 4) return false;
                                    ProcessInvite(args);
                                    return true;
                                }
                            case "send_im":
                                {
                                    if (args.Length < 3) return false;
                                    UUID sendTo = UUID.Zero;
                                    if (!UUID.TryParse(args[1].Trim(), out sendTo)) return false;
                                    string msg = args[2].Trim();
                                    client.Self.InstantMessage(sendTo, msg);
                                    return true;
                                }
                            case "give_inventory":
                                {
                                    if (args.Length < 3) return false;
                                    UUID sendTo = UUID.Zero;
                                    UUID invItemID = UUID.Zero;
                                    if (!UUID.TryParse(args[1].Trim(), out sendTo)) return false;
                                    if (!UUID.TryParse(args[2].Trim(), out invItemID)) return false;
                                    if (!client.Inventory.Store.Contains(invItemID))
                                    {
                                        instance.TabConsole.DisplayNotificationInChat(
                                            string.Format("Tried to offer {0} but could not find it in my inventory", invItemID),
                                            ChatBufferTextStyle.Error);
                                        return false;
                                    }
                                    InventoryItem item = client.Inventory.Store[invItemID] as InventoryItem;
                                    if (item == null)
                                        return false;
                                    client.Inventory.GiveItem(item.UUID, item.Name, item.AssetType, sendTo, true);
                                    WorkPool.QueueUserWorkItem(sync =>
                                        instance.TabConsole.DisplayNotificationInChat(
                                            string.Format("Gave {0} to {1}", item.Name, instance.Names.Get(sendTo, true)),
                                            ChatBufferTextStyle.ObjectChat)
                                    );
                                    return true;
                                }
                        }
                    }
                    break;
            }

            return false;
        }

        private void ProcessInvite(string[] args)
        {
            if (args == null || args.Length < 4)
                return;

            WorkPool.QueueUserWorkItem(sync =>
            {
                try
                {
                    UUID invitee = UUID.Zero;
                    UUID groupID = UUID.Zero;
                    UUID roleID = UUID.Zero;
                    if (!UUID.TryParse(args[1].Trim(), out invitee)) return;
                    if (!UUID.TryParse(args[2].Trim(), out groupID)) return;
                    if (!UUID.TryParse(args[3].Trim(), out roleID)) return;

                    if (instance.Groups.ContainsKey(groupID))
                    {
                        AutoResetEvent gotMembers = new AutoResetEvent(false);
                        Dictionary<UUID, GroupMember> Members = null;
                        EventHandler<GroupMembersReplyEventArgs> handler = (sender, e) =>
                        {
                            if (e.GroupID != groupID)
                                return;
                            Members = e.Members;
                            gotMembers.Set();
                        };

                        client.Groups.GroupMembersReply += handler;
                        client.Groups.RequestGroupMembers(groupID);
                        bool success = gotMembers.WaitOne(30 * 1000, false);
                        client.Groups.GroupMembersReply -= handler;

                        if (Members != null && Members.ContainsKey(invitee))
                        {
                            instance.TabConsole.DisplayNotificationInChat(
                                string.Format("Not inviting {0} ({1}) to {2} ({3}), already member", instance.Names.Get(invitee, true), invitee, instance.Groups[groupID].Name, groupID),
                                ChatBufferTextStyle.ObjectChat);
                        }
                        else
                        {
                            instance.TabConsole.DisplayNotificationInChat(
                                string.Format("Inviting {0} ({1}) to {2} ({3})", instance.Names.Get(invitee, true), invitee, instance.Groups[groupID].Name, groupID),
                                ChatBufferTextStyle.ObjectChat);
                            client.Groups.Invite(groupID, new List<UUID>(1) { roleID }, invitee);
                        }
                    }
                    else
                    {
                        instance.TabConsole.DisplayNotificationInChat(
                            string.Format("Cannot invite to group {0}, I don't appear to be in it.", groupID),
                            ChatBufferTextStyle.Error);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Failed proccessing automation IM: " + ex.ToString(), Helpers.LogLevel.Warning);
                }
            });
        }
    }
}
