using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public class InvTreeView : ListViewNoFlicker
    {
        public class NodeMeta
        {
            public class NodeData
            {
                public bool IsExpanded;
            }

            public Dictionary<int, NodeData> Data = new Dictionary<int, NodeData>(4);
            
            public bool IsExpanded(int id)
            {
                return Data.ContainsKey(id) && Data[id].IsExpanded;
            }

            public void SetExpanded(int id, bool val)
            {
                if (!Data.ContainsKey(id))
                {
                    Data[id] = new NodeData();
                }
                Data[id].IsExpanded = val;
            }
        }

        public class Node
        {
            public Node()
            {
            }

            public InventoryNode ONode;
            public int Level;
            public bool IsDir { get { return ONode.Data is InventoryFolder; } }

            /// <summary>
            /// Is inventory metadata been added to libopenmetaverse node for this instance id
            /// </summary>
            /// <param name="id">ID</param>
            /// <returns>True if data is set for this instance</returns>
            public bool IsMetaSet(int id)
            {
                if (!(ONode.Tag is NodeMeta)) return false;
                var meta = (NodeMeta)ONode.Tag;
                return meta.Data.ContainsKey(id);
            }

            public bool IsExpanded(int id)
            {
                return (ONode.Tag is NodeMeta) && ((NodeMeta)ONode.Tag).IsExpanded(id);
            }

            public void SetExpanded(int id, bool val)
            {
                NodeMeta meta;
                if (ONode.Tag is NodeMeta)
                {
                    meta = (NodeMeta)ONode.Tag;
                }
                else
                {
                    meta = new NodeMeta();
                    ONode.Tag = meta;
                }

                meta.SetExpanded(id, val);
            }
        }

        public static InvTreeSorter Sorter;

        public enum TreeType
        {
            All,
            Recent,
            Worn
        }

        public TreeType InvType;

        RadegastInstance Instance;
        GridClient Client { get { return Instance.Client; } }
        InventoryConsole Console;
        Inventory Inv { get { return Client.Inventory.Store; } }
        List<Node> Mapper = new List<Node>(64);
        static int IconWidth = 16;
        static Image IcnPlus, IcnMinus, IcnLinkOverlay;
        string newItemName = string.Empty;
        static int Serial;
        ContextMenuStrip ctxInv = new ContextMenuStrip();

        int ID;

        public InvTreeView() : base() { }
        public InvTreeView(RadegastInstance instance, InventoryConsole console)
            : base()
        {
            ID = Serial++;
            Instance = instance;
            Console = console;

            FullRowSelect = true;
            CheckBoxes = false;
            View = System.Windows.Forms.View.Details;
            LabelEdit = true;
            LabelWrap = false;
            Columns.Clear();
            Columns.Add("Inventory", Width - 30);
            HeaderStyle = ColumnHeaderStyle.None;
            OwnerDraw = true;
            SmallImageList = frmMain.ResourceImages;
            InitSorter();
            InitIcons();
            VirtualMode = true;
        }

        void InitSorter()
        {
            if (Sorter == null)
            {
                Sorter = new InvTreeSorter();
                if (!Instance.GlobalSettings.ContainsKey("inv_sort_bydate"))
                    Instance.GlobalSettings["inv_sort_bydate"] = true;
                if (!Instance.GlobalSettings.ContainsKey("inv_sort_sysfirst"))
                    Instance.GlobalSettings["inv_sort_sysfirst"] = true;

                Sorter.ByDate = Instance.GlobalSettings["inv_sort_bydate"];
                Sorter.SystemFoldersFirst = Instance.GlobalSettings["inv_sort_sysfirst"];
            }
        }

        void InitIcons()
        {
            if (IcnPlus == null)
            {
                IcnPlus = frmMain.ResourceImages.Images[frmMain.ImageNames.IndexOf("arrow_right")];
                IcnMinus = frmMain.ResourceImages.Images[frmMain.ImageNames.IndexOf("arrow_down")];
                IcnLinkOverlay = frmMain.ResourceImages.Images[frmMain.ImageNames.IndexOf("inv_link_overlay")];
            }
        }

        public void UpdateMapper()
        {
            BeginUpdate();
            Mapper.Clear();
            var inv = new Node()
            {
                ONode = Inv.RootNode,
                Level = 0
            };
            if (!inv.IsMetaSet(ID))
            {
                inv.SetExpanded(ID, true);
            }

            Mapper.Add(inv);
            PrepareNodes(Mapper.Count - 1, inv.IsExpanded(ID));
            if (InvType == TreeType.All)
            {
                var lib = new Node()
                {
                    ONode = Inv.LibraryRootNode,
                    Level = 0
                };
                Mapper.Add(lib);
                PrepareNodes(Mapper.Count - 1, lib.IsExpanded(ID));
            }
            VirtualListSize = Mapper.Count;
            EndUpdate();
        }

        void PrepareNodes(int pos, bool add)
        {
            Node mbr = Mapper[pos];
            pos++;

            if (add)
            {
                PopulateDescendantMembers(ref pos, mbr);
            }
            else
            {
                int kids = ObtainExpandedChildrenCount(pos - 1);
                Mapper.RemoveRange(pos, kids);
            }

            VirtualListSize = Mapper.Count;
        }

        /// <summary>
        /// Populates the descendant members.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="mbr">The data node.</param>
        protected virtual void PopulateDescendantMembers(ref int pos, Node mbr)
        {
            if (InvType == TreeType.All)
            {
                List<InventoryNode> children = new List<InventoryNode>(mbr.ONode.Nodes.Values);
                children.Sort(Sorter);

                foreach (var m in children)
                {
                    Node newNode = new Node()
                    {
                        ONode = m,
                        Level = mbr.Level + 1,
                    };

                    Mapper.Insert(pos++, newNode);
                    if (newNode.IsExpanded(ID))
                    {
                        PopulateDescendantMembers(ref pos, newNode);
                    }
                }
            }
            else if (InvType == TreeType.Recent)
            {
            }
            else if (InvType == TreeType.Worn)
            {
            }
        }

        /// <summary>
        /// Obtains the expanded children count.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <returns></returns>
        private int ObtainExpandedChildrenCount(int pos)
        {
            int kids = 0;
            Node mi = Mapper[pos];
            int level = mi.Level;

            for (int i = pos + 1; i < Mapper.Count; i++, kids++)
            {
                Node mix = Mapper[i];
                int lvlx = mix.Level;
                if (lvlx <= level) break;
            }

            return kids;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ListViewItem item = GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    int xfrom = (item.IndentCount - 1) * IconWidth;
                    int xto = xfrom + IconWidth;
                    Node n = (Node)item.Tag;
                    if (e.X >= xfrom && e.X < xto)
                    {
                        n.SetExpanded(ID, !n.IsExpanded(ID));
                        item.Checked = !item.Checked;
                        PrepareNodes(item.Index, n.IsExpanded(ID));
                        return;
                    }
                }
            }

            if (e.Clicks == 1 && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                var item = GetItemAt(e.X, e.Y);
                if (item == null) return;

                if (item.Tag is InvTreeView.Node)
                {
                    var node = (InvTreeView.Node)item.Tag;
                    if (!node.IsDir)
                    {
                        Console.UpdateItemInfo(Instance.COF.RealInventoryItem(node.ONode.Data as InventoryItem));
                    }
                    else
                    {
                        Console.UpdateItemInfo(null);
                    }
                }
            }
            else if (e.Clicks == 1 && e.Button == System.Windows.Forms.MouseButtons.Right && SelectedIndices.Count > 0)
            {
                ShowContext(e.Location);
            }

            base.OnMouseClick(e);
        }

        protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
        {
            Node n;
            try
            {
                n = Mapper[e.ItemIndex];
            }
            catch
            {
                e.Item = new ListViewItem();
                return;
            }
            ListViewItem item = new ListViewItem(Console.ItemLabel(n.ONode.Data, false));
            var ix = InventoryConsole.GetIconIndex(n.ONode.Data);
            //item.StateImageIndex = ix;
            item.IndentCount = n.Level + 1;
            item.Tag = n;
            e.Item = item;
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            Graphics g = e.Graphics;

            e.DrawBackground();

            if (!(e.Item.Tag is Node))
                return;

            if (e.Item.Selected)
            {
                g.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }

            Node node = e.Item.Tag as Node;

            Image icon = null;
            int offset = IconWidth * node.Level;
            Rectangle rec = new Rectangle(e.Bounds.X + offset + 2 * IconWidth, e.Bounds.Y, e.Bounds.Width - offset - 2 * IconWidth, e.Bounds.Height);
            bool isLink = false;
            int iconIx = 0;

            if (!node.IsDir)
            {
                InventoryItem invItem = (InventoryItem)node.ONode.Data;
                isLink = invItem.IsLink();
                if (isLink)
                {
                    invItem = Instance.COF.RealInventoryItem(invItem);
                }
                iconIx = InventoryConsole.GetIconIndex(Instance.COF.RealInventoryItem(invItem));
            }
            else
            {
                iconIx = InventoryConsole.GetIconIndex(node.ONode.Data);
            }


            try
            {
                if (node.IsDir && node.ONode.Nodes.Count > 0)
                {
                    Image exp;
                    if (node.IsExpanded(ID))
                    {
                        exp = IcnMinus;
                    }
                    else
                    {
                        exp = IcnPlus;
                    }
                    g.DrawImageUnscaled(exp, e.Bounds.X + offset,  e.Bounds.Y);
                }

                icon = frmMain.ResourceImages.Images[iconIx];
                g.DrawImageUnscaled(icon, e.Bounds.X + offset + IconWidth, e.Bounds.Y);
                if (isLink)
                {
                    g.DrawImageUnscaled(IcnLinkOverlay, e.Bounds.X + offset + IconWidth, e.Bounds.Y);
                }
            }
            catch { }

            using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit))
            {
                string label = Console.ItemLabel(node.ONode.Data, false);
                SizeF len = e.Graphics.MeasureString(label, Font, rec.Width, sf);

                e.Graphics.DrawString(
                    label,
                    Font,
                    e.Item.Selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText,
                    rec,
                    sf);

            }
            base.OnDrawItem(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (Columns.Count > 0)
            {
                Columns[0].Width = Width - 30;
            }
            base.OnResize(e);
        }

        #region Context Menu
        List<ListViewItem> selection = new List<ListViewItem>();

        void ShowContext(Point loc)
        {
            selection.Clear();
            foreach (int i in SelectedIndices)
            {
                if (Items[i].Tag is InvTreeView.Node)
                {
                    selection.Add(Items[i]);
                }
            }

            ctxInv.Items.Clear();

            if (selection.Count == 1)
            {
                if (((InvTreeView.Node)selection[0].Tag).IsDir)
                {
                    AddFolderContext(((InvTreeView.Node)selection[0].Tag).ONode.Data as InventoryFolder);
                }
                else
                {
                    AddItemContext(Instance.COF.RealInventoryItem(((InvTreeView.Node)selection[0].Tag).ONode.Data as InventoryItem));
                }
            }

            ctxInv.Show(this, loc);
        }

        void AddItemContext(InventoryItem item)
        {
            ToolStripMenuItem ctxItem;

            if (item.InventoryType == InventoryType.LSL)
            {
                ctxItem = new ToolStripMenuItem("Edit script", null, OnInvItemContextClick);
                ctxItem.Name = "edit_script";
                ctxInv.Items.Add(ctxItem);
            }

            if (item.AssetType == AssetType.Texture)
            {
                ctxItem = new ToolStripMenuItem("View", null, OnInvItemContextClick);
                ctxItem.Name = "view_image";
                ctxInv.Items.Add(ctxItem);
            }

            if (item.InventoryType == InventoryType.Landmark)
            {
                ctxItem = new ToolStripMenuItem("Teleport", null, OnInvItemContextClick);
                ctxItem.Name = "lm_teleport";
                ctxInv.Items.Add(ctxItem);

                ctxItem = new ToolStripMenuItem("Info", null, OnInvItemContextClick);
                ctxItem.Name = "lm_info";
                ctxInv.Items.Add(ctxItem);
            }

            if (item.InventoryType == InventoryType.Notecard)
            {
                ctxItem = new ToolStripMenuItem("Open", null, OnInvItemContextClick);
                ctxItem.Name = "notecard_open";
                ctxInv.Items.Add(ctxItem);
            }

            if (item.InventoryType == InventoryType.Gesture)
            {
                ctxItem = new ToolStripMenuItem("Play", null, OnInvItemContextClick);
                ctxItem.Name = "gesture_play";
                ctxInv.Items.Add(ctxItem);

                ctxItem = new ToolStripMenuItem("Info", null, OnInvItemContextClick);
                ctxItem.Name = "gesture_info";
                ctxInv.Items.Add(ctxItem);
            }

            if (item.InventoryType == InventoryType.Animation)
            {
                if (!Client.Self.SignaledAnimations.ContainsKey(item.AssetUUID))
                {
                    ctxItem = new ToolStripMenuItem("Play", null, OnInvItemContextClick);
                    ctxItem.Name = "animation_play";
                    ctxInv.Items.Add(ctxItem);
                }
                else
                {
                    ctxItem = new ToolStripMenuItem("Stop", null, OnInvItemContextClick);
                    ctxItem.Name = "animation_stop";
                    ctxInv.Items.Add(ctxItem);
                }
            }

            if (item.InventoryType == InventoryType.Object)
            {
                ctxItem = new ToolStripMenuItem("Rez inworld", null, OnInvItemContextClick);
                ctxItem.Name = "rez_inworld";
                ctxInv.Items.Add(ctxItem);
            }

            ctxItem = new ToolStripMenuItem("Rename", null, OnInvItemContextClick);
            ctxItem.Name = "rename_item";
            ctxInv.Items.Add(ctxItem);

            ctxInv.Items.Add(new ToolStripSeparator());

            ctxItem = new ToolStripMenuItem("Cut", null, OnInvItemContextClick);
            ctxItem.Name = "cut_item";
            ctxInv.Items.Add(ctxItem);

            ctxItem = new ToolStripMenuItem("Copy", null, OnInvItemContextClick);
            ctxItem.Name = "copy_item";
            ctxInv.Items.Add(ctxItem);

            if (Instance.InventoryClipboard != null)
            {
                ctxItem = new ToolStripMenuItem("Paste", null, OnInvItemContextClick);
                ctxItem.Name = "paste_item";
                ctxInv.Items.Add(ctxItem);

                if (Instance.InventoryClipboard.Item is InventoryItem)
                {
                    ctxItem = new ToolStripMenuItem("Paste as Link", null, OnInvItemContextClick);
                    ctxItem.Name = "paste_item_link";
                    ctxInv.Items.Add(ctxItem);
                }
            }

            ctxItem = new ToolStripMenuItem("Delete", null, OnInvItemContextClick);
            ctxItem.Name = "delete_item";

            if (Instance.COF.IsAttached(item) || Instance.COF.IsWorn(item))
            {
                ctxItem.Enabled = false;
            }
            ctxInv.Items.Add(ctxItem);

            if (Instance.COF.IsAttached(item) && Instance.RLV.AllowDetach(item))
            {
                ctxItem = new ToolStripMenuItem("Detach from yourself", null, OnInvItemContextClick);
                ctxItem.Name = "detach";
                ctxInv.Items.Add(ctxItem);
            }

            if (!Instance.COF.IsAttached(item) && (item.InventoryType == InventoryType.Object || item.InventoryType == InventoryType.Attachment))
            {
                ToolStripMenuItem ctxItemAttach = new ToolStripMenuItem("Attach to");
                ctxInv.Items.Add(ctxItemAttach);

                ToolStripMenuItem ctxItemAttachHUD = new ToolStripMenuItem("Attach to HUD");
                ctxInv.Items.Add(ctxItemAttachHUD);

                foreach (AttachmentPoint pt in Enum.GetValues(typeof(AttachmentPoint)))
                {
                    if (!pt.ToString().StartsWith("HUD"))
                    {
                        string name = Utils.EnumToText(pt);

                        InventoryItem alreadyAttached = null;
                        if ((alreadyAttached = Console.AttachmentAt(pt)) != null)
                        {
                            name += " (" + alreadyAttached.Name + ")";
                        }

                        ToolStripMenuItem ptItem = new ToolStripMenuItem(name, null, OnInvItemContextClick);
                        ptItem.Name = pt.ToString();
                        ptItem.Tag = pt;
                        ptItem.Name = "attach_to";
                        ctxItemAttach.DropDownItems.Add(ptItem);
                    }
                    else
                    {
                        string name = Utils.EnumToText(pt).Substring(3);

                        InventoryItem alreadyAttached = null;
                        if ((alreadyAttached = Console.AttachmentAt(pt)) != null)
                        {
                            name += " (" + alreadyAttached.Name + ")";
                        }

                        ToolStripMenuItem ptItem = new ToolStripMenuItem(name, null, OnInvItemContextClick);
                        ptItem.Name = pt.ToString();
                        ptItem.Tag = pt;
                        ptItem.Name = "attach_to";
                        ctxItemAttachHUD.DropDownItems.Add(ptItem);
                    }
                }

                ctxItem = new ToolStripMenuItem("Add to Worn", null, OnInvItemContextClick);
                ctxItem.Name = "wear_attachment_add";
                ctxInv.Items.Add(ctxItem);

                ctxItem = new ToolStripMenuItem("Wear", null, OnInvItemContextClick);
                ctxItem.Name = "wear_attachment";
                ctxInv.Items.Add(ctxItem);
            }

            if (item is InventoryWearable)
            {
                ctxInv.Items.Add(new ToolStripSeparator());

                if (Instance.COF.IsWorn(item))
                {
                    ctxItem = new ToolStripMenuItem("Take off", null, OnInvItemContextClick);
                    ctxItem.Name = "item_take_off";
                    ctxInv.Items.Add(ctxItem);
                }
                else
                {
                    ctxItem = new ToolStripMenuItem("Wear", null, OnInvItemContextClick);
                    ctxItem.Name = "item_wear";
                    ctxInv.Items.Add(ctxItem);
                }
            }

            Instance.ContextActionManager.AddContributions(ctxInv, item);

        }

        void OnInvItemContextClick(object sender, EventArgs e)
        {
            string cmd = ((ToolStripMenuItem)sender).Name;
            InventoryItem item = ((InvTreeView.Node)selection[0].Tag).ONode.Data as InventoryItem;

            // Copy, cut, and delete works on links directly
            // The rest operate on the item that is pointed by the link
            if (cmd != "copy_item" && cmd != "cut_item" && cmd != "delete_item")
            {
                item = Instance.COF.RealInventoryItem(item);
            }

            switch (cmd)
            {
                case "copy_item":
                    Instance.InventoryClipboard = new InventoryClipboard(ClipboardOperation.Copy, item);
                    break;

                case "cut_item":
                    Instance.InventoryClipboard = new InventoryClipboard(ClipboardOperation.Cut, item);
                    break;

                case "paste_item":
                    PerformClipboardOperation(Client.Inventory.Store[item.ParentUUID] as InventoryFolder);
                    break;

                case "paste_item_link":
                    PerformLinkOperation(Client.Inventory.Store[item.ParentUUID] as InventoryFolder);
                    break;

                case "delete_item":
                    Client.Inventory.MoveItem(item.UUID, Client.Inventory.FindFolderForType(AssetType.TrashFolder), item.Name);
                    break;

                case "rename_item":
                    selection[0].BeginEdit();
                    break;

                case "detach":
                    Instance.COF.Detach(item);
                    break;

                case "wear_attachment":
                    Instance.COF.Attach(item, AttachmentPoint.Default, true);
                    break;

                case "wear_attachment_add":
                    Instance.COF.Attach(item, AttachmentPoint.Default, false);
                    break;

                case "attach_to":
                    AttachmentPoint pt = (AttachmentPoint)((ToolStripMenuItem)sender).Tag;
                    Instance.COF.Attach(item, pt, true);
                    break;

                case "edit_script":
                    ScriptEditor se = new ScriptEditor(Instance, (InventoryLSL)item);
                    se.Detached = true;
                    return;

                case "view_image":
                    Console.UpdateItemInfo(item);
                    break;

                case "item_take_off":
                    Instance.COF.RemoveFromOutfit(item);
                    break;

                case "item_wear":
                    Instance.COF.AddToOutfit(item, true);
                    break;

                case "lm_teleport":
                    Instance.TabConsole.DisplayNotificationInChat("Teleporting to " + item.Name);
                    Client.Self.RequestTeleport(item.AssetUUID);
                    break;

                case "lm_info":
                    Console.UpdateItemInfo(item);
                    break;

                case "notecard_open":
                    Console.UpdateItemInfo(item);
                    break;

                case "gesture_info":
                    Console.UpdateItemInfo(item);
                    break;

                case "gesture_play":
                    Client.Self.PlayGesture(item.AssetUUID);
                    break;

                case "animation_play":
                    Dictionary<UUID, bool> anim = new Dictionary<UUID, bool>();
                    anim.Add(item.AssetUUID, true);
                    Client.Self.Animate(anim, true);
                    break;

                case "animation_stop":
                    Dictionary<UUID, bool> animStop = new Dictionary<UUID, bool>();
                    animStop.Add(item.AssetUUID, false);
                    Client.Self.Animate(animStop, true);
                    break;

                case "rez_inworld":
                    Instance.MediaManager.PlayUISound(UISounds.ObjectRez);
                    Vector3 rezpos = new Vector3(2, 0, 0);
                    rezpos = Client.Self.SimPosition + rezpos * Client.Self.Movement.BodyRotation;
                    Client.Inventory.RequestRezFromInventory(Client.Network.CurrentSim, Quaternion.Identity, rezpos, item);
                    break;
            }
        }

        void AddFolderContext(InventoryFolder folder)
        {
            ToolStripMenuItem ctxItem;

            ctxItem = new ToolStripMenuItem("New Folder", null, OnInvFolderContextClick);
            ctxItem.Name = "new_folder";
            ctxInv.Items.Add(ctxItem);

            ctxItem = new ToolStripMenuItem("New Note", null, OnInvFolderContextClick);
            ctxItem.Name = "new_notecard";
            ctxInv.Items.Add(ctxItem);

            ctxItem = new ToolStripMenuItem("New Script", null, OnInvFolderContextClick);
            ctxItem.Name = "new_script";
            ctxInv.Items.Add(ctxItem);

            ctxItem = new ToolStripMenuItem("Backup...", null, OnInvFolderContextClick);
            ctxItem.Name = "backup";
            ctxInv.Items.Add(ctxItem);

            ctxInv.Items.Add(new ToolStripSeparator());

            ctxItem = new ToolStripMenuItem("Expand", null, OnInvFolderContextClick);
            ctxItem.Name = "expand";
            ctxInv.Items.Add(ctxItem);

            ctxItem = new ToolStripMenuItem("Expand All", null, OnInvFolderContextClick);
            ctxItem.Name = "expand_all";
            ctxInv.Items.Add(ctxItem);

            ctxItem = new ToolStripMenuItem("Collapse", null, OnInvFolderContextClick);
            ctxItem.Name = "collapse";
            ctxInv.Items.Add(ctxItem);

            ctxItem = new ToolStripMenuItem("Collapse All", null, OnInvFolderContextClick);
            ctxItem.Name = "collapse_all";
            ctxInv.Items.Add(ctxItem);

            ctxInv.Items.Add(new ToolStripSeparator());

            if (folder.PreferredType == AssetType.TrashFolder)
            {
                ctxItem = new ToolStripMenuItem("Empty Trash", null, OnInvFolderContextClick);
                ctxItem.Name = "empty_trash";
                ctxInv.Items.Add(ctxItem);
            }

            if (folder.PreferredType == AssetType.LostAndFoundFolder)
            {
                ctxItem = new ToolStripMenuItem("Empty Lost and Found", null, OnInvFolderContextClick);
                ctxItem.Name = "empty_lost_found";
                ctxInv.Items.Add(ctxItem);
            }

            if (folder.PreferredType == AssetType.Unknown ||
                folder.PreferredType == AssetType.OutfitFolder)
            {
                ctxItem = new ToolStripMenuItem("Rename", null, OnInvFolderContextClick);
                ctxItem.Name = "rename_folder";
                ctxInv.Items.Add(ctxItem);

                ctxInv.Items.Add(new ToolStripSeparator());

                ctxItem = new ToolStripMenuItem("Cut", null, OnInvFolderContextClick);
                ctxItem.Name = "cut_folder";
                ctxInv.Items.Add(ctxItem);

                ctxItem = new ToolStripMenuItem("Copy", null, OnInvFolderContextClick);
                ctxItem.Name = "copy_folder";
                ctxInv.Items.Add(ctxItem);
            }

            if (Instance.InventoryClipboard != null)
            {
                ctxItem = new ToolStripMenuItem("Paste", null, OnInvFolderContextClick);
                ctxItem.Name = "paste_folder";
                ctxInv.Items.Add(ctxItem);

                if (Instance.InventoryClipboard.Item is InventoryItem)
                {
                    ctxItem = new ToolStripMenuItem("Paste as Link", null, OnInvFolderContextClick);
                    ctxItem.Name = "paste_folder_link";
                    ctxInv.Items.Add(ctxItem);
                }
            }

            if (folder.PreferredType == AssetType.Unknown ||
                folder.PreferredType == AssetType.OutfitFolder)
            {
                ctxItem = new ToolStripMenuItem("Delete", null, OnInvFolderContextClick);
                ctxItem.Name = "delete_folder";
                ctxInv.Items.Add(ctxItem);

                ctxInv.Items.Add(new ToolStripSeparator());
            }

            if (folder.PreferredType == AssetType.Unknown || folder.PreferredType == AssetType.OutfitFolder)
            {
                ctxItem = new ToolStripMenuItem("Take off Items", null, OnInvFolderContextClick);
                ctxItem.Name = "outfit_take_off";
                ctxInv.Items.Add(ctxItem);

                ctxItem = new ToolStripMenuItem("Add to Outfit", null, OnInvFolderContextClick);
                ctxItem.Name = "outfit_add";
                ctxInv.Items.Add(ctxItem);

                ctxItem = new ToolStripMenuItem("Replace Outfit", null, OnInvFolderContextClick);
                ctxItem.Name = "outfit_replace";
                ctxInv.Items.Add(ctxItem);
            }

            Instance.ContextActionManager.AddContributions(ctxInv, folder);
        }

        void SetExpandedRecursive(Node n, bool expanded)
        {
            n.SetExpanded(ID, expanded);
            foreach (var child in n.ONode.Nodes.Values)
            {
                if (child.Data is InventoryFolder)
                {
                    Node newNode;
                    if (child.Tag is Node)
                    {
                        newNode = (Node)child.Tag;
                    }
                    else
                    {
                        newNode = new Node()
                        {
                            ONode = child,
                            Level = n.Level + 1
                        };
                        child.Tag = newNode;
                    }
                    SetExpandedRecursive(newNode, expanded);
                }
            }
        }

        void OnInvFolderContextClick(object sender, EventArgs e)
        {
            string cmd = ((ToolStripMenuItem)sender).Name;
            InventoryFolder f = ((Node)Items[SelectedIndices[0]].Tag).ONode.Data as InventoryFolder;

            switch (cmd)
            {
                case "backup":
                    (new InventoryBackup(Instance, f.UUID)).Show();
                    break;

                case "expand":
                    ((Node)Items[SelectedIndices[0]].Tag).SetExpanded(ID, true);
                    UpdateMapper();
                    break;

                case "expand_all":
                    SetExpandedRecursive((Node)Items[SelectedIndices[0]].Tag, true);
                    UpdateMapper();
                    break;

                case "collapse":
                    ((Node)Items[SelectedIndices[0]].Tag).SetExpanded(ID, false);
                    UpdateMapper();
                    break;

                case "collapse_all":
                    SetExpandedRecursive((Node)Items[SelectedIndices[0]].Tag, false);
                    UpdateMapper();
                    break;

                case "new_folder":
                    newItemName = "New folder";
                    Client.Inventory.CreateFolder(f.UUID, "New folder");
                    break;

                case "new_notecard":
                    Client.Inventory.RequestCreateItem(f.UUID, "New Note", "Radegast note: " + DateTime.Now.ToString(),
                        AssetType.Notecard, UUID.Zero, InventoryType.Notecard, PermissionMask.All, NotecardCreated);
                    break;

                case "new_script":
                    Client.Inventory.RequestCreateItem(f.UUID, "New script", "Radegast script: " + DateTime.Now.ToString(),
                        AssetType.LSLText, UUID.Zero, InventoryType.LSL, PermissionMask.All, ScriptCreated);
                    break;

                case "cut_folder":
                    Instance.InventoryClipboard = new InventoryClipboard(ClipboardOperation.Cut, f);
                    break;

                case "copy_folder":
                    Instance.InventoryClipboard = new InventoryClipboard(ClipboardOperation.Copy, f);
                    break;

                case "paste_folder":
                    PerformClipboardOperation(f);
                    break;

                case "paste_folder_link":
                    PerformLinkOperation(f);
                    break;


                case "delete_folder":
                    Client.Inventory.MoveFolder(f.UUID, Client.Inventory.FindFolderForType(AssetType.TrashFolder), f.Name);
                    break;

                case "empty_trash":
                    {
                        DialogResult res = MessageBox.Show("Are you sure you want to empty your trash?", "Confirmation", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.OK)
                        {
                            Client.Inventory.EmptyTrash();
                        }
                    }
                    break;

                case "empty_lost_found":
                    {
                        DialogResult res = MessageBox.Show("Are you sure you want to empty your lost and found folder?", "Confirmation", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.OK)
                        {
                            Client.Inventory.EmptyLostAndFound();
                        }
                    }
                    break;

                case "rename_folder":
                    selection[0].BeginEdit();
                    break;

                case "outfit_replace":
                    List<InventoryItem> newOutfit = new List<InventoryItem>();
                    foreach (InventoryBase item in Client.Inventory.Store.GetContents(f))
                    {
                        if (item is InventoryItem)
                            newOutfit.Add((InventoryItem)item);
                    }
                    Instance.COF.ReplaceOutfit(newOutfit);
                    break;

                case "outfit_add":
                    List<InventoryItem> addToOutfit = new List<InventoryItem>();
                    foreach (InventoryBase item in Client.Inventory.Store.GetContents(f))
                    {
                        if (item is InventoryItem)
                            addToOutfit.Add((InventoryItem)item);
                    }
                    Instance.COF.AddToOutfit(addToOutfit, true);
                    break;

                case "outfit_take_off":
                    List<InventoryItem> removeFromOutfit = new List<InventoryItem>();
                    foreach (InventoryBase item in Client.Inventory.Store.GetContents(f))
                    {
                        if (item is InventoryItem)
                            removeFromOutfit.Add((InventoryItem)item);
                    }
                    Instance.COF.RemoveFromOutfit(removeFromOutfit);
                    break;
            }
        }

        void NotecardCreated(bool success, InventoryItem item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => NotecardCreated(success, item)));
                return;
            }

            if (!success)
            {
                Instance.TabConsole.DisplayNotificationInChat("Creation of notecard failed");
                return;
            }

            Instance.TabConsole.DisplayNotificationInChat("New notecard created, enter notecard name and press enter", ChatBufferTextStyle.Invisible);
            /* TODO
            var node = findNodeForItem(item.ParentUUID);
            if (node != null) node.Expand();
            node = findNodeForItem(item.UUID);
            if (node != null)
            {
                invTree.SelectedNode = node;
                node.BeginEdit();
            }
             */
        }

        void ScriptCreated(bool success, InventoryItem item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => ScriptCreated(success, item)));
                return;
            }

            if (!success)
            {
                Instance.TabConsole.DisplayNotificationInChat("Creation of script failed");
                return;
            }

            Instance.TabConsole.DisplayNotificationInChat("New script created, enter script name and press enter", ChatBufferTextStyle.Invisible);
            /* TODO
            var node = findNodeForItem(item.ParentUUID);
            if (node != null) node.Expand();
            node = findNodeForItem(item.UUID);
            if (node != null)
            {
                invTree.SelectedNode = node;
                node.BeginEdit();
            }
             */
        }

        void PerformClipboardOperation(InventoryFolder dest)
        {
            if (Instance.InventoryClipboard == null) return;

            if (dest == null) return;

            if (Instance.InventoryClipboard.Operation == ClipboardOperation.Cut)
            {
                if (Instance.InventoryClipboard.Item is InventoryItem)
                {
                    Client.Inventory.MoveItem(Instance.InventoryClipboard.Item.UUID, dest.UUID, Instance.InventoryClipboard.Item.Name);
                }
                else if (Instance.InventoryClipboard.Item is InventoryFolder)
                {
                    if (Instance.InventoryClipboard.Item.UUID != dest.UUID)
                    {
                        Client.Inventory.MoveFolder(Instance.InventoryClipboard.Item.UUID, dest.UUID, Instance.InventoryClipboard.Item.Name);
                    }
                }

                Instance.InventoryClipboard = null;
            }
            else if (Instance.InventoryClipboard.Operation == ClipboardOperation.Copy)
            {
                if (Instance.InventoryClipboard.Item is InventoryItem)
                {
                    Client.Inventory.RequestCopyItem(Instance.InventoryClipboard.Item.UUID, dest.UUID, Instance.InventoryClipboard.Item.Name, Instance.InventoryClipboard.Item.OwnerID, (InventoryBase target) =>
                    {
                    }
                    );
                }
                else if (Instance.InventoryClipboard.Item is InventoryFolder)
                {
                    WorkPool.QueueUserWorkItem((object state) =>
                    {
                        UUID newFolderID = Client.Inventory.CreateFolder(dest.UUID, Instance.InventoryClipboard.Item.Name, AssetType.Unknown);
                        System.Threading.Thread.Sleep(1000);

                        // FIXME: for some reason copying a bunch of items in one operation does not work

                        //List<UUID> items = new List<UUID>();
                        //List<UUID> folders = new List<UUID>();
                        //List<string> names = new List<string>();
                        //UUID oldOwner = UUID.Zero;

                        foreach (InventoryBase oldItem in Client.Inventory.Store.GetContents((InventoryFolder)Instance.InventoryClipboard.Item))
                        {
                            //folders.Add(newFolderID);
                            //names.Add(oldItem.Name);
                            //items.Add(oldItem.UUID);
                            //oldOwner = oldItem.OwnerID;
                            Client.Inventory.RequestCopyItem(oldItem.UUID, newFolderID, oldItem.Name, oldItem.OwnerID, (InventoryBase target) => { });
                        }

                        //if (folders.Count > 0)
                        //{
                        //    Client.Inventory.RequestCopyItems(items, folders, names, oldOwner, (InventoryBase target) => { });
                        //}
                    }
                    );
                }
            }
        }

        void PerformLinkOperation(InventoryFolder dest)
        {
            if (Instance.InventoryClipboard == null) return;

            if (dest == null) return;

            Client.Inventory.CreateLink(dest.UUID, Instance.InventoryClipboard.Item, (bool success, InventoryItem item) => { });
        }
        #endregion Context Menu

        protected new virtual int VirtualListSize
        {
            get
            {
                return base.VirtualListSize;
            }

            set
            {
                if (Instance.MonoRuntime)
                {
                    base.VirtualListSize = value;
                    return;
                }

                if (value == this.VirtualListSize || value < 0) return; // Get around the 'private' marker on 'virtualListSize' field using reflection
                if (virtualListSizeFieldInfo == null)
                {
                    virtualListSizeFieldInfo = typeof(ListView).GetField("virtualListSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    System.Diagnostics.Debug.Assert(virtualListSizeFieldInfo != null);
                } // Set the base class private field so that it keeps on working
                virtualListSizeFieldInfo.SetValue(this, value); // Send a raw message to change the virtual list size *without* changing the scroll position
                if (this.IsHandleCreated && !this.DesignMode)
                {
                    Win32.SendMessage(this.Handle, Win32.LVM_SETITEMCOUNT, value, (IntPtr)Win32.LVSICF_NOSCROLL);
                }
            }
        }

        static private System.Reflection.FieldInfo virtualListSizeFieldInfo;

        #region Sorter class
        // Create a node sorter that implements the IComparer interface.
        public class InvTreeSorter : IComparer<InventoryNode>
        {
            bool _sysfirst = true;
            bool _bydate = true;

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
            public bool ByDate { set { _bydate = value; } get { return _bydate; } }

            /*
            public int Compare(object x, object y)
            {
                InventoryNode tx = x as InventoryNode;
                InventoryNode ty = y as InventoryNode;
             */
            public int Compare(InventoryNode tx, InventoryNode ty)
            {
                if (tx.Data is InventoryFolder && ty.Data is InventoryFolder)
                {
                    return CompareFolders(tx.Data as InventoryFolder, ty.Data as InventoryFolder);
                }
                else if (tx.Data is InventoryFolder && ty.Data is InventoryItem)
                {
                    return -1;
                }
                else if (tx.Data is InventoryItem && ty.Data is InventoryFolder)
                {
                    return 1;
                }

                // Two items
                if (!(tx.Data is InventoryItem) || !(ty.Data is InventoryItem))
                {
                    return 0;
                }

                InventoryItem item1 = (InventoryItem)tx.Data;
                InventoryItem item2 = (InventoryItem)ty.Data;

                if (_bydate)
                {
                    if (item1.CreationDate < item2.CreationDate)
                    {
                        return 1;
                    }
                    else if (item1.CreationDate > item2.CreationDate)
                    {
                        return -1;
                    }
                }
                return string.Compare(item1.Name, item2.Name);
            }
        }
        #endregion
    }
}
