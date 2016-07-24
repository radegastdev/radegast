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
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class frmGroupInfo : RadegastForm
    {
        private RadegastInstance instance;

        public Group Group { get; set; }
        public GroupDetails GroupDetails { get; set; }

        public frmGroupInfo(RadegastInstance instance, Group group)
            : base(instance)
        {
            InitializeComponent();
            Disposed += new System.EventHandler(frmGroupInfo_Disposed);
            AutoSavePosition = true;

            this.instance = instance;
            this.Group = group;

            GroupDetails = new GroupDetails(instance, group);
            GroupDetails.Dock = DockStyle.Fill;
            ClientSize = new Size(GroupDetails.Width, GroupDetails.Height);
            MinimumSize = Size;
            Controls.Add(GroupDetails);
            Text = group.Name + " - Group information";
            instance.Netcom.ClientDisconnected += new System.EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void frmGroupInfo_Disposed(object sender, System.EventArgs e)
        {
        }

        void Netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            ((Radegast.Netcom.RadegastNetcom)sender).ClientDisconnected -= new System.EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);

            if (InvokeRequired)
            {
                if (IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Netcom_ClientDisconnected(sender, e)));
                }
                return;
            }

            GroupDetails.Dispose();
            Close();
        }
    }
}
