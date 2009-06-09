using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RadegastNc;
using OpenMetaverse;

namespace Radegast
{
    public partial class MasterTab : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private Avatar avatar;
        private UUID selectedID;
        private Primitive selectedPrim;

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
            client.Avatars.OnPointAt += new AvatarManager.PointAtCallback(Avatars_OnPointAt);
            client.Objects.OnObjectProperties += new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);

        }

        void MasterTab_Disposed(object sender, EventArgs e)
        {
            client.Avatars.OnPointAt -= new AvatarManager.PointAtCallback(Avatars_OnPointAt);
            client.Objects.OnObjectProperties -= new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);
        }

        void Objects_OnObjectProperties(Simulator simulator, Primitive.ObjectProperties properties)
        {
            if (selectedPrim != null) {
                if (selectedPrim.ID == properties.ObjectID) {
                    selectedPrim.Properties = properties;
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


        void Avatars_OnPointAt(UUID sourceID, UUID targetID, Vector3d targetPos,
    PointAtType pointType, float duration, UUID id)
        {
            if (sourceID == avatar.ID && targetID != UUID.Zero) {
                selectedID = targetID;
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
                client.Self.RequestSit(selectedPrim.ID, Vector3.Zero);
                client.Self.Sit();
            }
        }

        private void standBtn_Click(object sender, EventArgs e)
        {
            client.Self.Stand();
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

            try {
                if (res == DialogResult.OK) {
                    PrimSerializer s = new PrimSerializer(client);
                    string primsXmls = s.GetSerializedPrims(client.Network.CurrentSim, selectedPrim.LocalID);
                    System.IO.File.WriteAllText(dlg.FileName, primsXmls);
                    MessageBox.Show(mainWindow, "Successfully saved " + dlg.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception excp) {
                MessageBox.Show(mainWindow, excp.Message, "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void loadBtn_Click(object sender, EventArgs e)
        {
            WindowWrapper mainWindow = new WindowWrapper(frmMain.ActiveForm.Handle);
            System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open object file";
            dlg.Filter = "XML file (*.xml)|*.xml";
            dlg.Multiselect = false;
            DialogResult res = dlg.ShowDialog();

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
            {
                try {
                    if (res == DialogResult.OK) {
                        PrimDeserializer d = new PrimDeserializer(client);
                        string primsXmls = System.IO.File.ReadAllText(dlg.FileName);
                        d.CreateObjectFromXml(primsXmls);
                        MessageBox.Show(mainWindow, "Successfully imported " + dlg.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } catch (Exception excp) {
                    MessageBox.Show(mainWindow, excp.Message, "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }));

            t.IsBackground = true;
            t.Start();
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
