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
            Group = group;

            GroupDetails = new GroupDetails(instance, group) {Dock = DockStyle.Fill};
            ClientSize = new Size(GroupDetails.Width, GroupDetails.Height);
            MinimumSize = Size;
            Controls.Add(GroupDetails);
            Text = group.Name + " - Group information";
            instance.Netcom.ClientDisconnected += new System.EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);

            GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void frmGroupInfo_Disposed(object sender, System.EventArgs e)
        {
        }

        void Netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            ((Netcom.RadegastNetcom)sender).ClientDisconnected -= new System.EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);

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
