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
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast.Plugin.IRC
{
    [Plugin(Name = "IRC Relay", Description = "Relays SL group chat to a IRC network", Version = "0.1")]
    public class IRCPlugin : IRadegastPlugin
    {
        RadegastInstance instance;
        GridClient Client => instance.Client;

        ToolStripMenuItem IRCButton;
        int relayNr = 0;

        public void StartPlugin(RadegastInstance inst)
        {
            instance = inst;

            IRCButton = new ToolStripMenuItem("New IRC Relay...");
            IRCButton.Click += new EventHandler(IRCButton_Click);
            instance.MainForm.PluginsMenu.DropDownItems.Add(IRCButton);
        }

        public void StopPlugin(RadegastInstance instance)
        {
            List<RadegastTab> toRemove = new List<RadegastTab>();
            foreach (RadegastTab tab in this.instance.TabConsole.Tabs.Values)
            {
                if (tab.Control is RelayConsole)
                    toRemove.Add(tab);
            }
            
            foreach (var tab in toRemove)
                tab.Close();

            if (IRCButton != null)
            {
                IRCButton.Dispose();
                IRCButton = null;
            }
        }

        void IRCButton_Click(object sender, EventArgs e)
        {
            relayNr++;
            string tabName = "irc_relay_" + relayNr;

            instance.TabConsole.AddTab(tabName, "IRC Relay " + relayNr, new RelayConsole(instance));
            instance.TabConsole.SelectTab(tabName);
        }
    }
}
