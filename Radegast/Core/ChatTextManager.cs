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
// $Id$
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Radegast.Netcom;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using System.Web.Script.Serialization;
using System.ComponentModel;

namespace Radegast
{
    public class ChatTextManager : IDisposable
    {

        public event EventHandler<ChatLineAddedArgs> ChatLineAdded;

        private RadegastInstance instance;
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private GridClient client { get { return instance.Client; } }
        private ITextPrinter textPrinter;

        private List<ChatBufferItem> textBuffer;

        private bool showTimestamps;

        public static Dictionary<string, Settings.FontSettings> FontSettings = new Dictionary<string, Settings.FontSettings>()
        {
            {"Normal", new Settings.FontSettings {
                Name = "Normal",
                ForeColor = SystemColors.WindowText,
                BackColor = Color.Transparent,
                Font = Settings.FontSettings.DefaultFont,
            }},
            {"StatusBlue", new Settings.FontSettings {
                Name = "StatusBlue",
                ForeColor = Color.Blue,
                BackColor = Color.Transparent,
                Font = Settings.FontSettings.DefaultFont,
            }},
            {"StatusDarkBlue", new Settings.FontSettings {
                Name = "StatusDarkBlue",
                ForeColor = Color.DarkBlue,
                BackColor = Color.Transparent,
                Font = Settings.FontSettings.DefaultFont,
            }},
            {"LindenChat", new Settings.FontSettings {
                Name = "LindenChat",
                ForeColor = Color.DarkGreen,
                BackColor = Color.Transparent,
                Font = Settings.FontSettings.DefaultFont,
            }},
            {"ObjectChat", new Settings.FontSettings {
                Name = "ObjectChat",
                ForeColor = Color.DarkCyan,
                BackColor = Color.Transparent,
                Font = Settings.FontSettings.DefaultFont,
            }},
            {"StartupTitle", new Settings.FontSettings {
                Name = "StartupTitle",
                ForeColor = SystemColors.WindowText,
                BackColor = Color.Transparent,
                Font = new Font(Settings.FontSettings.DefaultFont, FontStyle.Bold),
            }},
            {"Alert", new Settings.FontSettings {
                Name = "Alert",
                ForeColor = Color.DarkRed,
                BackColor = Color.Transparent,
                Font = Settings.FontSettings.DefaultFont,
            }},
            {"Error", new Settings.FontSettings {
                Name = "Error",
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                Font = Settings.FontSettings.DefaultFont,
            }},
            {"OwnerSay", new Settings.FontSettings {
                Name = "OwnerSay",
                ForeColor = Color.FromArgb(255, 180, 150, 0),
                BackColor = Color.Transparent,
                Font = Settings.FontSettings.DefaultFont,
            }},
        };

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
            var serializer = new JavaScriptSerializer();

            if (s["chat_timestamps"].Type == OSDType.Unknown)
            {
                s["chat_timestamps"] = OSD.FromBoolean(true);
            }
            if (s["chat_fonts"].Type == OSDType.Unknown)
            {
                try
                {
                    s["chat_fonts"] = serializer.Serialize(FontSettings);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to save default font settings: " + ex.Message);
                }
            }

            try
            {
                FontSettings = serializer.Deserialize<Dictionary<string, Settings.FontSettings>>(s["chat_fonts"]);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to read chat font settings: " + ex.Message);
            }

            showTimestamps = s["chat_timestamps"].AsBoolean();

            s.OnSettingChanged += new Settings.SettingChangedCallback(s_OnSettingChanged);
        }

        void s_OnSettingChanged(object sender, SettingsEventArgs e)
        {
            if (e.Key == "chat_timestamps" && e.Value != null)
            {
                showTimestamps = e.Value.AsBoolean();
                ReprintAllText();
            }
            else if(e.Key == "chat_fonts")
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    FontSettings = serializer.Deserialize<Dictionary<string, Settings.FontSettings>>(e.Value);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to read new font settings: " + ex.Message);
                }
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
                DateTime.Now, "Alert message", UUID.Zero, ": " + e.Message, ChatBufferTextStyle.Alert);

            ProcessBufferItem(item, true);
        }

        private void netcom_ChatReceived(object sender, ChatEventArgs e)
        {
            ProcessIncomingChat(e);
        }

        public void PrintStartupMessage()
        {
            ChatBufferItem title = new ChatBufferItem(
                DateTime.Now, "", UUID.Zero, Properties.Resources.RadegastTitle + "." + RadegastBuild.CurrentRev, ChatBufferTextStyle.StartupTitle);

            ChatBufferItem ready = new ChatBufferItem(
                DateTime.Now, "", UUID.Zero, "Ready.", ChatBufferTextStyle.StatusBlue);

            ProcessBufferItem(title, true);
            ProcessBufferItem(ready, true);
        }

        private Object SyncChat = new Object();

        public void ProcessBufferItem(ChatBufferItem item, bool addToBuffer)
        {
            if (ChatLineAdded != null)
            {
                ChatLineAdded(this, new ChatLineAddedArgs(item));
            }

            lock (SyncChat)
            {
                instance.LogClientMessage("chat.txt", item.From + item.Text);
                if (addToBuffer) textBuffer.Add(item);

                if (showTimestamps)
                {
                    textPrinter.ForeColor = SystemColors.GrayText;
                    textPrinter.PrintText(item.Timestamp.ToString("[HH:mm] "));
                }

                if(FontSettings.ContainsKey(item.Style.ToString()))
                {
                    var fontSetting = FontSettings[item.Style.ToString()];
                    textPrinter.ForeColor = fontSetting.ForeColor;
                    textPrinter.BackColor = fontSetting.BackColor;
                    textPrinter.Font = fontSetting.Font;
                }
                else
                {
                    textPrinter.ForeColor = SystemColors.WindowText;
                    textPrinter.BackColor = Color.Transparent;
                    textPrinter.Font = Settings.FontSettings.DefaultFont;
                }

                if (item.Style == ChatBufferTextStyle.Normal && item.ID != UUID.Zero && instance.GlobalSettings["av_name_link"])
                {
                    textPrinter.InsertLink(item.From, string.Format("secondlife:///app/agent/{0}/about", item.ID));
                    textPrinter.PrintTextLine(item.Text);
                }
                else
                {
                    textPrinter.PrintTextLine(item.From + item.Text);
                }
            }
        }

        //Used only for non-public chat
        private void ProcessOutgoingChat(ChatSentEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

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
                DateTime.Now, string.Format("(channel {0}) {1}", e.Channel, client.Self.Name), client.Self.AgentID, sb.ToString(), ChatBufferTextStyle.StatusDarkBlue);

            ProcessBufferItem(item, true);

            sb = null;
        }

        private void ProcessIncomingChat(ChatEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message)) return;

            // Check if the sender agent is muted
            if (e.SourceType == ChatSourceType.Agent &&
                null != client.Self.MuteList.Find(me => me.Type == MuteType.Resident && me.ID == e.SourceID)
                ) return;

            // Check if it's script debug
            if (e.Type == ChatType.Debug && !instance.GlobalSettings["show_script_errors"])
            {
                return;
            }

            // Check if sender object is muted
            if (e.SourceType == ChatSourceType.Object &&
                null != client.Self.MuteList.Find(me =>
                    (me.Type == MuteType.Resident && me.ID == e.OwnerID) // Owner muted
                    || (me.Type == MuteType.Object && me.ID == e.SourceID) // Object muted by ID
                    || (me.Type == MuteType.ByName && me.Name == e.FromName) // Object muted by name
                )) return;

            if (instance.RLV.Enabled && e.Message.StartsWith("@"))
            {
                instance.RLV.TryProcessCMD(e);
#if !DEBUG
                if (!instance.RLV.EnabledDebugCommands) {
                    return;
                }
#endif
            }

            ChatBufferItem item = new ChatBufferItem();
            item.ID = e.SourceID;
            item.RawMessage = e;
            StringBuilder sb = new StringBuilder();

            if (e.SourceType == ChatSourceType.Agent)
            {
                item.From = instance.Names.Get(e.SourceID, e.FromName);
            }
            else
            {
                item.From = e.FromName;
            }

            bool isEmote = e.Message.ToLower().StartsWith("/me ");

            if (!isEmote)
            {
                switch (e.Type)
                {

                    case ChatType.Whisper:
                        sb.Append(" whispers");
                        break;

                    case ChatType.Shout:
                        sb.Append(" shouts");
                        break;
                }
            }

            if (isEmote)
            {
                if (e.SourceType == ChatSourceType.Agent && instance.RLV.RestictionActive("recvemote", e.SourceID.ToString()))
                    sb.Append(" ...");
                else
                    sb.Append(e.Message.Substring(3));
            }
            else
            {
                sb.Append(": ");
                if (e.SourceType == ChatSourceType.Agent && !e.Message.StartsWith("/") && instance.RLV.RestictionActive("recvchat", e.SourceID.ToString()))
                    sb.Append("...");
                else
                    sb.Append(e.Message);
            }

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
                    else if (e.Type == ChatType.Debug)
                    {
                        item.Style = ChatBufferTextStyle.Error;
                    }
                    else
                    {
                        item.Style = ChatBufferTextStyle.ObjectChat;
                    }
                    break;
            }

            ProcessBufferItem(item, true);
            instance.TabConsole.Tabs["chat"].Highlight();

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

    public class ChatLineAddedArgs : EventArgs
    {
        ChatBufferItem mItem;
        public ChatBufferItem Item { get { return mItem; } }

        public ChatLineAddedArgs(ChatBufferItem item)
        {
            mItem = item;
        }
    }
}
