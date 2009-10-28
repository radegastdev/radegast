using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radegast.Plugin.Voice
{
    public partial class SettingsForm : Form
    {
        private VoiceControl control;
        public SettingsForm( VoiceControl pc)
        {
            control = pc;
            InitializeComponent();
        }

        internal void SetCStatus(string t)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                cStatus.Text = t;
            }));
        }

        internal void SetSStatus(string t)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                sStatus.Text = t;
            }));
        }

        private void micLevel_ValueChanged(object sender, EventArgs e)
        {
            control.vClient.MicLevel = micLevel.Value;
        }

        private void spkrLevel_ValueChanged(object sender, EventArgs e)
        {
            control.vClient.SpkrLevel = spkrLevel.Value;
        }

        internal void SetCaptureDevices(List<string> available, string current)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                foreach (string name in available)
                {
                    this.micDevice.Items.Add(name);
                }
                this.micDevice.SelectedItem = current;
            }));
        }

        internal void SetRenderDevices(List<string> available, string current)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                foreach (string name in available)
                {
                    this.spkrDevice.Items.Add(name);
                }
                this.spkrDevice.SelectedItem = current;
            }));

        }

        internal void SetMicDial(int level)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                this.sendlevel.Value = level;
            }));
        }
        internal void SetSpkrDial(int level)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                this.rcvLevel.Value = level;
            }));
        }

        private void micDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            control.vClient.SetMicDevice(micDevice.SelectedItem as string);
        }

        private void spkrDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            control.vClient.SetSpkrDevice(spkrDevice.SelectedItem as string);
        }

        private void spkrMute_CheckedChanged(object sender, EventArgs e)
        {
            control.vClient.SpkrMute = spkrMute.Checked;
        }

        private void micMute_CheckedChanged(object sender, EventArgs e)
        {
            control.vClient.MicMute = micMute.Checked;
        }

        internal void micMute_Set(bool on)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                micMute.Checked = on;
            }));
        }

        private void testMode_Click(object sender, EventArgs e)
        {
            talkMode.Checked = false;
            control.vClient.TestMode(true);
        }

        private void talkMode_Click(object sender, EventArgs e)
        {
            testMode.Checked = false;
            control.vClient.TestMode(false);
        }
    }
}
