using System;
using System.Windows.Forms;
using PayLoPOS.Model;
using PayLoPOS.Controller;
using System.Web.Script.Serialization;
using EzetapApi.models;
using System.Diagnostics;

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
            statusUserName.Text = "Welcome: "+Global.currentUser.name;
            this.Text = Global.currentUser.getSelectedOutletName();

            if(Global.currentUser.outlet.Count > 1)
            {
                switchOutlet.Visible = true;
            }
            else
            {
                switchOutlet.Visible = false;
            }

            //-- Adding list view columns
            listView1.Columns.Add("S/No", 50);
            listView1.Columns.Add("Ref #", 100);
            listView1.Columns.Add("Customer", 120);
            listView1.Columns.Add("Mobile", 100);
            listView1.Columns.Add("Amount", 100);
            listView1.Columns.Add("Sent At", 200);
            listView1.Columns.Add("Bill Status", 100);
            listView1.Columns.Add("Created By", 120);
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            clearTextBox();
            txtPaymentMode.SelectedIndex = 0;
            isSelectPending = true;
            lblToday_Click(sender, e);
            disableResentOption();

            if(Global.currentUser.outlet.Count > 1 && Global.isLogin == true)
            {
                ChooseOutlet outlet = new ChooseOutlet(this);
                outlet.ShowDialog();
            }
        }

        public void refreshOutlet()
        {
            this.Text = Global.currentUser.getSelectedOutletName();
            if(isSelectPending == true)
            {
                lblPendingBills_Click(null, null);
            }
            else
            {
                lblPaidBills_Click(null, null);
            }
        }

        private void textboxMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
               && !char.IsDigit(e.KeyChar))
                e.Handled = true;
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtMobile.Text.Length != 10)
            {
                MessageBox.Show("Please enter a valid mobile number");
            }
            else if (txtAmount.Text == "")
            {
                MessageBox.Show("Please enter a valid amount");
            }
            else if (txtPaymentMode.Text == "CHOOSE OPTION")
            {
                MessageBox.Show("Please choose payment option");
            }
            else
            {
                Global.updateBill(txtRef.Text, double.Parse(txtAmount.Text), txtMobile.Text, txtEmail.Text, txtName.Text);

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
                else if(txtPaymentMode.Text == "WALLET")
                {
                    payByWallet();
                }
                else if(txtPaymentMode.Text == "UPI")
                {
                    payByUPI();
                }
            }            
        }

        private void payByMPOS()
        {
            //-- Create bill
            DictionaryModel param = new DictionaryModel();
            param.add("mobile", txtMobile.Text);
            param.add("merchant_id", Global.currentUser.merchant_id.ToString());
            param.add("grand_total", String.Format("{0:0.00}", Convert.ToDouble(txtAmount.Text)));
            param.add("bill_no", txtRef.Text);
            param.add("name", txtName.Text);
            param.add("email", txtEmail.Text);

            Transaction transaction = new Transaction();
            transaction.amount = Double.Parse(txtAmount.Text);
            transaction.emailAddress = txtEmail.Text;
            transaction.customerMobile = txtMobile.Text;
            transaction.externalReference2 = "";

            //-- Initialize
            EzetapPaymentSync ezetap = new EzetapPaymentSync(this, null, transaction, param.getJSON());
            ezetap.ShowDialog();
            /*if (ezetap.init() == true)
            {
                ezetap.ShowDialog();
            }*/
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

            ConfirmCash cash = new ConfirmCash(null, param.getJSON(), txtMobile.Text, Convert.ToDouble(txtAmount.Text), 0);
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
                imgLoading.Visible = true;
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
                imgLoading.Visible = false;
            }
        }

        private async void payByUPI()
        {
            if (!Utility.IsEmailValid(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address");
            }
            else
            {
                VPAList list = new VPAList(this, null, txtMobile.Text, "");
                try
                {
                    imgLoading.Visible = true;
                    var response = await new RestClient().GetVPA(txtMobile.Text);
                    imgLoading.Visible = false;
                    list.showVPA(response.data);                    
                }
                catch(Exception ex)
                {
                    imgLoading.Visible = false;
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public async void submitVPA(string vpa, EnterVPA vpaDialog, long billId, string email)
        {
            if(billId == 0)
            {
                try
                {
                    vpaDialog.showLoading(true);
                    var response = await new RestClient().UPIPayment("0", txtMobile.Text, Double.Parse(txtAmount.Text), txtRef.Text, txtName.Text, txtEmail.Text, vpa);
                    if (response.status == 1)
                    {
                        MessageBox.Show(response.data.msg);
                        showCurrentActivity(response.data.msg);
                        vpaDialog.Close();
                        clearTextBox();
                        lblPaidBills_Click(null, null);
                        PaymentStatus ps = new PaymentStatus(1, response.data.msg, "UPI");
                        ps.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    vpaDialog.showLoading(false);
                }
            }
            else
            {
                Bill bill = null;
                foreach (Bill b in pendingBills.data.bills)
                {
                    if (billId == b.id)
                    {
                        bill = b;
                    }
                }

                if (bill != null)
                {
                    try
                    {
                        vpaDialog.showLoading(true);
                        var response = await new RestClient().UPIPayment(bill.id.ToString(), bill.mobile, bill.grand_total, bill.bill_no, bill.name, email, vpa);
                        if (response.status == 1)
                        {
                            MessageBox.Show(response.data.msg);
                            showCurrentActivity(response.data.msg);
                            vpaDialog.Close();
                            clearTextBox();
                            lblPaidBills_Click(null, null);
                            PaymentStatus ps = new PaymentStatus(1, response.data.msg, "UPI");
                            ps.ShowDialog();
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
                    finally
                    {
                        vpaDialog.showLoading(false);
                    }
                }
            }                        
        }

        private async void payByWallet()
        {
            if (!Utility.IsEmailValid(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address");
            }
            else
            {
                //-- Create bill
                DictionaryModel param = new DictionaryModel();
                param.add("mobile", txtMobile.Text);
                param.add("merchant_id", Global.currentUser.merchant_id.ToString());
                param.add("grand_total", String.Format("{0:0.00}", Convert.ToDouble(txtAmount.Text)));
                param.add("bill_no", txtRef.Text);
                param.add("name", txtName.Text);
                param.add("email", txtEmail.Text);
            
                try
                {
                    btnCreateBill.Enabled = false;
                    imgLoading.Visible = true;
                    var response = await new RestClient().MakePostRequest("v2/customer/create-bill?token=" + Properties.Settings.Default.accessToken, param.getJSON());
                    ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
                    btnCreateBill.Enabled = true;
                    imgLoading.Visible = false;
                    if (model.status == 1)
                    {
                        //-- Show wallet list with order  id.
                        WalletList list = new WalletList(this, null, Double.Parse(txtAmount.Text), model.data.order_id, txtMobile.Text, txtEmail.Text);
                        clearTextBox();
                        lblPendingBills_Click(null, null);
                        showCurrentActivity(model.data.msg);
                        list.ShowDialog();
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
                    imgLoading.Visible = false;

                }
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
                listViewLoading.Visible = true;
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

                listView1.Columns.Clear();
                listView1.Columns.Add("S/No", 50);
                listView1.Columns.Add("Ref #", 100);
                listView1.Columns.Add("Customer", 120);
                listView1.Columns.Add("Mobile", 100);
                listView1.Columns.Add("Amount", 100);
                listView1.Columns.Add("Sent At", 200);
                listView1.Columns.Add("Bill Status", 100);
                listView1.Columns.Add("Created By", 120);

                string[] arr = new string[8];
                double totalAmount = 0.0;
                var index = 0;
                foreach (Bill bill in pendingBills.data.bills)
                {
                    arr[0] = (index+1).ToString();
                    arr[1] = bill.bill_no;
                    arr[2] = bill.name;
                    arr[3] = bill.mobile;
                    arr[4] = "₹ " + bill.grand_total.ToString("0.00");
                    arr[5] = Utility.getDate(bill.sent_at, "dd-MM-yyyy HH:mm:ss", Utility.displayDateFormat);
                    arr[6] = "Pending";
                    arr[7] = bill.created_by;
                     
                    ListViewItem item = new ListViewItem(arr);
                    item.Tag = index;
                    index++;
                    listView1.Items.Add(item);
                    totalAmount += bill.grand_total;
                }
                lblTotalAmount.Text = "₹ " + totalAmount.ToString("0.00");
                listViewLoading.Visible = false;
            }
            catch(Exception e)
            {
                if(!e.Message.Contains("An error occurred while sending the request"))
                {
                    MessageBox.Show(e.Message);
                }
                listViewLoading.Visible = false;
            }
        }

        private async void getPaidBills()
        {
            isSelectPending = false;
            listView1.Items.Clear();
            try
            {
                listViewLoading.Visible = true;
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

                string[] arr = new string[13];
                double totalAmount = 0.0;
                var index = 0;

                listView1.Columns.Clear();
                listView1.Columns.Add("S/No", 50);
                listView1.Columns.Add("Ref #", 100);
                listView1.Columns.Add("Order ID", 100);
                listView1.Columns.Add("Transaction ID", 160);
                listView1.Columns.Add("Txn Time", 160);
                listView1.Columns.Add("Customer", 120);
                listView1.Columns.Add("Mobile", 90);
                listView1.Columns.Add("Amount", 60);
                listView1.Columns.Add("Mode", 150);
                listView1.Columns.Add("Txn Status", 80);
                listView1.Columns.Add("Refund Amount", 110);
                listView1.Columns.Add("Created By", 100);

                foreach (PaidBill bill in paidBills.data.txns)
                {
                    arr[0] = (index + 1).ToString();
                    arr[1] = bill.bill_no;
                    arr[2] = bill.orderid.ToString();
                    arr[3] = bill.txnid;
                    arr[4] = Utility.getDate(bill.created_at, "dd-MM-yyyy HH:mm:ss", Utility.displayDateFormat);
                    arr[5] = bill.name;
                    arr[6] = bill.mobile;
                    arr[7] = "₹ " + bill.amount.ToString("0.00");
                    arr[8] = bill.pay_method;
                    arr[9] = bill.refund_status;
                    arr[10] = "₹ " + bill.refunded_amount;
                    arr[11] = bill.created_by;

                    ListViewItem item = new ListViewItem(arr);
                    item.Tag = index;
                    index++;
                    listView1.Items.Add(item);
                    totalAmount += bill.amount;
                }
                lblTotalAmount.Text = "₹ " + totalAmount.ToString("0.00");
                listViewLoading.Visible = false;
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("An error occurred while sending the request"))
                {
                    MessageBox.Show(e.Message);
                }
                listViewLoading.Visible = false;
            }
        }

        public void lblPendingBills_Click(object sender, EventArgs e)
        {
            lblPendingBills.ForeColor = System.Drawing.Color.SeaGreen;
            lblPaidBills.ForeColor = System.Drawing.Color.Silver;
            disableResentOption();
            getPendingBills();
        }

        public void lblPaidBills_Click(object sender, EventArgs e)
        {
            lblPendingBills.ForeColor = System.Drawing.Color.Silver;
            lblPaidBills.ForeColor = System.Drawing.Color.SeaGreen;
            disableResentOption();
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

        public void reloadList()
        {
            if(isSelectPending == true)
            {
                lblPendingBills_Click(null, null);
            }
            else
            {
                lblPaidBills_Click(null, null);
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to logout from \""+Global.currentUser.getSelectedOutletName() + "\"", "PayLo POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                new RestClient().Logout();
                Properties.Settings.Default.accessToken = "";
                Properties.Settings.Default.Save();
                login.Show();
                login.showLoginPanel();
                this.Close();
            }
        }

        public void showCurrentActivity(string message)
        {
            statusCurrentActivity.Text = "Last Activity: " + message;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if(items.Count > 0)
            {
                enableReSentOption();
            }
            else
            {
                disableResentOption();
            }
        }

        private void lblPayment_Click(object sender, EventArgs e)
        {
            
        }

        public async void ReSendPayment(ChoosePaymentOption child,  string mode, long orderId, string email, string mobile="")
        {
            Bill bill = null;
            foreach(Bill b in pendingBills.data.bills)
            {
                if(orderId == b.id)
                {
                    bill = b;
                }
            }

            if (bill != null)
            {
                Global.updateBill(bill.bill_no, bill.grand_total, bill.mobile, bill.email, bill.name);

                if (mode == "CASH")
                {
                    ConfirmCash cash = new ConfirmCash(child, "", bill.mobile, bill.grand_total, orderId);
                    cash.dashboard = this;
                    cash.ShowDialog();
                }
                else if(mode == "MPOS")
                {
                    Transaction transaction = new Transaction();
                    transaction.amount = bill.grand_total;
                    transaction.emailAddress = email;
                    transaction.customerMobile = bill.mobile;
                    transaction.orderId = bill.order_code;
                    transaction.externalReference2 = bill.order_id.ToString();

                    //-- Initialize
                    EzetapPaymentSync ezetap = new EzetapPaymentSync(this, child, transaction, "");
                    ezetap.ShowDialog();
                    /*
                    if (ezetap.init() == true)
                    {
                        ezetap.ShowDialog();
                    }*/
                }
                else if (mode == "UPI")
                {
                    //child.Close();
                    VPAList list = new VPAList(this, null, bill.mobile, bill.order_id.ToString());
                    try
                    {
                        imgLoading.Visible = true;
                        var response = await new RestClient().GetVPA(bill.mobile);
                        imgLoading.Visible = false;
                        list.showVPA(response.data);
                    }
                    catch (Exception ex)
                    {
                        imgLoading.Visible = false;
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (mode == "WALLET")
                {
                    //-- Show wallet list with order  id.
                    WalletList list = new WalletList(this, child, bill.grand_total, bill.order_id, mobile, email);
                    list.ShowDialog();
                }
                //child.Close();
            }            
        }

        private void btnPayNow_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            foreach (ListViewItem item in items)
            {
                Bill bill = pendingBills.data.bills[Int16.Parse(item.Tag.ToString())];
                if (bill != null)
                {
                    ChoosePaymentOption option = new ChoosePaymentOption(this, bill.id, bill.mobile, bill.email, bill.order_id);
                    option.ShowDialog();
                }
                break;
            }
        }

        private void enableReSentOption()
        {
            if(isSelectPending == true)
            {
                btnPayNow.Enabled = true;
                btnViewTransaction.Enabled = true;
                btnPayNow.BackColor = System.Drawing.Color.SteelBlue;
                btnViewTransaction.BackColor = System.Drawing.Color.SteelBlue;
            }
            else
            {
                btnPayNow.Enabled = false;
                btnViewTransaction.Enabled = true;
                btnPayNow.BackColor = System.Drawing.Color.Silver;
                btnViewTransaction.BackColor = System.Drawing.Color.SteelBlue;
            }
        }

        private void disableResentOption()
        {
            btnPayNow.Enabled = false;
            btnViewTransaction.Enabled = false;
            btnPayNow.BackColor = System.Drawing.Color.Silver;
            btnViewTransaction.BackColor = System.Drawing.Color.Silver;
        }

        private void btnViewTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    if(isSelectPending == true)
                    {
                        Bill bill = pendingBills.data.bills[Int16.Parse(item.Tag.ToString())];
                        if (bill != null)
                        {
                            TransactionDetails td = new TransactionDetails(bill.order_id, bill.grand_total, bill.name, bill.mobile, bill.email, bill.bill_no, "", "PENDING");
                            td.ShowDialog();
                        }
                    }
                    else
                    {
                        PaidBill bill = paidBills.data.txns[Int16.Parse(item.Tag.ToString())];
                        if (bill != null)
                        {
                            TransactionDetails td = new TransactionDetails(bill.orderid, bill.amount, bill.name, bill.mobile, bill.email, bill.bill_no, bill.txnid, bill.refund_status);
                            td.ShowDialog();
                        }
                    }                    
                    break;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if(isSelectPending == true)
            {
                btnPayNow_Click(null, null);
            }
            else
            {
                btnViewTransaction_Click(null, null);
            }
        }

        private void switchOutlet_Click(object sender, EventArgs e)
        {
            ChooseOutlet outlet = new ChooseOutlet(this);
            outlet.ShowDialog();
        }
    }
}