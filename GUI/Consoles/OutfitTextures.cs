using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RadegastNc;
using OpenMetaverse;

namespace Radegast
{
    public partial class OutfitTextures : UserControl
    {
        private GridClient client;
        private RadegastNetcom netcom;
        private RadegastInstance instance;
        private Avatar avatar;

        public OutfitTextures(RadegastInstance instance, Avatar avatar)
        {
            InitializeComponent();

            this.instance = instance;
            this.netcom = this.instance.Netcom;
            this.client = this.instance.Client;
            this.avatar = avatar;
        }

        public void GetTextures()
        {
            lblName.Text = this.avatar.Name;

            int nTextures = 0;

            for (int j = 0; j < avatar.Textures.FaceTextures.Length; j++) {
                Primitive.TextureEntryFace face = avatar.Textures.FaceTextures[j];

                if (face != null) {
                    ImageType type = ImageType.Normal;

                    switch ((AppearanceManager.TextureIndex)j) {
                        case AppearanceManager.TextureIndex.HeadBaked:
                        case AppearanceManager.TextureIndex.EyesBaked:
                        case AppearanceManager.TextureIndex.UpperBaked:
                        case AppearanceManager.TextureIndex.LowerBaked:
                        case AppearanceManager.TextureIndex.SkirtBaked:
                        case AppearanceManager.TextureIndex.HairBaked:
                            type = ImageType.Baked;
                            break;
                    }

                    if (true ||type != ImageType.Baked) {
                        SLImageHandler img = new SLImageHandler(instance, face.TextureID, ((AppearanceManager.TextureIndex)j).ToString());
                        img.Location = new Point(0, nTextures++ * img.Height);
                        img.Dock = DockStyle.Top;
                        img.Height = 450;
                        pnlImages.Controls.Add(img);
                    }
                }
            }
        }
    }
}
