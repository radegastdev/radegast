using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class InventoryConsole : UserControl
    {
        RadegastInstance Instance;
        GridClient Client;
        Dictionary<UUID, TreeNode> FolderNodes = new Dictionary<UUID, TreeNode>();

        private InventoryManager Manager;
        private OpenMetaverse.Inventory Inventory;


        public InventoryConsole(RadegastInstance i)
        {
            InitializeComponent();
            Instance = i;
            Client = i.Client;
            Manager = Client.Inventory;
            Inventory = Manager.Store;
            Inventory.RootFolder.OwnerID = Client.Self.AgentID;
            invTree.ImageList = frmMain.ResourceImages;
            invTree.AfterExpand += new TreeViewEventHandler(TreeView_AfterExpand);
            invTree.MouseClick += new MouseEventHandler(invTree_MouseClick);
            invTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(invTree_NodeMouseDoubleClick);
            invTree.TreeViewNodeSorter = new InvNodeSorter();
            AddDir(null, Inventory.RootFolder);
            invTree.Nodes[0].Expand();
            Client.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);

        }

        void Avatars_OnAvatarNames(Dictionary<UUID, string> names)
        {
            if (txtCreator.Tag == null) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Avatars_OnAvatarNames(names);
                }));
                return;
            }

            if (names.ContainsKey((UUID)txtCreator.Tag))
            {
                txtCreator.Text = names[(UUID)txtCreator.Tag];
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            (new frmProfile(Instance, txtCreator.Text, (UUID)txtCreator.Tag)).Show();
            
        }

        void UpdateItemInfo(InventoryItem item)
        {
            btnProfile.Enabled = true;
            txtItemName.Text = item.Name;
            txtCreator.Text = Instance.getAvatarName(item.CreatorID);
            txtCreator.Tag = item.CreatorID;
            if (item.AssetUUID != null)
            {
                txtAssetID.Text = item.AssetUUID.ToString();
            }
            else
            {
                txtAssetID.Text = String.Empty;
            }
            pnlDetail.Controls.Clear();

            switch (item.AssetType)
            {
                case AssetType.Texture:
                    SLImageHandler image = new SLImageHandler(Instance, item.AssetUUID, item.Name);
                    image.Dock = DockStyle.Fill;
                    pnlDetail.Controls.Add(image);
                    break;
            }
        }

        void invTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is InventoryItem)
            {
                InventoryItem item = e.Node.Tag as InventoryItem;
                switch (item.AssetType)
                {
                        
                    case AssetType.Landmark:
                        frmMain.tabsConsole.DisplayNotificationInChat("Teleporting to " + item.Name);
                        Client.Self.RequestTeleport(item.AssetUUID);
                        break;
                }
            }
        }

        void invTree_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode node = invTree.GetNodeAt(new Point(e.X, e.Y));
            if (node == null) return;

            if (e.Button == MouseButtons.Left)
            {
                invTree.SelectedNode = node;
                if (node.Tag is InventoryItem)
                {
                    UpdateItemInfo(node.Tag as InventoryItem);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                invTree.SelectedNode = node;
                if (node.Tag is InventoryFolder)
                {
                    InventoryFolder f = node.Tag as InventoryFolder;
                    folderContextTitle.Text = f.Name;

                    if (f.PreferredType == AssetType.Unknown)
                    {
                        folderContextDelete.Enabled = true;
                    }
                    else
                    {
                        folderContextDelete.Enabled = false;
                    }
                    
                    folderContext.Show(invTree, new Point(e.X, e.Y));
                }
                Logger.Log("Right click on node: " + node.Name, Helpers.LogLevel.Debug, Client);
            }
        }

        #region Context menu folder
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = invTree.SelectedNode;
            if (node == null) return;

            node.Nodes.Clear();
            TreeNode dummy = new TreeNode();
            dummy.Name = "DummyTreeNode";
            dummy.Text = "Loading...";
            node.Nodes.Add(dummy);

            DisplayFolder(node, node.Tag as InventoryFolder);

        }
        #endregion

        int GetDirImageIndex(string t)
        {
            int res = frmMain.ImageNames.IndexOf("inv_folder_" + t);
            if (res == -1)
            {
                switch (t)
                {
                    case "trashfolder":
                        return frmMain.ImageNames.IndexOf("inv_folder_trash");

                    case "lostandfoundfolder":
                        return frmMain.ImageNames.IndexOf("inv_folder_lostandfound");

                    case "lsltext":
                        return frmMain.ImageNames.IndexOf("inv_folder_script");
                }
                return frmMain.ImageNames.IndexOf("inv_folder_plain_closed");
            }
            return res;
        }

        TreeNode AddDir(TreeNode parentNode, InventoryFolder f)
        {
            TreeNode dirNode = new TreeNode();

            TreeNode dummy = new TreeNode();
            dummy.Name = "DummyTreeNode";
            dummy.Text = "Loading...";
            dirNode.ImageIndex = -1;
            dirNode.SelectedImageIndex = -1;
            dirNode.Nodes.Add(dummy);

            dirNode.Name = f.Name;
            dirNode.Text = f.Name;
            dirNode.Tag = f;
            dirNode.ImageIndex = GetDirImageIndex(f.PreferredType.ToString().ToLower());
            dirNode.SelectedImageIndex = dirNode.ImageIndex;
            if (parentNode == null)
            {
                invTree.Nodes.Add(dirNode);
            }
            else
            {
                parentNode.Nodes.Add(dirNode);
            }
            return dirNode;
        }

        int GetItemImageIndex(string t)
        {
            int res = frmMain.ImageNames.IndexOf("inv_item_" + t);
            if (res == -1)
            {
                if (t == "lsltext")
                {
                    return frmMain.ImageNames.IndexOf("inv_item_script");
                }
                else if (t == "callingcard")
                {
                    return frmMain.ImageNames.IndexOf("inv_item_callingcard_offline");
                }
            }
            return res;
        }

        TreeNode AddItem(TreeNode parent, InventoryItem item)
        {
            TreeNode itemNode = new TreeNode();
            itemNode.Name = item.Name;
            itemNode.Text = item.Name;
            itemNode.Tag = item;
            int img = -1;
            if (item is InventoryWearable)
            {
                InventoryWearable w = item as InventoryWearable;
                img = GetItemImageIndex(w.WearableType.ToString().ToLower());
            }
            else
            {
                img = GetItemImageIndex(item.AssetType.ToString().ToLower());
            }
            itemNode.ImageIndex = img;
            itemNode.SelectedImageIndex = img;
            parent.Nodes.Add(itemNode);
            return itemNode;
        }

        void TreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            DisplayFolder(node, node.Tag as InventoryFolder);
        }

        void DisplayFolder(TreeNode parent, InventoryFolder folder)
        {
            if (folder == null) return;

            if (!FolderNodes.ContainsKey(folder.UUID))
            {
                FolderNodes.Add(folder.UUID, parent);
            }

            List<InventoryBase> contents =
                Client.Inventory.FolderContents(folder.UUID, folder.OwnerID, true, true, InventorySortOrder.ByName, 3000);
            parent.Nodes.Clear();
            if (contents == null) return;

            foreach (InventoryBase item in contents)
            {
                if (item is InventoryFolder)
                {
                    AddDir(parent, item as InventoryFolder);
                }
                else
                {
                    AddItem(parent, item as InventoryItem);
                }
            }

        }

        private void invTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            invTree.SelectedNode = e.Item as TreeNode;
            invTree.DoDragDrop(e.Item, DragDropEffects.Copy);
        }

     }

    // Create a node sorter that implements the IComparer interface.
    public class InvNodeSorter : IComparer
    {
        bool _sysfirst = true;

        int CompareFolders(InventoryFolder x, InventoryFolder y)
        {
            if (_sysfirst)
            {
                if (x.PreferredType != AssetType.Unknown && y.PreferredType == AssetType.Unknown)
                {
                    return -1;
                }
                else if (x.PreferredType == AssetType.Unknown && y.PreferredType != AssetType.Unknown)
                {
                    return 1;
                }
            }
            return String.Compare(x.Name, y.Name);
        }

        public bool SystemFoldersFirst { set { _sysfirst = value; } get { return _sysfirst; } }

        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            if (tx.Tag is InventoryFolder && ty.Tag is InventoryFolder)
            {
                return CompareFolders(tx.Tag as InventoryFolder, ty.Tag as InventoryFolder);
            }
            return string.Compare(tx.Text, ty.Text);
        }
    }

}
