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
using System.Collections.Generic;
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
