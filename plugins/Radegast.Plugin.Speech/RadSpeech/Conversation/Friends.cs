// 
// Radegast Metaverse Client Speech Interface
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
// $Id: Friends.cs 203 2009-09-16 17:26:02Z mojitotech@gmail.com $
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radegast;
using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    class Friends : Mode
    {
        private FriendsConsole frTab;
        public bool Announce { get; set; }
        private System.Windows.Forms.ListBox friends;

        #region statechange
        internal Friends(PluginControl pc)
            : base(pc)
        {
            Title = "friends";
            Announce = false;
            frTab = (FriendsConsole)control.instance.TabConsole.Tabs["friends"].Control;
            friends = frTab.listFriends;

            control.instance.Client.Friends.FriendOffline +=
                new EventHandler<FriendInfoEventArgs>(Friends_OnFriendOffline);
            control.instance.Client.Friends.FriendOnline +=
                new EventHandler<FriendInfoEventArgs>(Friends_OnFriendOnline);
        }

        /// <summary>
        /// Friends conversation becomes active
        /// </summary>
        internal override void Start()
        {
            base.Start();
            friends.SelectedIndexChanged += new EventHandler(friends_ItemSelectionChanged);
            Talker.SayMore("Friends");
        }

        /// <summary>
        /// Friends conversation becomes inactive
        /// </summary>
        internal override void Stop()
        {
            friends.SelectedIndexChanged -= new EventHandler(friends_ItemSelectionChanged);
            base.Stop();
        }
        #endregion

        #region generalevents
        private void Friends_OnFriendOffline(object sender, FriendInfoEventArgs e)
        {
            if (Announce)
                Talker.Say( "Your friend " + e.Friend.Name + " went off line.");
        }

        private void Friends_OnFriendOnline(object sender, FriendInfoEventArgs e)
        {
            if (Announce)
                Talker.Say("Your friend " + e.Friend.Name + " came on line.");
        }
        #endregion

        #region focusevents
 
        /// <summary>
        /// Speech input commands for Friends conversation
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal override bool Hear(string message)
        {
            switch (message)
            {
                case "who is online":
                    ListFriends();
                    return true;
            }

            return false;
        }
        #endregion

        /// <summary>
        /// Was previous selection multiple
        /// </summary>
        bool prevMultiple = false;
        /// <summary>
        /// Announce which friend has been selected in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void friends_ItemSelectionChanged(object sender, EventArgs e)
        {
            FriendInfo f = (FriendInfo)friends.SelectedItem;
            bool multiple = friends.SelectedItems.Count > 1;
            string desc = f.Name;
            bool IsSelected = true;

            if (multiple || prevMultiple)
            {
                desc += IsSelected ? " selected" : " deselected";
                Talker.SayMore(desc);
            }
            else if (IsSelected)
            {
                if (!f.IsOnline)
                    desc += " is off line.";
                Talker.SayMore(desc);
            }

            prevMultiple = multiple;
        }

        /// <summary>
        /// Speak names of all online friends.
        /// </summary>
        private void ListFriends()
        {
            // A filtered query of the Freidns list to select the
            // ones online.
            List<FriendInfo> onlineFriends =
                control.instance.Client.Friends.FriendList.FindAll(delegate(FriendInfo f) { return f.IsOnline; });

            string list = "";
            foreach (FriendInfo f in onlineFriends)
            {
                list += f.Name + ", ";
            }
            list += "are online.";
            Talker.SayMore(list);
        }

    }
}
