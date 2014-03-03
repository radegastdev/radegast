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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using Radegast;
using OpenMetaverse;
using Meebey.SmartIrc4net;

namespace Radegast.Plugin.IRC
{
    public partial class RelayConsole : RadegastTabControl
    {
        public IrcClient irc;

        TabsConsole TC { get { return instance.TabConsole; } }
        RichTextBoxPrinter textPrinter;
        private List<string> chatHistory = new List<string>();
        private int chatPointer;
        bool connecting = false;
        UUID relayGroup;

        public RelayConsole(RadegastInstance instance)
            : base(instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(RelayConsole_Disposed);
            textPrinter = new RichTextBoxPrinter(rtbChatText);

            irc = new IrcClient();
            irc.SendDelay = 200;
            irc.AutoReconnect = true;
            irc.CtcpVersion = Properties.Resources.RadegastTitle;
            irc.Encoding = Encoding.UTF8;

            TC.OnTabAdded += new TabsConsole.TabCallback(TC_OnTabAdded);
            TC.OnTabRemoved += new TabsConsole.TabCallback(TC_OnTabRemoved);
            irc.OnError += new ErrorEventHandler(irc_OnError);
            irc.OnRawMessage += new IrcEventHandler(irc_OnRawMessage);
            irc.OnChannelMessage += new IrcEventHandler(irc_OnChannelMessage);
            irc.OnConnected += new EventHandler(irc_OnConnected);
            irc.OnDisconnected += new EventHandler(irc_OnDisconnected);

            client.Self.IM += new EventHandler<InstantMessageEventArgs>(Self_IM);

            RefreshGroups();
        }

        void RelayConsole_Disposed(object sender, EventArgs e)
        {
            client.Self.IM -= new EventHandler<InstantMessageEventArgs>(Self_IM);

            TC.OnTabAdded -= new TabsConsole.TabCallback(TC_OnTabAdded);
            TC.OnTabRemoved -= new TabsConsole.TabCallback(TC_OnTabRemoved);

            irc.OnError -= new ErrorEventHandler(irc_OnError);
            irc.OnRawMessage -= new IrcEventHandler(irc_OnRawMessage);
            irc.OnChannelMessage -= new IrcEventHandler(irc_OnChannelMessage);
            irc.OnConnected -= new EventHandler(irc_OnConnected);
            irc.OnDisconnected -= new EventHandler(irc_OnDisconnected);

            if (irc.IsConnected)
            {
                irc.AutoReconnect = false;
                irc.Disconnect();
            }
        }

        void TC_OnTabRemoved(object sender, TabEventArgs e)
        {
            RefreshGroups();
        }

        void TC_OnTabAdded(object sender, TabEventArgs e)
        {
            if (e.Tab.Control is GroupIMTabWindow)
                RefreshGroups();
        }

        void RefreshGroups()
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => RefreshGroups()));
                return;
            }

            ddGroup.DropDownItems.Clear();

            bool foundActive = false;

            foreach (var tab in TC.Tabs)
            {
                if (!(tab.Value.Control is GroupIMTabWindow)) continue;

                UUID session = new UUID(tab.Key);
                if (session == relayGroup)
                    foundActive = true;

                ToolStripButton item = new ToolStripButton(instance.Groups[session].Name, null, groupSelect, session.ToString());
                ddGroup.DropDownItems.Add(item);
            }

            if (!foundActive)
            {
                relayGroup = UUID.Zero;
                ddGroup.Text = "Groups (none)";
            }

        }

        void groupSelect(object sender, EventArgs e)
        {
            if (sender is ToolStripButton)
            {
                UUID session = new UUID(((ToolStripButton)sender).Name);
                relayGroup = session;
                ddGroup.Text = string.Format("Groups ({0})", instance.Groups[session].Name);
            }
        }

        private void PrintMsg(string fromName, string message)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => PrintMsg(fromName, message)));
                return;
            }

            DateTime timestamp = DateTime.Now;

            if (message == null)
                message = string.Empty;

            if (true)
            {
                textPrinter.ForeColor = Color.Gray;
                textPrinter.PrintText(timestamp.ToString("[HH:mm] "));
            }

            textPrinter.ForeColor = Color.Black;

            StringBuilder sb = new StringBuilder();

            if (message.StartsWith("/me "))
            {
                sb.Append(fromName);
                sb.Append(message.Substring(3));
            }
            else
            {
                sb.Append(fromName);
                sb.Append(": ");
                sb.Append(message);
            }

            //instance.LogClientMessage(sessionName + ".txt", sb.ToString());
            textPrinter.PrintTextLine(sb.ToString());
            sb = null;
        }

        private void btnConnectPannel_Click(object sender, EventArgs e)
        {
            pnlConnectionSettings.Visible = true;
        }

        private void IrcThread(object param)
        {
            object[] args = (object[])param;
            string server = (string)args[0];
            int port = (int)args[1];
            string nick = (string)args[2];
            string chan = (string)args[3];

            connecting = true;
            PrintMsg("System", "Connecting...");

            try
            {
                irc.Connect(server, port);
                PrintMsg("System", "Logging in...");
                irc.Login(nick, "Radegast SL Relay", 0, nick);
                irc.RfcJoin(chan);
                connecting = false;
            }
            catch (Exception ex)
            {
                PrintMsg("System", "An error has occured: " + ex.Message);
            }

            try
            {
                irc.Listen();
                if (irc.IsConnected)
                {
                    irc.AutoReconnect = false;
                    irc.Disconnect();
                }
            }
            catch { }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (connecting)
            {
                PrintMsg("System", "Already connecting");
                return;
            }

            if (irc.IsConnected)
            {
                PrintMsg("System", "Already connected");
                return;
            }


            Thread IRCConnection = new Thread(new ParameterizedThreadStart(IrcThread));
            IRCConnection.Name = "IRC Thread";
            IRCConnection.IsBackground = true;
            int port = 6667;
            int.TryParse(txtPort.Text, out port);
            IRCConnection.Start(new object[] { txtServer.Text, port, txtNick.Text, txtChan.Text });
        }

        void irc_OnRawMessage(object sender, IrcEventArgs e)
        {
            if (e.Data.Type == ReceiveType.Unknown || e.Data.Type == ReceiveType.ChannelMessage) return;
            PrintMsg(e.Data.Nick, e.Data.Type + ": " + e.Data.Message);
        }

        void irc_OnError(object sender, ErrorEventArgs e)
        {
            PrintMsg("Error", e.ErrorMessage);
        }

        void irc_OnDisconnected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    Invoke(new MethodInvoker(() => irc_OnDisconnected(sender, e)));
                return;
            }

            lblConnected.Text = "not connected";
            btnSend.Enabled = false;
        }

        void irc_OnConnected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    Invoke(new MethodInvoker(() => irc_OnConnected(sender, e)));
                return;
            }
            lblConnected.Text = "connected";
            pnlConnectionSettings.Visible = false;
            btnSend.Enabled = true;
        }

        void irc_OnChannelMessage(object sender, IrcEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(sync =>
                {
                    PrintMsg(e.Data.Nick, e.Data.Message);
                    if (relayGroup != UUID.Zero)
                    {
                        client.Self.InstantMessageGroup(relayGroup, string.Format("(irc:{2}) {0}: {1}", e.Data.Nick, e.Data.Message, e.Data.Channel));
                    }
                }
            );
        }

        void Self_IM(object sender, InstantMessageEventArgs e)
        {
            if (e.IM.IMSessionID == relayGroup && irc.IsConnected && e.IM.FromAgentID != client.Self.AgentID)
            {
                string[] lines = Regex.Split(e.IM.Message, "\n+");

                for (int l = 0; l < lines.Length; l++)
                {

                    string[] words = lines[l].Split(' ');
                    string outstr = string.Empty;

                    for (int i = 0; i < words.Length; i++)
                    {
                        outstr += words[i] + " ";
                        if (outstr.Length > 380)
                        {
                            PrintMsg(irc.Nickname, string.Format("{0}: {1}", e.IM.FromAgentName, outstr.Remove(outstr.Length - 1)));
                            irc.SendMessage(SendType.Message, txtChan.Text, string.Format("{0}: {1}", e.IM.FromAgentName, outstr.Remove(outstr.Length - 1)));
                            outstr = string.Empty;
                        }
                    }
                    PrintMsg(irc.Nickname, string.Format("{0}: {1}", e.IM.FromAgentName, outstr.Remove(outstr.Length - 1)));
                    irc.SendMessage(SendType.Message, txtChan.Text, string.Format("{0}: {1}", e.IM.FromAgentName, outstr.Remove(outstr.Length - 1)));
                }
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (irc.IsConnected)
            {
                irc.AutoReconnect = false;
                irc.Disconnect();
            }
        }

        void SendMsg()
        {
            string msg = cbxInput.Text;
            if (msg == string.Empty) return;

            chatHistory.Add(cbxInput.Text);
            chatPointer = chatHistory.Count;

            cbxInput.Text = string.Empty;
            if (irc.IsConnected)
            {
                PrintMsg(irc.Nickname, msg);
                irc.SendMessage(SendType.Message, txtChan.Text, msg);
            }
        }

        void ChatHistoryPrev()
        {
            if (chatPointer == 0) return;
            chatPointer--;
            if (chatHistory.Count > chatPointer)
            {
                cbxInput.Text = chatHistory[chatPointer];
                cbxInput.SelectionStart = cbxInput.Text.Length;
                cbxInput.SelectionLength = 0;
            }
        }

        void ChatHistoryNext()
        {
            if (chatPointer == chatHistory.Count) return;
            chatPointer++;
            if (chatPointer == chatHistory.Count)
            {
                cbxInput.Text = string.Empty;
                return;
            }
            cbxInput.Text = chatHistory[chatPointer];
            cbxInput.SelectionStart = cbxInput.Text.Length;
            cbxInput.SelectionLength = 0;
        }

        private void cbxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                ChatHistoryPrev();
                return;
            }

            if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                ChatHistoryNext();
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                SendMsg();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMsg();
        }
    }
}
