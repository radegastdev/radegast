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
using System.ComponentModel;
using OpenMetaverse;

namespace Radegast
{
    public class AgentNameTextBox : System.Windows.Forms.TextBox
    {
        private UUID agentID;
        private GridClient client { get { return RadegastInstance.GlobalInstance.Client; } }
        private RadegastInstance instance { get { return RadegastInstance.GlobalInstance; } }

        [Browsable(false)]
        public UUID AgentID
        {
            get { return agentID; }

            set
            {
                if (agentID == value) return;

                agentID = value;

                if (agentID == UUID.Zero)
                {
                    SetName(string.Empty);
                }
                else
                {
                    SetupHandlers();
                    string name = instance.Names.Get(agentID);
                    SetName(name);
                }
            }
        }

        public AgentNameTextBox()
            : base()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Disposed += new EventHandler(CleanupHandlers);
        }

        void SetupHandlers()
        {
            if (instance == null || instance.Names == null) return;
            instance.Names.NameUpdated += new EventHandler<UUIDNameReplyEventArgs>(Names_NameUpdated);
        }

        void CleanupHandlers(object sender, EventArgs e)
        {
            if (instance != null && instance.Names != null)
            {
                instance.Names.NameUpdated -= new EventHandler<UUIDNameReplyEventArgs>(Names_NameUpdated);
            }
        }

        void Names_NameUpdated(object sender, UUIDNameReplyEventArgs e)
        {
            if (e.Names.ContainsKey(agentID))
            {
                SetName(e.Names[agentID]);
            }
        }

        void SetName(string name)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { SetName(name); }));
                return;
            }

            base.Text = name;
        }
    }
}
