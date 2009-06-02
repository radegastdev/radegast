using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class GroupsDialog : Form
    {
        GridClient client;
        RadegastInstance instance;
        Dictionary<UUID, Group> groups = new Dictionary<UUID, Group>();

        public GroupsDialog(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(GroupsDialog_Disposed);
            this.client = instance.Client;
            this.instance = instance;

            // Callbacks
            client.Groups.OnCurrentGroups += new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);

            client.Groups.RequestCurrentGroups();

        }

        void GroupsDialog_Disposed(object sender, EventArgs e)
        {
            client.Groups.OnCurrentGroups -= new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
        }

        public void UpdateDisplay()
        {
            grpsCombo.Items.Clear();
            grpsCombo.Text = "";
            int i = 0;
            foreach (Group g in groups.Values) {
                grpsCombo.Items.Add(g.Name);
                if (g.ID == client.Self.ActiveGroup) {
                    grpsCombo.SelectedIndex = i;
                }
                i++;
            }
        }

        void Groups_OnCurrentGroups(Dictionary<UUID, Group> igroups)
        {
            groups = igroups;
            this.Invoke(new MethodInvoker(UpdateDisplay));
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void activateBtn_Click(object sender, EventArgs e)
        {
            if (groups == null) {
                return;
            }

            int i = 0;

            foreach (Group g in groups.Values) {
                if (grpsCombo.Text == g.Name) {
                    client.Groups.ActivateGroup(g.ID);
                    break;
                }
                i++;
            }
        }

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            if (groups == null) {
                return;
            }

            int i = 0;

            foreach (Group g in groups.Values) {
                if (grpsCombo.Text == g.Name) {
                    client.Groups.LeaveGroup(g.ID);
                    groups.Remove(g.ID);
                    UpdateDisplay();
                    break;
                }
                i++;
            }
        }

        private void imBtn_Click(object sender, EventArgs e)
        {
            if (groups == null) {
                return;
            }

            int i = 0;

            foreach (Group g in groups.Values) {
                if (grpsCombo.Text == g.Name) {
                    if (!instance.TabConsole.TabExists(g.ID.ToString()))
                    {
                        instance.TabConsole.AddGroupIMTab(g.ID, g.Name);
                        instance.TabConsole.tabs[g.ID.ToString()].Highlight();
                        instance.TabConsole.tabs[g.ID.ToString()].Select();

                    } else {
                        SleekTab t = instance.TabConsole.tabs[g.ID.ToString()];
                        if (!t.Selected) {
                            t.Highlight();
                        }
                    }
                    break;
                }
                i++;
            }

        }
    }
}