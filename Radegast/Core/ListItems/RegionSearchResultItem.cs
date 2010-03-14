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
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;
using Radegast.Netcom;

namespace Radegast
{
    public class RegionSearchResultItem
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;

        public GridRegion region;
        private System.Drawing.Image mapImage;

        private ListBox listBox;
        private int listIndex;

        private bool imageDownloading = false;
        private bool imageDownloaded = false;

        private bool gettingAgentCount = false;
        private bool gotAgentCount = false;
        private BackgroundWorker agentCountWorker;

        public RegionSearchResultItem(RadegastInstance instance, GridRegion region, ListBox listBox)
        {
            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;
            this.region = region;
            this.listBox = listBox;

            agentCountWorker = new BackgroundWorker();
            agentCountWorker.DoWork += new DoWorkEventHandler(agentCountWorker_DoWork);
            agentCountWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(agentCountWorker_RunWorkerCompleted);

            AddClientEvents();
        }

        private void agentCountWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<OpenMetaverse.MapItem> items =
                client.Grid.MapItems(
                    region.RegionHandle,
                    OpenMetaverse.GridItemType.AgentLocations,
                    OpenMetaverse.GridLayerType.Terrain, 500);

            if (items != null)
                e.Result = (byte)items.Count;
        }

        private void agentCountWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gettingAgentCount = false;
            gotAgentCount = true;

            if (e.Result != null)
                region.Agents = (byte)e.Result;

            RefreshListBox();
        }

        private void AddClientEvents()
        {
        }

        //Separate thread
        private void Assets_OnImageReceived(AssetTexture texture)
        {
            if (texture.AssetID != region.MapImageID) return;
            if (texture.AssetData == null) return;

            OpenMetaverse.Imaging.ManagedImage tmp;

            if (!OpenMetaverse.Imaging.OpenJPEG.DecodeToImage(texture.AssetData, out tmp, out mapImage))
            {
                return;
            }

            imageDownloading = false;
            imageDownloaded = true;
            listBox.BeginInvoke(new MethodInvoker(RefreshListBox));
            listBox.BeginInvoke(new OnMapImageRaise(OnMapImageDownloaded), new object[] { EventArgs.Empty });
        }

        //UI thread
        private void RefreshListBox()
        {
            listBox.Refresh();
        }

        public void RequestMapImage(float priority)
        {
            if (region.MapImageID == UUID.Zero)
            {
                imageDownloaded = true;
                OnMapImageDownloaded(EventArgs.Empty);
                return;
            }

            client.Assets.RequestImage(region.MapImageID, delegate(TextureRequestState state, AssetTexture assetTexture)
            {
                if (state == TextureRequestState.Finished)
                {
                    Assets_OnImageReceived(assetTexture);
                }
            });
            imageDownloading = true;
        }

        public void RequestAgentLocations()
        {
            gettingAgentCount = true;
            agentCountWorker.RunWorkerAsync();
        }

        public override string ToString()
        {
            return region.Name;
        }

        public event EventHandler MapImageDownloaded;
        private delegate void OnMapImageRaise(EventArgs e);
        protected virtual void OnMapImageDownloaded(EventArgs e)
        {
            if (MapImageDownloaded != null) MapImageDownloaded(this, e);
        }

        public GridRegion Region
        {
            get { return region; }
        }

        public System.Drawing.Image MapImage
        {
            get { return mapImage; }
        }

        public bool IsImageDownloaded
        {
            get { return imageDownloaded; }
        }

        public bool IsImageDownloading
        {
            get { return imageDownloading; }
        }

        public bool GettingAgentCount
        {
            get { return gettingAgentCount; }
        }

        public bool GotAgentCount
        {
            get { return gotAgentCount; }
        }

        public int ListIndex
        {
            get { return listIndex; }
            set { listIndex = value; }
        }
    }
}
