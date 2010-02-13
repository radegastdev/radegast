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
// $Id$
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Radegast.Netcom;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public class ChatTextManager : IDisposable
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private GridClient client { get { return instance.Client; } }
        private ITextPrinter textPrinter;

        private List<ChatBufferItem> textBuffer;

        private bool showTimestamps;
        private bool MUEmotes;

        public ChatTextManager(RadegastInstance instance, ITextPrinter textPrinter)
        {
            this.textPrinter = textPrinter;
            this.textBuffer = new List<ChatBufferItem>();

            this.instance = instance;
            InitializeConfig();

            // Callbacks
            netcom.ChatReceived += new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.ChatSent += new EventHandler<ChatSentEventArgs>(netcom_ChatSent);
            netcom.AlertMessageReceived += new EventHandler<AlertMessageEventArgs>(netcom_AlertMessageReceived);
        }

        public void Dispose()
        {
            netcom.ChatReceived -= new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.ChatSent -= new EventHandler<ChatSentEventArgs>(netcom_ChatSent);
            netcom.AlertMessageReceived -= new EventHandler<AlertMessageEventArgs>(netcom_AlertMessageReceived);
        }

        private void InitializeConfig()
        {
            Settings s = instance.GlobalSettings;

            if (s["chat_timestamps"].Type == OSDType.Unknown)
                s["chat_timestamps"] = OSD.FromBoolean(true);

            showTimestamps = s["chat_timestamps"].AsBoolean();

            if (s["mu_emotes"].Type == OSDType.Unknown)
                s["mu_emotes"] = OSD.FromBoolean(false);

            MUEmotes = s["mu_emotes"].AsBoolean();

            s.OnSettingChanged += new Settings.SettingChangedCallback(s_OnSettingChanged);
        }

        void s_OnSettingChanged(object sender, SettingsEventArgs e)
        {
            if (e.Key == "chat_timestamps" && e.Value != null)
            {
                showTimestamps = e.Value.AsBoolean();
                ReprintAllText();
            }
        }

        private void netcom_ChatSent(object sender, ChatSentEventArgs e)
        {
            if (e.Channel == 0) return;

            ProcessOutgoingChat(e);
        }

        private void netcom_AlertMessageReceived(object sender, AlertMessageEventArgs e)
        {
            if (e.Message.ToLower().Contains("autopilot canceled")) return; //workaround the stupid autopilot alerts

            ChatBufferItem item = new ChatBufferItem(
                DateTime.Now, "Alert message: " + e.Message, ChatBufferTextStyle.Alert);

            ProcessBufferItem(item, true);
        }

        private void netcom_ChatReceived(object sender, ChatEventArgs e)
        {
            ProcessIncomingChat(e);
        }

        public void PrintStartupMessage()
        {
            ChatBufferItem title = new ChatBufferItem(
                DateTime.Now, Properties.Resources.RadegastTitle + "." + RadegastBuild.CurrentRev, ChatBufferTextStyle.StartupTitle);

            ChatBufferItem ready = new ChatBufferItem(
                DateTime.Now, "Ready.", ChatBufferTextStyle.StatusBlue);

            ProcessBufferItem(title, true);
            ProcessBufferItem(ready, true);
        }

        private Object SyncChat = new Object();

        public void ProcessBufferItem(ChatBufferItem item, bool addToBuffer)
        {
            lock (SyncChat)
            {
                instance.LogClientMessage("chat.txt", item.Text);
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

                    case ChatBufferTextStyle.OwnerSay:
                        textPrinter.ForeColor = Color.FromArgb(255, 180, 150, 0);
                        break;
                }

                textPrinter.PrintTextLine(item.Text);
            }
        }

        //Used only for non-public chat
        private void ProcessOutgoingChat(ChatSentEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("(channel {0}) {1}", e.Channel, client.Self.Name);

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

            if (instance.RLV.Enabled && e.Message.StartsWith("@"))
            {
                instance.RLV.TryProcessCMD(e);
#if !DEBUG
                return;
#endif
            }

            StringBuilder sb = new StringBuilder();
            // if (e.SourceType == ChatSourceType.Object) {
            //    sb.Append(e.Position + " ");
            // }
            sb.Append(e.FromName);

            switch (e.Type)
            {

                case ChatType.Whisper:
                    sb.Append(" whispers");
                    break;

                case ChatType.Shout:
                    sb.Append(" shouts");
                    break;
            }

            if (e.Message.StartsWith("/me "))
            {
                sb.Append(" ");
                sb.Append(e.Message.Substring(3));
            }
            else if (MUEmotes && e.Message.StartsWith(":"))
            {
                sb.Append(" ");
                sb.Append(e.Message.Substring(1));
            }
            else
            {
                sb.Append(": ");
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
                    if (e.Type == ChatType.OwnerSay)
                    {
                        item.Style = ChatBufferTextStyle.OwnerSay;
                    }
                    else
                    {
                        item.Style = ChatBufferTextStyle.ObjectChat;
                    }
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
