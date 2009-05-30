using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using RadegastNc;

namespace Radegast
{
    public partial class frmDebugLog : Form
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;

        //Workaround for window handle exception on login
        private List<DebugLogMessage> initQueue = new List<DebugLogMessage>();

        public frmDebugLog(RadegastInstance instance)
        {
            InitializeComponent();

            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;
            AddClientEvents();

            this.Disposed += new EventHandler(frmDebugLog_Disposed);
        }

        private void frmDebugLog_Disposed(object sender, EventArgs e)
        {
        }

        private void AddClientEvents()
        {
        }

        //called on GUI thread
        private void ReceivedLogMessage(string message, Helpers.LogLevel level)
        {
            RichTextBox rtb = null;

            switch (level)
            {
                case Helpers.LogLevel.Info:
                    rtb = rtbInfo;
                    break;

                case Helpers.LogLevel.Warning:
                    rtb = rtbWarning;
                    break;

                case Helpers.LogLevel.Error:
                    rtb = rtbError;
                    break;

                case Helpers.LogLevel.Debug:
                    rtb = rtbDebug;
                    break;
            }

            rtb.AppendText("[" + DateTime.Now.ToString() + "] " + message + "\n");
        }

        private void ProcessLogMessage(DebugLogMessage logMessage)
        {
            ReceivedLogMessage(logMessage.Message, logMessage.Level);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void frmDebugLog_Shown(object sender, EventArgs e)
        {
            if (initQueue.Count > 0)
                foreach (DebugLogMessage msg in initQueue) ProcessLogMessage(msg);
        }
    }
}