using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Threading;
using OpenMetaverse;

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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ListPlugins();
        }

        private void btnUnload_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvwPlugins.SelectedItems)
            {
                instance.PluginManager.UnloadPlugin((IRadegastPlugin)item.Tag);
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
                        if (dlg.FileNames[i].ToLower().EndsWith(@".dll"))
                        {
                            try
                            {
                                Assembly assembly = Assembly.LoadFile(dlg.FileNames[i]);
                                instance.PluginManager.LoadAssembly(dlg.FileNames[i], assembly, true);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log("Error loading " + dlg.FileNames[i], Helpers.LogLevel.Error, ex);
                            }
                        }
                        else
                        {
                            try
                            {
                                instance.PluginManager.LoadCSharpScript(File.ReadAllText(dlg.FileNames[i]));
                            }
                            catch (Exception ex)
                            {
                                Logger.Log("Error loading " + dlg.FileNames[i], Helpers.LogLevel.Error, ex);
                            }
                        }
                    }

                    ThreadPool.QueueUserWorkItem(sync =>
                        {
                            Thread.Sleep(2000);
                            if (IsHandleCreated)
                                Invoke(new MethodInvoker(() => ListPlugins()));
                        }
                    );
                }
            }
        }
    }
}
