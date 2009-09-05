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
// $Id: FriendshipOfferNotification.cs 118 2009-07-20 00:39:00Z latifer $
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Radegast
{
    /// <summary>
    /// What kind of notification this is (blue dialog)
    /// </summary>
    public enum NotificationType
    {
        Generic,
        FriendshipOffer,
        GroupInvitation,
        GroupNotice,
        PermissionsRequest,
        ScriptDialog,
        Teleport,
        InventoryOffer
    }

    /// <summary>
    /// Fired when blue dialog notification is displayed
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Type of notfication
        /// </summary>
        public NotificationType Type;

        /// <summary>
        /// Instance of Radegast where the event occured
        /// </summary>
        public RadegastInstance Instance;

        /// <summary>
        /// Notification text
        /// </summary>
        public string Text = string.Empty;

        /// <summary>
        /// Buttons displayed on the notification window
        /// </summary>
        public List<Button> Buttons = new List<Button>();

        /// <summary>
        /// Create new event args object
        /// </summary>
        /// <param name="instance">Instance of Radegast notification is coming from</param>
        public NotificationEventArgs(RadegastInstance instance)
        {
            Instance = instance;
        }
    }

    /// <summary>
    /// Base class for all notificatiosn (blue dialogs)
    /// </summary>
    public class Notification : UserControl
    {
        /// <summary>
        /// Notification type
        /// </summary>
        public NotificationType Type;

        /// <summary>
        /// Callback when blue dialog notification is displayed
        /// </summary>
        /// <param name="sender">Notification dialog</param>
        /// <param name="e">Notification parameters</param>
        public delegate void NotificationCallback(object sender, NotificationEventArgs e);

        /// <summary>
        /// Fired when a notification is displayed
        /// </summary>
        public static event NotificationCallback OnNotificationDisplayed;

        public Notification()
        {
            Type = NotificationType.Generic;
        }

        public Notification(NotificationType type)
        {
            Type = type;
        }

        protected void FireNotificationCallback(NotificationEventArgs e)
        {
            if (OnNotificationDisplayed == null) return;

            try
            {
                e.Type = this.Type;
                ThreadPool.QueueUserWorkItem((object o) => OnNotificationDisplayed(this, e));
            }
            catch (Exception ex)
            {
                OpenMetaverse.Logger.Log("Error executing notification callback", OpenMetaverse.Helpers.LogLevel.Warning, ex);
            }
        }
    }
}
