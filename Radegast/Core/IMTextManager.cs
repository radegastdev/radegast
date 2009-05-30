using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using RadegastNc;
using OpenMetaverse;

namespace Radegast
{
    public class IMTextManager
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private ITextPrinter textPrinter;
        private UUID sessionID;

        private ArrayList textBuffer;

        private bool showTimestamps;

        public IMTextManager(RadegastInstance instance, ITextPrinter textPrinter, UUID sessionID)
        {
            this.sessionID = sessionID;

            this.textPrinter = textPrinter;
            this.textBuffer = new ArrayList();

            this.instance = instance;
            netcom = this.instance.Netcom;
            AddNetcomEvents();

            showTimestamps = this.instance.Config.CurrentConfig.IMTimestamps;
            this.instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
        }

        private void Config_ConfigApplied(object sender, ConfigAppliedEventArgs e)
        {
            showTimestamps = e.AppliedConfig.IMTimestamps;
            ReprintAllText();
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
