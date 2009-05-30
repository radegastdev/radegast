using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using RadegastNc;

namespace Radegast
{
    public partial class MainConsole : UserControl, ISleekTabControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;

        public MainConsole(RadegastInstance instance)
        {
            InitializeComponent();

            this.instance = instance;
            netcom = this.instance.Netcom;
            AddNetcomEvents();

            cbxLocation.SelectedIndex = 0;
            InitializeConfig();

            this.instance.MainForm.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            instance.Config.CurrentConfig.FirstName = txtFirstName.Text;
            instance.Config.CurrentConfig.LastName = txtLastName.Text;

            if (netcom.LoginOptions.IsPasswordMD5)
                instance.Config.CurrentConfig.PasswordMD5 = txtPassword.Text;
            else
                instance.Config.CurrentConfig.PasswordMD5 = Utils.MD5(txtPassword.Text);

            instance.Config.CurrentConfig.LoginLocationType = cbxLocation.SelectedIndex;
            instance.Config.CurrentConfig.LoginLocation = cbxLocation.Text;

            instance.Config.CurrentConfig.LoginGrid = cbxGrid.SelectedIndex;
            instance.Config.CurrentConfig.LoginUri = txtCustomLoginUri.Text;
        }

        private void AddNetcomEvents()
        {
            netcom.ClientLoggingIn += new EventHandler<OverrideEventArgs>(netcom_ClientLoggingIn);
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggingOut += new EventHandler<OverrideEventArgs>(netcom_ClientLoggingOut);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
        }

        private void InitializeConfig()
        {
            txtFirstName.Text = instance.Config.CurrentConfig.FirstName;
            txtLastName.Text = instance.Config.CurrentConfig.LastName;
            txtPassword.Text = instance.Config.CurrentConfig.PasswordMD5;
            netcom.LoginOptions.IsPasswordMD5 = true;

            cbxLocation.SelectedIndex = instance.Config.CurrentConfig.LoginLocationType;
            cbxLocation.Text = instance.Config.CurrentConfig.LoginLocation;

            cbxGrid.SelectedIndex = instance.Config.CurrentConfig.LoginGrid;
            txtCustomLoginUri.Text = instance.Config.CurrentConfig.LoginUri;
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            switch (e.Status)
            {
                case LoginStatus.ConnectingToLogin:
                    lblLoginStatus.Text = "Connecting to login server...";
                    lblLoginStatus.ForeColor = Color.Black;
                    break;

                case LoginStatus.ConnectingToSim:
                    lblLoginStatus.Text = "Connecting to region...";
                    lblLoginStatus.ForeColor = Color.Black;
                    break;

                case LoginStatus.Redirecting:
                    lblLoginStatus.Text = "Redirecting...";
                    lblLoginStatus.ForeColor = Color.Black;
                    break;

                case LoginStatus.ReadingResponse:
                    lblLoginStatus.Text = "Reading response...";
                    lblLoginStatus.ForeColor = Color.Black;
                    break;
                    
                case LoginStatus.Success:
                    lblLoginStatus.Text = "Logged in as " + netcom.LoginOptions.FullName;
                    lblLoginStatus.ForeColor = Color.FromArgb(0, 128, 128, 255);
                    proLogin.Visible = false;

                    btnLogin.Text = "Logout";
                    btnLogin.Enabled = true;
                    instance.Client.Groups.RequestCurrentGroups();
                    break;

                case LoginStatus.Failed:
                    lblLoginStatus.Text = e.Message;
                    lblLoginStatus.ForeColor = Color.Red;

                    proLogin.Visible = false;

                    btnLogin.Text = "Retry";
                    btnLogin.Enabled = true;
                    break;
            }
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            pnlLoginPrompt.Visible = true;
            pnlLoggingIn.Visible = false;

            btnLogin.Text = "Exit";
            btnLogin.Enabled = true;
        }

        private void netcom_ClientLoggingOut(object sender, OverrideEventArgs e)
        {
            btnLogin.Enabled = false;

            lblLoginStatus.Text = "Logging out...";
            lblLoginStatus.ForeColor = Color.FromKnownColor(KnownColor.ControlText);

            proLogin.Visible = true;
        }

        private void netcom_ClientLoggingIn(object sender, OverrideEventArgs e)
        {
            lblLoginStatus.Text = "Logging in...";
            lblLoginStatus.ForeColor = Color.FromKnownColor(KnownColor.ControlText);

            proLogin.Visible = true;
            pnlLoggingIn.Visible = true;
            pnlLoginPrompt.Visible = false;

            btnLogin.Enabled = false;
        }

        private void BeginLogin()
        {
            netcom.LoginOptions.FirstName = txtFirstName.Text;
            netcom.LoginOptions.LastName = txtLastName.Text;
            netcom.LoginOptions.Password = txtPassword.Text;
            netcom.LoginOptions.UserAgent = Properties.Resources.RadegastTitle;
            netcom.LoginOptions.Author = Properties.Resources.SleekAuthor;

            switch (cbxLocation.SelectedIndex)
            {
                case -1: //Custom
                    netcom.LoginOptions.StartLocation = StartLocationType.Custom;
                    netcom.LoginOptions.StartLocationCustom = txtCustomLoginUri.Text;
                    break;

                case 0: //Home
                    netcom.LoginOptions.StartLocation = StartLocationType.Home;
                    break;

                case 1: //Last
                    netcom.LoginOptions.StartLocation = StartLocationType.Last;
                    break;
            }

            switch (cbxGrid.SelectedIndex)
            {
                case 0: //Main grid
                    netcom.LoginOptions.Grid = LoginGrid.MainGrid;
                    break;

                case 1: //Beta grid
                    netcom.LoginOptions.Grid = LoginGrid.BetaGrid;
                    break;

                case 2: //Custom
                    if (txtCustomLoginUri.TextLength == 0 ||
                        txtCustomLoginUri.Text.Trim().Length == 0)
                    {
                        MessageBox.Show("You must specify the Login Uri to connect to a custom grid.", "SLeek", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    netcom.LoginOptions.Grid = LoginGrid.Custom;
                    netcom.LoginOptions.GridCustomLoginUri = txtCustomLoginUri.Text;
                    break;
            }

            netcom.Login();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            switch (btnLogin.Text)
            {
                case "Login": BeginLogin(); break;

                case "Retry":
                    pnlLoginPrompt.Visible = true;
                    pnlLoggingIn.Visible = false;
                    btnLogin.Text = "Login";
                    break;

                case "Logout": 
                    netcom.Logout();
                    break;

                case "Exit":
                    Application.Exit();
                    break;
            }
        }

        #region ISleekTabControl Members

        public void RegisterTab(SleekTab tab)
        {
            tab.DefaultControlButton = btnLogin;
        }

        #endregion

        private void cbxGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxGrid.SelectedIndex == 2) //Custom option is selected
            {
                txtCustomLoginUri.Enabled = true;
                txtCustomLoginUri.Select();
            }
            else
            {
                txtCustomLoginUri.Enabled = false;
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            netcom.LoginOptions.IsPasswordMD5 = false;
        }

    }
}
