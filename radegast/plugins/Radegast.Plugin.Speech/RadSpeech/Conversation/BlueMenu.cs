using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radegast;
using System.Windows.Forms;

namespace RadegastSpeech.Conversation
{
    /// <summary>
    /// Represents a conversation about notifications and dialogs
    /// </summary>
    class BlueMenu : Mode
    {
        private NotificationEventArgs note;

        internal BlueMenu(PluginControl pc, NotificationEventArgs arg) : base(pc)
        {
            note = arg;

            // Watch for being closed by keyboard before we get to talk about it.
            note.OnNotificationClicked +=
                new Notification.NotificationClickedCallback(note_OnNotificationClicked);
            note.OnNotificationClosed +=
                new Notification.NotificationCallback(note_OnNotificationClosed);
        }

        internal override void Start()
        {
            base.Start();

            switch (note.Type)
            {
                case NotificationType.GroupNotice:
                    Announcement = "Group notice.";
                    break;
                case NotificationType.FriendshipOffer:
                    Announcement = "Friendship offer.";
                    break;
                case NotificationType.InventoryOffer:
                    Announcement = "Inventory offer.";
                    break;
                case NotificationType.GroupInvitation:
                    Announcement = "Group invitation.";
                    break;
                case NotificationType.ScriptDialog:
                    Announcement = "A script asks you,";
                    break;
                case NotificationType.PermissionsRequest:
                    Announcement = "A script wants permission.";
                    break;
                case NotificationType.Teleport:
                    Announcement = "Teleport offer.";
                    break;
                default:
                    Announcement = "Attention.";
                    break;
            }
            Talker.SayMore(Announcement, Talk.BeepType.Dialog);
            Announcement = note.Text;
            Talker.SayMore(Announcement);

            ReadOptions();
        }

        void note_OnNotificationClosed(object sender, NotificationEventArgs e)
        {
            note.OnNotificationClosed -=
                new Notification.NotificationCallback(note_OnNotificationClosed);
            note.OnNotificationClicked -=
                new Notification.NotificationClickedCallback(note_OnNotificationClicked);
            FinishInterruption();
        }

        void note_OnNotificationClicked(object sender, EventArgs e, NotificationEventArgs notice)
        {
            Button b = sender as Button;
            Talker.SayMore( b.Text, RadegastSpeech.Talk.BeepType.Good);
        }

        void ReadOptions()
        {
            string options = "";
            if (note.Buttons.Count == 1)
            {
                Talker.SayMore("Say OK to acknowledge.", Talk.BeepType.Dialog);
            }
            else
            {
                options = "Choose one of these: ";

                foreach (Button node in note.Buttons)
                {
                    options += node.Text + ", ";
                }
                Talker.SayMore(options, Talk.BeepType.Dialog);
            }
        }

        internal override bool Hear(string message)
        {
            message = message.ToLower();

            // OK is an acceptible response if only one choise.
            if (note.Buttons.Count==1 && message=="ok")
            {
                note.Buttons[0].PerformClick();
                Talker.SayMore("Thank you.");
//                FinishInterruption();
                return true;
            }

            // Otherwise match against available choices.
            foreach (Button node in note.Buttons)
            {
                // Watch out for ampersands and capital letters
                string button = node.Text.ToLower().Replace("&", "");
                if (message == node.Text)
                {
                    Talker.SayMore("Doing " + message);
                    node.PerformClick();
//                    FinishInterruption();
                    return true;
                }
            }

            if (message == "repeat")
            {
                Talker.SayMore(Announcement, Talk.BeepType.Dialog);
                ReadOptions();

                return true;
            }

            ReadOptions();
            return true;
        }

    }
}
