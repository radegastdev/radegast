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
    public partial class GroupIMTabWindow : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private UUID session;
        private IMTextManager textManager;
        private GridClient client;
        private AgentManager.GroupChatJoinedCallback grpJoinCallback;

        ManualResetEvent WaitForSessionStart = new ManualResetEvent(false);

        public GroupIMTabWindow(RadegastInstance instance, UUID session)
        {
            InitializeComponent();

            this.instance = instance;
            this.client = instance.Client;
            netcom = this.instance.Netcom;

            this.session = session;

            textManager = new IMTextManager(this.instance, new RichTextBoxPrinter(rtbIMText), this.session);
            this.Disposed += new EventHandler(IMTabWindow_Disposed);
            ApplyConfig(this.instance.Config.CurrentConfig);
            this.instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            client.Self.RequestJoinGroupChat(session);
            grpJoinCallback = new AgentManager.GroupChatJoinedCallback(Self_OnGroupChatJoin);
            client.Self.OnGroupChatJoin += grpJoinCallback;

        }

        void Self_OnGroupChatJoin(UUID groupChatSessionID, string sessionName, UUID tmpSessionID, bool success)
        {
            if (groupChatSessionID != session && tmpSessionID != session) {
                return;
            }

            if (InvokeRequired) {
                Invoke(new MethodInvoker(
                    delegate()
                    {
                        Self_OnGroupChatJoin(groupChatSessionID, sessionName, tmpSessionID, success);
                    }
                    )
                );
                return;
            }
            if (success) {
                textManager.TextPrinter.PrintTextLine("Join Group Chat Success!", Color.Green);
                WaitForSessionStart.Set();
            } else {
                textManager.TextPrinter.PrintTextLine("Join Group Chat failed.", Color.Red);
            }
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

        private void AddNetcomEvents()
        {
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
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

        private void IMTabWindow_Disposed(object sender, EventArgs e)
        {
            CleanUp();
        }

        public void CleanUp()
        {
            if (grpJoinCallback != null) {
                client.Self.OnGroupChatJoin -= grpJoinCallback;
            }
            textManager.CleanUp();
            textManager = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //netcom.SendInstantMessage(cbgInput.Text, target, session);
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
            if (message.Length > 1023) message = message.Remove(1023);

            if (!client.Self.GroupChatSessions.ContainsKey(session)) {
                WaitForSessionStart.Reset();
                client.Self.RequestJoinGroupChat(session);
            } else {
                WaitForSessionStart.Set();
            }

            if (WaitForSessionStart.WaitOne(10000, false)) {
                client.Self.InstantMessageGroup(session, message);
            } else {
                textManager.TextPrinter.PrintTextLine("Cannot send group IM.", Color.Red);
            }
            this.ClearIMInput();
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
