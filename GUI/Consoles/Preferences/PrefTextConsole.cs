using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Radegast
{
    public partial class PrefTextConsole : UserControl, IPreferencePane
    {
        private RadegastInstance instance;
        private ConfigManager config;

        public PrefTextConsole(RadegastInstance instance)
        {
            InitializeComponent();

            this.instance = instance;
            config = this.instance.Config;

            chkChatTimestamps.Checked = config.CurrentConfig.ChatTimestamps;
            chkIMTimestamps.Checked = config.CurrentConfig.IMTimestamps;
        }

        #region IPreferencePane Members

        string IPreferencePane.Name
        {
            get { return "Text"; }
        }

        Image IPreferencePane.Icon
        {
            get { return Properties.Resources.documents_32; }
        }

        void IPreferencePane.SetPreferences()
        {
            config.CurrentConfig.ChatTimestamps = chkChatTimestamps.Checked;
            config.CurrentConfig.IMTimestamps = chkIMTimestamps.Checked;
        }

        #endregion
    }
}
