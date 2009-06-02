using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OpenMetaverse;
using OpenMetaverse.Packets;

namespace Radegast
{
    public partial class AnimTab : UserControl
    {
        private GridClient client;
        private RadegastInstance instance;
        private Avatar av;
        private List<UUID> seenAnim = new List<UUID>();
        private int n = 0;
        private static bool checkedDir = false;

        public AnimTab(RadegastInstance instance, Avatar av)
        {
            InitializeComponent();
            Disposed += new EventHandler(AnimTab_Disposed);
            this.instance = instance;
            this.av = av;
            this.client = instance.Client;

            // Callbacks
            client.Avatars.OnAvatarAnimation += new AvatarManager.AvatarAnimationCallback(Avatars_OnAvatarAnimationUpdate);
        }

        void AnimTab_Disposed(object sender, EventArgs e)
        {
            client.Avatars.OnAvatarAnimation -= new AvatarManager.AvatarAnimationCallback(Avatars_OnAvatarAnimationUpdate);
        }

        void Avatars_OnAvatarAnimationUpdate(UUID avatarID, InternalDictionary<UUID, int> anims)
        {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    Avatars_OnAvatarAnimationUpdate(avatarID, anims);
                }));
                return;
            }
            if (avatarID == av.ID)
            {
                anims.ForEach(delegate(UUID AnimID)
                {
                    if (!seenAnim.Contains(AnimID))
                    {
                        Logger.Log("New anim for " + av.Name + ": " + AnimID, Helpers.LogLevel.Debug);
                        seenAnim.Add(AnimID);
                        AnimDetail ad = new AnimDetail(instance, av, AnimID, n);
                        ad.Location = new Point(0, n++ * ad.Height);
                        ad.Dock = DockStyle.Top;
                        Controls.Add(ad);
                    }
                });
            }
        }

        private void AnimTab_Load(object sender, EventArgs e)
        {
            if (!checkedDir) {
                checkedDir = true;
                if (!Directory.Exists(instance.animCacheDir))
                {
                    Directory.CreateDirectory(instance.animCacheDir);
                }
            }
        }

    }
}
