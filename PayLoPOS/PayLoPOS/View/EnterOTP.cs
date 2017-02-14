using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Dashboard parent;

        public EnterOTP()
        {
            InitializeComponent();
        }

        public EnterOTP(Dashboard p, string mobile, long orderId, string displayName , string walletName, string email)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.Text = displayName;
            this.walletName = walletName;
            this.mobile = mobile;
            this.email = email;
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
                    var response = await new RestClient().WalletPayment(orderId, walletName, txtOTP.Text, mobile, email);
                    if(response.status == 1)
                    {
                        MessageBox.Show(response.data.msg);
                        this.parent.lblPaidBills_Click(null, null);
                        this.parent.showCurrentActivity(response.data.msg);
                        this.parent.clearTextBox();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                        this.parent.showCurrentActivity(response.data.msg);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.parent.showCurrentActivity(ex.Message);
                }
            }
        }

        private async void ResendOTP_Click(object sender, EventArgs e)
        {
            try
            {
                string json = new JavaScriptSerializer().Serialize(new { order_id = orderId, wallet = walletName }); ;
                var response = await new RestClient().GenerateWalletOTP(json);
                MessageBox.Show(response.data.msg);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
