using System;
using System.Windows.Forms;
using PayLoPOS.Model;
using PayLoPOS.Controller;
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
            statusUserName.Text = "Welcome: "+Global.currentUser.name;
            this.Text = Global.currentUser.outlet[0].outlet_name;
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            clearTextBox();
            txtPaymentMode.SelectedIndex = 0;
            isSelectPending = true;
            lblToday_Click(sender, e);
            disableResentOption();
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
            if(txtEmail.Text == "")
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

                Transaction transaction = new Transaction();
                transaction.amount = Double.Parse(txtAmount.Text);
                transaction.emailAddress = txtEmail.Text;
                transaction.customerMobile = txtMobile.Text;
                transaction.externalReference2 = "";

                //-- Initialize
                EzetapPaymentSync ezetap = new EzetapPaymentSync(this, transaction, param.getJSON());
                if (ezetap.init() == true)
                {
                    ezetap.ShowDialog();
                }
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

            ConfirmCash cash = new ConfirmCash(param.getJSON(), txtMobile.Text, Convert.ToDouble(txtAmount.Text), 0);
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
                }
                else
                {
                    MessageBox.Show(model.data.msg);
                }
                showCurrentActivity(model.data.msg);
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
            if (txtEmail.Text == "")
            {
                MessageBox.Show("Please enter a valid email address");
            }
            else
            {
                try
                {
                    var response = await new RestClient().UPIPayment("0", txtMobile.Text, Double.Parse(txtAmount.Text), txtRef.Text, txtName.Text, txtEmail.Text, "");
                    if(response.status == 1)
                    {
                        MessageBox.Show(response.data.msg);
                    }
                    else
                    {
                        if(response.data.errorCode == "VPA_REQUIRED")
                        {
                            //-- Open enter VPA
                            EnterVPA vpa = new EnterVPA(this, 0, txtEmail.Text);
                            vpa.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show(response.data.msg);
                        }
                    }
                }
                catch (Exception ex)
                {
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
                        PaymentStatus ps = new PaymentStatus(1, response.data.msg, Double.Parse(txtAmount.Text), "UPI", response.data.bill_id.ToString(), txtMobile.Text);
                        ps.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                        showCurrentActivity(response.data.msg);
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
                            PaymentStatus ps = new PaymentStatus(1, response.data.msg, bill.grand_total, "UPI", response.data.bill_id.ToString(), bill.mobile);
                            ps.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show(response.data.msg);
                            showCurrentActivity(response.data.msg);
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
            if (txtEmail.Text == "")
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
                        WalletList list = new WalletList(this, Double.Parse(txtAmount.Text), model.data.order_id, txtMobile.Text, txtEmail.Text);
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
                listView1.Columns.Add("Amount", 60);
                listView1.Columns.Add("Ref #", 100);
                listView1.Columns.Add("Status", 80);
                listView1.Columns.Add("Staff", 120);
                listView1.Columns.Add("Mobile", 100);
                listView1.Columns.Add("Name", 120);
                listView1.Columns.Add("Email", 200);
                listView1.Columns.Add("Date & Time", 200);

                string[] arr = new string[8];
                double totalAmount = 0.0;
                var index = 0;
                foreach (Bill bill in pendingBills.data.bills)
                {
                    arr[0] = bill.grand_total.ToString("0.00");
                    arr[1] = bill.bill_no;
                    arr[2] = "Pending";
                    arr[3] = bill.created_by;
                    arr[4] = bill.mobile;
                    arr[5] = bill.name;
                    arr[6] = bill.email;
                    arr[7] = Utility.getDate(bill.sent_at, "dd-MM-yyyy HH:mm:ss", Utility.displayDateFormat); ;
                    ListViewItem item = new ListViewItem(arr);
                    item.Tag = index;
                    index++;
                    listView1.Items.Add(item);
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

                string[] arr = new string[10];
                double totalAmount = 0.0;
                var index = 0;

                listView1.Columns.Clear();
                listView1.Columns.Add("Amount", 60);
                listView1.Columns.Add("Status", 80);
                listView1.Columns.Add("Mode", 160);
                listView1.Columns.Add("Staff", 100);
                listView1.Columns.Add("Mobile", 90);
                listView1.Columns.Add("Name", 90);
                listView1.Columns.Add("Transaction ID", 160);
                listView1.Columns.Add("Ref #", 100);
                listView1.Columns.Add("Order ID", 100);
                listView1.Columns.Add("Date & Time", 160);

                foreach (PaidBill bill in paidBills.data.txns)
                {
                    arr[0] = bill.amount.ToString("0.00");
                    if(bill.refund_status == "NA" || bill.refund_status == "N/A")
                    {
                        arr[1] = bill.txn_status;
                    }
                    else
                    {
                        arr[1] = bill.refund_status;
                    }
                    
                    arr[2] = bill.pay_method;
                    arr[3] = bill.created_by;
                    arr[4] = bill.mobile;
                    arr[5] = bill.name;
                    arr[6] = bill.txnid;
                    arr[7] = bill.bill_no;
                    arr[8] = bill.orderid.ToString();
                    arr[9] = Utility.getDate(bill.created_at, "dd-MM-yyyy HH:mm:ss", Utility.displayDateFormat);

                    ListViewItem item = new ListViewItem(arr);
                    item.Tag = index;
                    index++;
                    listView1.Items.Add(item);
                    totalAmount += bill.amount;
                }
                lblTotalAmount.Text = "₹ " + totalAmount.ToString("0.00");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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

        private void label8_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to logout from \""+Global.currentUser.outlet[0].outlet_name+"\"", "PayLo POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        public async void ReSendPayment(ChoosePaymentOption child,  string mode, long orderId, string email)
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
                if (mode == "CASH")
                {
                    ConfirmCash cash = new ConfirmCash("", bill.mobile, bill.grand_total, orderId);
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
                    EzetapPaymentSync ezetap = new EzetapPaymentSync(this, transaction, "");
                    if (ezetap.init() == true)
                    {
                        ezetap.ShowDialog();
                    }
                }
                else if (mode == "UPI")
                {
                    try
                    {
                        child.showLoading(true);
                        var response = await new RestClient().UPIPayment( bill.id.ToString(), bill.mobile, bill.grand_total, bill.bill_no, bill.name, email, "");
                        if (response.status == 1)
                        {
                            MessageBox.Show(response.data.msg);
                        }
                        else
                        {
                            if (response.data.errorCode == "VPA_REQUIRED")
                            {
                                child.showLoading(false);
                                //-- Open enter VPA
                                EnterVPA vpa = new EnterVPA(this, bill.id, bill.email);
                                vpa.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show(response.data.msg);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        child.showLoading(false);
                    }
                }
                else if (mode == "WALLET")
                {
                    //-- Show wallet list with order  id.
                    WalletList list = new WalletList(this, bill.grand_total, bill.order_id, bill.mobile, email);
                    list.ShowDialog();
                }
                child.Close();
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
                            string status = "";
                            if(bill.refund_status == "NA" || bill.refund_status == "N/A")
                            {
                                status = bill.txn_status;
                            }
                            else
                            {
                                status = bill.refund_status;
                            }
                            TransactionDetails td = new TransactionDetails(bill.orderid, bill.amount, bill.name, bill.mobile, "", bill.bill_no, bill.txnid, status);
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
    }
}