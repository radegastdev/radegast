using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using OpenMetaverse;
using OpenMetaverse.Imaging;

namespace Radegast
{
    public static class ImageHelper
    {
        public static System.Drawing.Image Decode(byte[] j2cdata)
        {
            System.Drawing.Image image = null;
            ManagedImage tmp;

            try {
                if (!OpenJPEG.DecodeToImage(j2cdata, out tmp, out image)) {
                    throw new Exception("error");
                }
            } catch (Exception ex) {
                Logger.Log("Failed decoding image:" + ex.Message, Helpers.LogLevel.Debug);
            }
            
            return image;
        }
    }
}
