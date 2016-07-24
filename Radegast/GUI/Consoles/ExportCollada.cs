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
using System.Text;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;
using System.Windows.Forms;
using System.IO;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using OpenMetaverse.Assets;
using OpenMetaverse.Imaging;

namespace Radegast
{
	public partial class ExportCollada : RadegastTabControl
	{
		#region Private Variables
        DAEExport Exporter;
		#endregion
		
		#region Constructor
        public ExportCollada(RadegastInstance instance, Primitive prim)
            : base(instance)
		{
			InitializeComponent();
            Exporter = new DAEExport(instance, prim);
            Exporter.Progress += new EventHandler<DAEStatutsEventArgs>(Exporter_Progress);
            UpdateInfo();
            cbImageType.Text = "TGA";

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
		}
		#endregion
		
		#region Private Methods
		void LogMessage(string format, params object[] args)
		{
			if (InvokeRequired)
			{
				if (IsHandleCreated || !instance.MonoRuntime)
					BeginInvoke(new MethodInvoker(() => LogMessage(format, args)));
				return;
			}
			txtLog.AppendText(String.Format(format + "\r\n",args));
			txtLog.SelectionStart = txtLog.TextLength;
			txtLog.ScrollToCaret();
		}

        void UpdateInfo()
        {
            Primitive root = Exporter.Prims[0];
            if (root.Properties != null)
            {
                objectName.Text = root.Properties.Name;
            }
            else
            {
                objectName.Text = "Object";
            }
            objectUUID.Text = root.ID.ToString();
            primCount.Text = Exporter.Prims.Count.ToString();
            exportablePrims.Text = Exporter.ExportablePrims.ToString();
            textureCount.Text = Exporter.Textures.Count.ToString();
            exportableTextures.Text = Exporter.ExportableTextures.ToString();
            texturesPanel.Controls.Clear();
            foreach (UUID textureID in Exporter.Textures)
            {
                var img = new SLImageHandler(instance, textureID, string.Empty);
                img.Height = 96;
                img.Width = 96;
                img.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                texturesPanel.Controls.Add(img);
            }
        }
		
		void ValidatePath(string fname)
		{
			string path = Path.GetDirectoryName(fname);
			if (Directory.Exists(path))
				btnExport.Enabled = true;
			else
				btnExport.Enabled = false;
		}
		#endregion
		
		#region Event Handlers
		void TxtFileNameTextChanged(object sender, EventArgs e)
		{
			ValidatePath(txtFileName.Text);
		}
		
		void BtnBrowseClick(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = "Export Collada File";
            dlg.Filter = "Collada (*.dae)|*.dae|All Files (*.*)|*.*";
            if (txtFileName.Text.Trim() == string.Empty)
            {
                dlg.FileName = RadegastInstance.SafeFileName(objectName.Text);
            }
            else
            {
                dlg.FileName =  Path.GetFileName(txtFileName.Text);
            }

			DialogResult res = dlg.ShowDialog();
			
			if (res == DialogResult.OK)
			{
				txtFileName.Text = dlg.FileName;
				ValidatePath(dlg.FileName);
			}
		}
		
		void BtnExportClick(object sender, EventArgs e)
		{
            Exporter.ImageFormat = cbImageType.Text;
            try
            {
                Exporter.Export(txtFileName.Text);
            }
            catch (Exception ex)
            {
                LogMessage("Export failed: {0}", ex.ToString());
            }
		}

        private void cbExportTextures_CheckedChanged(object sender, EventArgs e)
        {
            Exporter.ExportTextures = cbImageType.Enabled = cbExportTextures.Checked;
        }

        void Exporter_Progress(object sender, DAEStatutsEventArgs e)
        {
            LogMessage(e.Message);
        }
        #endregion
    }
}
