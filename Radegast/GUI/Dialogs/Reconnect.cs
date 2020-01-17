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
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public partial class frmReconnect : RadegastForm
    {
        private RadegastInstance instance;
        private int reconnectTime;
        
        public int ReconnectTime
        {
            get => reconnectTime;
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

            GUI.GuiHelpers.ApplyGuiFixes(this);
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
