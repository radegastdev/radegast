/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using Radegast.Netcom;

namespace Radegast
{
    public partial class LoginConsole : UserControl, IRadegastTabControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom => instance.Netcom;

        public LoginConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(MainConsole_Disposed);

            this.instance = instance;
            AddNetcomEvents();

            pnlSplash.BackgroundImage = instance.GlobalSettings["hide_login_graphics"].AsBoolean() 
                ? null : Properties.Resources.radegast_main_screen2;

            if (!instance.GlobalSettings.ContainsKey("remember_login"))
            {
                instance.GlobalSettings["remember_login"] = true;
            }

            instance.GlobalSettings.OnSettingChanged += new Settings.SettingChangedCallback(GlobalSettings_OnSettingChanged);

            lblVersion.Text = Properties.Resources.RadegastTitle + " " + Assembly.GetExecutingAssembly().GetName().Version;

            Load += new EventHandler(LoginConsole_Load);

            GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        private void MainConsole_Disposed(object sender, EventArgs e)
        {
            instance.GlobalSettings.OnSettingChanged -= new Settings.SettingChangedCallback(GlobalSettings_OnSettingChanged);
            RemoveNetcomEvents();
        }

        void LoginConsole_Load(object sender, EventArgs e)
        {
            if (!instance.GlobalSettings["theme_compatibility_mode"] && instance.PlainColors)
            {
                panel1.BackColor = Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(225)))));
            }

            cbxLocation.SelectedIndex = 0;
            cbxUsername.SelectedIndexChanged += cbxUsername_SelectedIndexChanged;
            InitializeConfig();
        }

        private void AddNetcomEvents()
        {
            netcom.ClientLoggingIn += new EventHandler<OverrideEventArgs>(netcom_ClientLoggingIn);
            netcom.ClientLoginStatus += new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggingOut += new EventHandler<OverrideEventArgs>(netcom_ClientLoggingOut);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
        }

        private void RemoveNetcomEvents()
        {
            netcom.ClientLoggingIn -= new EventHandler<OverrideEventArgs>(netcom_ClientLoggingIn);
            netcom.ClientLoginStatus -= new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggingOut -= new EventHandler<OverrideEventArgs>(netcom_ClientLoggingOut);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
        }

        void GlobalSettings_OnSettingChanged(object sender, SettingsEventArgs e)
        {
            if (e.Key == "hide_login_graphics")
            {
                if (e.Value.AsBoolean())
                    pnlSplash.BackgroundImage = null;
                else
                    pnlSplash.BackgroundImage = Properties.Resources.radegast_main_screen2;
            }
        }

        private void SaveConfig()
        {
            Settings s = instance.GlobalSettings;
            SavedLogin sl = new SavedLogin();

            string username = cbxUsername.Text;

            if (cbxUsername.SelectedIndex > 0 && cbxUsername.SelectedItem is SavedLogin login)
            {
                username = login.Username;
            }

            if (cbxGrid.SelectedIndex == cbxGrid.Items.Count - 1) // custom login uri
            {
                sl.GridID = "custom_login_uri";
                sl.CustomURI = txtCustomLoginUri.Text;
            }
            else
            {
                sl.GridID = (cbxGrid.SelectedItem as Grid).ID;
                sl.CustomURI = string.Empty;
            }

            string savedLoginsKey = $"{username}%{sl.GridID}";

            if (!(s["saved_logins"] is OSDMap))
            {
                s["saved_logins"] = new OSDMap();
            }

            if (cbRemember.Checked)
            {

                sl.Username = s["username"] = username;

                if (LoginOptions.IsPasswordMD5(txtPassword.Text))
                {
                    sl.Password = txtPassword.Text;
                    s["password"] = txtPassword.Text;
                }
                else
                {
                    if (txtPassword.Text.Length > 16)
                    {
                        sl.Password = Utils.MD5(txtPassword.Text.Substring(0, 16));
                        s["password"] = Utils.MD5(txtPassword.Text.Substring(0, 16));
                    }
                    else
                    {
                        sl.Password = Utils.MD5(txtPassword.Text);
                        s["password"] = Utils.MD5(txtPassword.Text);
                    }
                }
                sl.CustomStartLocation = cbxLocation.SelectedIndex == -1 
                    ? cbxLocation.Text : string.Empty;
                sl.StartLocationType = cbxLocation.SelectedIndex;
                ((OSDMap)s["saved_logins"])[savedLoginsKey] = sl.ToOSD();
            }
            else if (((OSDMap)s["saved_logins"]).ContainsKey(savedLoginsKey))
            {
                ((OSDMap)s["saved_logins"]).Remove(savedLoginsKey);
            }

            s["login_location_type"] = OSD.FromInteger(cbxLocation.SelectedIndex);
            s["login_location"] = OSD.FromString(cbxLocation.Text);

            s["login_grid"] = OSD.FromInteger(cbxGrid.SelectedIndex);
            s["login_uri"] = OSD.FromString(txtCustomLoginUri.Text);
            s["remember_login"] = cbRemember.Checked;
        }

        private void ClearConfig()
        {
            Settings s = instance.GlobalSettings;
            s["username"] = string.Empty;
            s["password"] = string.Empty;
        }

        private void InitializeConfig()
        {
            // Initilize grid dropdown
            int gridIx = -1;

            cbxGrid.Items.Clear();
            for (int i = 0; i < instance.GridManger.Count; i++)
            {
                cbxGrid.Items.Add(instance.GridManger[i]);
                if (MainProgram.s_CommandLineOpts.Grid == instance.GridManger[i].ID)
                    gridIx = i;
            }
            cbxGrid.Items.Add("Custom");

            if (gridIx != -1)
            {
                cbxGrid.SelectedIndex = gridIx;
            }


            Settings s = instance.GlobalSettings;
            cbRemember.Checked = s["remember_login"];

            // Setup login name
            string savedUsername;

            if (string.IsNullOrEmpty(MainProgram.s_CommandLineOpts.Username))
            {
                savedUsername = s["username"];
            }
            else
            {
                savedUsername = MainProgram.s_CommandLineOpts.Username;
            }

            cbxUsername.Items.Add(savedUsername);

            try
            {
                if (s["saved_logins"] is OSDMap)
                {
                    OSDMap savedLogins = (OSDMap)s["saved_logins"];
                    foreach (string loginKey in savedLogins.Keys)
                    {
                        SavedLogin sl = SavedLogin.FromOSD(savedLogins[loginKey]);
                        cbxUsername.Items.Add(sl);
                    }
                }
            }
            catch
            {
                cbxUsername.Items.Clear();
                cbxUsername.Text = string.Empty;
            }

            cbxUsername.SelectedIndex = 0;

            // Fill in saved password or use one specified on the command line
            txtPassword.Text = string.IsNullOrEmpty(MainProgram.s_CommandLineOpts.Password) 
                ? s["password"].AsString() : MainProgram.s_CommandLineOpts.Password;


            // Setup login location either from the last used or
            // override from the command line
            if (string.IsNullOrEmpty(MainProgram.s_CommandLineOpts.Location))
            {
                // Use last location as default
                if (s["login_location_type"].Type == OSDType.Unknown)
                {
                    cbxLocation.SelectedIndex = 1;
                    s["login_location_type"] = OSD.FromInteger(1);
                }
                else
                {
                    cbxLocation.SelectedIndex = s["login_location_type"].AsInteger();
                    cbxLocation.Text = s["login_location"].AsString();
                }
            }
            else
            {
                switch (MainProgram.s_CommandLineOpts.Location)
                {
                    case "home":
                        cbxLocation.SelectedIndex = 0;
                        break;

                    case "last":
                        cbxLocation.SelectedIndex = 1;
                        break;

                    default:
                        cbxLocation.SelectedIndex = -1;
                        cbxLocation.Text = MainProgram.s_CommandLineOpts.Location;
                        break;
                }
            }


            // Set grid dropdown to last used, or override from command line
            if (string.IsNullOrEmpty(MainProgram.s_CommandLineOpts.Grid))
            {
                cbxGrid.SelectedIndex = s["login_grid"].AsInteger();
            }
            else if (gridIx == -1) // --grid specified but not found
            {
                MessageBox.Show($"Grid specified with --grid {MainProgram.s_CommandLineOpts.Grid} not found",
                    "Grid not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
            }

            // Restore login uri from settings, or command line
            if (string.IsNullOrEmpty(MainProgram.s_CommandLineOpts.LoginUri))
            {
                txtCustomLoginUri.Text = s["login_uri"].AsString();
            }
            else
            {
                txtCustomLoginUri.Text = MainProgram.s_CommandLineOpts.LoginUri;
                cbxGrid.SelectedIndex = cbxGrid.Items.Count - 1;
            }

            // Start logging in if autologin enabled from command line
            if (MainProgram.s_CommandLineOpts.AutoLogin)
            {
                BeginLogin();
            }
        }

        private void netcom_ClientLoginStatus(object sender, LoginProgressEventArgs e)
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
                    lblLoginStatus.ForeColor = Color.Red;
                    if (e.FailReason == "tos")
                    {
                        lblLoginStatus.Text = "Must agree to Terms of Service before logging in";
                        pnlTos.Visible = true;
                        txtTOS.Text = e.Message.Replace("\n", "\r\n");
                        btnLogin.Enabled = false;
                    }
                    else
                    {
                        lblLoginStatus.Text = e.Message;
                        btnLogin.Enabled = true;
                    }
                    proLogin.Visible = false;

                    btnLogin.Text = "Retry";
                    break;
                case LoginStatus.None:
                default:
                    throw new ArgumentOutOfRangeException();
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
            string username = cbxUsername.Text;

            if (cbxUsername.SelectedIndex > 0 && cbxUsername.SelectedItem is SavedLogin login)
            {
                username = login.Username;
            }

            string[] parts = System.Text.RegularExpressions.Regex.Split(username.Trim(), @"[. ]+");

            if (parts.Length == 2)
            {
                netcom.LoginOptions.FirstName = parts[0];
                netcom.LoginOptions.LastName = parts[1];
            }
            else
            {
                netcom.LoginOptions.FirstName = username.Trim();
                netcom.LoginOptions.LastName = "Resident";
            }

            netcom.LoginOptions.Password = txtPassword.Text;
            netcom.LoginOptions.Channel = Properties.Resources.ProgramName; // Channel
            netcom.LoginOptions.Version = Properties.Resources.RadegastTitle; // Version
            netcom.AgreeToTos = cbTOS.Checked;

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

            if (cbxGrid.SelectedIndex == cbxGrid.Items.Count - 1) // custom login uri
            {
                if (txtCustomLoginUri.TextLength == 0 || txtCustomLoginUri.Text.Trim().Length == 0)
                {
                    MessageBox.Show("You must specify the Login Uri to connect to a custom grid.", Properties.Resources.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                netcom.LoginOptions.Grid = new Grid("custom", "Custom", txtCustomLoginUri.Text);
                netcom.LoginOptions.GridCustomLoginUri = txtCustomLoginUri.Text;
            }
            else
            {
                netcom.LoginOptions.Grid = cbxGrid.SelectedItem as Grid;
            }

            if (netcom.LoginOptions.Grid.Platform != "SecondLife")
            {
                instance.Client.Settings.MULTIPLE_SIMS = true;
                instance.Client.Settings.HTTP_INVENTORY = !instance.GlobalSettings["disable_http_inventory"];
            }
            else
            {
                // UDP inventory is deprecated as of 2015-03-30 and no longer supported.
                // https://community.secondlife.com/t5/Second-Life-Server/Deploy-for-the-week-of-2015-03-30/td-p/2919194
                instance.Client.Settings.HTTP_INVENTORY = true;
            }

            netcom.Login();
            SaveConfig();
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
            }
        }

        #region IRadegastTabControl Members

        public void RegisterTab(RadegastTab tab)
        {
            tab.DefaultControlButton = btnLogin;
        }

        #endregion

        private void cbxGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxGrid.SelectedIndex == cbxGrid.Items.Count - 1) //Custom option is selected
            {
                txtCustomLoginUri.Enabled = true;
                txtCustomLoginUri.Select();
            }
            else
            {
                txtCustomLoginUri.Enabled = false;
            }
        }

        private void cbTOS_CheckedChanged(object sender, EventArgs e)
        {
            btnLogin.Enabled = cbTOS.Checked;
        }

        private void cbRemember_CheckedChanged(object sender, EventArgs e)
        {
            instance.GlobalSettings["remember_login"] = cbRemember.Checked;
            if (!cbRemember.Checked)
            {
                ClearConfig();
            }
        }

        private void cbxUsername_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxUsername.SelectedIndexChanged -= cbxUsername_SelectedIndexChanged;

            if (cbxUsername.SelectedIndex > 0
                && cbxUsername.SelectedItem is SavedLogin)
            {
                SavedLogin sl = (SavedLogin)cbxUsername.SelectedItem;
                cbxUsername.Text = sl.Username;
                cbxUsername.Items[0] = sl.Username;
                cbxUsername.SelectedIndex = 0;
                txtPassword.Text = sl.Password;
                cbxLocation.SelectedIndex = sl.StartLocationType;
                if (sl.StartLocationType == -1)
                {
                    cbxLocation.Text = sl.CustomStartLocation;
                }
                if (sl.GridID == "custom_login_uri")
                {
                    cbxGrid.SelectedIndex = cbxGrid.Items.Count - 1;
                    txtCustomLoginUri.Text = sl.CustomURI;
                }
                else
                {
                    foreach (var item in cbxGrid.Items)
                    {
                        if (item is Grid grid && grid.ID == sl.GridID)
                        {
                            cbxGrid.SelectedItem = grid;
                            break;
                        }
                    }
                }
            }

            cbxUsername.SelectedIndexChanged += cbxUsername_SelectedIndexChanged;
        }
    }

    public class SavedLogin
    {
        public string Username;
        public string Password;
        public string GridID;
        public string CustomURI;
        public int StartLocationType;
        public string CustomStartLocation;

        public OSDMap ToOSD()
        {
            OSDMap ret = new OSDMap(4)
            {
                ["username"] = Username,
                ["password"] = Password,
                ["grid"] = GridID,
                ["custom_url"] = CustomURI,
                ["location_type"] = StartLocationType,
                ["custom_location"] = CustomStartLocation
            };
            return ret;
        }

        public static SavedLogin FromOSD(OSD data)
        {
            if (!(data is OSDMap)) return null;
            OSDMap map = (OSDMap)data;
            SavedLogin ret = new SavedLogin
            {
                Username = map["username"],
                Password = map["password"],
                GridID = map["grid"],
                CustomURI = map["custom_url"]
            };
            if (map.ContainsKey("location_type"))
            {
                ret.StartLocationType = map["location_type"];
            }
            else
            {
                ret.StartLocationType = 1;
            }
            ret.CustomStartLocation = map["custom_location"];
            return ret;
        }

        public override string ToString()
        {
            RadegastInstance instance = RadegastInstance.GlobalInstance;
            string gridName;
            if (GridID == "custom_login_uri")
            {
                gridName = "Custom Login URI";
            }
            else if (instance.GridManger.KeyExists(GridID))
            {
                gridName = instance.GridManger[GridID].Name;
            }
            else
            {
                gridName = GridID;
            }
            return $"{Username} -- {gridName}";
        }
    }
}
