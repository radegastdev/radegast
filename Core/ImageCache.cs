using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenMetaverse;

namespace Radegast
{
    public class ImageCache
    {
        private Dictionary<UUID, System.Drawing.Image> cache = new Dictionary<UUID, System.Drawing.Image>();
        private Dictionary<UUID, byte[]> j2cache = new Dictionary<UUID, byte[]>();

        public ImageCache()
        {

        }

        public bool ContainsImage(UUID imageID)
        {
            return cache.ContainsKey(imageID);
        }

        public bool ContainsJ2Image(UUID imageID)
        {
            return j2cache.ContainsKey(imageID);
        }

        public void AddImage(UUID imageID, System.Drawing.Image image)
        {
            if (!cache.ContainsKey(imageID)) {
                cache.Add(imageID, image);
            }
        }

        public void AddJ2Image(UUID imageID, byte[] image)
        {
            if (!j2cache.ContainsKey(imageID)) {
                j2cache.Add(imageID, image);
            }
        }

        public void RemoveImage(UUID imageID)
        {
            cache.Remove(imageID);
        }

        public void RemoveJ2Image(UUID imageID)
        {
            j2cache.Remove(imageID);
        }

        public System.Drawing.Image GetImage(UUID imageID)
        {
            return cache[imageID];
        }

        public byte[] GetJ2Image(UUID imageID)
        {
            return j2cache[imageID];
        }
    }
}
