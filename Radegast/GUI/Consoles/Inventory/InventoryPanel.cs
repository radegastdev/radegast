using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using OpenMetaverse;

namespace Radegast
{
    public partial class InventoryPanel : UserControl
    {
        RadegastInstance Instance;
        InventoryConsole Console;
        GridClient Client { get { return Instance.Client; } }

        public InventoryPanel()
        {
            InitializeComponent();
        }

        public List<InvTreeView> Trees;
        public InventoryPanel(RadegastInstance instance, InventoryConsole console)
        {
            InitializeComponent();

            this.Instance = instance;
            this.Console = console;

            Trees = new List<InvTreeView>(3);

            Trees.Add(new InvTreeViewAll(instance, console)
            {
                Dock = DockStyle.Fill
            });
            tabAll.Controls.Add(Trees[Trees.Count - 1]);

            Trees.Add(new InvTreeViewRecent(instance, console)
            {
                Dock = DockStyle.Fill
            });
            tabRecent.Controls.Add(Trees[Trees.Count - 1]);

            Trees.Add(new InvTreeViewWorn(instance, console)
            {
                Dock = DockStyle.Fill
            });
            tabWorn.Controls.Add(Trees[Trees.Count - 1]);

            Load += (sender, e) =>
            {
                Trees[0].Select();
            };

            foreach (var t in Trees)
            {
                t.MouseClick += new MouseEventHandler(t_MouseClick);
            }

            UpdateMapper();
        }

        void t_MouseClick(object sender, MouseEventArgs e)
        {
            InvTreeView t = (InvTreeView)sender;
        }

        public void UpdateMapper()
        {
            foreach (var t in Trees)
            {
                t.UpdateMapper();
            }
        }
    }
}
