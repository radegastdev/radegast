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
// $Id: FriendshipOfferNotification.cs 175 2009-08-29 13:52:32Z latifer@gmail.com $
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;
using OpenMetaverse.Packets;

namespace Radegast
{
    public partial class ntfInventoryOffer : Notification
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private InstantMessage msg;
        private AssetType type = AssetType.Unknown;
        private UUID objectID = UUID.Zero;
        UUID destinationFolderID;

        public ntfInventoryOffer(RadegastInstance instance, InstantMessage msg)
            : base (NotificationType.InventoryOffer)
        {
            InitializeComponent();
            Disposed += new EventHandler(ntfInventoryOffer_Disposed);

            this.instance = instance;
            this.msg = msg;

            instance.Names.NameUpdated += new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);

            if (msg.BinaryBucket.Length > 0)
            {
                type = (AssetType)msg.BinaryBucket[0];
                destinationFolderID = client.Inventory.FindFolderForType(type);

                if (msg.BinaryBucket.Length == 17)
                {
                    objectID = new UUID(msg.BinaryBucket, 1);
                }

                if (msg.Dialog == InstantMessageDialog.InventoryOffered)
                {
                    txtInfo.Text = string.Format("{0} has offered you {1} \"{2}\".", msg.FromAgentName, type.ToString(), msg.Message);
                }
                else if (msg.Dialog == InstantMessageDialog.TaskInventoryOffered)
                {
                    txtInfo.Text = objectOfferText();
                }

                // Fire off event
                NotificationEventArgs args = new NotificationEventArgs(instance);
                args.Text = txtInfo.Text;
                args.Buttons.Add(btnAccept);
                args.Buttons.Add(btnDiscard);
                args.Buttons.Add(btnIgnore);
                FireNotificationCallback(args);
            }
            else
            {
                Logger.Log("Wrong format of the item offered", Helpers.LogLevel.Warning, client);
            }

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void ntfInventoryOffer_Disposed(object sender, EventArgs e)
        {
            instance.Names.NameUpdated -= new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
        }

        void Avatars_UUIDNameReply(object sender, UUIDNameReplyEventArgs e)
        {
            if (e.Names.Keys.Contains(msg.FromAgentID))
            {
                instance.Names.NameUpdated -= new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
                BeginInvoke(new MethodInvoker(() => txtInfo.Text = objectOfferText()));
            }
        }

        private string objectOfferText()
        {
            return string.Format("Object \"{0}\" owned by {1} has offered you {2}", msg.FromAgentName, instance.Names.Get(msg.FromAgentID), msg.Message); 
        }

        private void SendReply(InstantMessageDialog dialog, byte[] bucket)
        {
            client.Self.InstantMessage(client.Self.Name, msg.FromAgentID, string.Empty, msg.IMSessionID, dialog, InstantMessageOnline.Offline, client.Self.SimPosition, client.Network.CurrentSim.RegionID, bucket);
            
            if (dialog == InstantMessageDialog.InventoryAccepted)
            {
                client.Inventory.RequestFetchInventory(objectID, client.Self.AgentID);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (type == AssetType.Unknown) return;

            if (msg.Dialog == InstantMessageDialog.InventoryOffered)
            {
                SendReply(InstantMessageDialog.InventoryAccepted, destinationFolderID.GetBytes());
            }
            else if (msg.Dialog == InstantMessageDialog.TaskInventoryOffered)
            {
                SendReply(InstantMessageDialog.TaskInventoryAccepted, destinationFolderID.GetBytes());
            }
            instance.MainForm.RemoveNotification(this);
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            if (type == AssetType.Unknown) return;

            if (msg.Dialog == InstantMessageDialog.InventoryOffered)
            {
                SendReply(InstantMessageDialog.InventoryDeclined, Utils.EmptyBytes);
                try
                {
                    client.Inventory.Move(
                        client.Inventory.Store.Items[objectID].Data,
                        (InventoryFolder)client.Inventory.Store.Items[client.Inventory.FindFolderForType(FolderType.Trash)].Data);
                }
                catch (Exception) { }
            }
            else if (msg.Dialog == InstantMessageDialog.TaskInventoryOffered)
            {
                SendReply(InstantMessageDialog.TaskInventoryDeclined, Utils.EmptyBytes);
            }
            instance.MainForm.RemoveNotification(this);
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }

        private void txtInfo_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            instance.MainForm.ProcessLink(e.LinkText, true);
        }
    }
}
