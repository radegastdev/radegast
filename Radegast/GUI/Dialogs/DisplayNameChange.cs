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
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif


namespace Radegast
{
    public partial class DisplayNameChange : RadegastForm
    {
        public DisplayNameChange()
        {
            InitializeComponent();

            GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        public DisplayNameChange(RadegastInstance inst)
            : base(inst)
        {
            InitializeComponent();
            Disposed += new EventHandler(DisplayNameChange_Disposed);
            AutoSavePosition = true;

            Client.Self.SetDisplayNameReply += new EventHandler<SetDisplayNameReplyEventArgs>(Self_SetDisplayNameReply);

            GUI.GuiHelpers.ApplyGuiFixes(this);
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
            ThreadPool.QueueUserWorkItem(sync =>
                {
                    Client.Avatars.GetDisplayNames(new List<UUID>() { Client.Self.AgentID },
                        (success, names, badIDs) =>
                        {
                            if (!success || names.Length < 1)
                            {
                                UpdateStatus("Failed to get curret name");
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
