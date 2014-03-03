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
	/// Description of ImageUploader.
	/// </summary>
	public class ImageUploader
	{
		GridClient Client;
		DateTime start;
		AutoResetEvent UploadCompleteEvent = new AutoResetEvent(false);
		public UUID TextureID = UUID.Zero;
		public TimeSpan Duration;
		public string Status;
		public bool Success;
		
		public ImageUploader(GridClient client)
		{
			Client = client;
		}
		
		public bool UploadImage(string filename, string desc, UUID folder)
		{
			TextureID = UUID.Zero;
			string inventoryName = Path.GetFileNameWithoutExtension(filename);
			uint timeout = 180 * 1000;
			byte[] data = File.ReadAllBytes(filename);
			start = DateTime.Now;
			
			Client.Inventory.RequestCreateItemFromAsset(data,inventoryName,desc,AssetType.Texture,InventoryType.Texture,folder,
			                                            delegate(bool success, string status, UUID itemID, UUID assetID)
			                                            {
			                                            	TextureID = assetID;
			                                            	Success = success;
			                                            	Status = status;
			                                            	UploadCompleteEvent.Set();
			                                            });
			if (UploadCompleteEvent.WaitOne((int)timeout, false))
			{
				if (TextureID != UUID.Zero)
					return true;
				else
					return false;
			}
			else
			{
				Status = "Texture upload timed out";
				return false;
			}
		}
	}
}
