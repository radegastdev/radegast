// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id$
//
using System;
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using Radegast.Netcom;

namespace Radegast
{
    public partial class MainConsole : UserControl, ISleekTabControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom { get { return instance.Netcom; } }

        public MainConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(MainConsole_Disposed);

            this.instance = instance;
            AddNetcomEvents();

            cbxLocation.SelectedIndex = 0;
            InitializeConfig();
        }

        void MainConsole_Disposed(object sender, EventArgs e)
        {
            RemoveNetcomEvents();
        }


        private void AddNetcomEvents()
        {
            netcom.ClientLoggingIn += new EventHandler<OverrideEventArgs>(netcom_ClientLoggingIn);
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggingOut += new EventHandler<OverrideEventArgs>(netcom_ClientLoggingOut);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
        }

        private void RemoveNetcomEvents()
        {
            netcom.ClientLoggingIn -= new EventHandler<OverrideEventArgs>(netcom_ClientLoggingIn);
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggingOut -= new EventHandler<OverrideEventArgs>(netcom_ClientLoggingOut);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
        }

        private void SaveConfig()
        {
            Settings s = instance.GlobalSettings;

            s["first_name"] = OSD.FromString(txtFirstName.Text);
            s["last_name"] = OSD.FromString(txtLastName.Text);

            if (netcom.LoginOptions.IsPasswordMD5)
                s["password"] = OSD.FromString(txtPassword.Text);
            else
                s["password"] = OSD.FromString(Utils.MD5(txtPassword.Text));

            s["login_location_type"] = OSD.FromInteger(cbxLocation.SelectedIndex);
            s["login_location"] = OSD.FromString(cbxLocation.Text);

            s["login_grid"] = OSD.FromInteger(cbxGrid.SelectedIndex);
            s["login_uri"] = OSD.FromString(txtCustomLoginUri.Text);
        }

        private void InitializeConfig()
        {
            Settings s = instance.GlobalSettings;

            txtFirstName.Text = s["first_name"].AsString();
            txtLastName.Text = s["last_name"].AsString();
            txtPassword.Text = s["password"].AsString();
            netcom.LoginOptions.IsPasswordMD5 = true;

            cbxLocation.SelectedIndex = s["login_location_type"].AsInteger();
            cbxLocation.Text = s["login_location"].AsString();

            cbxGrid.SelectedIndex = s["login_grid"].AsInteger();
            txtCustomLoginUri.Text = s["login_uri"].AsString();
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
            netcom.LoginOptions.Author = Properties.Resources.RadegastAuthor;

            switch (cbxLocation.SelectedIndex)
            {
                case -1: //Custom
                    netcom.LoginOptions.StartLocation = StartLocationType.Custom;
                    netcom.LoginOptions.StartLocationCustom = cbxLocation.Text;
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
            SaveConfig();
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
