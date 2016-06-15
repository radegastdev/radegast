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
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using OpenMetaverse;

namespace Radegast
{
    public partial class AnimTab : UserControl
    {
        private GridClient client;
        private RadegastInstance instance;
        private Avatar av;
        private List<UUID> seenAnim = new List<UUID>();
        private int n = 0;

        public AnimTab(RadegastInstance instance, Avatar av)
        {
            InitializeComponent();
            Disposed += new EventHandler(AnimTab_Disposed);
            this.instance = instance;
            this.av = av;
            this.client = instance.Client;

            // Callbacks
            client.Avatars.AvatarAnimation += new EventHandler<AvatarAnimationEventArgs>(Avatars_AvatarAnimation);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void AnimTab_Disposed(object sender, EventArgs e)
        {
            client.Avatars.AvatarAnimation -= new EventHandler<AvatarAnimationEventArgs>(Avatars_AvatarAnimation);
        }

        void Avatars_AvatarAnimation(object sender, AvatarAnimationEventArgs e)
        {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(() => Avatars_AvatarAnimation(sender, e)));
                return;
            }
            if (e.AvatarID== av.ID)
            {
                foreach (Animation a in e.Animations)
                {
                    if (!seenAnim.Contains(a.AnimationID))
                    {
                        Logger.Log("New anim for " + av.Name + ": " + a.AnimationID, Helpers.LogLevel.Debug);
                        seenAnim.Add(a.AnimationID);
                        AnimDetail ad = new AnimDetail(instance, av, a.AnimationID, n);
                        ad.Location = new Point(0, n++ * ad.Height);
                        ad.Dock = DockStyle.Top;
                        Controls.Add(ad);
                    }
                }
            }
        }

        private void AnimTab_Load(object sender, EventArgs e)
        {
        }
    }
}
