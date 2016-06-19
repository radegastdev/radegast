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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;
using System.Windows.Forms;
using System.IO;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using OpenMetaverse.Assets;
using OpenMetaverse.Imaging;

namespace Radegast
{
	public partial class ImportConsole : RadegastTabControl
	{
		#region Private Varaibles
		PrimImporter Importer;
		GridClient Client;
		List<UUID> FailedUploads = new List<UUID>();
		DateTime start;
		string sFileName;
		#endregion
		
		#region Constructors
		public ImportConsole(GridClient client)
		{
			InitializeComponent();
			Client = client;
			Importer = new PrimImporter(client);
			Importer.LogMessage = LogMessage;
			sFileName = "";
			objectName.Text = "";
			primCount.Text = "";
			textureCount.Text = "";

			Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
		}
		#endregion
		
		#region Private Methods
		void LogMessage(string format, params object[] args)
		{
			if (InvokeRequired)
			{
				if (IsHandleCreated || !instance.MonoRuntime)
					BeginInvoke(new MethodInvoker(() => LogMessage(format, args)));
				return;
			}
			txtLog.AppendText(String.Format(format + "\r\n",args));
			txtLog.SelectionStart = txtLog.TextLength;
			txtLog.ScrollToCaret();
		}
		
		void ValidateFileName()
		{
			string fileName = txtFileName.Text;
			if (File.Exists(fileName))
			{
				txtLog.Clear();
				LogMessage("Loading {0}...",fileName);
				string xml = File.ReadAllText(fileName);
				List<Primitive> prims = Helpers.OSDToPrimList(OSDParser.DeserializeLLSDXml(xml));
				int count = prims.Count();
				string name = "";
				string desc = "";
				Importer.Textures = new Dictionary<UUID, UUID>();
				Importer.SculptTextures = new Dictionary<UUID, UUID>();
				LogMessage("Parsing Object Data...");
				foreach(Primitive prim in prims)
				{
					if (prim.ParentID == 0)
					{
						name = prim.Properties.Name;
						desc = prim.Properties.Description;
					}
					
					if (prim.Textures.DefaultTexture.TextureID != Primitive.TextureEntry.WHITE_TEXTURE &&
					    !Importer.Textures.ContainsKey(prim.Textures.DefaultTexture.TextureID))
						Importer.Textures.Add(prim.Textures.DefaultTexture.TextureID,UUID.Zero);
					
					for (int j = 0; j < prim.Textures.FaceTextures.Length; j++)
					{
						if (prim.Textures.FaceTextures[j] != null &&
						    prim.Textures.FaceTextures[j].TextureID != Primitive.TextureEntry.WHITE_TEXTURE &&
						    !Importer.Textures.ContainsKey(prim.Textures.FaceTextures[j].TextureID))
							Importer.Textures.Add(prim.Textures.FaceTextures[j].TextureID,UUID.Zero);
					}
					
					if (prim.Sculpt != null && prim.Sculpt.SculptTexture != UUID.Zero && !Importer.Textures.ContainsKey(prim.Sculpt.SculptTexture))
						Importer.Textures.Add(prim.Sculpt.SculptTexture,UUID.Zero);
					if (prim.Sculpt != null && prim.Sculpt.SculptTexture != UUID.Zero && !Importer.SculptTextures.ContainsKey(prim.Sculpt.SculptTexture))
						Importer.SculptTextures.Add(prim.Sculpt.SculptTexture,UUID.Zero);
					
				}
				objectName.Text = name;
				primCount.Text = prims.Count.ToString();
				textureCount.Text = Importer.Textures.Count().ToString();
				LogMessage("Reading complete, Ready to import...");
			}
		}
		
		UUID FindOrMakeInventoryFolder(string name)
		{
			List<InventoryBase> folders = Client.Inventory.FolderContents(Client.Inventory.FindFolderForType(AssetType.Texture),Client.Self.AgentID,true,false,InventorySortOrder.ByName,15000);
			UUID dir = UUID.Zero;
			foreach(InventoryBase item in folders)
			{
				if (item.Name == name)
					dir = item.UUID;
			}
			
			if (dir == UUID.Zero)
				dir = Client.Inventory.CreateFolder(Client.Inventory.FindFolderForType(AssetType.Texture),name);
			return dir;
		}
		
		void UploadImages()
		{
			ImageUploader upldr = new ImageUploader(Client);
			string path = Path.Combine(Path.GetDirectoryName(txtFileName.Text),Path.GetFileNameWithoutExtension(txtFileName.Text));
			
			UUID uploaddir = FindOrMakeInventoryFolder("Import_" + Path.GetFileNameWithoutExtension(txtFileName.Text));
            FailedUploads.Clear();
			LogMessage("Begining Uploading of Textures...");
			
			List<UUID> textures = new List<UUID>();
			if (Importer.TextureUse == PrimImporter.TextureSet.NewUUID)
				textures = Importer.Textures.Keys.ToList<UUID>();
			else if (Importer.TextureUse == PrimImporter.TextureSet.SculptUUID)
				textures = Importer.SculptTextures.Keys.ToList<UUID>();
			
			foreach (UUID texture in textures)
			{
				if (texture == UUID.Zero)
					continue;
				string file = Path.Combine(path,texture.ToString() + ".jp2");
				if (!File.Exists(file))
				{
					LogMessage("Failed to find texture {0}",texture.ToString());
					continue;
				}
				LogMessage("Uploading texture {0}...",texture.ToString());
				bool ret = upldr.UploadImage(file,"Import of " + Path.GetFileNameWithoutExtension(txtFileName.Text),uploaddir);
				if (ret)
				{
					LogMessage("Uploaded texture {0} with new UUID: {1}\r\nUpload took {2} seconds",
					           texture.ToString(), upldr.TextureID.ToString(),upldr.Duration);
					if (Importer.TextureUse == PrimImporter.TextureSet.NewUUID)
						Importer.Textures[texture] = upldr.TextureID;
					else if (Importer.TextureUse == PrimImporter.TextureSet.SculptUUID)
						Importer.SculptTextures[texture] = upldr.TextureID;
				}
				else
				{
					LogMessage("Upload of texture {0} failed, reason: {1}",texture.ToString(), upldr.Status);
					FailedUploads.Add(texture);
				}
			}
		}
		
		void UploadImagesRetry()
		{
			ImageUploader upldr = new ImageUploader(Client);
			string path = Path.Combine(Path.GetDirectoryName(txtFileName.Text),Path.GetFileNameWithoutExtension(txtFileName.Text));
			
			UUID uploaddir = FindOrMakeInventoryFolder("Import_" + Path.GetFileNameWithoutExtension(txtFileName.Text));
			FailedUploads = new List<UUID>();
			LogMessage("Retrying Uploading of failed Textures...");
			
			List<UUID> textures = new List<UUID>(FailedUploads);
			FailedUploads = new List<UUID>();
			
			foreach(UUID texture in textures)
			{
				string file = Path.Combine(path,texture.ToString() + ".jp2");
				LogMessage("Uploading texture {0}...",texture.ToString());
				bool ret = upldr.UploadImage(file,"Import of " + Path.GetFileNameWithoutExtension(txtFileName.Text),uploaddir);
				if (ret)
				{
					LogMessage("Uploaded texture {0} with new UUID: {1}\r\nUpload took {2} seconds",
					           texture.ToString(), upldr.TextureID.ToString(), upldr.Duration);
					if (Importer.TextureUse == PrimImporter.TextureSet.NewUUID)
						Importer.Textures[texture] = upldr.TextureID;
					else if (Importer.TextureUse == PrimImporter.TextureSet.SculptUUID)
						Importer.SculptTextures[texture] = upldr.TextureID;
				}
				else
				{
					LogMessage("Upload of texture {0} failed, reason: {1}",texture.ToString(),upldr.Status);
					FailedUploads.Add(texture);
				}
			}
		}
		
		void EnableWindow()
		{
			this.Enabled = true;
		}
		#endregion
		
		#region Event Handlers
		void ChckRezAtLocCheckedChanged(object sender, EventArgs e)
		{
			txtX.Enabled = chckRezAtLoc.Checked;
			txtY.Enabled = chckRezAtLoc.Checked;
			txtZ.Enabled = chckRezAtLoc.Checked;
		}
		
		void BtnBrowseClick(object sender, EventArgs e)
		{
			WindowWrapper mainWindow = new WindowWrapper(frmMain.ActiveForm.Handle);
			System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Open object file";
			dlg.Filter = "XML file (*.xml)|*.xml";
			dlg.Multiselect = false;
			DialogResult res = dlg.ShowDialog();
			
			if (res != DialogResult.OK)
				return;
			txtFileName.Text = dlg.FileName;
			ValidateFileName();
		}
		
		void TxtFileNameLeave(object sender, EventArgs e)
		{
			if (sFileName != txtFileName.Text)
			{
				sFileName = txtFileName.Text;
				ValidateFileName();
			}
		}
		
		void BtnUploadClick(object sender, EventArgs e)
		{
			this.Enabled = false;
			if (cmbImageOptions.SelectedIndex == -1)
			{
				MessageBox.Show("You must select an Image Option before you can import an object.","Import Object Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				this.Enabled = true;
				return;
			}
			switch(cmbImageOptions.SelectedIndex)
			{
				case 0:
					Importer.TextureUse = PrimImporter.TextureSet.OriginalUUID;
					break;
				case 1:
					Importer.TextureUse = PrimImporter.TextureSet.NewUUID;
					break;
				case 2:
					Importer.TextureUse = PrimImporter.TextureSet.SculptUUID;
					break;
				case 3:
					Importer.TextureUse = PrimImporter.TextureSet.WhiteUUID;
					break;
			}
			if (chckRezAtLoc.Checked)
			{
				float x = 0.0f;
				float y = 0.0f;
				float z = 0.0f;
				if (!float.TryParse(txtX.Text,out x))
				{
					MessageBox.Show("X Coordinate needs to be a Float position!  Example: 1.500","Import Object Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					this.Enabled = true;
					return;
				}
				if (!float.TryParse(txtY.Text,out y))
				{
					MessageBox.Show("Y Coordinate needs to be a Float position!  Example: 1.500","Import Object Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					this.Enabled = true;
					return;
				}
				if (!float.TryParse(txtZ.Text,out z))
				{
					MessageBox.Show("Z Coordinate needs to be a Float position!  Example: 1.500","Import Object Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					this.Enabled = true;
					return;
				}
				Importer.RezAt = new Vector3(x,y,z);
			}
			else
			{
				Importer.RezAt = Client.Self.SimPosition;
				Importer.RezAt.Z += 3.5f;
			}
			
			Thread t = new Thread(new System.Threading.ThreadStart(delegate()
			{
				try
				{
					start = DateTime.Now;
					// First upload Images that will be needed by the Importer, if required by user.
					if (Importer.TextureUse == PrimImporter.TextureSet.NewUUID || Importer.TextureUse == PrimImporter.TextureSet.SculptUUID)
						UploadImages();
					
					// Check to see if there are any failed uploads.
					if (FailedUploads.Count > 0)
					{
						DialogResult res = MessageBox.Show(String.Format("Failed to upload {0} textures, which to try again?",FailedUploads.Count),
						                                   "Import - Upload Texture Error",MessageBoxButtons.YesNo, MessageBoxIcon.Error);
						if (res == DialogResult.Yes)
							UploadImagesRetry();
						
						if (FailedUploads.Count != 0)
						{
							MessageBox.Show(String.Format("Failed to upload {0} textures on second try, aborting!",FailedUploads.Count),
							                "Import - Upload Texture Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
							LogMessage("Failed to import object, due to texture error, review the log for further information");
							return;
						}
					}
					
					LogMessage("Texture Upload completed");
					LogMessage("Importing Prims...");
					// If we get here, then we successfully uploaded the textures, continue with the upload of the Prims.
					Importer.ImportFromFile(txtFileName.Text);
					LogMessage("Import successful.");
					LogMessage("Total Time: {0}",DateTime.Now.Subtract(start));
				}
				catch (Exception ex)
				{
					LogMessage("Import failed. Reason: {0}",ex.Message);
					MessageBox.Show(ex.Message,"Importing failed.",MessageBoxButtons.OK,MessageBoxIcon.Error);
				}
				
				BeginInvoke(new MethodInvoker(() => EnableWindow()));
			}));
			t.IsBackground = true;
			t.Start();
		}
		#endregion
	}
}
