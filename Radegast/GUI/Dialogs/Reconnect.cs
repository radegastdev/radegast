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
// $Id: TabsConsole.cs 361 2009-10-24 15:04:57Z latifer $
//
using System;
using System.Windows.Forms;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public partial class frmReconnect : RadegastForm
    {
        private RadegastInstance instance;
        private int reconnectTime;
        
        public int ReconnectTime
        {
            get { return reconnectTime; }
            set
            {
                reconnectTime = value;
                tmrReconnect.Enabled = true;
            }
        }

        public frmReconnect(RadegastInstance instance, int time)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmReconnect_Disposed);
            this.instance = instance;
            ReconnectTime = time;
            lblAutoReconnect.Text = string.Format("Auto reconnect in {0} seconds.", reconnectTime);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void frmReconnect_Disposed(object sender, EventArgs e)
        {
        }

        private void tmrReconnect_Tick(object sender, EventArgs e)
        {
            lblAutoReconnect.Text = string.Format("Auto reconnect in {0} seconds.", --reconnectTime);
            if (reconnectTime <= 0)
            {
                instance.Reconnect();
                Close();
            }
        }

        private void btnReconnectNow_Click(object sender, EventArgs e)
        {
            instance.Reconnect();
            Close();
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            instance.GlobalSettings["auto_reconnect"] = OSD.FromBoolean(false);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
