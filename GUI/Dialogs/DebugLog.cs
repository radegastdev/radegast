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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using Radegast.Netcom;

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