using System;
using System.Windows.Forms;
using PayLoPOS.Model;
using PayLoPOS.Controller;
using System.Diagnostics;
using System.Web.Script.Serialization;
using EzetapApi.models;

namespace PayLoPOS.View
{

    enum DateRange
    {
        Today = 1,
        OneWeek,
        OneMonth
    }

    public partial class Dashboard : Form
    {

        public Login login;

        private PendingBills pendingBills;

        private PaidBillResponse paidBills;

        private Boolean isSelectPending;

        private DateRange dateRange = DateRange.Today;

        public Dashboard()
        {
            InitializeComponent();
            lblUserName.Text = "Welcome: "+Global.currentUser.name;
            this.Text = Global.currentUser.outlet[0].outlet_name;
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            txtPaymentMode.SelectedIndex = 0;
            isSelectPending = true;
            lblToday_Click(sender, e);
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            if(txtPaymentMode.Text == "CHOOSE OPTION")
            {
                MessageBox.Show("Please choose payment option");
            }
            else if(txtAmount.Text == "")
            {
                MessageBox.Show("Please enter a valid amount");
            }
            else if (txtMobile.Text.Length != 10)
            {
                MessageBox.Show("Please enter a valid mobile number");
            }
            else
            {
                if(txtPaymentMode.Text == "CASH")
                {
                    payByCash();
                }
                else if (txtPaymentMode.Text == "SEND LINK")
                {
                    payBySendLink();
                }
                else if (txtPaymentMode.Text == "MPOS")
                {
                    payByMPOS();
                }
            }            
        }

        private void payByMPOS()
        {
            if(txtEmail.Text == "")
            {
                MessageBox.Show("Please enter a valid email address");
            }
            else
            {
                Transaction transaction = new Transaction();
                transaction.amount = Double.Parse(txtAmount.Text);
                transaction.orderId = Guid.NewGuid().ToString("N").Substring(20);
                transaction.emailAddress = txtEmail.Text;
                transaction.customerMobile = txtMobile.Text;

                //-- Initialize
                EzetapPaymentSync ezetap = new EzetapPaymentSync(this, transaction);
                if(ezetap.init() == true)
                {
                    ezetap.ShowDialog();
                }



                /*
                EzetapPayment ezetap = new EzetapPayment(transaction);
                if(ezetap.init() == true)
                {
                    if(ezetap.startApi() == true)
                    {
                        ezetap.ShowDialog();
                    }
                }*/
            }
        }

        private void payByCash()
        {
            DictionaryModel param = new DictionaryModel();
            param.add("bill_id", "");
            param.add("mobile", txtMobile.Text);
            param.add("merchant_id", Global.currentUser.merchant_id.ToString());
            param.add("amount", String.Format("{0:0.00}", Convert.ToDouble(txtAmount.Text)));
            param.add("ref_no", txtRef.Text);
            param.add("name", txtName.Text);
            param.add("email", txtEmail.Text);

            ConfirmCash cash = new ConfirmCash(param.getJSON(), txtMobile.Text, Convert.ToDouble(txtAmount.Text));
            cash.dashboard = this;
            cash.ShowDialog();
        }

        private async void payBySendLink()
        {
            DictionaryModel param = new DictionaryModel();
            param.add("link", "1");
            param.add("mobile", txtMobile.Text);
            param.add("merchant_id", Global.currentUser.merchant_id.ToString());
            param.add("grand_total", String.Format("{0:0.00}", Convert.ToDouble(txtAmount.Text)));
            param.add("bill_no", txtRef.Text);
            param.add("name", txtName.Text);
            param.add("email", txtEmail.Text);

            try
            {
                btnCreateBill.Enabled = false;
                var response = await new RestClient().MakePostRequest("v2/customer/create-bill?token=" + Properties.Settings.Default.accessToken, param.getJSON());
                ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
                if (model.status == 1)
                {
                    clearTextBox();
                    lblPendingBills_Click(null, null);
                    showCurrentActivity(model.data.msg);
                }
                else
                {
                    MessageBox.Show(model.data.msg);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnCreateBill.Enabled = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if(Properties.Settings.Default.accessToken != "")
            {
                Application.Exit();
            }
        }


        private async void getPendingBills()
        {
            isSelectPending = true;
            listView1.Items.Clear();
            try
            {
                DictionaryModel param = new DictionaryModel();
                param.add("token", Properties.Settings.Default.accessToken);
                param.add("merchant_id", Global.currentUser.merchant_id.ToString());
                param.add("status", "0");
                if(dateRange == DateRange.Today)
                {
                    param.add("start", DateTime.Now.ToString("dd-MM-yyyy"));
                }
                else if(dateRange == DateRange.OneWeek)
                {
                    param.add("start", Utility.getPreviousDateDate(-7).ToString("dd-MM-yyyy"));
                }
                else
                {
                    param.add("start", Utility.getPreviousDateDate(-30).ToString("dd-MM-yyyy"));
                }
                param.add("end", DateTime.Now.ToString("dd-MM-yyyy"));
                this.pendingBills = await new RestClient().GetPendingBills(param);
                lblTodaySale.Text = "₹ " + pendingBills.today_sale.ToString("0.00");

                string[] arr = new string[6];
                double totalAmount = 0.0;
                foreach (Bill bill in pendingBills.data.bills)
                {
                    arr[0] = bill.grand_total.ToString("0.00");
                    arr[1] = bill.bill_no;
                    arr[2] = bill.mobile;
                    arr[3] = bill.name;
                    arr[4] = Utility.getDate(bill.sent_at, "dd-MM-yyyy HH:mm:ss", Utility.displayDateFormat); ;
                    listView1.Items.Add(new ListViewItem(arr));
                    totalAmount += bill.grand_total;
                }
                lblTotalAmount.Text = "₹ " + totalAmount.ToString("0.00");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private async void getPaidBills()
        {
            isSelectPending = false;
            listView1.Items.Clear();
            try
            {
                DictionaryModel param = new DictionaryModel();
                param.add("token", Properties.Settings.Default.accessToken);
                param.add("merchant_id", Global.currentUser.merchant_id.ToString());
                param.add("status", "0");
                if (dateRange == DateRange.Today)
                {
                    param.add("start", DateTime.Now.ToString("dd-MM-yyyy"));
                }
                else if (dateRange == DateRange.OneWeek)
                {
                    param.add("start", Utility.getPreviousDateDate(-7).ToString("dd-MM-yyyy"));
                }
                else
                {
                    param.add("start", Utility.getPreviousDateDate(-30).ToString("dd-MM-yyyy"));
                }
                this.paidBills = await new RestClient().GetPaidBills(param);
                lblTodaySale.Text = "₹ " + paidBills.data.today_sale.ToString("0.00");

                string[] arr = new string[6];
                double totalAmount = 0.0;
                foreach (PaidBill bill in paidBills.data.txns)
                {
                    arr[0] = bill.amount.ToString("0.00");
                    arr[1] = bill.bill_no;
                    arr[2] = bill.mobile;
                    arr[3] = bill.name;
                    arr[4] = Utility.getDate(bill.created_at, "dd-MM-yyyy HH:mm:ss", Utility.displayDateFormat);
                    listView1.Items.Add(new ListViewItem(arr));
                    totalAmount += bill.amount;
                }
                lblTotalAmount.Text = "₹ " + totalAmount.ToString("0.00");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void lblPendingBills_Click(object sender, EventArgs e)
        {
            lblPendingBills.ForeColor = System.Drawing.Color.SeaGreen;
            lblPaidBills.ForeColor = System.Drawing.Color.Silver;
            getPendingBills();
        }

        public void lblPaidBills_Click(object sender, EventArgs e)
        {
            lblPendingBills.ForeColor = System.Drawing.Color.Silver;
            lblPaidBills.ForeColor = System.Drawing.Color.SeaGreen;
            getPaidBills();
        }

        private void lblToday_Click(object sender, EventArgs e)
        {
            dateRange = DateRange.Today;
            lblToday.ForeColor = System.Drawing.Color.White;
            lblOneWeek.ForeColor = System.Drawing.Color.Silver;
            lblOneMonth.ForeColor = System.Drawing.Color.Silver;
            updateBillsList();
        }

        private void lblOneWeek_Click(object sender, EventArgs e)
        {
            dateRange = DateRange.OneWeek;
            lblToday.ForeColor = System.Drawing.Color.Silver;
            lblOneWeek.ForeColor = System.Drawing.Color.White;
            lblOneMonth.ForeColor = System.Drawing.Color.Silver;
            updateBillsList();
        }

        private void lblOneMonth_Click(object sender, EventArgs e)
        {
            dateRange = DateRange.OneMonth;
            lblToday.ForeColor = System.Drawing.Color.Silver;
            lblOneWeek.ForeColor = System.Drawing.Color.Silver;
            lblOneMonth.ForeColor = System.Drawing.Color.White;
            updateBillsList();
        }

        public void updateBillsList()
        {
            if(isSelectPending == true)
            {
                getPendingBills();
            }
            else
            {
                getPaidBills();
            }
        }

        public void clearTextBox()
        {
            txtRef.Text = "";
            txtMobile.Text = "";
            txtAmount.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtPaymentMode.SelectedIndex = 0;
        }

        private void label8_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to logout from \""+Global.currentUser.outlet[0].outlet_name+"\"", "PayLo POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Properties.Settings.Default.accessToken = "";
                Properties.Settings.Default.Save();
                login.Show();
                login.showLoginPanel();
                this.Close();
            }
        }

        public void showCurrentActivity(string message)
        {
            toolStripCurrentActivity.Text = "Current Activity: " + message;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}