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
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse.StructuredData;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;

using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public partial class MasterTab : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private Avatar avatar;
        public UUID selectedID;
        public Primitive selectedPrim;

        public MasterTab(RadegastInstance instance, Avatar avatar)
        {
            InitializeComponent();
            Disposed += new EventHandler(MasterTab_Disposed);

            if (!instance.advancedDebugging)
            {
                saveBtn.Visible = false;
                texturesBtn.Visible = false;
            }

            this.instance = instance;
            this.avatar = avatar;
            
            // Callbacks
            client.Avatars.ViewerEffectPointAt += new EventHandler<ViewerEffectPointAtEventArgs>(Avatars_ViewerEffectPointAt);
            client.Objects.ObjectProperties += new EventHandler<ObjectPropertiesEventArgs>(Objects_ObjectProperties);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void MasterTab_Disposed(object sender, EventArgs e)
        {
            client.Avatars.ViewerEffectPointAt -= new EventHandler<ViewerEffectPointAtEventArgs>(Avatars_ViewerEffectPointAt);
            client.Objects.ObjectProperties -= new EventHandler<ObjectPropertiesEventArgs>(Objects_ObjectProperties);
        }

        void Objects_ObjectProperties(object sender, ObjectPropertiesEventArgs e)
        {
            if (selectedPrim != null) {
                if (selectedPrim.ID == e.Properties.ObjectID) {
                    selectedPrim.Properties = e.Properties;
                    UpdateDisplay();
                }
            }
        }

        void UpdateDisplay()
        {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    UpdateDisplay();
                }));
                return;
            }
            lastPrimName.Text = selectedPrim.Properties.Name;
            lastPrimLocalID.Text = selectedPrim.LocalID.ToString();
        }

        void UpdateLLUUID()
        {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    UpdateLLUUID();
                }));
                return;
            }
            lastPrimLLUUID.Text = selectedID.ToString();
            lastPrimLocalID.Text = selectedPrim.LocalID.ToString();
            sitBitn.Enabled = true;
            if (selectedPrim.ParentID != 0) {
                objInfoBtn.Enabled = true;
            } else {
                objInfoBtn.Enabled = false;
            }
            touchBtn.Enabled = true;
            if ((selectedPrim.Flags & PrimFlags.Money) != 0)
            {
                payBtn.Enabled = true;
            }
            else
            {
                payBtn.Enabled = false;
            }
            saveBtn.Enabled = true;
            if (selectedPrim.Textures != null) {
                texturesBtn.Enabled = true;
            }
            btnPoint.Enabled = true;
        }

        void Avatars_ViewerEffectPointAt(object sender, ViewerEffectPointAtEventArgs e)
        {
            if (e.SourceID == avatar.ID && e.TargetID != UUID.Zero) {
                selectedID = e.TargetID;
                selectedPrim = client.Network.CurrentSim.ObjectsPrimitives.Find(
                    delegate(Primitive prim) { return prim.ID == selectedID; }
                );
                if (selectedPrim != null) {
                    client.Objects.SelectObject(client.Network.CurrentSim, selectedPrim.LocalID);
                    UpdateLLUUID();
                }
            }
        }

        private void objInfoBtn_Click(object sender, EventArgs e)
        {
            selectedPrim = client.Network.CurrentSim.ObjectsPrimitives.Find(
                    delegate(Primitive prim) { return prim.LocalID == selectedPrim.ParentID; }
            );
            selectedID = selectedPrim.ID;
            UpdateLLUUID();
            client.Objects.SelectObject(client.Network.CurrentSim, selectedPrim.LocalID);
        }

        private void sitBitn_Click(object sender, EventArgs e)
        {
            if (selectedPrim != null) {
                instance.State.SetSitting(true, selectedPrim.ID);
            }
        }

        private void standBtn_Click(object sender, EventArgs e)
        {
            instance.State.SetSitting(false, UUID.Zero);
        }

        private void touchBtn_Click(object sender, EventArgs e)
        {
            client.Self.Touch(selectedPrim.LocalID);
        }

        private void payBtn_Click(object sender, EventArgs e)
        {
            (new frmPay(instance, selectedPrim.ID, selectedPrim.Properties.Name, true)).ShowDialog();
        }

        private void texturesBtn_Click(object sender, EventArgs e)
        {
            pnlImages.Controls.Clear();

            List<UUID> textures = new List<UUID>();

            textures.Add(selectedPrim.Textures.DefaultTexture.TextureID);

            for (int i = 0; i < selectedPrim.Textures.FaceTextures.Length; i++) {
                if (selectedPrim.Textures.FaceTextures[i] != null && (!textures.Contains(selectedPrim.Textures.FaceTextures[i].TextureID))) {
                    textures.Add(selectedPrim.Textures.FaceTextures[i].TextureID);
                }
            }

            int nTextures = 0;

            foreach (UUID textureID in textures) {
                SLImageHandler img = new SLImageHandler(instance, textureID, "Texture " + (nTextures + 1).ToString());
                img.Location = new Point(0, nTextures++ * img.Height);
                img.Dock = DockStyle.Top;
                img.Height = 450;
                pnlImages.Controls.Add(img);
//                nTextures++;
            }

            if (selectedPrim.Sculpt != null && selectedPrim.Sculpt.SculptTexture != UUID.Zero)
            {
                SLImageHandler img = new SLImageHandler(instance, selectedPrim.Sculpt.SculptTexture, "Sculp Texture");
                img.Location = new Point(0, nTextures * img.Height);
                img.Dock = DockStyle.Top;
                img.Height = 450;
                pnlImages.Controls.Add(img);
            }


        }

        private void saveBtn_Click(object sender, EventArgs e)
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
                            string primsXmls = s.GetSerializedPrims(client.Network.CurrentSim, selectedPrim.LocalID);
                            System.IO.File.WriteAllText(dlg.FileName, primsXmls);
                            s.CleanUp();
                            s = null;
                            MessageBox.Show(mainWindow, "Successfully saved " + dlg.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception excp)
                        {
                            MessageBox.Show(mainWindow, excp.Message, "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }));
                t.IsBackground = true;
                t.Start();
            }
        }
        private void loadBtn_Click(object sender, EventArgs e)
        {
            PrimDeserializer.ImportFromFile(client);
        }

        private void btnPoint_Click(object sender, EventArgs e)
        {
            if (instance.State.IsPointing)
            {
                instance.State.UnSetPointing();
                btnPoint.Text = "Point at";
            }
            else
            {
                instance.State.SetPointing(selectedPrim, 3);
                btnPoint.Text = "Unpoint";
            }
        }


    }

    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }

}
