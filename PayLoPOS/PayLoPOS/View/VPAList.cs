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

        public VPAList(Dashboard parent, string mobile, string orderId)
        {
            InitializeComponent();
            this.mobile = mobile;
            this.orderId = orderId;
            this.parent = parent;
            txtVPA.Focus();
            fetchVPAList();
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
                UPIPayment(txtVPA.Text);
            }
            else if(items.Count > 0)
            {
                UPIPayment(items[0].Text);
            }
            else
            {
                MessageBox.Show("Please enter a valid VPA or select VPA from list");
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
                    parent.lblPendingBills_Click(null, null);
                }
                MessageBox.Show(response.data.msg);
                parent.showCurrentActivity(response.data.msg);
            }
            catch (Exception ex)
            {
                btnContinue.Enabled = true;
                imgLoading.Visible = false;
                MessageBox.Show(ex.Message);
            }
        }

        private async void fetchVPAList()
        {
            try
            {
                var response = await new RestClient().GetVPA(mobile);
                if(response.status == 1)
                {
                    foreach(string vpa in response.data)
                    {
                        listView1.Items.Add(new ListViewItem(vpa));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
