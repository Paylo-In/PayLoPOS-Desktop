﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EzetapApi.models;
using EzetapApi;
using PayLoPOS.Model;

namespace PayLoPOS.View
{
    ////////////////Type defs///////////
    using EPIC_STATUS = System.Int32;
    using EPIC_BOOL = System.Int32;

    using System.Diagnostics;
    using System.Web.Script.Serialization;

    enum APIName
    {
        LOGIN = 1,
        PREPARE,
        TRANSACTION
    }

    public partial class EzetapPaymentSync : Form, StatusCallback
    {
        EzeConfig config = new EzeConfig("EzetapWindowsSdkDemo", "1.0.0.0");

        EzeApi appLib;

        private Transaction transaction;

        Form parent;

        string usesDetails;

        APIName apiName;

        public EzetapPaymentSync(Form p, Transaction txn)
        {
            parent = p;
            this.transaction = txn;
            InitializeComponent();
        }

        private void EzetapPaymentSync_Load(object sender, EventArgs e)
        {
            showSuccess("Connecting...");
            apiName = APIName.LOGIN;
            usesDetails = appLib.usage("");
            appLib.login(this, "EztapDemo", "74cfc32d-005b-4158-ad63-8c8418c3da8b", "");
        }

        public bool init()
        {
            //-- Initialize
            appLib = new EzeApi(config);
            if (appLib.initialize(true) == 0)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Initilization Failed");
                return false;
            }
        }

        List<string> strAppendText = new List<string>();
        public void AppendText(string text)
        {
            string s1 = string.Format("{0}", text);
            Action<string> d1 = SetCallbackText;
            parent.Invoke(d1, s1);
        }
        void SetCallbackText(string str)
        {
            Debug.WriteLine(str);
            if(apiName == APIName.LOGIN)
            {
                if(str.Contains("SERVER_RESULT: Login message: "))
                {
                    EzetapResponse response = new JavaScriptSerializer().Deserialize<EzetapResponse>(str.Replace("SERVER_RESULT: Login message: ", ""));
                    if(response.success == true)
                    {
                        showSuccess("Prepare Device ...");
                        apiName = APIName.PREPARE;
                        usesDetails = appLib.usage("preparedevice");
                        appLib.prepareDevice(this);
                    }
                    else
                    {
                        showError("Device failed to Prepare. Please reconnect the device and try again.");
                    }
                }
                else if(str.Contains("API Result: -2147418109 ("))
                {
                    showError("Device failed to connect.Please reconnect the device and try again.");
                }
            }
            else if(apiName == APIName.PREPARE)
            {
                if (str.Contains("NOTIFICATION: code 2 ("))
                {
                    showSuccess("Validating device session with server");
                }
                else if(str.Contains("EPIC_PREPARE_RESULT: code -2147418093 "))
                {
                    showError("Could not communicate with Ezetap device. Please reconnect the device and try again.");
                }
                else if (str.Contains("EPIC_PREPARE_RESULT: code 0 ("))
                {
                    showSuccess("Device connected ... ");
                    apiName = APIName.TRANSACTION;
                    usesDetails = appLib.usage("transaction <orderId> <amount> [amountOther] [Reference2] [Reference3] [customerMobile] [customerEmail]");
                    appLib.cardTransaction(this, transaction);
                }
            }
            else if (apiName == APIName.TRANSACTION)
            {
                Debug.WriteLine(str);
                if (str.Contains("NOTIFICATION: code 1 ("))
                {
                    showSuccess("Identifying device");
                }
                else if (str.Contains("NOTIFICATION: code 4 ("))
                {
                    showSuccess("Preparing device for transaction");
                }
                else if (str.Contains("NOTIFICATION: code 5 ("))
                {
                    showSuccess("Please Swipe or Insert Card");
                }
                else if (str.Contains("NOTIFICATION: code 23 ("))
                {
                    showSuccess("Bad swipe detected. Please Swipe card again");
                }
                else if (str.Contains("NOTIFICATION: code 8 ("))
                {
                    showSuccess("Please enter PIN on device pinpad within 30 seconds");
                }
                else if (str.Contains("NOTIFICATION: code 9 ("))
                {
                    showSuccess("PIN is entered");
                }
                else if (str.Contains("NOTIFICATION: code 6 ("))
                {
                    showSuccess("Reading data from device");
                }
                else if (str.Contains("NOTIFICATION: code 10 ("))
                {
                    showSuccess("Authorization by server is in progress");
                }
                else if (str.Contains("EPIC_TRANSACTION_RESULT: code 0 ("))
                {
                    showSuccess("Authorization by server is in progress");
                }
                else if (str.Contains("NOTIFICATION: code 12 ("))
                {
                    showSuccess("Please Swipe or Insert card again");
                }
                else if (str.Contains("SERVER_RESULT: Txn ID:"))
                {
                    showSuccess(str);
                    EzetapResponse response = new JavaScriptSerializer().Deserialize<EzetapResponse>(getJSONString(str));
                    if(response.success == true)
                    {
                        showSuccess("Payment Success");
                    }
                    else
                    {
                        showError("Payment Failed");
                    }
                }
            }
        }

        private string getJSONString(string str)
        {
            string json = "";
            string[] temp = str.Split('{');
            bool isFirst = false;
            foreach (var s in temp)
            {
                if (isFirst)
                {
                    json += "{" + s;
                }
                isFirst = true;
            }
            return json;
        }

        private void showError(string message)
        {
            lblMessage.ForeColor = Color.Red;
            finalMessage(message);
        }

        private void showSuccess(string message)
        {
            if (message != "API Result: Operation result pending")
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
                msg = msg.Replace(i.ToString() + "B", "B");
            }
            lblMessage.Text = msg;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }
    }
}