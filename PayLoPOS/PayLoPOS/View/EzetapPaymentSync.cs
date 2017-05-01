using System;
using System.Drawing;
using System.Windows.Forms;

using PayLoPOS.Model;
using PayLoPOS.Controller;
using EzetapApi;
using EzetapApi.models;

namespace PayLoPOS.View
{
    using EzetapApi.helper;
    ////////////////Type defs///////////

    using System.Diagnostics;
    using System.Reflection;
    using System.Web.Script.Serialization;

    public partial class EzetapPaymentSync : Form, StatusCallback
    {
        //-- Application Required Keys
        //-- Live
        string apiKey_LIVE = "6d4c3717-9845-4caf-abc0-611018b37af7";
        string userName = "PAYLO";

        //-- Dummy
        string apiKey_TEST = "e73abbf5-f851-4a31-9d3c-44eb986346d0";
        //-----------------------------------------------

        bool hasInitialized = false;
        bool hasLoggedIn = false;
        bool isVoidTxn = false;
        //static EzeApi api;

        private Transaction transaction;
        Dashboard parent;
        ChoosePaymentOption subParent;
        RefundTransaction refundParent;
        string jsonParams;

        string apiKey;

        public EzetapPaymentSync(Dashboard p, ChoosePaymentOption subParent, Transaction txn, string param)
        {
            parent = p;
            this.subParent = subParent;
            transaction = txn;
            jsonParams = param;
            refundParent = null;
            if(Global.isApplicationLive == true)
            {
                apiKey = apiKey_LIVE;
            }
            else
            {
                apiKey = apiKey_TEST;
            }

            InitializeComponent();
        }

        public EzetapPaymentSync(RefundTransaction parent, Dashboard dashboard, Transaction txn)
        {
            this.parent = dashboard;
            subParent = null;
            refundParent = parent;
            transaction = txn;

            if (Global.isApplicationLive == true)
            {
                apiKey = apiKey_LIVE;
            }
            else
            {
                apiKey = apiKey_TEST;
            }

            InitializeComponent();
        }

        private void EzetapPaymentSync_Load(object sender, EventArgs e)
        {
            showSuccess("Connecting...");
            initializeEzetap();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            showSuccess("Connecting...");
            initializeEzetap();
        }

        public void initializeEzetap()
        {
            if (!hasInitialized && Global.api == null)
            {
                hasInitialized = true;
                AssemblyName name = Assembly.GetExecutingAssembly().GetName();
                EzeConfig cfg = new EzeConfig(name.Name.ToString(), name.Version.ToString(), !(Global.isApplicationLive), true);
                Debug.WriteLine("Initialized api");
                Global.api = new EzeApi(cfg);
                if (Global.api.initialize(true) != 0)
                {
                    showError("Ezetap connection failed");
                }
            }

            showSuccess("Ezetap device is connected");
            EzetapLogin();
        }

        public async void EzetapLogin()
        {
            if (!hasLoggedIn)
            {
                showSuccess("Connecting...");
                var loginResult = await Global.api.loginWithAppKeyAsync(apiKey, userName, new Progress<EzetapProgressMsg>(txt =>
                {
                    showSuccess(txt.msg);
                }));

                if (!loginResult.success)
                {
                    showError("Login Failed. " + loginResult.msg.msg);
                }
                else
                {
                    hasLoggedIn = true;
                }
            }

            prepareDevice();
        }

        public async void prepareDevice()
        {
            var prepareResult = await Global.api.prepareDeviceAsync(new Progress<EzetapProgressMsg>(txt =>
            {
                Debug.Write(txt.msg);
            }));

            if (prepareResult.success)
            {
                if (transaction.externalReference2 == "")
                {
                    createBill();
                }
                else
                {
                    EzetapMakePayment();
                }
            }
            else
            {
                showError(prepareResult.msg.msg);
            }
        }

        DelegatedCallback callbackTester = new DelegatedCallback(null);
        public void AppendText(EzetapProgressMsg text)
        {
            if(isVoidTxn == true)
            {
                callbackTester.AppendText(text);
                Action<EzetapProgressMsg> d1 = SetCallbackText;
                this.Invoke(d1, text);
            }
        }

        void SetCallbackText(EzetapProgressMsg text)
        {
            if(isVoidTxn == true)
            {
                if (text.ret == 0)
                {
                    if(refundParent != null)
                    {
                        refundParent.Close();
                    }
                    logout(true);
                    parent.lblPaidBills_Click(null, null);
                    parent.showCurrentActivity("Ezytap - Refund initiated successfully");
                    showSuccess(text.msg);
                }
                else if (text.ret != 1)
                {
                    showError(text.msg);
                }

                string s1;
                s1 = string.Format("{0} {1} {2} => {3}", DelegatedCallback.isComplete() ? "Complete" : "In Progress", text.ret, text.codeMsg, text.Text);
                Debug.WriteLine(s1);
            }
        }

        public void EzytapVoidTransaction()
        {
            showSuccess("Start refund processing...");
            isVoidTxn = true;
            Global.api.voidTransaction(this, transaction.orderId);
        }

        public async void EzetapMakePayment()
        {
            if (hasLoggedIn)
            {

                if (transaction.externalReference2 == "VoidTxn")
                {
                    EzytapVoidTransaction();
                }
                else
                {
                    showSuccess("Start payment processing...");
                    var paymentResult = await Global.api.cardTransactionAsync(transaction, new Progress<EzetapProgressMsg>(txt =>
                    {
                        showSuccess(txt.msg);
                    }));

                    if (paymentResult.success)
                    {
                        logout(paymentResult.success);
                        updatePayment((paymentResult.responseObj as TransactionResponse));
                    }
                    else
                    {
                        logout(false);
                        showError(paymentResult.msg.msg);
                    }
                }
            }
        }

        private async void logout(bool updateAmount)
        {
            hasLoggedIn = false;
            var logoutResult = await Global.api.logoutAsync(new Progress<EzetapProgressMsg>(txt =>
            {
                Debug.WriteLine(string.Format("IprogressLogout: {0}", txt));
            }));
        }

        public async void createBill()
        {
            try
            {
                var response = await new RestClient().MakePostRequest("v2/customer/create-bill?token=" + Properties.Settings.Default.accessToken, jsonParams);
                Debug.WriteLine(response);
                ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
                if (model.status == 1)
                {
                    if (model.data.order_code != null)
                    {
                        parent.clearTextBox();
                        transaction.orderId = model.data.order_code;
                        transaction.externalReference2 = model.data.order_id.ToString();
                        EzetapMakePayment();
                    }
                    else
                    {
                        showError("Order code not found");
                    }
                }
                else
                {
                    showError(model.data.msg);
                }
            }
            catch(Exception ex)
            {
                showError(ex.Message);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            /*if (Global.api != null)
            {
                Global.api.Exit();
                Debug.WriteLine("Close Ezetap API");
            }*/
        }
        
        private async void updatePayment(TransactionResponse txnResponse)
        {
            try
            {
                var json = new JavaScriptSerializer().Serialize(txnResponse);
                showSuccess("Validating payment ...");
                Debug.WriteLine("Ezetap Response:" + json);
                ResponseModel model = await new RestClient().UpdateEzetapPayment(json);
                if (model.status == 1)
                {
                    showSuccess("Payment Success");
                    parent.lblPaidBills_Click(null, null);
                    parent.showCurrentActivity(model.data.msg);
                    parent.clearTextBox();
                    PaymentStatus ps = new PaymentStatus(1, model.data.msg, "MPOS");
                    ps.ShowDialog();
                }
                else
                {
                    showError("Payment Failed");
                    parent.lblPendingBills_Click(null, null);
                    PaymentStatus ps = new PaymentStatus(0, model.data.msg, "MPOS");
                    ps.ShowDialog();
                }
                this.Close();
                if(subParent != null)
                {
                    subParent.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void showError(string message)
        {
            if (message.Contains("Operation result pending"))
            {
                return;
            }
            button1.Visible = true;
            lblMessage.ForeColor = Color.Red;
            finalMessage(message);
        }

        private void showSuccess(string message)
        {
            if (message.Contains("Operation result pending")){
                return;
            }
            else if (message.Contains("Operation completed successfully"))
            {
                message = "Refund initiated successfully";
            }
            button1.Visible = false;
            lblMessage.ForeColor = Color.SeaGreen;
            finalMessage(message);
        }

        private void finalMessage(string message)
        {
            Debug.WriteLine(message);
            lblMessage.Text = message;
        }
    }
}
