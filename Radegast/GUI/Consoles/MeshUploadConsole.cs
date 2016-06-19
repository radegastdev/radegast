// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using OpenMetaverse;

namespace Radegast
{
    public partial class MeshUploadConsole : RadegastTabControl
    {
        bool Running = false;
        bool UploadImages;
        Queue<string> FileNames = new Queue<string>();
        Thread UploadThread;

        public MeshUploadConsole()
        {
            InitializeComponent();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        public MeshUploadConsole(RadegastInstance instance)
            : base(instance)
        {
            InitializeComponent();

            Disposed += new EventHandler(MeshUploadConsole_Disposed);
            instance.Netcom.ClientConnected += new EventHandler<EventArgs>(Netcom_ClientConnected);
            instance.Netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);
            UpdateButtons();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void MeshUploadConsole_Disposed(object sender, EventArgs e)
        {
            if (UploadThread != null && UploadThread.IsAlive)
            {
                try
                {
                    UploadThread.Abort();
                }
                catch { }
                UploadThread = null;
                Running = false;
            }
        }

        void Netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Netcom_ClientDisconnected(sender, e)));
                }
                return;
            }

            if (UploadThread != null && UploadThread.IsAlive)
            {
                try
                {
                    UploadThread.Abort();
                }
                catch { }
                UploadThread = null;
                Running = false;
            }

            UpdateButtons();
        }

        void Netcom_ClientConnected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Netcom_ClientConnected(sender, e)));
                }
                return;
            }

            UpdateButtons();
        }

        void Msg(string msg)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Msg(msg)));
                }
                return;
            }

            txtUploadLog.AppendText(msg + "\n");
        }
        void UpdateButtons()
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => UpdateButtons()));
                }
                return;
            }

            UploadImages = cbUploadImages.Checked;

            if (client.Network.Connected)
            {
                if (Running)
                {
                    btnBrowse.Enabled = false;
                    btnStart.Enabled = false;
                }
                else
                {
                    btnBrowse.Enabled = true;
                    lock (FileNames)
                    {
                        btnStart.Enabled = FileNames.Count > 0;
                    }
                }
            }
            else
            {
                btnBrowse.Enabled = true;
                btnStart.Enabled = false;
            }

            lock (FileNames)
            {
                lblStatus.Text = string.Format("{0} files remaining", FileNames.Count);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var o = new OpenFileDialog();
            o.Filter = "Collada files (*.dae)|*.dae|All files (*.*)|*.*";
            o.Multiselect = true;
            var res = o.ShowDialog();

            if (res != System.Windows.Forms.DialogResult.OK)
                return;

            lock (FileNames)
            {
                FileNames.Clear();
                foreach (var fname in o.FileNames)
                {
                    FileNames.Enqueue(fname);
                }
                txtUploadLog.Clear();
                txtUploadLog.AppendText("Ready.");
            }

            UpdateButtons();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            lock (FileNames)
            {
                if (Running || FileNames.Count == 0)
                {
                    return;
                }
            }

            UploadThread = new Thread(new ThreadStart(PerformUpload));
            UploadThread.Name = "Mesh Upload Thread";
            UploadThread.IsBackground = true;
            UploadThread.Start();
            txtUploadLog.Clear();
        }

        void PerformUpload()
        {
            Running = true;
            UpdateButtons();

            while (FileNames.Count > 0)
            {
                Msg(string.Empty);

                string filename;
                lock (FileNames)
                {
                    filename = FileNames.Dequeue();
                }
                Msg(string.Format("Processing: {0}", filename));

                var parser = new OpenMetaverse.ImportExport.ColladaLoader();
                var prims = parser.Load(filename, UploadImages);
                if (prims == null || prims.Count == 0)
                {
                    Msg("Error: Failed to parse collada file");
                    continue;
                }

                Msg(string.Format("Parse collada file success, found {0} objects", prims.Count));
                Msg("Uploading...");

                var uploader = new OpenMetaverse.ImportExport.ModelUploader(client, prims, Path.GetFileNameWithoutExtension(filename), "Radegast " + DateTime.Now.ToString());
                var uploadDone = new AutoResetEvent(false);

                uploader.IncludePhysicsStub = true;
                uploader.UseModelAsPhysics = false;

                uploader.Upload((res =>
                {
                    if (res == null)
                    {
                        Msg("Upload failed.");
                    }
                    else
                    {
                        Msg("Upload success.");
                    }

                    uploadDone.Set();
                }));

                if (!uploadDone.WaitOne(4 * 60 * 1000))
                {
                    Msg("Message upload timeout");
                }
            }

            Running = false;
            UpdateButtons();
            Msg("Done.");
        }

    }
}
