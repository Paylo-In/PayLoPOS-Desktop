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
using EzetapApi.models;
using System.Diagnostics;

namespace PayLoPOS.View
{
    public partial class RefundTransaction : Form
    {
        Dashboard parent;
        string txnId;
        double amount;
        double refundedAmount;
        double refundableAmount;
        string refundMethod;
        string pgTxnId;

        public RefundTransaction(Dashboard parent, string txnId, double amount,double refundedAmount, double refundableAmount, string refundMethod, string pgTxnId)
        {
            this.parent = parent;
            this.txnId = txnId;
            this.amount = amount;
            this.refundableAmount = refundableAmount;
            this.refundedAmount = refundedAmount;
            this.refundMethod = refundMethod;
            this.pgTxnId = pgTxnId;
            InitializeComponent();
        }

        private void RefundTransaction_Load(object sender, EventArgs e)
        {
            txtAmount.Text = refundableAmount.ToString("0.00");
            txtPassword.Text = "";
            lblRefunded.Text = "Refunded Amount :     " + refundedAmount.ToString("0.00");
            lblRefundable.Text = "Refundable Amount :  " + refundableAmount.ToString("0.00");
            if (refundMethod.ToLower() == "ezytap")
            {
                txtAmount.Enabled = false;
            }
            else
            {
                txtAmount.Enabled = true;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            double refundAmount = Convert.ToDouble(txtAmount.Text);
            if (refundAmount <= 0)
            {
                MessageBox.Show("Please enter a valid amount");
            }
            else if (txtPassword.Text.Length <= 0)
            {
                MessageBox.Show("Please enter your current password");
            }
            else if(refundMethod.ToLower() == "ezytap")
            {
                imgLoading.Visible = true;
                button1.Enabled = false;
                try
                {
                    var response = await new RestClient().authenticateUser(refundAmount, txnId, txtPassword.Text);
                    imgLoading.Visible = false;
                    if (response.status == 1)
                    {
                        Transaction transaction = new Transaction();
                        transaction.orderId = pgTxnId;
                        transaction.externalReference2 = "VoidTxn";
                        Debug.WriteLine("pgTxnId: " + pgTxnId);

                        //-- Initialize
                        EzetapPaymentSync ezetap = new EzetapPaymentSync(this, parent, transaction);
                        ezetap.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                    }
                }
                catch (Exception ex)
                {
                    imgLoading.Visible = false;
                    MessageBox.Show(ex.Message);
                }
                button1.Enabled = true;
            }
            else
            {
                imgLoading.Visible = true;
                button1.Enabled = false;
                try
                {
                    var response = await new RestClient().refundTransaction(refundAmount, txnId, txtPassword.Text);
                    if(response.status == 1)
                    {
                        imgLoading.Visible = false;
                        MessageBox.Show(response.data.msg);
                        Close();
                        parent.showCurrentActivity(refundMethod + " - " + response.data.msg);
                        parent.lblPaidBills_Click(sender, e);
                    }
                    else
                    {
                        imgLoading.Visible = false;
                        MessageBox.Show(response.data.msg);
                    }
                }
                catch (Exception ex)
                {
                    imgLoading.Visible = false;
                    MessageBox.Show(ex.Message);
                }
                button1.Enabled = true;
            }
        }

        private void textboxNumberic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
               && !char.IsDigit(e.KeyChar)
               && e.KeyChar != '.')
                e.Handled = true;

            // only allow one decimal point
            if (e.KeyChar == '.'
                && txtAmount.Text.IndexOf('.') > -1)
                e.Handled = true;
        }
    }
}
