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

using System;
using System.Collections.Generic;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public class InitialOutfit
    {

        RadegastInstance Instance;
        GridClient Client { get { return Instance.Client; }}
        Inventory Store;

        public InitialOutfit(RadegastInstance instance)
        {
            this.Instance = instance;
            Store = Client.Inventory.Store;
        }

        public static InventoryNode FindNodeMyName(InventoryNode root, string name)
        {
            if (root.Data.Name == name)
            {
                return root;
            }

            foreach (var node in root.Nodes.Values)
            {
                var ret = FindNodeMyName(node, name);
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        public UUID CreateFolder(UUID parent, string name, FolderType type)
        {
            UUID ret = UUID.Zero;
            AutoResetEvent folderCreated = new AutoResetEvent(false);

            EventHandler<InventoryObjectAddedEventArgs> handler = (object sender, InventoryObjectAddedEventArgs e) =>
            {
                if (e.Obj.Name == name && e.Obj is InventoryFolder && ((InventoryFolder)e.Obj).PreferredType == type)
                {
                    ret = e.Obj.UUID;
                    folderCreated.Set();
                    Logger.Log("Created folder " + e.Obj.Name, Helpers.LogLevel.Info);
                }
            };

            Client.Inventory.Store.InventoryObjectAdded += handler;
            ret = Client.Inventory.CreateFolder(parent, name, type);
            bool success = folderCreated.WaitOne(20 * 1000, false);
            Client.Inventory.Store.InventoryObjectAdded -= handler;

            if (success)
            {
                return ret;
            }
            else
            {
                return UUID.Zero;
            }
        }

        List<InventoryBase> FetchFolder(InventoryFolder folder)
        {
            List<InventoryBase> ret = new List<InventoryBase>();

            AutoResetEvent folderFetched = new AutoResetEvent(false);

            EventHandler<FolderUpdatedEventArgs> handler = (object sender, FolderUpdatedEventArgs e) =>
            {
                if (e.FolderID == folder.UUID)
                {
                    ret = Store.GetContents(e.FolderID);
                    folderFetched.Set();
                }
            };

            Client.Inventory.FolderUpdated += handler;
            Client.Inventory.RequestFolderContents(folder.UUID, folder.OwnerID, true, true, InventorySortOrder.ByName);
            bool success = folderFetched.WaitOne(20 * 1000, false);
            Client.Inventory.FolderUpdated -= handler;
            return ret;
        }

        public void CheckFolders()
        {
            // Check if we have clothing folder
            var clothingID = Client.Inventory.FindFolderForType(FolderType.Clothing);
            if (clothingID == Store.RootFolder.UUID)
            {
                clothingID = CreateFolder(Store.RootFolder.UUID, "Clothing", FolderType.Clothing);
            }

            // Check if we have trash folder
            var trashID = Client.Inventory.FindFolderForType(FolderType.Trash);
            if (trashID == Store.RootFolder.UUID)
            {
                trashID = CreateFolder(Store.RootFolder.UUID, "Trash", FolderType.Trash);
            }
        }

        public UUID CopyFolder(InventoryFolder folder, UUID destination)
        {
            UUID newFolderID = CreateFolder(destination, folder.Name, folder.PreferredType);
            //var newFolder = (InventoryFolder)Store[newFolderID];

            var items = FetchFolder(folder);
            foreach (var item in items)
            {
                if (item is InventoryItem)
                {
                    AutoResetEvent itemCopied = new AutoResetEvent(false);
                    Client.Inventory.RequestCopyItem(item.UUID, newFolderID, item.Name, item.OwnerID, (newItem) =>
                    {
                        itemCopied.Set();
                    });
                    if (itemCopied.WaitOne(20 * 1000, false))
                    {
                        Logger.Log("Copied item " + item.Name, Helpers.LogLevel.Info);
                    }
                    else
                    {
                        Logger.Log("Failed to copy item " + item.Name, Helpers.LogLevel.Warning);
                    }
                }
                else if (item is InventoryFolder)
                {
                    CopyFolder((InventoryFolder)item, newFolderID);
                }
            }

            return newFolderID;
        }

        public void SetInitialOutfit(string outfit)
        {
            Thread t = new Thread(() => PerformInit(outfit));

            t.IsBackground = true;
            t.Name = "Initial outfit thread";
            t.Start();
        }

        void PerformInit(string initialOutfitName)
        {
            Logger.Log("Starting intial outfit thread (first login)", Helpers.LogLevel.Debug);
            var outfitFolder = FindNodeMyName(Store.LibraryRootNode, initialOutfitName);

            if (outfitFolder == null)
            {
                return;
            }

            CheckFolders();

            UUID newClothingFolder = CopyFolder((InventoryFolder)outfitFolder.Data, Client.Inventory.FindFolderForType(AssetType.Clothing));

            List<InventoryItem> newOutfit = new List<InventoryItem>();
            foreach (var item in Store.GetContents(newClothingFolder))
            {
                if (item is InventoryWearable || item is InventoryAttachment || item is InventoryObject)
                {
                    newOutfit.Add((InventoryItem)item);
                }
            }

            Instance.COF.ReplaceOutfit(newOutfit);
            Logger.Log("Intial outfit thread (first login) exiting", Helpers.LogLevel.Debug);
        }
    }
}
