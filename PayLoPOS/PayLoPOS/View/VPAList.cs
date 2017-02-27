using PayLoPOS.Controller;
using PayLoPOS.Model;
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
    public partial class VPAList : Form
    {
        private string mobile;
        private string orderId;
        Dashboard parent;
        ChoosePaymentOption subParent;

        public VPAList(Dashboard parent, ChoosePaymentOption subParent, string mobile, string orderId)
        {
            InitializeComponent();
            this.mobile = mobile;
            this.orderId = orderId;
            this.parent = parent;
            this.subParent = subParent;
            txtVPA.Focus();
            //fetchVPAList();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtVPA.Text = "";
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if (txtVPA.Text != "")
            {
                var index = 0;
                foreach (char c in txtVPA.Text)
                {
                    if(c == '@')
                    {
                        index++;
                    }
                }

                if(index == 1)
                {
                    UPIPayment(txtVPA.Text);
                }
                else
                {
                    MessageBox.Show("Please enter a valid VPA");
                    txtVPA.Focus();
                }
                
            }
            else if(items.Count > 0)
            {
                UPIPayment(items[0].Text);
            }
            else
            {
                MessageBox.Show("Please enter a valid VPA");
                txtVPA.Focus();
            }
        }

        private async void UPIPayment(string vpa)
        {
            try
            {
                btnContinue.Enabled = false;
                imgLoading.Visible = true;

                ResponseModel response;

                if (orderId == "")
                {
                    response = await new RestClient().UPIPayment("", Global.currentBill.mobile, Global.currentBill.amount, Global.currentBill.reference, Global.currentBill.name, Global.currentBill.email, vpa);
                }
                else
                {
                    response = await new RestClient().ResendUPIPayment(orderId, mobile, vpa);
                }
                
                btnContinue.Enabled = true;
                imgLoading.Visible = false;
                if (response.status == 1 || response.data.bill == 1)
                {
                    Close();
                    if(subParent != null)
                    {
                        subParent.Close();
                    }
                    else
                    {
                        parent.lblPendingBills_Click(null, null);
                    }
                    parent.showCurrentActivity(response.data.msg);
                }
                MessageBox.Show(response.data.msg);
            }
            catch (Exception ex)
            {
                btnContinue.Enabled = true;
                imgLoading.Visible = false;
                MessageBox.Show(ex.Message);
            }
        }

        public async void fetchVPAList()
        {
            var response = await new RestClient().GetVPA(mobile);
            if (response.status == 1)
            {
                foreach (string vpa in response.data)
                {
                    listView1.Items.Add(new ListViewItem(vpa));
                }
            }
        }

        public void showVPA(List<string> vpaList)
        {
            foreach (string vpa in vpaList)
            {
                listView1.Items.Add(new ListViewItem(vpa));
            }

            if (listView1.Items.Count > 0)
            {
                Size = new Size(310, 400);
                listView1.Visible = true;
            }
            else
            {
                Size = new Size(310, 150);
                listView1.Visible = false;
            }
            ShowDialog();
        }
    }
}
