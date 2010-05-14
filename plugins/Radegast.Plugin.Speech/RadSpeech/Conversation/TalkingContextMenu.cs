using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Radegast;

namespace RadegastSpeech.Conversation
{
    /// <summary>
    /// Talking version of a context menu.
    /// </summary>
    class TalkingContextMenu
    {
        private RadegastContextMenuStrip menuStrip;
        private PluginControl control;
        private string menuFor;
        private OpenMetaverse.UUID id;

        internal TalkingContextMenu(PluginControl pc)
        {
            control = pc;
            RadegastContextMenuStrip.OnContentMenuOpened += OnContentMenuOpened;
            RadegastContextMenuStrip.OnContentMenuItemSelected += OnContentMenuItemSelected;
            RadegastContextMenuStrip.OnContentMenuItemClicked += OnContentMenuItemClicked;
            RadegastContextMenuStrip.OnContentMenuClosing += OnContentMenuClosing;
        }

        internal void Start()
        {
            menuStrip = null;
        }

        internal void Stop()
        {
            menuStrip = null;
        }

        /// <summary>
        /// Select context menu by speaking it
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        internal bool Hear(string txt)
        {
            if (menuStrip==null) return false;

            if (txt == "cancel")
            {
                menuStrip.Close();
                return true;
            }

            foreach (var item in menuStrip.AllChoices())
            {
                if (item.Text == txt)
                {
                    if (item.Enabled)
                    {
                        control.talker.SayMore("Done.", Talk.BeepType.Good);
                        item.PerformClick();
                    }
                    else
                        control.talker.SayMore(txt + " is disabled.", Talk.BeepType.Bad);

                    return true;
                }
            }
           return false;
        }

        internal void SetObjectName(string name)
        {
        }

        private ToolStripMenuItem MakeSpeechAction()
        {
            return new ToolStripMenuItem("Speech...", null, new EventHandler(OnSpeechMenu));
        }

        /// <summary>
        /// Announce the opening of a context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContentMenuOpened(object sender, RadegastContextMenuStrip.ContextMenuEventArgs e)
        {
            lock (e.Menu)
            {
                menuStrip = e.Menu;
                menuFor = string.Empty;
                id = OpenMetaverse.UUID.Zero;

                // Figure out what this menu applies to.
                if (menuStrip.Selection is string)
                {
                    menuFor = (string)menuStrip.Selection;
                }
                else if (menuStrip.Selection is ListViewItem)
                {
                    ListViewItem lv = menuStrip.Selection as ListViewItem;
                    menuFor = lv.Text;
                }
                else if (menuStrip.Selection is OpenMetaverse.InventoryItem)
                {
                    // Something in Inventory.
                    menuFor = ((OpenMetaverse.InventoryItem)(menuStrip.Selection)).Name;
                }
                else if (menuStrip.Selection is OpenMetaverse.FriendInfo)
                {
                    // A Friend.
                    OpenMetaverse.FriendInfo f = menuStrip.Selection as OpenMetaverse.FriendInfo;
                    menuFor = f.Name;
                }
                else if (menuStrip.Selection is OpenMetaverse.Primitive)
                {
                    // Something in the Objects list.
                    OpenMetaverse.Primitive p = menuStrip.Selection as OpenMetaverse.Primitive;
                    if (p.Properties != null)
                        menuFor = p.Properties.Name;
                    else
                        menuFor = "Loading object";
                }

                // Remove parenthesized distance, etc
                int lParen = menuFor.IndexOf('(');
                if (lParen > 0)
                    menuFor = menuFor.Substring(0, lParen);

                // Stop reading old choices if moving fast.
                control.talker.Flush();
                control.talker.SayMore(menuFor + " menu.", Talk.BeepType.Open);
            }
        }

        /// <summary>
        /// Put up the voice assignment dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSpeechMenu( object sender, EventArgs e )
        {
            System.Windows.Forms.Form va =
                new RadegastSpeech.GUI.VoiceAssignment(control, menuFor, id);
            va.Show();
        }

        /// <summary>
        /// Announce closing of a context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContentMenuClosing(object sender, RadegastContextMenuStrip.ContextMenuEventArgs e)
        {
            lock (e.Menu)
            {
                control.talker.SayMore("Menu closed.", Talk.BeepType.Close);
                menuStrip = null;
            }
        }
        
        private void OnContentMenuItemSelected(object sender, RadegastContextMenuStrip.ContextMenuEventArgs e)
        {
            lock (e.Menu)
            {
                if (e.MenuItem == null) return;

                if (e.MenuItem.Enabled)
                {
                    control.talker.SayMore(e.MenuItem.Text); 
                }
                else
                {
                    control.talker.SayMore("Disabled " + e.MenuItem.Text);
                }
            }
        }

        private void OnContentMenuItemClicked(object sender, RadegastContextMenuStrip.ContextMenuEventArgs e)
        {
            if (!e.MenuItem.Enabled)
            {
                control.talker.SayMore(e.MenuItem.Text + " is disabled.", Talk.BeepType.Bad);
                return;
            }

            control.talker.Flush();
            control.talker.SayMore("Doing " + e.MenuItem.Text, Talk.BeepType.Good );
        }
    }
}
