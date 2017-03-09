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
using PayLoPOS.Model;

namespace PayLoPOS.View
{
    public partial class TransactionDetails : Form
    {
        private long orderId;

        public TransactionDetails(long orderId, double amount, string name, string mobile, string email, string reference, string transactionId, string status)
        {
            InitializeComponent();
            this.orderId = orderId;
            lblAmount.Text = "₹ " + amount.ToString("0.00");
            if(status.ToUpper() == "PENDING")
            {
                lblStatus.ForeColor = Color.Yellow;
            }
            else if(status.ToUpper() == "SUCCESS")
            {
                lblStatus.ForeColor = Color.Lime;
            }
            else
            {
                lblStatus.ForeColor = Color.Orange;
            }
            lblStatus.Text = status.ToUpper();
            lblName.Text = name;
            lblMobile.Text = mobile;
            lblEmail.Text = email;
            lblRef.Text = reference;
            lblOrderId.Text = orderId.ToString();
            lblTransactionId.Text = transactionId;

        }

        private void TransactionDetails_Load(object sender, EventArgs e)
        {
            getTransactionHistory();
        }

        private async void getTransactionHistory()
        {
            try{
                listView1.Items.Clear();
                var response = await new RestClient().GetTransactionDetails(orderId);
                if(response.status == 1)
                {
                    string[] arr = new string[7];
                    foreach(TransactionHistoryBill bill in response.data.txns)
                    {
                        arr[0] = Utility.getDate(bill.created_at, "dd-MM-yyyy HH:mm:ss", Utility.displayDateFormat);
                        arr[1] = bill.bill_id.ToString();
                        arr[2] = bill.txnid;
                        arr[3] = bill.pg_txnid;
                        arr[4] = bill.pay_method;
                        arr[5] = bill.txn_status;
                        arr[6] = bill.pg_message;
                        listView1.Items.Add(new ListViewItem(arr));
                    }

                    if(response.data.txns.Count <= 0)
                    {
                        lblTransactions.Text = "Transactions : " + response.data.msg;
                    }
                    else
                    {
                        lblTransactions.Text = "Transactions";
                    }
                }
                else
                {
                    MessageBox.Show(response.data.msg);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
