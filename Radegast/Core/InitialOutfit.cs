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
using System.Collections.Generic;
using System.Threading;
using OpenMetaverse;

namespace Radegast
{
    public class InitialOutfit
    {

        RadegastInstance Instance;
        GridClient Client => Instance.Client;
        Inventory Store;

        public InitialOutfit(RadegastInstance instance)
        {
            Instance = instance;
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

            EventHandler<InventoryObjectAddedEventArgs> handler = (sender, e) =>
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

            EventHandler<FolderUpdatedEventArgs> handler = (sender, e) =>
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
            Thread t = new Thread(() => PerformInit(outfit)) {IsBackground = true, Name = "Initial outfit thread"};

            t.Start();
        }

        void PerformInit(string initialOutfitName)
        {
            Logger.Log("Starting initial outfit thread (first login)", Helpers.LogLevel.Debug);
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
