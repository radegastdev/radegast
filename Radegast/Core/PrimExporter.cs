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
using System.IO;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using OpenMetaverse.Assets;

namespace Radegast
{
	/// <summary>
	/// Description of PrimExporter.
	/// </summary>
	public class PrimExporter
	{
		List<UUID> Textures = new List<UUID>();
		AutoResetEvent GotPermissionsEvent = new AutoResetEvent(false);
		Primitive.ObjectProperties Properties;
		bool GotPermissions = false;
		UUID SelectedObject = UUID.Zero;
		
		Dictionary<UUID, Primitive> PrimsWaiting = new Dictionary<UUID, Primitive>();
		AutoResetEvent AllPropertiesReceived = new AutoResetEvent(false);
		GridClient Client;
		string ExportDirectory;
		private uint uLocalID;
		public delegate void LogMessageDelegate(string format, params object[] args);
		public LogMessageDelegate LogMessage;
		
		public PrimExporter(GridClient client)
		{
			Client = client;
			Client.Objects.ObjectPropertiesFamily += new EventHandler<ObjectPropertiesFamilyEventArgs>(Objects_OnObjectPropertiesFamily);
			Client.Objects.ObjectProperties += new EventHandler<ObjectPropertiesEventArgs>(Objects_OnObjectProperties);
		}
		
		public void CleanUp()
		{
			Client.Objects.ObjectPropertiesFamily -= new EventHandler<ObjectPropertiesFamilyEventArgs>(Objects_OnObjectPropertiesFamily);
			Client.Objects.ObjectProperties += new EventHandler<ObjectPropertiesEventArgs>(Objects_OnObjectProperties);
		}
		
		public void ExportToFile(string filename, uint localID)
		{
			Primitive exportPrim;
			uint localid;
			
			ExportDirectory = Path.Combine(Path.GetDirectoryName(filename),Path.GetFileNameWithoutExtension(filename));
			Directory.CreateDirectory(ExportDirectory);
			
			exportPrim = Client.Network.CurrentSim.ObjectsPrimitives.Find(
				delegate(Primitive prim) { return prim.LocalID == localID; }
			);
			
			if (exportPrim != null)
			{
				if (exportPrim.ParentID != 0)
					localid = exportPrim.ParentID;
				else
					localid = exportPrim.LocalID;
				
				uLocalID = localid;
				// Check for export permission first
				Client.Objects.RequestObjectPropertiesFamily(Client.Network.CurrentSim, exportPrim.ID);
				GotPermissionsEvent.WaitOne(1000 * 10, false);
				
				if (!GotPermissions)
				{
					throw new Exception("Couldn't fetch permissions for the requested object, try again");
				}
				else
				{
					GotPermissions = false;

					// Must be Owner and Creator of the item to export, per Linden Lab's TOS
					if (!(Properties.CreatorID == Client.Self.AgentID &&
					     Properties.OwnerID == Client.Self.AgentID))
					{
						string msg = "That object is owned by {0}, Created by {1} we don't have permission to export it. Your UUID: {2}";
						throw new Exception(String.Format(msg,Properties.OwnerID,Properties.CreatorID,Client.Self.AgentID));
					}
					
					List<Primitive> prims = Client.Network.CurrentSim.ObjectsPrimitives.FindAll(
						delegate(Primitive prim)
						{
							return (prim.LocalID == localid || prim.ParentID == localid);
						}
					);
					
					bool complete = RequestObjectProperties(prims, 250);
					
					string output = OSDParser.SerializeLLSDXmlString(Helpers.PrimListToOSD(prims));
					File.WriteAllText(filename,output);
					
					List<ImageRequest> textureRequests = new List<ImageRequest>();
					
					lock(Textures)
					{
						for (int i = 0; i < prims.Count; i++)
						{
							Primitive prim = prims[i];
							UUID texture;
							
							if (prim.Textures.DefaultTexture.TextureID != Primitive.TextureEntry.WHITE_TEXTURE &&
							    !Textures.Contains(prim.Textures.DefaultTexture.TextureID))
							{
								texture = new UUID(prim.Textures.DefaultTexture.TextureID);
								Textures.Add(texture);
							}
							
							for (int j = 0; j < prim.Textures.FaceTextures.Length; j++)
							{
								if (prim.Textures.FaceTextures[j] != null &&
								    prim.Textures.FaceTextures[j].TextureID != Primitive.TextureEntry.WHITE_TEXTURE &&
								    !Textures.Contains(prim.Textures.FaceTextures[j].TextureID))
								{
									texture = new UUID(prim.Textures.FaceTextures[j].TextureID);
									Textures.Add(texture);
								}
							}
							
							if (prim.Sculpt != null && prim.Sculpt.SculptTexture != UUID.Zero && !Textures.Contains(prim.Sculpt.SculptTexture))
							{
								texture = new UUID(prim.Sculpt.SculptTexture);
								Textures.Add(texture);
							}
						}
						
						FindImagesInInventory();
						
						for (int i = 0; i < Textures.Count; i++)
							textureRequests.Add(new ImageRequest(Textures[i], ImageType.Normal, 1013000.0f, 0));
						
						foreach (ImageRequest request in textureRequests)
						{
							Client.Assets.RequestImage(request.ImageID, request.Type, Assets_OnImageReceived);
						}
					}
				}
			}
			else
			{
				throw new Exception("Couldn't find id " + localID.ToString() + " in the " +
				                    Client.Network.CurrentSim.ObjectsPrimitives.Count + 
				                    " objects currently indexed in the current simulator.");
			}
		}
		
		private List<InventoryNode> TraverseDir(InventoryNode node)
		{
			List<InventoryNode> nodes = new List<InventoryNode>(node.Nodes.Values);
			List<InventoryNode> textures = new List<InventoryNode>();
			foreach(InventoryNode n in nodes)
			{
				if (n.Data is InventoryFolder)
				{
					List<InventoryNode> nn = TraverseDir(n);
					foreach(InventoryNode i in nn)
						textures.Add(i);
				}
				else
				{
					InventoryItem item = (InventoryItem)n.Data;
					if (item.InventoryType == InventoryType.Texture)
						textures.Add(n);
				}
			}
			return textures;
		}
		
		private void FindImagesInInventory()
		{
			List<InventoryNode> nodes = TraverseDir(Client.Inventory.Store.RootNode);
			List<UUID> oldTextures = new List<UUID>(Textures);
			Textures = new List<UUID>();
			
			foreach (InventoryNode n in nodes)
			{
				InventoryItem texture = (InventoryItem)n.Data;
				if (oldTextures.Contains(texture.AssetUUID))
				{
					LogMessage("Found Texture {0}: {1}",texture.AssetUUID.ToString(),texture.Name);
					PermissionMask fullPerm = PermissionMask.Modify | PermissionMask.Copy | PermissionMask.Transfer;
					if ((texture.Permissions.OwnerMask & fullPerm) == fullPerm)
					{
						Textures.Add(texture.AssetUUID);
						LogMessage("Texture {0} will be exported",texture.Name);
						oldTextures.Remove(texture.AssetUUID);
					}
					else
						LogMessage("Texture {0} is not full perm, will not export.",texture.Name);
				}
			}
			foreach (UUID texture in oldTextures)
				LogMessage("Failed to find {0}, will not export",texture.ToString());
		}
		
		private bool RequestObjectProperties(List<Primitive> objects, int msPerRequest)
		{
			uint[] localids = new uint[objects.Count];
			
			lock (PrimsWaiting)
			{
				PrimsWaiting.Clear();
				
				for (int i = 0; i < objects.Count; ++i)
				{
					localids[i] = objects[i].LocalID;
					PrimsWaiting.Add(objects[i].ID,objects[i]);
				}
			}
			
			Client.Objects.SelectObjects(Client.Network.CurrentSim, localids);
			
			return AllPropertiesReceived.WaitOne(2000 + msPerRequest * objects.Count, false);
		}
		
		void Objects_OnObjectPropertiesFamily(object sender, ObjectPropertiesFamilyEventArgs e)
		{
			Properties = new Primitive.ObjectProperties();
			Properties.SetFamilyProperties(e.Properties);
			if (e.Properties.CreatorID == UUID.Zero)
			{
				Client.Objects.SelectObject(Client.Network.CurrentSim,uLocalID);
			}
			else
			{
				GotPermissions = true;
				GotPermissionsEvent.Set();
			}
		}
		
		void Objects_OnObjectProperties(object sender, ObjectPropertiesEventArgs e)
		{
			if (e.Properties.ObjectID == Properties.ObjectID)
			{
				if (e.Properties.CreatorID != UUID.Zero)
				{
					Properties.CreatorID = e.Properties.CreatorID;
					Properties.Permissions = e.Properties.Permissions;
					GotPermissions = true;
					GotPermissionsEvent.Set();
				}
			}
			lock (PrimsWaiting)
			{
				PrimsWaiting.Remove(e.Properties.ObjectID);
				
				if (PrimsWaiting.Count == 0)
					AllPropertiesReceived.Set();
			}
		}
		
		void Assets_OnImageReceived(TextureRequestState state, AssetTexture asset)
		{
			if (state == TextureRequestState.Finished && Textures.Contains(asset.AssetID))
			{
				lock (Textures)
					Textures.Remove(asset.AssetID);
				
				if (state == TextureRequestState.Finished)
				{
					try
					{
						File.WriteAllBytes(Path.Combine(ExportDirectory,asset.AssetID + ".jp2"), asset.AssetData);
						LogMessage("Successfully downloaded texture {0}",asset.AssetID.ToString());
					}
					catch (Exception ex)
					{
						LogMessage("Failed to download texture {0}\r\nReason: {1}",asset.AssetID.ToString(),ex.Message);
					}
				}
				else
				{
					LogMessage("Failed to download texture {0}\r\nReason: {1}",asset.AssetID.ToString(),state);
				}
				lock (Textures)
				{
					if (Textures.Count == 0)
						LogMessage("Texture Download complete!");
				}
			}
		}
	}
}
