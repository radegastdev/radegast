using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace Radegast
{
    public class FriendsListItem
    {
        private FriendInfo friend;

        public FriendsListItem(FriendInfo friend)
        {
            this.friend = friend;
        }

        public override string ToString()
        {
            return (friend.IsOnline ? "0 " : "1 ") + friend.Name;
        }

        public FriendInfo Friend
        {
            get { return friend; }
            set { friend = value; }
        }
    }
}
