/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class Landmark : DettachableControl
    {
        private RadegastInstance instance;
        private GridClient client => instance.Client;
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

            GUI.GuiHelpers.ApplyGuiFixes(this);
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

            parcel = e.Parcel;

            pnlDetail.Visible = true;
            if (parcel.SnapshotID != UUID.Zero)
            {
                SLImageHandler img = new SLImageHandler(instance, parcel.SnapshotID, "") {Dock = DockStyle.Fill};
                pnlDetail.Controls.Add(img);
                pnlDetail.Disposed += (senderx, ex) =>
                {
                    img.Dispose();
                };
                img.BringToFront();
            }

            btnTeleport.Enabled = true;
            btnShowOnMap.Enabled = true;

            if (parcelLocation)
            {
                localPosition = new Vector3
                {
                    X = parcel.GlobalX % 256,
                    Y = parcel.GlobalY % 256,
                    Z = parcel.GlobalZ
                };
            }

            txtParcelName.Text = decodedLandmark == null 
                ? $"{parcel.Name} - {parcel.SimName} " 
                : $"{parcel.Name} - {parcel.SimName} ({(int) decodedLandmark.Position.X}, {(int) decodedLandmark.Position.Y}, {(int) decodedLandmark.Position.Z}) ";

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
