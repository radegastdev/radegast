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
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Windows.Forms;
using System.IO;
using OpenMetaverse;

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

            GUI.GuiHelpers.ApplyGuiFixes(this);
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
                img.BorderStyle = BorderStyle.FixedSingle;
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
