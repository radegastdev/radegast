using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class PluginsTab : RadegastTabControl
    {
        public PluginsTab(RadegastInstance instance)
            : base(instance)
        {
            InitializeComponent();
            ListPlugins();
            PluginsTab_SizeChanged(this, EventArgs.Empty);

            GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void ListPlugins()
        {
            List<PluginInfo> plugins = instance.PluginManager.Plugins;
            lvwPlugins.Items.Clear();

            foreach (PluginInfo plugin in plugins)
            {
                ListViewItem item = new ListViewItem();
                item.Text = plugin.Attribures.Name;
                item.Tag = plugin;
                item.SubItems.Add(plugin.Attribures.Description);
                item.SubItems.Add(plugin.Attribures.Version);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ListPlugins();
        }

        private void btnUnload_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvwPlugins.SelectedItems)
            {
                var plugin = item.Tag as PluginInfo;
                if (plugin == null)
                {
                    Logger.Log($"ERROR Attempting to unload a null plugin: {item}", Helpers.LogLevel.Warning);
                    continue;
                }

                try
                {
                    instance.PluginManager.UnloadPlugin(plugin);
                }
                catch (Exception ex)
                {
                    instance.TabConsole.DisplayNotificationInChat($"ERROR unable to unload plugin: {plugin} because {ex}", ChatBufferTextStyle.Error);
                }
            }
            ListPlugins();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Load plugin file (source or compiled)";
                dlg.Filter = "Plugin source (*.cs)|*.cs|Plugin binary (*.dll)|*.dll|All files (*.*)|*.*";
                dlg.Multiselect = true;
                DialogResult res = dlg.ShowDialog();

                if (res == DialogResult.OK)
                {
                    for (int i = 0; i < dlg.FileNames.Length; i++)
                    {
                        try
                        {
                            instance.PluginManager.LoadPlugin(dlg.FileNames[i]);
                        }
                        catch (Exception ex)
                        {
                            instance.TabConsole.DisplayNotificationInChat($"ERROR unable to load plugin: {dlg.FileNames[i]} because {ex}", ChatBufferTextStyle.Error);
                        }
                    }

                    ListPlugins();
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvwPlugins.SelectedItems)
            {
                var plugin = item.Tag as PluginInfo;
                if (plugin == null)
                {
                    Logger.Log($"ERROR Attempting to reload a null plugin: {item}", Helpers.LogLevel.Warning);
                    continue;
                }

                try
                {
                    instance.PluginManager.UnloadPlugin(plugin);
                    instance.PluginManager.LoadPlugin(plugin.FileName);
                }
                catch (Exception ex)
                {
                    instance.TabConsole.DisplayNotificationInChat($"ERROR unable to reload plugin: {item} because {ex}", ChatBufferTextStyle.Error);
                }
                ListPlugins();
            }
        }
    }
}
