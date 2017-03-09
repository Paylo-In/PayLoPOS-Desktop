using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using PayLoPOS.Model;
using PayLoPOS.Controller;

using EzetapApi.helper;
using EzetapApi;
using EzetapApi.models;

namespace PayLoPOS.View
{
    ////////////////Type defs///////////

    using System.Diagnostics;
    using System.Reflection;
    using System.Web.Script.Serialization;

    public partial class EzetapPaymentSync : Form
    {
        //-- Application Required Keys
        //-- Live
        string apiKey = "6d4c3717-9845-4caf-abc0-611018b37af7";
        string userName = "PAYLO";
        bool isDemo = false;

        //-- Dummy
        //string apiKey = "74cfc32d-005b-4158-ad63-8c8418c3da8b";
        //string userName = "PAYLO";
        //bool isDemo = true;
        //-----------------------------------------------


        bool hasInitialized = false;
        bool hasLoggedIn = false;
        EzeApi api;

        private Transaction transaction;
        Dashboard parent;
        ChoosePaymentOption subParent;
        string jsonParams;

        public EzetapPaymentSync(Dashboard p, ChoosePaymentOption subParent, Transaction txn, string param)
        {
            parent = p;
            this.subParent = subParent;
            transaction = txn;
            jsonParams = param;
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
            if (!hasInitialized)
            {
                hasInitialized = true;
                AssemblyName name = Assembly.GetExecutingAssembly().GetName();
                EzeConfig cfg = new EzeConfig(name.Name.ToString(), name.Version.ToString(), isDemo, true);
                api = new EzeApi(cfg);
                if (api.initialize(true) != 0)
                {
                    showError("Ezetap Initialization Failed");
                }
            }
            EzetapLogin();
        }

        public async void EzetapLogin()
        {
            if (!hasLoggedIn)
            {
                showSuccess("Authenticating User...");
                var loginResult = await api.loginWithAppKeyAsync(apiKey, userName, new Progress<EzetapProgressMsg>(txt =>
                {
                    showSuccess(string.Format("IprogressLogin: {0}", txt));
                }));

                if (!loginResult.success)
                {
                    showError("Login Failed. " + loginResult.msg.Text);
                }
                else
                {
                    hasLoggedIn = true;
                }
            }

            if (hasLoggedIn)
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
        }

        public async void EzetapMakePayment()
        {
            if (hasLoggedIn)
            {
                showSuccess("Preparing Device...");
                var paymentResult = await api.cardTransactionAsync(transaction, new Progress<EzetapProgressMsg>(txt =>
                {
                    showSuccess(txt.Text);
                }));

                if (paymentResult.success)
                {
                    updatePayment(((TransactionResponse)paymentResult.responseObj));
                }
                else
                {
                    showError(paymentResult.msg.Text);
                }
            }
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
            if (api != null)
            {
                api.Exit();
            }
        }

        private void parseMessage(string message)
        {
            if(message.Contains("SERVER_RESULT: code "))
            {
                message = message.Replace("SERVER_RESULT: code ", "");
                string[] temp = message.Split(' ');
                message = message.Replace(temp[0] + " (", "");
                message = message.Substring(0, message.Length - 1);
                showError(message);
                button1.Visible = true;
            }
            else if(message.Contains("EPIC_TRANSACTION_RESULT: code "))
            {
                message = message.Replace("EPIC_TRANSACTION_RESULT: code ", "");
                string[] temp = message.Split(' ');
                message = message.Replace(temp[0] + " (", "");
                message = message.Substring(0, message.Length - 1);
                showError(message);
                button1.Visible = true;
            }
        }
        
        private async void updatePayment(TransactionResponse txnResponse)
        {
            try
            {
                var json = new JavaScriptSerializer().Serialize(txnResponse);
                showSuccess("Validating payment ...");
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
            button1.Visible = true;
            lblMessage.ForeColor = Color.Red;
            finalMessage(message);
        }

        private void showSuccess(string message)
        {
            button1.Visible = false;
            lblMessage.ForeColor = Color.SeaGreen;
            finalMessage(message);
        }

        private void finalMessage(string message)
        {
            Debug.WriteLine(message);

            if(message.Contains("NOTIFICATION: "))
            {
                message = message.Replace("NOTIFICATION: ", "");
                string[] temp = message.Split(' ');
                message = message.Replace(temp[0] + " ", "");
            }
            else if (message.Contains("EPIC_TRANSACTION_RESULT: "))
            {
                message = message.Replace("EPIC_TRANSACTION_RESULT: ", "");
                string[] temp = message.Split(' ');

                if (temp[1].Contains("EZETAP_"))
                {
                    message = message.Replace(temp[0] + " " + temp[1], "");
                }
                else
                {
                    message = message.Replace(temp[0] + " ", "");
                }                
            }
            else if(message.Contains("API_Result: "))
            {
                message = lblMessage.Text;
            }

            lblMessage.Text = message;
        }
    }
}
