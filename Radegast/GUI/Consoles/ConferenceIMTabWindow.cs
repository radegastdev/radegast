using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RadegastNc;
using OpenMetaverse;

namespace Radegast
{
    public partial class ConferenceIMTabWindow : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private UUID session;
        private IMTextManager textManager;
        private GridClient client;
        private List<UUID> participants = new List<UUID>();
        ManualResetEvent WaitForSessionStart = new ManualResetEvent(false);

        public ConferenceIMTabWindow(RadegastInstance instance, UUID session)
        {
            InitializeComponent();
            Disposed += new EventHandler(IMTabWindow_Disposed);

            this.instance = instance;
            this.client = instance.Client;
            netcom = this.instance.Netcom;

            this.session = session;

            textManager = new IMTextManager(this.instance, new RichTextBoxPrinter(rtbIMText), this.session);
            
            ApplyConfig(this.instance.Config.CurrentConfig);

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            this.instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);

            this.client.Self.ChatterBoxAcceptInvite(session);
        }

        private void IMTabWindow_Disposed(object sender, EventArgs e)
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected -= new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            this.instance.Config.ConfigApplied -= new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            CleanUp();
        }

        private void Config_ConfigApplied(object sender, ConfigAppliedEventArgs e)
        {
            ApplyConfig(e.AppliedConfig);
        }

        private void ApplyConfig(Config config)
        {
            /*
            if (config.InterfaceStyle == 0) //System
                toolStrip1.RenderMode = ToolStripRenderMode.System;
            else if (config.InterfaceStyle == 1) //Office 2003
                toolStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
             */
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
            textManager.CleanUp();
            textManager = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage(cbgInput.Text);
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
                cbgInput.Enabled = false;
                btnSend.Enabled = false;
                return;
            }

        }

        private void cbxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (cbgInput.Text.Length == 0) return;

            string message = cbgInput.Text;

            SendMessage(message);

            this.ClearIMInput();
        }

        private void SendMessage(string msg)
        {
            string message = msg;
            if (message.Length > 1023)
            {
                message = message.Remove(1023);
            }
            if (message.Length > 0)
            {
                client.Self.InstantMessageGroup(session, message);
            }
        }

        private void ClearIMInput()
        {
            cbgInput.Items.Add(cbgInput.Text);
            cbgInput.Text = string.Empty;
        }

        public void SelectIMInput()
        {
            cbgInput.Select();
        }

        private void rtbIMText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (e.LinkText.StartsWith("http://") || e.LinkText.StartsWith("ftp://"))
                System.Diagnostics.Process.Start(e.LinkText);
            else
                System.Diagnostics.Process.Start("http://" + e.LinkText);
        }

        private void cbxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
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
