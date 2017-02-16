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
        public PaymentStatus(int status, string message, double amount, string mode, string billNo, string mobile)
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
            lblAmount.Text = amount.ToString("0.00");
            lblMode.Text = mode;
            lblBillNo.Text = billNo;
            lblMobile.Text = mobile;
        }
    }
}
