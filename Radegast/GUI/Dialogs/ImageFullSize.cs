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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Imaging;

namespace Radegast
{
    public partial class ImageFullSize : Form
    {
        private UUID imageID;
        private Image image;
        private ImageCache cache;

        public ImageFullSize(RadegastInstance instance, UUID image, string label, Image img)
        {
            InitializeComponent();

            this.imageID = image;
            this.image = img;
            cache = instance.ImageCache;

            if (instance.advancedDebugging)
            {
                ContextMenuStrip = cmsImage;
            }

            ClientSize = new Size(img.Height, img.Width);
            BackgroundImage = img;
            BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void tbtnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(image);
        }

        private void tbtnCopyUUID_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(imageID.ToString(), TextDataFormat.Text);
        }

        private void tbtnSave_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.RestoreDirectory = true;
            dlg.Title = "Save image as...";
            dlg.Filter = "Targa (*.tga)|*.tga|Jpeg2000 (*.j2c)|*.j2c|PNG (*.png)|*.png|Jpeg (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp";



            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int type = dlg.FilterIndex;
                if (type == 2)
                { // jpeg200
                    File.WriteAllBytes(dlg.FileName, cache.GetJ2Image(imageID));
                }
                else if (type == 1)
                { // targa
                    File.WriteAllBytes(dlg.FileName, new ManagedImage(new Bitmap(cache.GetImage(imageID))).ExportTGA());
                }
                else if (type == 3)
                { // png
                    cache.GetImage(imageID).Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                else if (type == 4)
                { // jpg
                    cache.GetImage(imageID).Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else
                { // BMP
                    cache.GetImage(imageID).Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }

            dlg.Dispose();
            dlg = null;
        }
    }
}