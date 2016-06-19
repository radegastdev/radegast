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
using System.Windows.Forms;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;

using OpenMetaverse;

namespace Radegast
{
    public partial class AttachmentDetail : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private Avatar av;
        private Primitive attachment;

        public AttachmentDetail(RadegastInstance instance, Avatar av, Primitive attachment)
        {
            InitializeComponent();
            Disposed += new EventHandler(AttachmentDetail_Disposed);

            this.instance = instance;
            this.av = av;
            this.attachment = attachment;

            if (!instance.advancedDebugging)
            {
                btnSave.Visible = false;
                boxID.Visible = false;
                lblAttachment.Visible = false;
            }

            // Callbacks
            client.Objects.ObjectProperties += new EventHandler<ObjectPropertiesEventArgs>(Objects_ObjectProperties);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void AttachmentDetail_Disposed(object sender, EventArgs e)
        {
            client.Objects.ObjectProperties -= new EventHandler<ObjectPropertiesEventArgs>(Objects_ObjectProperties);
        }

        private void AttachmentDetail_Load(object sender, EventArgs e)
        {
            boxID.Text = attachment.ID.ToString();

            if (attachment.Properties == null)
            {
                client.Objects.SelectObject(client.Network.CurrentSim, attachment.LocalID);
            }
            else
            {
                UpdateControls();
            }
        }

        private void UpdateControls()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateControls));
                return;
            }

            lblAttachmentPoint.Text = attachment.PrimData.AttachmentPoint.ToString();
            boxName.Text = attachment.Properties.Name;

            if ((attachment.Flags & PrimFlags.Touch) == 0)
            {
                btnTouch.Visible = false;
            }

            List<Primitive> parts = client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim)
                {
                    return (prim.LocalID == attachment.LocalID || prim.ParentID == attachment.LocalID);
                }
            );

            lblPrimCount.Text = "Prims: " + parts.Count.ToString();
        }

        void Objects_ObjectProperties(object sender, ObjectPropertiesEventArgs e)
        {
            if (e.Properties.ObjectID == attachment.ID)
            {
                attachment.Properties = e.Properties;
                UpdateControls();
            }
        }

        private void btnTouch_Click(object sender, EventArgs e)
        {
            client.Self.Touch(attachment.LocalID);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WindowWrapper mainWindow = new WindowWrapper(frmMain.ActiveForm.Handle);
            System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.RestoreDirectory = true;
            dlg.Title = "Save object as...";
            dlg.Filter = "XML file (*.xml)|*.xml";
            DialogResult res = dlg.ShowDialog();

            if (res == DialogResult.OK)
            {
                Thread t = new Thread(new ThreadStart(delegate()
                    {
                        try
                        {
                            PrimSerializer s = new PrimSerializer(client);
                            string primsXmls = s.GetSerializedAttachmentPrims(client.Network.CurrentSim, attachment.LocalID);
                            System.IO.File.WriteAllText(dlg.FileName, primsXmls);
                            s.CleanUp();
                            s = null;
                            MessageBox.Show(mainWindow, "Successfully saved " + dlg.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception excp)
                        {
                            MessageBox.Show(mainWindow, excp.Message, "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                ));
                t.IsBackground = true;
                t.Start();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Rendering.frmPrimWorkshop pw = new Rendering.frmPrimWorkshop(instance, attachment.LocalID);
            pw.Show();
            pw.SetView(new Vector3(0f, 0.5f, 0f), 0, 0, 90, -10);
        }
    }
}
