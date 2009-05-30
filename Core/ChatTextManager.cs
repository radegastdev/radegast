using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RadegastNc;
using OpenMetaverse;

namespace Radegast
{
    public class ChatTextManager
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;
        private ITextPrinter textPrinter;

        private List<ChatBufferItem> textBuffer;

        private bool showTimestamps;

        public ChatTextManager(RadegastInstance instance, ITextPrinter textPrinter)
        {
            this.textPrinter = textPrinter;
            this.textBuffer = new List<ChatBufferItem>();

            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;
            AddNetcomEvents();

            showTimestamps = this.instance.Config.CurrentConfig.ChatTimestamps;
            this.instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
        }

        private void Config_ConfigApplied(object sender, ConfigAppliedEventArgs e)
        {
            showTimestamps = e.AppliedConfig.ChatTimestamps;
            ReprintAllText();
        }

        private void AddNetcomEvents()
        {
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            netcom.ChatReceived += new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.ChatSent += new EventHandler<ChatSentEventArgs>(netcom_ChatSent);
            netcom.AlertMessageReceived += new EventHandler<AlertMessageEventArgs>(netcom_AlertMessageReceived);
        }

        private void netcom_ChatSent(object sender, ChatSentEventArgs e)
        {
            if (e.Channel == 0) return;

            ProcessOutgoingChat(e);
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status == LoginStatus.Success)
            {
                ChatBufferItem loggedIn = new ChatBufferItem(
                    DateTime.Now,
                    "Logged into Second Life as " + netcom.LoginOptions.FullName + ".",
                    ChatBufferTextStyle.StatusBlue);

                ChatBufferItem loginReply = new ChatBufferItem(
                    DateTime.Now, "Login reply: " + e.Message, ChatBufferTextStyle.StatusDarkBlue);

                ProcessBufferItem(loggedIn, true);
                ProcessBufferItem(loginReply, true);
            }
            else if (e.Status == LoginStatus.Failed)
            {
                ChatBufferItem loginError = new ChatBufferItem(
                    DateTime.Now, "Login error: " + e.Message, ChatBufferTextStyle.Error);

                ProcessBufferItem(loginError, true);
            }
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            ChatBufferItem item = new ChatBufferItem(
                DateTime.Now, "Logged out of Second Life.\n", ChatBufferTextStyle.StatusBlue);

            ProcessBufferItem(item, true);
        }

        private void netcom_AlertMessageReceived(object sender, AlertMessageEventArgs e)
        {
            if (e.Message.ToLower().Contains("autopilot canceled")) return; //workaround the stupid autopilot alerts

            ChatBufferItem item = new ChatBufferItem(
                DateTime.Now, "Alert message: " + e.Message, ChatBufferTextStyle.Alert);

            ProcessBufferItem(item, true);
        }

        private void netcom_ClientDisconnected(object sender, ClientDisconnectEventArgs e)
        {
            if (e.Type == NetworkManager.DisconnectType.ClientInitiated) return;

            ChatBufferItem item = new ChatBufferItem(
                DateTime.Now, "Client disconnected. Message: " + e.Message, ChatBufferTextStyle.Error);

            ProcessBufferItem(item, true);
        }

        private void netcom_ChatReceived(object sender, ChatEventArgs e)
        {
            ProcessIncomingChat(e);
        }

        public void PrintStartupMessage()
        {
            ChatBufferItem title = new ChatBufferItem(
                DateTime.Now, Properties.Resources.RadegastTitle, ChatBufferTextStyle.StartupTitle);

            ChatBufferItem ready = new ChatBufferItem(
                DateTime.Now, "Ready.\n", ChatBufferTextStyle.StatusBlue);

            ProcessBufferItem(title, true);
            ProcessBufferItem(ready, true);
        }

        public void ProcessBufferItem(ChatBufferItem item, bool addToBuffer)
        {
            if (addToBuffer) textBuffer.Add(item);

            if (showTimestamps)
            {
                textPrinter.ForeColor = Color.Gray;
                textPrinter.PrintText(item.Timestamp.ToString("[HH:mm] "));
            }

            switch (item.Style)
            {
                case ChatBufferTextStyle.Normal:
                    textPrinter.ForeColor = Color.Black;
                    break;

                case ChatBufferTextStyle.StatusBlue:
                    textPrinter.ForeColor = Color.Blue;
                    break;

                case ChatBufferTextStyle.StatusDarkBlue:
                    textPrinter.ForeColor = Color.DarkBlue;
                    break;

                case ChatBufferTextStyle.LindenChat:
                    textPrinter.ForeColor = Color.DarkGreen;
                    break;

                case ChatBufferTextStyle.ObjectChat:
                    textPrinter.ForeColor = Color.DarkCyan;
                    break;

                case ChatBufferTextStyle.StartupTitle:
                    textPrinter.ForeColor = Color.Black;
                    textPrinter.Font = new Font(textPrinter.Font, FontStyle.Bold);
                    break;

                case ChatBufferTextStyle.Alert:
                    textPrinter.ForeColor = Color.DarkRed;
                    break;

                case ChatBufferTextStyle.Error:
                    textPrinter.ForeColor = Color.Red;
                    break;
            }

            textPrinter.PrintTextLine(item.Text);
        }

        //Used only for non-public chat
        private void ProcessOutgoingChat(ChatSentEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(channel ");
            sb.Append(e.Channel);
            sb.Append(") You");

            switch (e.Type)
            {
                case ChatType.Normal:
                    sb.Append(": ");
                    break;

                case ChatType.Whisper:
                    sb.Append(" whisper: ");
                    break;

                case ChatType.Shout:
                    sb.Append(" shout: ");
                    break;
            }

            sb.Append(e.Message);

            ChatBufferItem item = new ChatBufferItem(
                DateTime.Now, sb.ToString(), ChatBufferTextStyle.StatusDarkBlue);

            ProcessBufferItem(item, true);

            sb = null;
        }

        private void ProcessIncomingChat(ChatEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message)) return;

            StringBuilder sb = new StringBuilder();
            if (e.SourceType == ChatSourceType.Object) {
                sb.Append(e.Position + " ");
            }
            if (e.Message.StartsWith("/me "))
            {
                sb.Append(e.FromName);
                sb.Append(e.Message.Substring(3));
            }
            else if (e.FromName == netcom.LoginOptions.FullName && e.SourceType == ChatSourceType.Agent)
            {
                sb.Append("You");

                switch (e.Type)
                {
                    case ChatType.Normal:
                        sb.Append(": ");
                        break;

                    case ChatType.Whisper:
                        sb.Append(" whisper: ");
                        break;

                    case ChatType.Shout:
                        sb.Append(" shout: ");
                        break;
                }

                sb.Append(e.Message);
            }
            else
            {
                sb.Append(e.FromName);

                switch (e.Type)
                {
                    case ChatType.Normal:
                        sb.Append(": ");
                        break;

                    case ChatType.Whisper:
                        sb.Append(" whispers: ");
                        break;

                    case ChatType.Shout:
                        sb.Append(" shouts: ");
                        break;
                }

                sb.Append(e.Message);
            }

            ChatBufferItem item = new ChatBufferItem();
            item.Timestamp = DateTime.Now;
            item.Text = sb.ToString();

            switch (e.SourceType)
            {
                case ChatSourceType.Agent:
                    item.Style =
                        (e.FromName.EndsWith("Linden") ?
                        ChatBufferTextStyle.LindenChat : ChatBufferTextStyle.Normal);
                    break;

                case ChatSourceType.Object:
                    item.Style = ChatBufferTextStyle.ObjectChat;
                    break;
            }

            ProcessBufferItem(item, true);
            sb = null;
        }

        public void ReprintAllText()
        {
            textPrinter.ClearText();

            foreach (ChatBufferItem item in textBuffer)
            {
                ProcessBufferItem(item, false);
            }
        }

        public void ClearInternalBuffer()
        {
            textBuffer.Clear();
        }

        public ITextPrinter TextPrinter
        {
            get { return textPrinter; }
            set { textPrinter = value; }
        }
    }
}
