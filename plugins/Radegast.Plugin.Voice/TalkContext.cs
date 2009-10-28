using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Radegast;

namespace Radegast.Plugin.Voice
{
    class TalkContext
    {
        private RadegastContextMenuStrip menuStrip;

        internal TalkContext()
        {
            RadegastContextMenuStrip.OnContentMenuItemClicked += OnContentMenuItemClicked;
        }

        internal void Start()
        {
            menuStrip = null;
        }

        internal void Stop()
        {
            menuStrip = null;
        }

        internal void SetObjectName(string name)
        {
        }

        private void OnContentMenuItemClicked(object sender, RadegastContextMenuStrip.ContextMenuEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item.Tag is Participant)
            {
                Participant p = item.Tag as Participant;
                switch (e.MenuItem.Text)
                {
                    case "Mute":
                        p.IsMuted = true;
                        break;
                    case "Unmute":
                        p.IsMuted = false;
                        break;
                    case "Volume":
                        // TODO Volume
                        break;
                }
            }
        }
    }
}