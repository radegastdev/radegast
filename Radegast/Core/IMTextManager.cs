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
using System.Collections;
using System.Drawing;
using System.Text;
using Radegast.Netcom;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public class IMTextManager
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private ITextPrinter textPrinter;
        private UUID sessionID;
        private string sessionName;

        private ArrayList textBuffer;

        private bool showTimestamps;

        public IMTextManager(RadegastInstance instance, ITextPrinter textPrinter, UUID sessionID, string sessionName)
        {
            this.sessionID = sessionID;
            this.sessionName = sessionName;
            this.textPrinter = textPrinter;
            this.textBuffer = new ArrayList();

            this.instance = instance;
            AddNetcomEvents();
            InitializeConfig();
        }

        private void InitializeConfig()
        {
            Settings s = instance.GlobalSettings;

            if (s["im_timestamps"].Type == OSDType.Unknown)
                s["im_timestamps"] = OSD.FromBoolean(true);

            showTimestamps = s["im_timestamps"].AsBoolean();

            s.OnSettingChanged += new Settings.SettingChangedCallback(s_OnSettingChanged);
        }

        void s_OnSettingChanged(object sender, SettingsEventArgs e)
        {
            if (e.Key == "im_timestamps" && e.Value != null)
            {
                showTimestamps = e.Value.AsBoolean();
                ReprintAllText();
            }
        }

        private void AddNetcomEvents()
        {
            netcom.InstantMessageReceived += new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
            netcom.InstantMessageSent += new EventHandler<InstantMessageSentEventArgs>(netcom_InstantMessageSent);
        }

        private void RemoveNetcomEvents()
        {
            netcom.InstantMessageReceived -= netcom_InstantMessageReceived;
            netcom.InstantMessageSent -= netcom_InstantMessageSent;
        }

        private void netcom_InstantMessageSent(object sender, InstantMessageSentEventArgs e)
        {
            if (e.SessionID != sessionID) return;
            
            textBuffer.Add(e);
            ProcessIM(e);
        }

        private void netcom_InstantMessageReceived(object sender, InstantMessageEventArgs e)
        {
            if (e.IM.IMSessionID != sessionID) return;
            if (e.IM.Dialog == InstantMessageDialog.StartTyping ||
                e.IM.Dialog == InstantMessageDialog.StopTyping)
                return;

            textBuffer.Add(e);
            ProcessIM(e);
        }

        public void ProcessIM(object e)
        {
            if (e is InstantMessageEventArgs)
                this.ProcessIncomingIM((InstantMessageEventArgs)e);
            else if (e is InstantMessageSentEventArgs)
                this.ProcessOutgoingIM((InstantMessageSentEventArgs)e);
        }

        private void ProcessOutgoingIM(InstantMessageSentEventArgs e)
        {
            PrintIM(e.Timestamp, netcom.LoginOptions.FullName, e.Message);
        }

        private void ProcessIncomingIM(InstantMessageEventArgs e)
        {
            PrintIM(DateTime.Now, e.IM.FromAgentName, e.IM.Message);
        }

        private void PrintIM(DateTime timestamp, string fromName, string message)
        {
            if (showTimestamps)
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

            instance.LogClientMessage(sessionName + ".txt", sb.ToString());
            textPrinter.PrintTextLine(sb.ToString());
            sb = null;
        }

        public void ReprintAllText()
        {
            textPrinter.ClearText();

            foreach (object obj in textBuffer)
                ProcessIM(obj);
        }

        public void ClearInternalBuffer()
        {
            textBuffer.Clear();
        }

        public void CleanUp()
        {
            RemoveNetcomEvents();

            textBuffer.Clear();
            textBuffer = null;

            textPrinter = null;
        }

        public ITextPrinter TextPrinter
        {
            get { return textPrinter; }
            set { textPrinter = value; }
        }

        public bool ShowTimestamps
        {
            get { return showTimestamps; }
            set { showTimestamps = value; }
        }

        public UUID SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }
    }
}
