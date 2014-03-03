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
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Radegast;
using OpenMetaverse;

namespace Radegast.Plugin.IRC
{
    [Radegast.Plugin(Name = "IRC Relay", Description = "Relays SL group chat to a IRC network", Version = "0.1")]
    public class IRCPlugin : IRadegastPlugin
    {
        RadegastInstance Instance;
        GridClient Client { get { return Instance.Client; } }

        ToolStripMenuItem IRCButton;
        int relayNr = 0;

        public void StartPlugin(RadegastInstance inst)
        {
            Instance = inst;

            IRCButton = new ToolStripMenuItem("New IRC Relay...");
            IRCButton.Click += new EventHandler(IRCButton_Click);
            Instance.MainForm.PluginsMenu.DropDownItems.Add(IRCButton);
        }

        public void StopPlugin(RadegastInstance instance)
        {
            List<RadegastTab> toRemove = new List<RadegastTab>();
            foreach (RadegastTab tab in Instance.TabConsole.Tabs.Values)
            {
                if (tab.Control is RelayConsole)
                    toRemove.Add(tab);
            }
            
            for (int i = 0; i < toRemove.Count; i++)
                toRemove[i].Close();

            if (IRCButton != null)
            {
                IRCButton.Dispose();
                IRCButton = null;
            }
        }

        void IRCButton_Click(object sender, EventArgs e)
        {
            relayNr++;
            string tabName = "irc_relay_" + relayNr.ToString();
            Instance.TabConsole.AddTab(tabName, "IRC Relay " + relayNr.ToString(), new RelayConsole(Instance));
            Instance.TabConsole.SelectTab(tabName);
        }
    }
}
