using System;
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

            Trees.Add(new InvTreeView(instance)
            {
                InvType = InvTreeView.TreeType.All,
                Dock = DockStyle.Fill
            });
            tabAll.Controls.Add(Trees[Trees.Count - 1]);

            Trees.Add(new InvTreeView(instance)
            {
                InvType = InvTreeView.TreeType.Recent,
                Dock = DockStyle.Fill
            });
            tabRecent.Controls.Add(Trees[Trees.Count - 1]);

            Trees.Add(new InvTreeView(instance)
            {
                InvType = InvTreeView.TreeType.Worn,
                Dock = DockStyle.Fill
            });
            tabWorn.Controls.Add(Trees[Trees.Count - 1]);

            Load += (sender, e) =>
            {
                Trees[0].Select();
            };

            UpdateMapper();
        }

        public void UpdateMapper()
        {
            foreach (var t in Trees)
            {
                t.UpdateMapper();
            }
        }

        private void tabWorn_Click(object sender, EventArgs e)
        {

        }
    }
}
