using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class frmPay : Form
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private UUID target;
        private string name;
        private UUID owner;
        private bool isObject;
        private Button[] buttons;
        private int[] defaultAmounts = new int[4] { 1, 5, 10, 20 };

        public frmPay(RadegastInstance instance, UUID target, string name, bool isObject)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmPay_Disposed);

            this.instance = instance;
            this.target = target;
            this.name = name;
            this.isObject = isObject;

            // Buttons
            buttons = new Button[4] { btnFastPay1, btnFastPay2, btnFastPay3, btnFastPay4 };
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Click += new EventHandler(frmPay_Click);
                buttons[i].Text = string.Format("L${0}", defaultAmounts[i]);
                buttons[i].Tag = defaultAmounts[i];
            }

            // Callbacks
            client.Objects.OnPayPriceReply += new ObjectManager.PayPriceReply(Objects_OnPayPriceReply);
            client.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
            instance.OnAvatarName += new RadegastInstance.OnAvatarNameCallBack(instance_OnAvatarName);

            if (isObject)
            {
                client.Objects.RequestPayPrice(client.Network.CurrentSim, target);
                client.Objects.RequestObjectPropertiesFamily(client.Network.CurrentSim, target);
                lblObject.Visible = true;
                lblObject.Text = string.Format("Via object: {0}: ", name);
            }
            else
            {
                lblObject.Visible = false;
                lblResident.Text = string.Format("Pay resident: {0}", name);
            }
        }

        void frmPay_Disposed(object sender, EventArgs e)
        {
            client.Objects.OnPayPriceReply -= new ObjectManager.PayPriceReply(Objects_OnPayPriceReply);
            client.Objects.OnObjectPropertiesFamily -= new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
            instance.OnAvatarName -= new RadegastInstance.OnAvatarNameCallBack(instance_OnAvatarName);
        }

        void frmPay_Click(object sender, EventArgs e)
        {
            int amount = (int)((Button)sender).Tag;

            if (!isObject)
            {
                client.Self.GiveAvatarMoney(target, amount);
            }
            else
            {
                client.Self.GiveObjectMoney(target, amount, name);
            }
            Close();
        }

        void UpdateResident()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    UpdateResident();
                }
                ));
                return;
            }

            lblResident.Text = string.Format("Pay resident: {0}", instance.getAvatarName(owner));
        }

        void instance_OnAvatarName(UUID agentID, string agentName)
        {
            if (agentID == owner)
            {
                UpdateResident();
            }
        }

        void Objects_OnObjectPropertiesFamily(Simulator simulator, Primitive.ObjectProperties props, ReportType type)
        {
            if (props.ObjectID == target)
            {
                owner = props.OwnerID;
                UpdateResident();
            }
        }

        void Objects_OnPayPriceReply(Simulator simulator, UUID objectID, int defaultPrice, int[] buttonPrices)
        {
            if (objectID != target) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Objects_OnPayPriceReply(simulator, objectID, defaultPrice, buttonPrices);
                    }
                ));
                return;
            }

            switch (defaultPrice)
            {
                case (int)PayPriceType.Default:
                    txtAmount.Text = string.Empty;
                    break;
                case (int)PayPriceType.Hide:
                    txtAmount.Visible = false;
                    break;
                default:
                    txtAmount.Text = defaultPrice.ToString();
                    break;
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                switch (buttonPrices[i])
                {
                    case (int)PayPriceType.Default:
                        buttons[i].Text = string.Format("L${0}", defaultAmounts[i]);
                        buttons[i].Tag = defaultAmounts[i];
                        break;
                    case (int)PayPriceType.Hide:
                        buttons[i].Visible = false;
                        break;
                    default:
                        buttons[i].Text = string.Format("L${0}", buttonPrices[i]);
                        buttons[i].Tag = buttonPrices[i];
                        break;
                }

            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            int amount;
            if (int.TryParse(txtAmount.Text, out amount) && amount > 0)
            {
                if (!isObject)
                {
                    client.Self.GiveAvatarMoney(target, amount);
                }
                else
                {
                    client.Self.GiveObjectMoney(target, amount, name);
                }
            }
            Close();
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                int amount;
                if (int.TryParse(txtAmount.Text, out amount))
                {
                    btnPay.Enabled = true;
                }
                else
                {
                    btnPay.Enabled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}