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
        private float m_audioVolume;

        private float audioVolume
        {
            get
            {
                return m_audioVolume;
            }
            set
            {
                if (value >= 0f && value < 1f)
                {
                    m_audioVolume = value;
                    parcelStream.Volume = m_audioVolume;
                }
            }
        }
        private System.Threading.Timer configTimer;
        private const int saveConfigTimeout = 1000;
        private bool playing;
        private string currentURL;
        private Media.Stream parcelStream;
        private readonly object parcelMusicLock = new object();


        public MediaConsole(RadegastInstance instance)
        {
            InitializeComponent();
            DisposeOnDetachedClose = false;
            Text = "Media";

            Disposed += new EventHandler(MediaConsole_Disposed);

            this.instance = instance;
            this.parcelStream = new Media.Stream();

            s = instance.GlobalSettings;

            // Set some defaults in case we don't have them in config
            audioVolume = 0.2f;
            objVolume.Value = 50;
            instance.MediaManager.ObjectVolume = 1f;

            // Restore settings
            if (s["parcel_audio_url"].Type != OSDType.Unknown)
                txtAudioURL.Text = s["parcel_audio_url"].AsString();
            if (s["parcel_audio_vol"].Type != OSDType.Unknown)
                audioVolume = (float)s["parcel_audio_vol"].AsReal();
            if (s["parcel_audio_play"].Type != OSDType.Unknown)
                cbPlayAudioStream.Checked = s["parcel_audio_play"].AsBoolean();
            if (s["parcel_audio_keep_url"].Type != OSDType.Unknown)
                cbKeep.Checked = s["parcel_audio_keep_url"].AsBoolean();
            if (s["object_audio_enable"].Type != OSDType.Unknown)
                cbObjSoundEnable.Checked = s["object_audio_enable"].AsBoolean();
            if (s["object_audio_vol"].Type != OSDType.Unknown)
            {
                instance.MediaManager.ObjectVolume = (float)s["object_audio_vol"].AsReal();
                objVolume.Value = (int)(50f * instance.MediaManager.ObjectVolume);
            }
            if (s["ui_audio_vol"].Type != OSDType.Unknown)
            {
                instance.MediaManager.UIVolume = (float)s["ui_audio_vol"].AsReal();
                UIVolume.Value = (int)(50f * instance.MediaManager.UIVolume);
            }

            volAudioStream.Value = (int)(audioVolume * 50);
            instance.MediaManager.ObjectEnable = cbObjSoundEnable.Checked;

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

            objVolume.Scroll += new EventHandler(volObject_Scroll);
            cbObjSoundEnable.CheckedChanged += new EventHandler(cbObjEnableChanged);

            // Network callbacks
            client.Parcels.ParcelProperties += new EventHandler<ParcelPropertiesEventArgs>(Parcels_ParcelProperties);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        private void MediaConsole_Disposed(object sender, EventArgs e)
        {
            Stop();

            client.Parcels.ParcelProperties -= new EventHandler<ParcelPropertiesEventArgs>(Parcels_ParcelProperties);

            if (configTimer != null)
            {
                configTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                configTimer.Dispose();
                configTimer = null;
            }
        }

        void Parcels_ParcelProperties(object sender, ParcelPropertiesEventArgs e)
        {
            if (cbKeep.Checked || e.Result != ParcelResult.Single) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Parcels_ParcelProperties(sender, e)));
                return;
            }
            lock (parcelMusicLock)
            {
                txtAudioURL.Text = e.Parcel.MusicURL;
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
        }

        private void Stop()
        {
            lock (parcelMusicLock)
            {
                playing = false;
                if (parcelStream != null)
                    parcelStream.Dispose();
                parcelStream = null;
                lblStation.Tag = lblStation.Text = string.Empty;
                txtSongTitle.Text = string.Empty;
            }
        }

        private void Play()
        {
            lock (parcelMusicLock)
            {
                Stop();
                playing = true;
                parcelStream = new Media.Stream();
                parcelStream.Volume = audioVolume;
                parcelStream.PlayStream(currentURL);
                parcelStream.OnStreamInfo += new Media.Stream.StreamInfoCallback(ParcelMusic_OnStreamInfo);
            }
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
            s["object_audio_vol"] = OSD.FromReal(this.instance.MediaManager.ObjectVolume);
            s["object_audio_enable"] = OSD.FromBoolean(cbObjSoundEnable.Checked);
            s["ui_audio_vol"] = OSD.FromReal(this.instance.MediaManager.UIVolume);
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
            lock (parcelMusicLock)
                if (parcelStream != null)
                    audioVolume = volAudioStream.Value / 50f;
        }

        private void volObject_Scroll(object sender, EventArgs e)
        {
            instance.MediaManager.ObjectVolume = objVolume.Value / 50f;
            configTimer.Change(saveConfigTimeout, System.Threading.Timeout.Infinite);
        }

        void cbObjEnableChanged(object sender, EventArgs e)
        {
            instance.MediaManager.ObjectEnable = cbObjSoundEnable.Checked;
            configTimer.Change(saveConfigTimeout, System.Threading.Timeout.Infinite);
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
            lock (parcelMusicLock) if (!playing)
                {
                    currentURL = txtAudioURL.Text;
                    Play();
                }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lock (parcelMusicLock) if (playing)
                {
                    currentURL = string.Empty;
                    Stop();
                }
        }

        private void UIVolume_Scroll(object sender, EventArgs e)
        {
            instance.MediaManager.UIVolume = UIVolume.Value / 50f;
            configTimer.Change(saveConfigTimeout, System.Threading.Timeout.Infinite);
        }
        #endregion GUI event handlers
    }
}
