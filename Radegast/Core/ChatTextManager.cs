/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Radegast.Netcom;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using System.Reflection;
using System.Text.Json;

namespace Radegast
{
    public class ChatTextManager : IDisposable
    {

        public event EventHandler<ChatLineAddedArgs> ChatLineAdded;

        private RadegastInstance instance;
        private RadegastNetcom netcom => instance.Netcom;
        private GridClient client => instance.Client;

        private List<ChatBufferItem> textBuffer;

        private bool showTimestamps;

        public static Dictionary<string, Settings.FontSetting> fontSettings = new Dictionary<string, Settings.FontSetting>();

        public ChatTextManager(RadegastInstance instance, ITextPrinter textPrinter)
        {
            TextPrinter = textPrinter;
            textBuffer = new List<ChatBufferItem>();

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
            {
                s["chat_timestamps"] = OSD.FromBoolean(true);
            }
            if (s["chat_fonts"].Type == OSDType.Unknown)
            {
                try
                {
                    s["chat_fonts"] = JsonSerializer.Serialize(Settings.DefaultFontSettings);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to save default font settings: " + ex.Message);
                }
            }

            try
            {
                fontSettings = JsonSerializer.Deserialize<Dictionary<string, Settings.FontSetting>>(s["chat_fonts"]);
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
                    fontSettings = JsonSerializer.Deserialize<Dictionary<string, Settings.FontSetting>>(e.Value);
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
                DateTime.Now, "",
                UUID.Zero,
                Properties.Resources.RadegastTitle + " " + Assembly.GetExecutingAssembly().GetName().Version, 
                ChatBufferTextStyle.StartupTitle);

            ChatBufferItem ready = new ChatBufferItem(
                DateTime.Now, "", UUID.Zero, "Ready.", ChatBufferTextStyle.StatusBlue);

            ProcessBufferItem(title, true);
            ProcessBufferItem(ready, true);
        }

        private Object SyncChat = new Object();

        public void ProcessBufferItem(ChatBufferItem item, bool addToBuffer)
        {
            ChatLineAdded?.Invoke(this, new ChatLineAddedArgs(item));

            lock (SyncChat)
            {
                instance.LogClientMessage("chat.txt", item.From + item.Text);
                if (addToBuffer) textBuffer.Add(item);

                if (showTimestamps)
                {
                    if(fontSettings.ContainsKey("Timestamp"))
                    {
                        var fontSetting = fontSettings["Timestamp"];
                        TextPrinter.ForeColor = fontSetting.ForeColor;
                        TextPrinter.BackColor = fontSetting.BackColor;
                        TextPrinter.Font = fontSetting.Font;
                        TextPrinter.PrintText(item.Timestamp.ToString("[HH:mm] "));
                    }
                    else
                    {
                        TextPrinter.ForeColor = SystemColors.GrayText;
                        TextPrinter.BackColor = Color.Transparent;
                        TextPrinter.Font = Settings.FontSetting.DefaultFont;
                        TextPrinter.PrintText(item.Timestamp.ToString("[HH:mm] "));
                    }
                }

                if(fontSettings.ContainsKey("Name"))
                {
                    var fontSetting = fontSettings["Name"];
                    TextPrinter.ForeColor = fontSetting.ForeColor;
                    TextPrinter.BackColor = fontSetting.BackColor;
                    TextPrinter.Font = fontSetting.Font;
                }
                else
                {
                    TextPrinter.ForeColor = SystemColors.WindowText;
                    TextPrinter.BackColor = Color.Transparent;
                    TextPrinter.Font = Settings.FontSetting.DefaultFont;
                }

                if (item.Style == ChatBufferTextStyle.Normal && item.ID != UUID.Zero && instance.GlobalSettings["av_name_link"])
                {
                    TextPrinter.InsertLink(item.From, $"secondlife:///app/agent/{item.ID}/about");
                }
                else
                {
                    TextPrinter.PrintText(item.From);
                }

                if(fontSettings.ContainsKey(item.Style.ToString()))
                {
                    var fontSetting = fontSettings[item.Style.ToString()];
                    TextPrinter.ForeColor = fontSetting.ForeColor;
                    TextPrinter.BackColor = fontSetting.BackColor;
                    TextPrinter.Font = fontSetting.Font;
                }
                else
                {
                    TextPrinter.ForeColor = SystemColors.WindowText;
                    TextPrinter.BackColor = Color.Transparent;
                    TextPrinter.Font = Settings.FontSetting.DefaultFont;
                }
                TextPrinter.PrintTextLine(item.Text);
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
                DateTime.Now, $"(channel {e.Channel}) {client.Self.Name}", client.Self.AgentID, sb.ToString(), ChatBufferTextStyle.StatusDarkBlue);

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

            ChatBufferItem item = new ChatBufferItem {ID = e.SourceID, RawMessage = e};
            StringBuilder sb = new StringBuilder();

            item.From = e.SourceType == ChatSourceType.Agent 
                ? instance.Names.Get(e.SourceID, e.FromName) 
                : e.FromName;

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
                    if(e.FromName.EndsWith("Linden"))
                    {
                        item.Style = ChatBufferTextStyle.LindenChat;
                    }
                    else if(isEmote)
                    {
                        item.Style = ChatBufferTextStyle.Emote;
                    }
                    else if(e.SourceID == client.Self.AgentID)
                    {
                        item.Style = ChatBufferTextStyle.Self;
                    }
                    else
                    {
                        item.Style = ChatBufferTextStyle.Normal;
                    }
                    break;
                case ChatSourceType.Object:
                    if (e.Type == ChatType.OwnerSay)
                    {
                        if(isEmote)
                        {
                            item.Style = ChatBufferTextStyle.Emote;
                        }
                        else
                        {
                            item.Style = ChatBufferTextStyle.OwnerSay;
                        }
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
            TextPrinter.ClearText();

            foreach (ChatBufferItem item in textBuffer)
            {
                ProcessBufferItem(item, false);
            }
        }

        public void ClearInternalBuffer()
        {
            textBuffer.Clear();
        }

        public ITextPrinter TextPrinter { get; set; }
    }

    public class ChatLineAddedArgs : EventArgs
    {
        public ChatBufferItem Item { get; }

        public ChatLineAddedArgs(ChatBufferItem item)
        {
            Item = item;
        }
    }
}
