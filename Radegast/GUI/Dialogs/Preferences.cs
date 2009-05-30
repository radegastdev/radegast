using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Radegast
{
    public partial class frmPreferences : Form
    {
        private RadegastInstance instance;
        private Dictionary<string, IPreferencePane> panes;
        private IPreferencePane selectedPane;

        public frmPreferences(RadegastInstance instance)
        {
            InitializeComponent();

            this.instance = instance;
            panes = new Dictionary<string, IPreferencePane>();
            
            AddPreferencePane(new PrefGeneralConsole(instance));
            AddPreferencePane(new PrefTextConsole(instance));
            lbxPanes.SelectedIndex = 0;
        }

        private void AddPreferencePane(IPreferencePane pane)
        {
            lbxPanes.Items.Add(new PreferencePaneListItem(pane.Name, pane.Icon));

            Control paneControl = (Control)pane;
            paneControl.Dock = DockStyle.Fill;
            paneControl.Visible = false;
            pnlPanes.Controls.Add(paneControl);

            panes.Add(pane.Name, pane);
        }

        private void SelectPreferencePane(string name)
        {
            IPreferencePane pane = panes[name];
            if (pane == selectedPane) return;
            
            Control paneControl = (Control)pane;
            Control selectedPaneControl = selectedPane as Control;

            paneControl.Visible = true;
            if (selectedPaneControl != null) selectedPaneControl.Visible = false;

            selectedPane = pane;
        }

        private void Apply()
        {
            foreach (KeyValuePair<string, IPreferencePane> kvp in panes)
                kvp.Value.SetPreferences();

            instance.Config.ApplyCurrentConfig();
        }

        private void lbxPanes_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index < 0) return;
            
            PreferencePaneListItem itemToDraw = (PreferencePaneListItem)lbxPanes.Items[e.Index];
            Brush textBrush = null;
            Font textFont = null;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                textBrush = new SolidBrush(Color.FromKnownColor(KnownColor.HighlightText));
                textFont = new Font(e.Font, FontStyle.Bold);
            }
            else
            {
                textBrush = new SolidBrush(Color.FromKnownColor(KnownColor.ControlText));
                textFont = new Font(e.Font, FontStyle.Regular);
            }
            
            SizeF stringSize = e.Graphics.MeasureString(itemToDraw.Name, textFont);
            float stringX = e.Bounds.Left + 4 + itemToDraw.Icon.Width;
            float stringY = e.Bounds.Top + 2 + ((itemToDraw.Icon.Height / 2) - (stringSize.Height / 2));

            e.Graphics.DrawImage(itemToDraw.Icon, e.Bounds.Left + 2, e.Bounds.Top + 2);
            e.Graphics.DrawString(itemToDraw.Name, textFont, textBrush, stringX, stringY);

            e.DrawFocusRectangle();

            textFont.Dispose();
            textBrush.Dispose();
            textFont = null;
            textBrush = null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Apply();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void lbxPanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxPanes.SelectedItem == null) return;
            PreferencePaneListItem item = (PreferencePaneListItem)lbxPanes.SelectedItem;

            SelectPreferencePane(item.Name);
        }
    }
}