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
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class Notecard : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private InventoryNotecard notecard;

        public Notecard(RadegastInstance instance, InventoryNotecard notecard)
        {
            InitializeComponent();
            Disposed += new EventHandler(Notecard_Disposed);

            this.instance = instance;
            this.notecard = notecard;

            txtName.Text = notecard.Name;
            txtDesc.Text = notecard.Description;
            lblStatus.Text = "Downloading";

            // Callbacks
            client.Assets.OnAssetReceived += new AssetManager.AssetReceivedCallback(Assets_OnAssetReceived);

            client.Assets.RequestInventoryAsset(notecard, true);
        }

        void Notecard_Disposed(object sender, EventArgs e)
        {
            client.Assets.OnAssetReceived -= new AssetManager.AssetReceivedCallback(Assets_OnAssetReceived);
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (transfer.AssetID != notecard.AssetUUID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Assets_OnAssetReceived(transfer, asset);
                    }
                ));
                return;
            }

            if (transfer.Success && asset.AssetType == AssetType.Notecard)
            {
                lblStatus.Text = "Done";
                AssetNotecard n = (AssetNotecard)asset;
                n.Decode();
                rtbContent.Text = n.BodyText;
            }
            else
            {
                lblStatus.Text = "Failed";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Downloading";
            client.Assets.RequestInventoryAsset(notecard, true);
        }
    }
}
