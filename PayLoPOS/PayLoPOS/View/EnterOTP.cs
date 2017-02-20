using System;
using System.Windows.Forms;
using PayLoPOS.Controller;
using System.Web.Script.Serialization;

namespace PayLoPOS.View
{
    public partial class EnterOTP : Form
    {
        private long orderId;
        private string walletName;
        private string mobile;
        private string email;
        double amount;
        Dashboard parent;

        public EnterOTP()
        {
            InitializeComponent();
        }

        private void textboxMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
               && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        public EnterOTP(Dashboard p, double amount, string mobile, long orderId, string displayName , string walletName, string email)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.Text = displayName;
            this.walletName = walletName;
            this.mobile = mobile;
            this.email = email;
            this.amount = amount;
            this.parent = p;
            if(this.mobile.Length > 3)
            {
                lblMessage.Text = "Please enter the One Time Password sent to customer mobile number +91-XXXXXXX" + mobile.Substring(mobile.Length - 3);
            }
        }

        private async void label4_Click(object sender, EventArgs e)
        {
            if(txtOTP.Text == "")
            {
                MessageBox.Show("Please enter a valid One Time Password (OTP)");
            }
            else
            {
                try
                {
                    lblVerify.Enabled = false;
                    imgLoading.Visible = true;
                    var response = await new RestClient().WalletPayment(orderId, walletName, txtOTP.Text, mobile, email);
                    lblVerify.Enabled = true;
                    imgLoading.Visible = false;
                    if (response.status == 1)
                    {
                        parent.lblPaidBills_Click(null, null);
                        parent.showCurrentActivity(response.data.msg);
                        parent.clearTextBox();
                        Close();
                        PaymentStatus ps = new PaymentStatus(1, response.data.msg, Text);
                        ps.ShowDialog();
                    }
                    else
                    {
                        txtOTP.Text = "";
                        MessageBox.Show(response.data.msg);
                        parent.showCurrentActivity(response.data.msg);
                    }
                }
                catch(Exception ex)
                {
                    txtOTP.Text = "";
                    MessageBox.Show(ex.Message);
                    this.parent.showCurrentActivity(ex.Message);
                }
                finally
                {
                    lblVerify.Enabled = true;
                    imgLoading.Visible = false;
                }
            }
        }

        private async void ResendOTP_Click(object sender, EventArgs e)
        {
            try
            {
                ResendOTP.Enabled = false;
                imgLoading.Visible = true;
                string json = new JavaScriptSerializer().Serialize(new { order_id = orderId, wallet = walletName }); ;
                var response = await new RestClient().GenerateWalletOTP(json);
                ResendOTP.Enabled = true;
                imgLoading.Visible = false;
                MessageBox.Show(response.data.msg);
            }
            catch(Exception ex)
            {
                ResendOTP.Enabled = true;
                imgLoading.Visible = false;
                MessageBox.Show(ex.Message);
            }
        }
    }
}
