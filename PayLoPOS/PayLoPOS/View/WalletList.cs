using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using PayLoPOS.Model;
using PayLoPOS.Controller;

namespace PayLoPOS.View
{
    public partial class WalletList : Form
    {
        Dashboard parent;
        long orderId;
        string mobile;
        string email;
        double amount;

        public WalletList(Dashboard p, double amount, long orderId, string mobile, string email)
        {
            parent = p;
            this.mobile = mobile;
            this.orderId = orderId;
            this.email = email;
            this.amount = amount;
            InitializeComponent();
            this.populateImages();
        }

        private void populateImages()
        {
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(80, 80);

            var index = 0;
            foreach(Wallet wallet in Global.currentUser.wallets)
            {
                if(wallet.name.ToLower() == "citrus")
                {
                    imgList.Images.Add(Properties.Resources.Citrus);
                }
                else if (wallet.name.ToLower() == "freecharge")
                {
                    imgList.Images.Add(Properties.Resources.freecharge);
                }
                else if (wallet.name.ToLower() == "payumoney")
                {
                    imgList.Images.Add(Properties.Resources.payumoney);
                }
                else
                {
                    var request = WebRequest.Create(wallet.icon);
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        imgList.Images.Add(Image.FromStream(stream));
                    }
                }
                listView1.Items.Add(wallet.display_name, index);
                index++;
            }

            listView1.LargeImageList = imgList;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(imgLoading.Visible == false)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    foreach (Wallet wallet in Global.currentUser.wallets)
                    {
                        if (wallet.display_name == item.Text)
                        {
                            makeWalletPayment(wallet);
                            break;
                        }
                    }
                }
            }
        }

        private async void makeWalletPayment(Wallet wallet)
        {
            try
            {
                imgLoading.Visible = true;       
                DictionaryModel param = new DictionaryModel();
                param.add("order_id", orderId.ToString());
                param.add("wallet", wallet.name);
                var response = await new RestClient().GenerateWalletOTP(param.getJSON());
                if(response.status == 1)
                {
                    imgLoading.Visible = false;
                    EnterOTP otp = new EnterOTP(this.parent, amount ,mobile, orderId, wallet.display_name, wallet.name, email);
                    otp.ShowDialog();
                    this.Close();
                }
                else
                {
                    imgLoading.Visible = false;
                    MessageBox.Show(response.data.msg);
                }
            }
            catch(Exception ex)
            {
                imgLoading.Visible = false;
                MessageBox.Show(ex.Message);
            }
        }
    }
}
