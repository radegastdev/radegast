using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class AttachmentDetail : UserControl
    {
        private GridClient client;
        private Avatar av;
        private Primitive attachment;

        public AttachmentDetail(GridClient iclinet, Avatar iav, Primitive iattachment)
        {
            InitializeComponent();
            Disposed += new EventHandler(AttachmentDetail_Disposed);

            client = iclinet;
            av = iav;
            attachment = iattachment;

            // Callbacks
            client.Objects.OnObjectProperties += new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);
        }

        void AttachmentDetail_Disposed(object sender, EventArgs e)
        {
            client.Objects.OnObjectProperties -= new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);
        }

        private void AttachmentDetail_Load(object sender, EventArgs e)
        {
            boxID.Text = attachment.ID.ToString();
            client.Objects.SelectObject(client.Network.CurrentSim, attachment.LocalID);
        }

        private void UpdateControls()
        {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    UpdateControls();
                }));
                return;
            }
            boxName.Text = attachment.Properties.Name;
            List<Primitive> parts = client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim)
                {
                    return (prim.LocalID == attachment.LocalID || prim.ParentID == attachment.LocalID);
                }
            ); 
            lblPrimCount.Text = "Prims: " + parts.Count.ToString(); ;
                
        }
        void Objects_OnObjectProperties(Simulator simulator, Primitive.ObjectProperties properties)
        {
            if (properties.ObjectID == attachment.ID) {
                attachment.Properties = properties;
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

            try {
                if (res == DialogResult.OK) {
                    PrimSerializer s = new PrimSerializer(client);
                    string primsXmls = s.GetSerializedAttachmentPrims(client.Network.CurrentSim, attachment.LocalID);
                    System.IO.File.WriteAllText(dlg.FileName, primsXmls);
                    MessageBox.Show(mainWindow, "Successfully saved " + dlg.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception excp) {
                MessageBox.Show(mainWindow, excp.Message, "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
