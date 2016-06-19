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
using System.Windows.Forms;
using OpenMetaverse;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;


namespace Radegast
{
    public partial class DisplayNameChange : RadegastForm
    {
        public DisplayNameChange()
        {
            InitializeComponent();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        public DisplayNameChange(RadegastInstance inst)
            : base(inst)
        {
            InitializeComponent();
            Disposed += new EventHandler(DisplayNameChange_Disposed);
            AutoSavePosition = true;

            Client.Self.SetDisplayNameReply += new EventHandler<SetDisplayNameReplyEventArgs>(Self_SetDisplayNameReply);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void DisplayNameChange_Disposed(object sender, EventArgs e)
        {
            Client.Self.SetDisplayNameReply += new EventHandler<SetDisplayNameReplyEventArgs>(Self_SetDisplayNameReply);
        }

        void Self_SetDisplayNameReply(object sender, SetDisplayNameReplyEventArgs e)
        {
            if (e.Status == 200)
            {
                UpdateStatus("Name successfully changed to: " + e.DisplayName.DisplayName);
            }
            else
            {
                UpdateStatus("Update failed: " + e.Reason);
            }
        }

        private void UpdateStatus(string msg)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated || !Instance.MonoRuntime)
                {
                    BeginInvoke(new MethodInvoker(() => UpdateStatus(msg)));
                }
                return;
            }

            lblStatus.Text = msg;
            Instance.TabConsole.DisplayNotificationInChat(msg, ChatBufferTextStyle.Invisible);

        }

        private void StartDisplayNameChage(string name)
        {
            WorkPool.QueueUserWorkItem(sync =>
                {
                    Client.Avatars.GetDisplayNames(new List<OpenMetaverse.UUID>() { Client.Self.AgentID },
                        (bool success, AgentDisplayName[] names, UUID[] badIDs) =>
                        {
                            if (!success || names.Length < 1)
                            {
                                UpdateStatus("Failed to get curret name");
                                return;
                            }
                            else
                            {
                                Client.Self.SetDisplayName(names[0].DisplayName, name);
                            }
                        }
                    );
                }
            );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtDN1.Text != txtDN2.Text)
            {
                UpdateStatus("Name mismatch");
                return;
            }

            if (txtDN1.Text.Length == 0)
            {
                UpdateStatus("Cannot chose an empty display name");
                return;
            }

            UpdateStatus("Requested display name update...");
            StartDisplayNameChage(txtDN1.Text);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateStatus("Requested display name reset...");
            StartDisplayNameChage(string.Empty);
        }
    }
}
