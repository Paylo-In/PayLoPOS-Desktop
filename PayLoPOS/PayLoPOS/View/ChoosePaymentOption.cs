using PayLoPOS.Controller;
using System;
using System.Windows.Forms;
using PayLoPOS.Model;

namespace PayLoPOS.View
{
    public partial class ChoosePaymentOption : Form
    {
        Dashboard parent;
        long orderId;
        long oid;

        public ChoosePaymentOption(Dashboard parent, long orderId, string mobile, string email, long oid)
        {
            this.parent = parent;
            this.orderId = orderId;
            this.oid = oid;
            InitializeComponent();
            lblMobile.Text = mobile;
            txtEmail.Text = email;
        }

        private void ChoosePaymentOption_Load(object sender, EventArgs e)
        {
            txtPaymentMode.Items.Clear();
            txtPaymentMode.Items.Add("CHOOSE OPTION");
            if (Global.currentUser.payment_options.cash == 1)
            {
                txtPaymentMode.Items.Add("CASH");
            }
            if (Global.currentUser.payment_options.mpos == 1)
            {
                txtPaymentMode.Items.Add("MPOS");
            }
            if (Global.currentUser.payment_options.link == 1)
            {
                txtPaymentMode.Items.Add("SEND LINK");
            }
            if (Global.currentUser.payment_options.upi == 1)
            {
                txtPaymentMode.Items.Add("UPI");
            }
            if (Global.currentUser.payment_options.wallet == 1)
            {
                txtPaymentMode.Items.Add("WALLET");
            }

            txtPaymentMode.SelectedIndex = 0;
        }

        private void textboxMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
               && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private async void lblSubmit_Click(object sender, EventArgs e)
        {
            if(txtPaymentMode.Text == "CHOOSE PAYMENT OPTION")
            {
                MessageBox.Show("Please choosen payment option");
            }
            else if((txtPaymentMode.Text == "UPI" || txtPaymentMode.Text == "WALLET") && txtEmail.Text == "")
            {
                MessageBox.Show("Please enter a valid email address");
            }
            else if(txtPaymentMode.Text == "SEND LINK")
            {
                try
                {
                    lblSubmit.Enabled = false;
                    imgLoading.Visible = true;
                    var response = await new RestClient().ResendLinkPayment(oid, lblMobile.Text);
                    if(response.status == 1)
                    {
                        this.Close();
                        parent.showCurrentActivity(response.data.msg);
                        parent.clearTextBox();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    lblSubmit.Enabled = true;
                    imgLoading.Visible = false;
                }
            }
            else if(txtPaymentMode.Text == "UPI")
            {
                VPAList list = new VPAList(parent, this, lblMobile.Text, oid.ToString());
                try
                {
                    imgLoading.Visible = true;
                    var response = await new RestClient().GetVPA(lblMobile.Text);
                    imgLoading.Visible = false;
                    list.showVPA(response.data);
                }
                catch (Exception ex)
                {
                    imgLoading.Visible = false;
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                parent.ReSendPayment(this, txtPaymentMode.Text, orderId, txtEmail.Text, lblMobile.Text);
            }
        }

        public void showLoading(bool isShow)
        {
            imgLoading.Visible = isShow;
            lblSubmit.Enabled = !isShow;
        }

        private void txtPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(txtPaymentMode.Text == "SEND LINK" || txtPaymentMode.Text == "WALLET")
            {
                lblMobile.Enabled = true;
            }
            else
            {
                lblMobile.Enabled = false;
            }
        }
    }
}
