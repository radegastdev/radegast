using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenMetaverse;

namespace Radegast
{

    public class FolderCopy
    {
        RadegastInstance Instance;
        GridClient Client;

        public FolderCopy(RadegastInstance instance)
        {
            this.Instance = instance;
            this.Client = this.Instance.Client;
        }

        public void GetFolders(string folder)
        {
            var f = FindFolder(folder, Client.Inventory.Store.LibraryRootNode);
            if (f == null) return;

            UUID dest = Client.Inventory.FindFolderForType(AssetType.Clothing);
            if (dest == UUID.Zero) return;

            var destFolder = (InventoryFolder)Client.Inventory.Store[dest];

            WorkPool.QueueUserWorkItem(sync =>
            {
                Instance.TabConsole.DisplayNotificationInChat("Starting copy operation...");
                foreach (var node in f.Nodes.Values)
                {
                    if (node.Data is InventoryFolder)
                    {
                        var s = (InventoryFolder)node.Data;
                        Instance.TabConsole.DisplayNotificationInChat(string.Format("  Copying {0} to {1}", s.Name, destFolder.Name));
                        CopyFolder(destFolder, s);
                    }
                }
                Instance.TabConsole.DisplayNotificationInChat("Done.");
            });
        }

        public void CopyFolder(InventoryFolder dest, InventoryFolder folder)
        {
            UUID newFolderID = Client.Inventory.CreateFolder(dest.UUID, folder.Name, FolderType.None);
            Thread.Sleep(500);
            var items = Client.Inventory.FolderContents(folder.UUID, folder.OwnerID, true, true, InventorySortOrder.ByDate, 45 * 1000);
            AutoResetEvent copied = new AutoResetEvent(false);
            foreach (var item in items)
            {
                if (item is InventoryItem)
                {
                    copied.Reset();
                    Client.Inventory.RequestCopyItem(item.UUID, newFolderID, item.Name, folder.OwnerID, (InventoryBase target) =>
                    {
                        Instance.TabConsole.DisplayNotificationInChat(string.Format("    * Copied {0} to {1}", item.Name, dest.Name));
                        copied.Set();
                    });
                    copied.WaitOne(15 * 1000, false);
                }
            }
        }

        public InventoryNode FindFolder(string folder, InventoryNode start)
        {
            if (start.Data.Name == folder)
            {
                return start;
            }

            foreach (var node in start.Nodes.Values)
            {
                if (node.Data is InventoryFolder)
                {
                    var n = FindFolder(folder, node);
                    if (n != null)
                    {
                        return n;
                    }
                }
            }

            return null;
        }
    }
}
