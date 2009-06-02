using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RadegastNc;
using OpenMetaverse;

namespace Radegast
{
    public partial class IMTabWindow : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private UUID target;
        private UUID session;
        private string toName;
        private IMTextManager textManager;
        private bool typing = false;

        public IMTabWindow(RadegastInstance instance, UUID target, UUID session, string toName)
        {
            InitializeComponent();
            Disposed += new EventHandler(IMTabWindow_Disposed);

            this.instance = instance;

            this.target = target;
            this.session = session;
            this.toName = toName;

            textManager = new IMTextManager(this.instance, new RichTextBoxPrinter(rtbIMText), this.session);
            ApplyConfig(this.instance.Config.CurrentConfig);
            this.instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
        }

        private void IMTabWindow_Disposed(object sender, EventArgs e)
        {
            this.instance.Config.ConfigApplied -= new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            CleanUp();
        }

        private void Config_ConfigApplied(object sender, ConfigAppliedEventArgs e)
        {
            ApplyConfig(e.AppliedConfig);
        }

        private void ApplyConfig(Config config)
        {
            if (config.InterfaceStyle == 0) //System
                toolStrip1.RenderMode = ToolStripRenderMode.System;
            else if (config.InterfaceStyle == 1) //Office 2003
                toolStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        }

        private void AddNetcomEvents()
        {
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
        }

        private void RemoveNetcomEvents()
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected -= new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            RefreshControls();
        }

        private void netcom_ClientDisconnected(object sender, ClientDisconnectEventArgs e)
        {
            RefreshControls();
        }

        public void CleanUp()
        {
            instance.TabConsole.RemoveTab(SessionId.ToString());
            textManager.CleanUp();
            textManager = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            netcom.SendInstantMessage(cbxInput.Text, target, session);
            this.ClearIMInput();
        }

        private void cbxInput_TextChanged(object sender, EventArgs e)
        {
            RefreshControls();
        }

        private void RefreshControls()
        {
            if (!netcom.IsLoggedIn)
            {
                cbxInput.Enabled = false;
                btnSend.Enabled = false;
                return;
            }

            if (cbxInput.Text.Length > 0)
            {
                btnSend.Enabled = true;

                if (!typing)
                {
                    netcom.SendIMStartTyping(target, session);
                    typing = true;
                }
            }
            else
            {
                btnSend.Enabled = false;
                netcom.SendIMStopTyping(target, session);
                typing = false;
            }
        }

        private void cbxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (cbxInput.Text.Length == 0) return;

            netcom.SendInstantMessage(cbxInput.Text, target, session);
            this.ClearIMInput();
        }

        private void ClearIMInput()
        {
            cbxInput.Items.Add(cbxInput.Text);
            cbxInput.Text = string.Empty;
        }

        public void SelectIMInput()
        {
            cbxInput.Select();
        }

        private void rtbIMText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (e.LinkText.StartsWith("http://") || e.LinkText.StartsWith("ftp://"))
                System.Diagnostics.Process.Start(e.LinkText);
            else
                System.Diagnostics.Process.Start("http://" + e.LinkText);
        }

        private void tbtnProfile_Click(object sender, EventArgs e)
        {
            (new frmProfile(instance, toName, target)).Show();
        }

        private void cbxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
        }

        public UUID TargetId
        {
            get { return target; }
            set { target = value; }
        }

        public string TargetName
        {
            get { return toName; }
            set { toName = value; }
        }

        public UUID SessionId
        {
            get { return session; }
            set { session = value; }
        }

        public IMTextManager TextManager
        {
            get { return textManager; }
            set { textManager = value; }
        }
    }
}
