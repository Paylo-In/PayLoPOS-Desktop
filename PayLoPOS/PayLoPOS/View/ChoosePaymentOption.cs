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

        public ChoosePaymentOption(Dashboard parent, long orderId)
        {
            this.parent = parent;
            this.orderId = orderId;
            InitializeComponent();
        }

        private void ChoosePaymentOption_Load(object sender, EventArgs e)
        {
            txtPaymentMode.SelectedIndex = 0;
        }

        private void lblSubmit_Click(object sender, EventArgs e)
        {
            if(txtPaymentMode.Text == "CHOOSE PAYMENT OPTION")
            {
                MessageBox.Show("Please choosen payment option");
            }
            else
            {
                parent.ReSendPayment(txtPaymentMode.Text, orderId);
                this.Close();
            }
        }
    }
}
