using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Radegast;
using OpenMetaverse;

namespace DemoTab
{
    [Radegast.Plugin(Name = "Demo Tab", Description = "Demonstration of how to add a new tab via plugin", Version = "1.0")]
    public partial class DemoTab : RadegastTabControl, IRadegastPlugin
    {
        ToolStripMenuItem ActivateTabButton;

        public DemoTab()
        {
            InitializeComponent();
        }

        public void StartPlugin(RadegastInstance inst)
        {
            this.instance = inst;
            ActivateTabButton = new ToolStripMenuItem("Demo Tab", null, MenuButtonClicked);
            instance.MainForm.PluginsMenu.DropDownItems.Add(ActivateTabButton);
            instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
            RegisterClientEvents(client);
        }

        public void StopPlugin(RadegastInstance inst)
        {
            ActivateTabButton.Dispose();
            instance.TabConsole.RemoveTab("demo_tab");
            UnregisterClientEvents(client);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(e.Client);
        }


        void RegisterClientEvents(GridClient client)
        {
            client.Self.ChatFromSimulator += new EventHandler<ChatEventArgs>(Self_ChatFromSimulator);
        }

        void UnregisterClientEvents(GridClient client)
        {
            if (client == null) return;
            client.Self.ChatFromSimulator -= new EventHandler<ChatEventArgs>(Self_ChatFromSimulator);
        }

        void Self_ChatFromSimulator(object sender, ChatEventArgs e)
        {
            // Boilerplate, make sure to be on the GUI thread
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Self_ChatFromSimulator(sender, e)));
                return;
            }

            txtChat.Text = e.Message;
        }


        void MenuButtonClicked(object sender, EventArgs e)
        {
            if (instance.TabConsole.TabExists("demo_tab"))
            {
                instance.TabConsole.Tabs["demo_tab"].Select();
            }
            else
            {
                instance.TabConsole.AddTab("demo_tab", "Demo Tab", this);
                instance.TabConsole.Tabs["demo_tab"].Select();
            }
        }

        private void btnSaySomething_Click(object sender, EventArgs e)
        {
            client.Self.Chat("Something", 0, ChatType.Normal);
        }

    }
}
