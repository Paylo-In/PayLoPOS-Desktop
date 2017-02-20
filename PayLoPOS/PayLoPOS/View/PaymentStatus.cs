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

namespace PayLoPOS.View
{
    public partial class PaymentStatus : Form
    {
        public PaymentStatus(int status, string message, string mode)
        {
            InitializeComponent();

            if(status == 1)
            {
                pictureBoxStatus.Image = Properties.Resources.success;
                lblStatus.Text = "Success";
                lblStatus.ForeColor = Color.SeaGreen;
            }
            else
            {
                pictureBoxStatus.Image = Properties.Resources.failed;
                lblStatus.Text = "Failed";
                lblStatus.ForeColor = Color.Red;
            }
            Text = lblStatus.Text;
            lblMessage.Text = message;
            lblMode.Text = mode;

            lblAmount.Text = "₹ " + Global.currentBill.amount.ToString("0.00");
            lblBillNo.Text = Global.currentBill.reference;
            lblMobile.Text = Global.currentBill.mobile;
            lblName.Text = Global.currentBill.name;
        }
    }
}
