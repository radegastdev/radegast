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

namespace Radegast
{
	/// <summary>
	/// Description of PrimImporter.
	/// </summary>
	public class PrimImporter
	{
		private enum ImporterState
		{
			RezzingParent,
			RezzingChildren,
			Linking,
			Idle
		}
		
		public enum TextureSet
		{
			OriginalUUID,
			NewUUID,
			SculptUUID,
			WhiteUUID
		}
		
		private class Linkset
		{
			public Primitive RootPrim;
			public List<Primitive> Children = new List<Primitive>();
			
			public Linkset()
			{
				RootPrim = new Primitive();
			}
			
			public Linkset(Primitive rootPrim)
			{
				RootPrim = rootPrim;
			}
		}
		
		Primitive currentPrim;
		Vector3 currentPosition;
		AutoResetEvent primDone = new AutoResetEvent(false);
		List<Primitive> primsCreated;
		List<uint> linkQueue;
		uint rootLocalID;
		ImporterState state = ImporterState.Idle;
		GridClient Client;
		
		string ImportDirectory;
		
		public delegate void LogMessageDelegate(string format, params object[] args);
		public Dictionary<UUID,UUID> Textures;
		public Dictionary<UUID,UUID> SculptTextures;
		public Vector3 RezAt;
		public TextureSet TextureUse = TextureSet.OriginalUUID;
		public LogMessageDelegate LogMessage;
		
		public PrimImporter(GridClient client)
		{
			Client = client;
			Textures = new Dictionary<UUID, UUID>();
			Client.Objects.ObjectUpdate += new EventHandler<PrimEventArgs>(Objects_OnNewPrim);
		}
		
		public void CleanUp()
		{
			Client.Objects.ObjectUpdate -= new EventHandler<PrimEventArgs>(Objects_OnNewPrim);
		}
		
		public void ImportFromFile(string filename)
		{
			ImportDirectory = Path.GetDirectoryName(filename);
			string xml;
			List<Primitive> prims;
			
			xml = File.ReadAllText(filename);
			
			prims = Helpers.OSDToPrimList(OSDParser.DeserializeLLSDXml(xml));
			
			Dictionary<uint, Linkset> linksets = new Dictionary<uint, Linkset>();
			for (int i = 0; i < prims.Count; i++)
			{
				Primitive prim = prims[i];
				
				if (prim.ParentID == 0)
				{
					if (linksets.ContainsKey(prim.LocalID))
						linksets[prim.LocalID].RootPrim = prim;
					else
						linksets[prim.LocalID] = new Linkset(prim);
				}
				else
				{
					if (!linksets.ContainsKey(prim.ParentID))
						linksets[prim.ParentID] = new Linkset();
					
					linksets[prim.ParentID].Children.Add(prim);
				}
			}
			
			primsCreated = new List<Primitive>();
			
			foreach (Linkset linkset in linksets.Values)
			{
				if (linkset.RootPrim.LocalID != 0)
				{
					state = ImporterState.RezzingParent;
					currentPrim = linkset.RootPrim;
					linkset.RootPrim.Position = RezAt;
					currentPosition = RezAt;
					
					// Rez the root prim with no rotation
					Quaternion rootRotation = linkset.RootPrim.Rotation;
					linkset.RootPrim.Rotation = Quaternion.Identity;
					
					Client.Objects.AddPrim(Client.Network.CurrentSim, linkset.RootPrim.PrimData, UUID.Zero,
						linkset.RootPrim.Position, linkset.RootPrim.Scale, linkset.RootPrim.Rotation);
					
					if (!primDone.WaitOne(10000, false))
						throw new Exception("Rez failed, timed out while creating the root prim.");
					
					Client.Objects.SetPosition(Client.Network.CurrentSim, primsCreated[primsCreated.Count - 1].LocalID, linkset.RootPrim.Position);
					
					state = ImporterState.RezzingChildren;
					
					// Rez the child prims
					foreach (Primitive prim in linkset.Children)
					{
						currentPrim = prim;
						currentPosition = prim.Position + linkset.RootPrim.Position;
						
						Client.Objects.AddPrim(Client.Network.CurrentSim, prim.PrimData, UUID.Zero, currentPosition,
						                       prim.Scale, prim.Rotation);
						
						if (!primDone.WaitOne(10000, false))
							throw new Exception("Rez failed, timed out while creating child prim.");
						Client.Objects.SetPosition(Client.Network.CurrentSim, primsCreated[primsCreated.Count - 1].LocalID, currentPosition);
					}
					
					// Create a list of the local IDs of the newly created prims
					List<uint> primIDs = new List<uint>(primsCreated.Count);
					primIDs.Add(rootLocalID); // Root prim is first in list.
					
					if (linkset.Children.Count != 0)
					{
						// Add the rest of the prims to the list of local IDs)
						foreach (Primitive prim in primsCreated)
						{
							if (prim.LocalID != rootLocalID)
								primIDs.Add(prim.LocalID);
						}
						
						linkQueue = new List<uint>(primIDs.Count);
						linkQueue.AddRange(primIDs);
						
						// Link and set the permissions + rotation
						state = ImporterState.Linking;
						Client.Objects.LinkPrims(Client.Network.CurrentSim, linkQueue);
						
						if (primDone.WaitOne(1000 * linkset.Children.Count, false))
							Client.Objects.SetRotation(Client.Network.CurrentSim, rootLocalID, rootRotation);
						else
							LogMessage("Warning: Failed to link {0} prims",linkQueue.Count);
					}
					else
					{
						Client.Objects.SetRotation(Client.Network.CurrentSim, rootLocalID, rootRotation);
					}
					
					// Set permissions on newly created prims
					Client.Objects.SetPermissions(Client.Network.CurrentSim, primIDs,
						PermissionWho.Everyone | PermissionWho.Group | PermissionWho.NextOwner,
						PermissionMask.All, true);
					
					state = ImporterState.Idle;
				}
				else
					LogMessage("WARNING: Skipping a linkset with a missing root prim");
				
				// Reset everything for the next linkset
				primsCreated.Clear();
			}
		}
		
		void Objects_OnNewPrim(object sender, PrimEventArgs e)
		{
			Primitive prim = e.Prim;
			
			if ((prim.Flags & PrimFlags.CreateSelected) == 0)
				return; // We received an update for an object we didn't create
			
			switch (state)
			{
				case ImporterState.RezzingParent:
					rootLocalID = prim.LocalID;
					goto case ImporterState.RezzingChildren;
				case ImporterState.RezzingChildren:
					if (!primsCreated.Contains(prim))
					{
						// Need to set the textures first, and we have the old UUID's, which will work fine
						// for anyone who has the cached UUID for the texture in question.
						
						// TODO: Is there a way to set all of this at once, and update more ObjectProperties stuff?
						Client.Objects.SetPosition(e.Simulator, prim.LocalID, currentPosition);
						if (TextureUse == TextureSet.NewUUID)
						{
							if (Textures[currentPrim.Textures.DefaultTexture.TextureID] == UUID.Zero)
								currentPrim.Textures.DefaultTexture.TextureID = Primitive.TextureEntry.WHITE_TEXTURE;
							else
								currentPrim.Textures.DefaultTexture.TextureID = Textures[currentPrim.Textures.DefaultTexture.TextureID];
							
							for (int j = 0; j < currentPrim.Textures.FaceTextures.Length; j++)
							{
								if (currentPrim.Textures.FaceTextures[j] != null &&
								    currentPrim.Textures.FaceTextures[j].TextureID != Primitive.TextureEntry.WHITE_TEXTURE)
								{
									if (Textures[currentPrim.Textures.FaceTextures[j].TextureID] == UUID.Zero)
										currentPrim.Textures.FaceTextures[j] = null;
									else
										currentPrim.Textures.FaceTextures[j].TextureID = Textures[currentPrim.Textures.FaceTextures[j].TextureID];
								}
							}
						}
						else if (TextureUse == TextureSet.WhiteUUID || TextureUse == TextureSet.SculptUUID)
						{
							currentPrim.Textures.DefaultTexture.TextureID = Primitive.TextureEntry.WHITE_TEXTURE;
							for (int j = 0; j < currentPrim.Textures.FaceTextures.Length; j++)
							{
								currentPrim.Textures.FaceTextures[j] = null;
							}
						}
						Client.Objects.SetTextures(e.Simulator, prim.LocalID, currentPrim.Textures);
						
						if (currentPrim.Light != null && currentPrim.Light.Intensity > 0)
						{
							Client.Objects.SetLight(e.Simulator, prim.LocalID, currentPrim.Light);
						}
						
						if (currentPrim.Flexible != null)
						{
							Client.Objects.SetFlexible(e.Simulator, prim.LocalID, currentPrim.Flexible);
						}
						
						if (currentPrim.Sculpt != null && currentPrim.Sculpt.SculptTexture != UUID.Zero)
						{
							if (TextureUse == TextureSet.NewUUID || TextureUse == TextureSet.SculptUUID)
							{
								if (Textures[currentPrim.Sculpt.SculptTexture] == UUID.Zero)
									currentPrim.Sculpt.SculptTexture = Primitive.TextureEntry.WHITE_TEXTURE;
								else
									currentPrim.Sculpt.SculptTexture = Textures[currentPrim.Sculpt.SculptTexture];
							}
							else if (TextureUse == TextureSet.WhiteUUID)
								currentPrim.Sculpt.SculptTexture = Primitive.TextureEntry.WHITE_TEXTURE;
							
							Client.Objects.SetSculpt(e.Simulator, prim.LocalID, currentPrim.Sculpt);
						}
						
						if (currentPrim.Properties != null && !String.IsNullOrEmpty(currentPrim.Properties.Name))
						{
							Client.Objects.SetName(e.Simulator, prim.LocalID, currentPrim.Properties.Name);
						}
						
						if (currentPrim.Properties != null && !String.IsNullOrEmpty(currentPrim.Properties.Description))
						{
							Client.Objects.SetDescription(e.Simulator, prim.LocalID, currentPrim.Properties.Description);
						}
						
						primsCreated.Add(prim);
						primDone.Set();
					}
					break;
				case ImporterState.Linking:
					lock(linkQueue)
					{
						int indx = linkQueue.IndexOf(prim.LocalID);
						if (indx != -1)
						{
							linkQueue.RemoveAt(indx);
							if (linkQueue.Count == 0)
								primDone.Set();
						}
					}
					break;
			}
		}
	}
}
