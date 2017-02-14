using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PayLoPOS.Model;
using PayLoPOS.Controller;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace PayLoPOS.View
{
    public partial class ConfirmCash : Form
    {
        public Dashboard dashboard;

        private string jsonParam { get; set; } = "";
        public string mobile { get; set; }
        public double amount { get; set; }
        public long orderId { get; set; } = 0;

        public ConfirmCash(string json, string mobile, double amount, long orderId)
        {
            InitializeComponent();
            jsonParam = json;
            this.mobile = mobile;
            this.amount = amount;
            this.orderId = orderId;
            lblTotalAmount.Text = "₹ " + amount.ToString("0.00");
            txtChange.Text = "₹ -" + amount.ToString("0.00");
            txtMobile.Text = mobile;
            txtCollectedAmount.Focus();
        }

        private async void ConfirmButton_Click(object sender, EventArgs e)
        {
            if(txtCollectedAmount.Text == "")
            {
                MessageBox.Show("Please enter collected amount");
            }
            else if(Convert.ToDouble(txtCollectedAmount.Text) < amount)
            {
                MessageBox.Show("Collected amount should be greater then or equal to ₹ " + amount.ToString("0.00"));
            }
            else
            {
                try
                {
                    ConfirmButton.Enabled = false;
                    if(orderId > 0)
                    {
                        var model = await new RestClient().PayByCashSync(orderId);
                        if (model.status == 1)
                        {
                            dashboard.clearTextBox();
                            dashboard.lblPaidBills_Click(sender, e);
                            dashboard.showCurrentActivity(model.data.msg);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(model.data.msg);
                        }
                    }
                    else
                    {
                        var response = await new RestClient().MakePostRequest("v2/payment/bill-with-cash?token=" + Properties.Settings.Default.accessToken, this.jsonParam);
                        ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
                        if (model.status == 1)
                        {
                            dashboard.clearTextBox();
                            dashboard.lblPaidBills_Click(sender, e);
                            dashboard.showCurrentActivity(model.data.msg);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(model.data.msg);
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtCollectedAmount_TextChanged(object sender, EventArgs e)
        {
            Debug.WriteLine(txtCollectedAmount.Text);
            
            if(txtCollectedAmount.Text != "")
            {
                txtChange.Text = "₹ " + (Convert.ToDouble(txtCollectedAmount.Text) - amount).ToString("0.00");
            }
            else
            {
                txtChange.Text = "₹ -" + amount.ToString("0.00");
            }
        }
    }
}
