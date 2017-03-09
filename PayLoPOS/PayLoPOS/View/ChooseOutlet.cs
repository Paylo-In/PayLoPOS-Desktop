using System;
using System.Windows.Forms;
using PayLoPOS.Model;
using PayLoPOS.Controller;

namespace PayLoPOS.View
{
    public partial class ChooseOutlet : Form
    {
        Dashboard parent;

        public ChooseOutlet(Dashboard parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void ChooseOutlet_Load(object sender, EventArgs e)
        {
            foreach(Outlet outlet in Global.currentUser.outlet)
            {
                ListViewItem item = new ListViewItem(outlet.outlet_name);
                item.Tag = outlet.id;
                outletList.Items.Add(item);
            }
        }

        private async void btnContinue_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = outletList.SelectedItems;
            if (items.Count > 0)
            {
                imgLoading.Visible = true;
                try
                {
                    var response = await new RestClient().SwitchOutlet(items[0].Tag.ToString(), Properties.Settings.Default.accessToken);
                    imgLoading.Visible = false;
                    if (response.status == 1)
                    {
                        Properties.Settings.Default.outletId = Convert.ToInt64(items[0].Tag.ToString());
                        Properties.Settings.Default.accessToken = response.data.token;
                        Properties.Settings.Default.Save();
                        Global.currentUser = response.data;
                        parent.refreshOutlet();
                        Close();
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
            }
        }

        private void outletList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = outletList.SelectedItems;
            if (items.Count > 0)
            {
                btnContinue.Enabled = true;
            }
            else
            {
                btnContinue.Enabled = false;
            }
        }
    }
}
