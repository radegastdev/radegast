// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
    public partial class OutfitTextures : UserControl
    {
        private RadegastInstance instance;
        private Avatar avatar;

        public OutfitTextures(RadegastInstance instance, Avatar avatar)
        {
            InitializeComponent();

            this.instance = instance;
            this.avatar = avatar;
        }

        public void GetTextures()
        {
            lblName.Text = this.avatar.Name;

            int nTextures = 0;

            for (int j = 0; j < avatar.Textures.FaceTextures.Length; j++) {
                Primitive.TextureEntryFace face = avatar.Textures.FaceTextures[j];

                if (face != null) {
                    //ImageType type = ImageType.Normal;

                    //switch ((AppearanceManager.TextureIndex)j) {
                    //    case AppearanceManager.TextureIndex.HeadBaked:
                    //    case AppearanceManager.TextureIndex.EyesBaked:
                    //    case AppearanceManager.TextureIndex.UpperBaked:
                    //    case AppearanceManager.TextureIndex.LowerBaked:
                    //    case AppearanceManager.TextureIndex.SkirtBaked:
                    //    case AppearanceManager.TextureIndex.HairBaked:
                    //        type = ImageType.Baked;
                    //        break;
                    //}

                    if (face.TextureID != AppearanceManager.DEFAULT_AVATAR_TEXTURE) {
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
