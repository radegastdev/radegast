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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class ntfLoadURL : Notification
    {
        LoadUrlEventArgs ev;
        RadegastInstance instance;

        public ntfLoadURL(RadegastInstance instance, LoadUrlEventArgs e)
        {
            InitializeComponent();
            Disposed += new EventHandler(ntfLoadURL_Disposed);
            
            this.ev = e;
            this.instance = instance;

            instance.Names.NameUpdated += new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);

            SetText();

            // Fire off event
            NotificationEventArgs args = new NotificationEventArgs(instance);
            args.Text = rtbText.Text;
            args.Buttons.Add(btnGoTo);
            args.Buttons.Add(btnCancel);
            FireNotificationCallback(args);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void ntfLoadURL_Disposed(object sender, EventArgs e)
        {
            instance.Names.NameUpdated -= new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
        }

        void Avatars_UUIDNameReply(object sender, UUIDNameReplyEventArgs e)
        {
            if (!e.Names.ContainsKey(ev.OwnerID)) return;

            instance.Names.NameUpdated -= new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);

            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => Avatars_UUIDNameReply(sender, e)));
                return;
            }

            SetText();
        }


        public void SetText()
        {
            rtbText.Text = string.Format("Do you want to load web page?{0}{1}From object: {2}, owner: {3}",
                Environment.NewLine + ev.URL +Environment.NewLine + Environment.NewLine,
                ev.Message + Environment.NewLine + Environment.NewLine,
                ev.ObjectName,
                instance.Names.Get(ev.OwnerID)
            );
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            instance.MainForm.ProcessLink(ev.URL);
            instance.MainForm.RemoveNotification(this);
        }

        private void rtbText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            instance.MainForm.ProcessLink(e.LinkText);
        }

        private void ntfLoadURL_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                rtbText.BackColor = Parent.BackColor;
            }
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            instance.Client.Self.UpdateMuteListEntry(MuteType.Object, ev.ObjectID, ev.ObjectName);
            instance.MainForm.RemoveNotification(this);
        }
    }
}
