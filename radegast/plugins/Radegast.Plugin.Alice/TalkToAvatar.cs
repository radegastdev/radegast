using System;
using System.Windows.Forms;
using AIMLbot;
using OpenMetaverse;

namespace Radegast.Plugin.Alice
{
    public class TalkToAvatar : ContextAction
    {
        private Bot aimlBot;
        public TalkToAvatar(RadegastInstance inst, Bot bot) : base(inst)
        {
            ContextType = typeof (Avatar);
            Label = "Talk to";
            aimlBot = bot;
        }

        public override bool Contributes(object o, System.Type type)
        {
            if (!IsEnabledInRadegast) return false;
            return type == typeof(FriendInfo) || type == typeof(Avatar);
        }

        public override object DeRef(object o)
        {
            return base.DeRef(o);
        }

        public override bool IsEnabled(object target)
        {
            if (!IsEnabledInRadegast) return false;
            Avatar a = target as Avatar;
            return (a != null && !string.IsNullOrEmpty(a.Name)) || base.IsEnabled(target);
        }

        protected bool IsEnabledInRadegast
        {
            get { return Instance.GlobalSettings["plugin.alice.enabled"].AsBoolean();  }
        }

        public override void OnInvoke(object sender, System.EventArgs e, object target)
        {
            ListViewItem ali = target as ListViewItem;
            string username = null;
            UUID uuid = UUID.Zero;
            if (ali != null)
            {
                uuid = (UUID)ali.Tag;
                username = instance.getAvatarName(uuid);
            }
            else
            {
                FriendInfo fi = target as FriendInfo;
                uuid =fi.UUID;
                username = instance.getAvatarName(uuid);

            }
            if (username==null)
            {
                instance.TabConsole.DisplayNotificationInChat("I dont know how to DeRef " + target + " bieng a  " +
                                                              target.GetType());                               
                return;
            }
            string outp = username + ", " + aimlBot.Chat("INTERJECTION", username).Output;
            if (outp.Length > 1000)
            {
                outp = outp.Substring(0, 1000);
            }

            Client.Self.Chat(outp, 0, ChatType.Normal);
        }
    }
}