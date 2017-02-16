using PayLoPOS.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            txtPaymentMode.SelectedIndex = 0;
        }

        private async void lblSubmit_Click(object sender, EventArgs e)
        {
            if(txtPaymentMode.Text == "CHOOSE PAYMENT OPTION")
            {
                MessageBox.Show("Please choosen payment option");
            }
            else if((txtPaymentMode.Text == "UPI" || txtPaymentMode.Text == "WALLET" || txtPaymentMode.Text == "MPOS") && txtEmail.Text == "")
            {
                MessageBox.Show("Please enter a valid email address");
            }
            else if(txtPaymentMode.Text == "SEND LINK")
            {
                try
                {
                    lblSubmit.Enabled = false;
                    imgLoading.Visible = true;
                    var response = await new RestClient().ResendLinkPayment(oid);
                    if(response.status == 1)
                    {
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                    }
                    parent.showCurrentActivity(response.data.msg);
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
            else
            {
                parent.ReSendPayment(this, txtPaymentMode.Text, orderId, txtEmail.Text);
                //this.Close();
            }
        }

        public void showLoading(bool isShow)
        {
            imgLoading.Visible = isShow;
            lblSubmit.Enabled = !isShow;
        }
    }
}
