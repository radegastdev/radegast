using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Radegast
{
    public partial class PrefGeneralConsole : UserControl, IPreferencePane
    {
        private RadegastInstance instance;
        private ConfigManager config;

        public PrefGeneralConsole(RadegastInstance instance)
        {
            InitializeComponent();

            this.instance = instance;
            config = this.instance.Config;

            if (config.CurrentConfig.InterfaceStyle == 0)
                rdoSystemStyle.Checked = true;
            else if (config.CurrentConfig.InterfaceStyle == 1)
                rdoOfficeStyle.Checked = true;
        }

        #region IPreferencePane Members

        string IPreferencePane.Name
        {
            get { return "General"; }
        }

        Image IPreferencePane.Icon
        {
            get { return Properties.Resources.applications_32; }
        }

        void IPreferencePane.SetPreferences()
        {
            if (rdoSystemStyle.Checked)
                config.CurrentConfig.InterfaceStyle = 0;
            else if (rdoOfficeStyle.Checked)
                config.CurrentConfig.InterfaceStyle = 1;
        }

        #endregion
    }
}
