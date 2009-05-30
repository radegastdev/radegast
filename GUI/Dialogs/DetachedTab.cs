using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using RadegastNc;

namespace Radegast
{
    public partial class frmDetachedTab : Form
    {
        private RadegastInstance instance;
        private SleekTab tab;

        //For reattachment
        private ToolStrip strip;
        private Panel container;

        public frmDetachedTab(RadegastInstance instance, SleekTab tab)
        {
            InitializeComponent();

            this.instance = instance;
            this.tab = tab;
            this.Controls.Add(tab.Control);
            tab.Control.BringToFront();

            AddTabEvents();
            this.Text = tab.Label + " (tab) - SLeek";

            ApplyConfig(this.instance.Config.CurrentConfig);
            this.instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
        }

        private void Config_ConfigApplied(object sender, ConfigAppliedEventArgs e)
        {
            ApplyConfig(e.AppliedConfig);
        }

        private void ApplyConfig(Config config)
        {
            if (config.InterfaceStyle == 0) //System
                tstMain.RenderMode = ToolStripRenderMode.System;
            else if (config.InterfaceStyle == 1) //Office 2003
                tstMain.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        }

        private void AddTabEvents()
        {
            tab.TabPartiallyHighlighted += new EventHandler(tab_TabPartiallyHighlighted);
            tab.TabUnhighlighted += new EventHandler(tab_TabUnhighlighted);
        }

        private void tab_TabUnhighlighted(object sender, EventArgs e)
        {
            tlblTyping.Visible = false;
        }

        private void tab_TabPartiallyHighlighted(object sender, EventArgs e)
        {
            tlblTyping.Visible = true;
        }

        private void frmDetachedTab_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tab.Detached)
            {
                if (tab.AllowClose)
                    tab.Close();
                else
                    tab.AttachTo(strip, container);
            }
        }

        private void tbtnReattach_Click(object sender, EventArgs e)
        {
            tab.AttachTo(strip, container);
            this.Close();
        }

        public ToolStrip ReattachStrip
        {
            get { return strip; }
            set { strip = value; }
        }

        public Panel ReattachContainer
        {
            get { return container; }
            set { container = value; }
        }
    }
}