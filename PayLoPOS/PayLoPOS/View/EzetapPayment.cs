using EzetapApi;
using EzetapApi.models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PayLoPOS.View
{
    public partial class EzetapPayment : Form
    {
        private string apiKey = "74cfc32d-005b-4158-ad63-8c8418c3da8b";
        private string userName = "anand";
        private Boolean isDemo = true;

        private Boolean isInitiate = false;
        private Boolean isLoggedIn = false;

        Transaction transaction;

        private EzeApi api;

        public EzetapPayment(Transaction transaction)
        {
            this.transaction = transaction;
            isInitiate = init();
            InitializeComponent();
        }

        private async void EzetapPayment_Load(object sender, EventArgs e)
        {
            isLoggedIn = await login();

            if (isLoggedIn == true)
            {
                makePayment(this.transaction);
            }
        }

        private void showError(string message)
        {
            lblMessage.ForeColor = Color.Red;
            finalMessage(message);
        }

        private void showSuccess(string message)
        {
            if(message != "API Result: Operation result pending")
            {
                lblMessage.ForeColor = Color.SeaGreen;
                finalMessage(message);
            }
        }

        private void finalMessage(string message)
        {
            Debug.WriteLine(message);
            string msg = message.Replace("NOTIFICATION: code ", "");
            msg = msg.Replace("EPIC_LOGIN_RESULT: code 0 (", "");
            msg = msg.Replace("EPIC_TRANSACTION_RESULT: code 0 (", "");
            msg = msg.Replace(")", "");
            for (int i = 1; i < 30; i++)
            {
                msg = msg.Replace(i.ToString() + " (", "");
            }
            for (int i = 1; i < 9; i++)
            {
                msg = msg.Replace(i.ToString()+"B", "B");
            }
            lblMessage.Text = msg;
        }

        public Boolean init()
        {
            if (!EzeApi.ModuleInit())
            {
                showError("Ezetap module initialization failed. Ensure COM component is registered successfully registered.");
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean startApi()
        {
            showSuccess("Start initialization....");
            AssemblyName name = Assembly.GetExecutingAssembly().GetName();
            EzeConfig cfg = new EzeConfig(name.Name.ToString(), name.Version.ToString(), isDemo);
            api = new EzeApi(cfg);
            if (api.initialize(true) != 0)
            {
                showError("Device Connection Failed....");
                return false;
            }
            else
            {
                showSuccess("Connected....");
                return true;
            }
        }

        public async Task<Boolean> login()
        {
            showSuccess("Start Payment....");/*
            var loginResult = await api.loginWithAppKeyAsync(apiKey, userName, new Progress<string>(txt =>
            {
                this.showSuccess(string.Format("{0}", txt));
            }));

            if (!loginResult.success)
            {
                showError("Device Connection Failed. Please restart device and retry again....");
                return false;
            }
            else
            {
                showSuccess("Ezetap Login Success");
                return true;
            }*/

            return false;
        }

        public async void makePayment(Transaction transaction)
        {
            showSuccess("Start Payment....");/*
            var prepareResult = await api.prepareDeviceAsync(new Progress<string>(txt =>
            {
                showSuccess(string.Format("{0}", txt));
            }));

            if (prepareResult.success)
            {
                var paymentResult = await api.cardTransactionAsync(transaction, new Progress<string>(txt =>
                {
                    showSuccess(string.Format("{0}", txt));
                }));
                
                if (paymentResult.success)
                {
                    showSuccess("Payment success");
                    await logout();
                }
                else
                {
                    showError("Payment Failed");
                    await logout();
                }
            }
            else
            {
                showError("Device Prepare Failed. Please restart device and retry again....");
                await logout();
            }*/
        }

        private async Task logout()
        {/*
            var logoutResult = await api.logoutAsync(new Progress<string>(txt =>
            {
                Debug.WriteLine(string.Format("IprogressLogout: {0}", txt));
            }));
            if (!logoutResult.success)
            {
                Debug.WriteLine("Logged out failed");
            }
            else
            {
                Debug.WriteLine("Logged out success");
            }*/
        }

        protected async override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            await logout();
            api.Exit2();
            api = null;
            Debug.WriteLine("Close MPOS Device");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await logout();
            api.Exit2();
            startApi();
            isLoggedIn = await login();
            if (isLoggedIn == true)
            {
                makePayment(this.transaction);
            }

        }
    }
}
