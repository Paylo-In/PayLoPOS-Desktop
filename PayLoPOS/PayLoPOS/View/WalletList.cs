using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        public WalletList(Dashboard p, long orderId, string mobile, string email)
        {
            parent = p;
            this.mobile = mobile;
            this.orderId = orderId;
            this.email = email;
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
                var request = WebRequest.Create(wallet.icon);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    imgList.Images.Add(Image.FromStream(stream));
                    listView1.Items.Add(wallet.display_name, index);
                    index++;
                }
            }

            listView1.LargeImageList = imgList;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            foreach (ListViewItem item in items)
            {
                foreach(Wallet wallet in Global.currentUser.wallets)
                {
                    if(wallet.display_name == item.Text)
                    {
                        makeWalletPayment(wallet);
                        break;
                    }
                }
            }
        }

        private async void makeWalletPayment(Wallet wallet)
        {
            try
            {                
                DictionaryModel param = new DictionaryModel();
                param.add("order_id", orderId.ToString());
                param.add("wallet", wallet.name);
                var response = await new RestClient().GenerateWalletOTP(param.getJSON());
                if(response.status == 1)
                {
                    EnterOTP otp = new EnterOTP(this.parent ,mobile, orderId, wallet.display_name, wallet.name, email);
                    otp.ShowDialog();
                    this.Close();
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
