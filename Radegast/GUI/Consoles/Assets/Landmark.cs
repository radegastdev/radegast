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
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class Landmark : DettachableControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private InventoryLandmark landmark;
        private AssetLandmark decodedLandmark;
        private UUID parcelID;
        private ParcelInfo parcel;
        private Vector3 localPosition;
        private bool parcelLocation = false;

        public Landmark(RadegastInstance instance, InventoryLandmark landmark)
        {
            this.landmark = landmark;
            Init(instance);
            client.Assets.RequestAsset(landmark.AssetUUID, landmark.AssetType, true, Assets_OnAssetReceived);
        }

        public Landmark(RadegastInstance instance, UUID parcelID)
        {
            this.parcelID = parcelID;
            Init(instance);
            parcelLocation = true;
            client.Parcels.RequestParcelInfo(parcelID);
        }

        void Init(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(Landmark_Disposed);

            this.instance = instance;

            // Callbacks
            client.Grid.RegionHandleReply += new EventHandler<RegionHandleReplyEventArgs>(Grid_RegionHandleReply);
            client.Parcels.ParcelInfoReply += new EventHandler<ParcelInfoReplyEventArgs>(Parcels_ParcelInfoReply);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void Landmark_Disposed(object sender, EventArgs e)
        {
            client.Grid.RegionHandleReply -= new EventHandler<RegionHandleReplyEventArgs>(Grid_RegionHandleReply);
            client.Parcels.ParcelInfoReply -= new EventHandler<ParcelInfoReplyEventArgs>(Parcels_ParcelInfoReply);
        }

        void Parcels_ParcelInfoReply(object sender, ParcelInfoReplyEventArgs e)
        {
            if (e.Parcel.ID != parcelID) return;

            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => Parcels_ParcelInfoReply(sender, e)));
                return;
            }

            this.parcel = e.Parcel;

            pnlDetail.Visible = true;
            if (parcel.SnapshotID != UUID.Zero)
            {
                SLImageHandler img = new SLImageHandler(instance, parcel.SnapshotID, "");
                img.Dock = DockStyle.Fill;
                pnlDetail.Controls.Add(img);
                pnlDetail.Disposed += (object senderx, EventArgs ex) =>
                {
                    img.Dispose();
                };
                img.BringToFront();
            }

            btnTeleport.Enabled = true;
            btnShowOnMap.Enabled = true;

            if (parcelLocation)
            {
                localPosition = new Vector3();
                localPosition.X = parcel.GlobalX % 256;
                localPosition.Y = parcel.GlobalY % 256;
                localPosition.Z = parcel.GlobalZ;
            }

            if (decodedLandmark == null)
            {
                txtParcelName.Text = string.Format("{0} - {1} ", parcel.Name, parcel.SimName);
            }
            else
            {
                txtParcelName.Text = string.Format("{0} - {1} ({2}, {3}, {4}) ", parcel.Name, parcel.SimName, (int)decodedLandmark.Position.X, (int)decodedLandmark.Position.Y, (int)decodedLandmark.Position.Z);
            }

            txtParcelDescription.Text = parcel.Description;
        }

        void Grid_RegionHandleReply(object sender, RegionHandleReplyEventArgs e)
        {
            if (decodedLandmark == null || decodedLandmark.RegionID != e.RegionID) return;

            parcelID = client.Parcels.RequestRemoteParcelID(decodedLandmark.Position, e.RegionHandle, e.RegionID);
            if (parcelID != UUID.Zero)
            {
                client.Parcels.RequestParcelInfo(parcelID);
            }
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (transfer.Success && asset.AssetType == AssetType.Landmark)
            {
                decodedLandmark = (AssetLandmark)asset;
                decodedLandmark.Decode();
                localPosition = decodedLandmark.Position;
                client.Grid.RequestRegionHandle(decodedLandmark.RegionID);
            }
        }

        private void btnTeleport_Click(object sender, EventArgs e)
        {
            instance.MainForm.WorldMap.DisplayLocation(parcel.SimName,
                (int)localPosition.X,
                (int)localPosition.Y,
                (int)localPosition.Z);
            instance.MainForm.WorldMap.DoTeleport();
        }

        private void btnShowOnMap_Click(object sender, EventArgs e)
        {
            instance.MainForm.WorldMap.Show();
            instance.MainForm.WorldMap.DisplayLocation(parcel.SimName, 
                (int)localPosition.X,
                (int)localPosition.Y,
                (int)localPosition.Z);
        }
    }
}
