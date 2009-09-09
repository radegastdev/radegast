using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using Radegast.Media;

namespace Radegast
{
    public partial class MediaConsole : DettachableControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private Settings s;
        private float audioVolume
        {
            get
            {
                return volAudioStream.Value / 50f;
            }
            set
            {
                if (value >= 0f && value < 1f)
                {
                    volAudioStream.Value = (int)(50f * value);
                }
            }
        }
        private System.Threading.Timer configTimer;
        private const int saveConfigTimeout = 3000;
        private bool playing;
        private string currentURL;
        private MediaManager mngr;

        public MediaConsole(RadegastInstance instance)
        {
            InitializeComponent();
            DisposeOnDetachedClose = false;
            Text = "Media";

            Disposed += new EventHandler(MediaConsole_Disposed);

            this.instance = instance;
            this.mngr = instance.MediaManager;

            s = instance.GlobalSettings;

            // Restore settings
            if (s["parcel_audio_url"].Type != OSDType.Unknown)
                txtAudioURL.Text = s["parcel_audio_url"].AsString();
            if (s["parcel_audio_vol"].Type != OSDType.Unknown)
                audioVolume = (float)s["parcel_audio_vol"].AsReal();
            if (s["parcel_audio_play"].Type != OSDType.Unknown)
                cbPlayAudioStream.Checked = s["parcel_audio_play"].AsBoolean();
            if (s["parcel_audio_keep_url"].Type != OSDType.Unknown)
                cbKeep.Checked = s["parcel_audio_keep_url"].AsBoolean();

            configTimer = new System.Threading.Timer(SaveConfig, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            if (!instance.MediaManager.SoundSystemAvailable)
            {
                foreach (Control c in pnlParcelAudio.Controls)
                    c.Enabled = false;
            }

            // GUI Events
            volAudioStream.Scroll += new EventHandler(volAudioStream_Scroll);
            txtAudioURL.TextChanged += new EventHandler(txtAudioURL_TextChanged);
            cbKeep.CheckedChanged += new EventHandler(cbKeep_CheckedChanged);
            cbPlayAudioStream.CheckedChanged += new EventHandler(cbPlayAudioStream_CheckedChanged);
            lblStation.Tag = lblStation.Text = string.Empty;
            lblStation.Click += new EventHandler(lblStation_Click);

            // Network callbacks
            client.Parcels.OnParcelProperties += new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);
        }

        private void MediaConsole_Disposed(object sender, EventArgs e)
        {
            Stop();

            client.Parcels.OnParcelProperties -= new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);

            if (configTimer != null)
            {
                configTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                configTimer.Dispose();
                configTimer = null;
            }
        }

        void Parcels_OnParcelProperties(Simulator simulator, Parcel parcel, ParcelResult result, int selectedPrims, int sequenceID, bool snapSelection)
        {
            if (cbKeep.Checked || result != ParcelResult.Single) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Parcels_OnParcelProperties(simulator, parcel, result, selectedPrims, sequenceID, snapSelection)));
                return;
            }

            txtAudioURL.Text = parcel.MusicURL;
            if (playing)
            {
                if (currentURL != txtAudioURL.Text)
                {
                    currentURL = txtAudioURL.Text;
                    Play();
                }
            }
            else if (cbPlayAudioStream.Checked)
            {
                currentURL = txtAudioURL.Text;
                Play();
            }
        }

        private void Stop()
        {
            playing = false;
            if (mngr.ParcelMusic != null)
                mngr.ParcelMusic.Dispose();
            mngr.ParcelMusic = null;
            lblStation.Tag = lblStation.Text = string.Empty;
            txtSongTitle.Text = string.Empty;
        }

        private void Play()
        {
            Stop();
            playing = true;
            mngr.ParcelMusic = new Sound(mngr.FMODSystem);
            mngr.ParcelMusic.Volume = audioVolume;
            mngr.ParcelMusic.PlayStream(currentURL);
            mngr.ParcelMusic.OnStreamInfo += new Sound.StreamInfoCallback(ParcelMusic_OnStreamInfo);
        }

        void ParcelMusic_OnStreamInfo(object sender, StreamInfoArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => ParcelMusic_OnStreamInfo(sender, e)));
                return;
            }

            switch (e.Key)
            {
                case "artist":
                    txtSongTitle.Text = e.Value;
                    break;

                case "title":
                    txtSongTitle.Text += " - " + e.Value;
                    break;

                case "icy-name":
                    lblStation.Text = e.Value;
                    break;

                case "icy-url":
                    lblStation.Tag = e.Value;
                    break;
            }
        }

        private void SaveConfig(object state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SaveConfig(state)));
                return;
            }

            s["parcel_audio_url"] = OSD.FromString(txtAudioURL.Text);
            s["parcel_audio_vol"] = OSD.FromReal(audioVolume);
            s["parcel_audio_play"] = OSD.FromBoolean(cbPlayAudioStream.Checked);
            s["parcel_audio_keep_url"] = OSD.FromBoolean(cbKeep.Checked);
        }

        #region GUI event handlers
        void lblStation_Click(object sender, EventArgs e)
        {
            if (lblStation.ToString() != string.Empty)
            {
                instance.MainForm.ProcessLink(lblStation.Tag.ToString());
            }
        }

        private void volAudioStream_Scroll(object sender, EventArgs e)
        {
            configTimer.Change(saveConfigTimeout, System.Threading.Timeout.Infinite);
            if (mngr.ParcelMusic != null)
                mngr.ParcelMusic.Volume = volAudioStream.Value / 50f;
        }

        private void txtAudioURL_TextChanged(object sender, EventArgs e)
        {
            configTimer.Change(saveConfigTimeout, System.Threading.Timeout.Infinite);
        }

        void cbPlayAudioStream_CheckedChanged(object sender, EventArgs e)
        {
            configTimer.Change(saveConfigTimeout, System.Threading.Timeout.Infinite);
        }

        void cbKeep_CheckedChanged(object sender, EventArgs e)
        {
            configTimer.Change(saveConfigTimeout, System.Threading.Timeout.Infinite);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!playing)
            {
                currentURL = txtAudioURL.Text;
                Play();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (playing)
            {
                currentURL = string.Empty;
                Stop();
            }
        }
        #endregion GUI event handlers
    }
}
