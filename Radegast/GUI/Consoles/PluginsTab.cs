using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Radegast
{
    public partial class PluginsTab : RadegastTabControl
    {
        public PluginsTab(RadegastInstance instance)
            :base(instance)
        {
            InitializeComponent();
            ListPlugins();
            PluginsTab_SizeChanged(this, EventArgs.Empty);
        }

        void ListPlugins()
        {
            List<IRadegastPlugin> plugins = instance.PluginManager.Plugins;
            lvwPlugins.Items.Clear();

            foreach (IRadegastPlugin plugin in plugins)
            {
                PluginAttribute attr = PluginManager.GetAttributes(plugin);
                ListViewItem item = new ListViewItem();
                item.Text = attr.Name;
                item.Tag = plugin;
                item.SubItems.Add(attr.Description);
                item.SubItems.Add(attr.Version);
                lvwPlugins.Items.Add(item);
            }

        }

        private void PluginsTab_SizeChanged(object sender, EventArgs e)
        {
            float w = (float)(lvwPlugins.Width - 30);
            lvwPlugins.Columns[0].Width = (int)(w * 0.3);
            lvwPlugins.Columns[1].Width = (int)(w * 0.6);
            lvwPlugins.Columns[2].Width = (int)(w * 0.1);
        }
    }
}
