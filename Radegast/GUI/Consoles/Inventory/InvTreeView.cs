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
            public Dictionary<int, NodeData> Data = new Dictionary<int, NodeData>(2);
        }

        public class Node
        {
            int Id;
            public Node(int id)
            {
                Id = id;
            }

            public InventoryNode ONode;
            public bool IsDir { get { return ONode.Data is InventoryFolder; } }
            public bool IsExpanded
            {
                get
                {
                    if (ONode.Tag is NodeMeta)
                    {
                        var meta = ONode.Tag as NodeMeta;
                        if (meta.Data.ContainsKey(Id))
                        {
                            return meta.Data[Id].IsExpanded;
                        }
                    }
                    return false;
                }
                set
                {
                    if (ONode.Tag is NodeMeta)
                    {
                        var meta = ONode.Tag as NodeMeta;
                        if (meta.Data.ContainsKey(Id))
                        {
                            meta.Data[Id].IsExpanded = value;
                        }
                        else
                        {
                            meta.Data[Id] = new NodeMeta.NodeData() { IsExpanded = value };
                        }
                    }
                    else
                    {
                        var meta = new NodeMeta();
                        meta.Data[Id] = new NodeMeta.NodeData() { IsExpanded = value };
                        ONode.Tag = meta;
                    }
                }
            }
            public int Level;
        }

        public int ID;
        public static InvTreeSorter Sorter;

        RadegastInstance Instance;
        GridClient Client { get { return Instance.Client; } }
        Inventory Inv { get { return Client.Inventory.Store; } }
        List<Node> Mapper = new List<Node>(64);
        int IconWidth = 16;
        static Image IcnPlus, IcnMinus;

        public InvTreeView(RadegastInstance instance)
            : base()
        {
            Instance = instance;

            FullRowSelect = false;
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

            UpdateMapper();
        }

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

        public void UpdateMapper()
        {
            BeginUpdate();
            Mapper.Clear();
            var inv = new Node(ID)
            {
                ONode = Inv.RootNode,
                Level = 0
            };
            var lib = new Node(ID)
            {
                ONode = Inv.LibraryRootNode,
                Level = 0
            };
            Mapper.Add(inv);
            PrepareNodes(Mapper.Count - 1, inv.IsExpanded);
            Mapper.Add(lib);
            PrepareNodes(Mapper.Count - 1, lib.IsExpanded);
            VirtualListSize = Mapper.Count;
            EndUpdate();
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
            }
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
        private void PopulateDescendantMembers(ref int pos, Node mbr)
        {
            List<InventoryNode> children = new List<InventoryNode>(mbr.ONode.Nodes.Values);
            children.Sort(Sorter);

            foreach (var m in children)
            {
                Node newNode = new Node(ID)
                {
                    ONode = m,
                    Level = mbr.Level + 1,
                };

                Mapper.Insert(pos++, newNode);
                if (newNode.IsExpanded)
                {
                    PopulateDescendantMembers(ref pos, newNode);
                }
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
                    int xfrom = item.IndentCount * IconWidth;
                    int xto = xfrom + IconWidth + 4;
                    Node n = (Node)item.Tag;
                    if (e.X >= xfrom && e.X < xto)
                    {
                        n.IsExpanded = !n.IsExpanded;
                        item.Checked = !item.Checked;
                        PrepareNodes(item.Index, n.IsExpanded);
                        return;
                    }
                }
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
            ListViewItem item = new ListViewItem(n.ONode.Data.Name);
            var ix = InventoryConsole.GetIconIndex(n.ONode.Data);
            //item.StateImageIndex = ix;
            item.IndentCount = n.Level;
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
            int iconIx = InventoryConsole.GetIconIndex(node.ONode.Data);
            int offset = (IconWidth + 2) * (node.Level + 2);
            Rectangle rec = new Rectangle(e.Bounds.X + offset, e.Bounds.Y, e.Bounds.Width - offset, e.Bounds.Height);

            try
            {
                icon = frmMain.ResourceImages.Images[iconIx];
                g.DrawImageUnscaled(icon, e.Bounds.X + offset - IconWidth, e.Bounds.Y);
                if (node.IsDir && node.ONode.Nodes.Count > 0)
                {
                    Image exp;
                    if (node.IsExpanded)
                    {
                        exp = IcnMinus;
                    }
                    else
                    {
                        exp = IcnPlus;
                    }
                    g.DrawImageUnscaled(exp, e.Bounds.X + offset - (IconWidth + 2) * 2, e.Bounds.Y);
                }

            }
            catch { }

            using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit))
            {
                string label = node.ONode.Data.Name; // ItemLabel(res.Inv, false);
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
