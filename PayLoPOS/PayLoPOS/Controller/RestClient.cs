using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Text;
using PayLoPOS.Model;
using System.Collections.Generic;

namespace PayLoPOS.Controller
{
    class RestClient
    {

        static string baseURL_TEST = "http://merchant.neelam.ts.paylo.in";
        static string baseURL_LIVE = "http://merchant.paylo.in";

        /*||***********************************************************
         *||  Login user
         *||***********************************************************
         *|| @param: mobile - Mobile number provided by user.
         *|| @param: passcode - Password provided by user. 
         *||***********************************************************
         */
        public async Task<User> Login(string mobile, string passcode)
        {
            var json = new JavaScriptSerializer().Serialize(new { mobile = mobile, passcode = passcode });
            var response = await MakePostRequest("v2/auth/verify-passcode", json);
            User user = new JavaScriptSerializer().Deserialize<User>(response);
            return user;
        }

        public async Task<ResponseModel> GenerateOTP(string mobile)
        {
            var json = new JavaScriptSerializer().Serialize(new { mobile = mobile });
            var response = await MakePostRequest("v2/auth/generate-otp", json);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<ResponseModel> ResendOTP(string mobile)
        {
            var json = new JavaScriptSerializer().Serialize(new { mobile = mobile });
            var response = await MakePostRequest("sign-up/resend-otp", json);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<User> CreatePassword(string mobile, string password, string otp)
        {
            var json = new JavaScriptSerializer().Serialize(new { mobile = mobile, passcode = password, otp = otp });
            var response = await MakePostRequest("v2/auth/verify-otp", json);
            User model = new JavaScriptSerializer().Deserialize<User>(response);
            return model;
        }

        /*||***********************************************************
         *||  Get user logged in profile
         *||***********************************************************
         */
        public async Task<User> GetProfile()
        {
            var response = await MakeGetRequest("v2/profile/init?token=" + Properties.Settings.Default.accessToken);
            Debug.WriteLine("User Profile:" + response);
            User user = new JavaScriptSerializer().Deserialize<User>(response);
            return user;
        }

        /*||***********************************************************
         *||  Get user pending bills
         *||***********************************************************
         *|| @param: param - Parameter dictionary for key valye pair.
         *||***********************************************************
         */
        public async Task<PendingBills> GetPendingBills(DictionaryModel param)
        {
            Debug.WriteLine(param.getStringUri());
            var response = await MakeGetRequest("v2/customer/bills?" + param.getStringUri());
            var bills = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(response);
            return new PendingBills(bills);
        }

        /*||***********************************************************
         *||  Get user paid bills
         *||***********************************************************
         *|| @param: param - Parameter dictionary for key valye pair.
         *||***********************************************************
         */
        public async Task<PaidBillResponse> GetPaidBills(DictionaryModel param)
        {
            var response = await MakeGetRequest("v2/txn/transaction-history?" + param.getStringUri());
            Debug.WriteLine("Paid Bills: " + response);
            var bills = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(response);
            return new PaidBillResponse(bills);
        }

        public async Task<User> SwitchOutlet(string outletId, string accessToken)
        {
            var json = new JavaScriptSerializer().Serialize(new { outlet_id = outletId });
            Debug.WriteLine("Switch Outlet: " + json + "    Token:" + accessToken);
            var response = await MakePostRequest("v2/profile/swtich-outlet?token=" + accessToken, json);
            Debug.WriteLine("Switch Outlet: " + response);
            User user = new JavaScriptSerializer().Deserialize<User>(response);
            return user;
        }

        public async Task<VPAResponse> GetVPA(string mobile)
        {
            var response = await MakeGetRequest("v2/customer/vpa?mobile=" + mobile + "&token=" + Properties.Settings.Default.accessToken);
            Debug.WriteLine("VPA List: " + response);
            VPAResponse res = new JavaScriptSerializer().Deserialize<VPAResponse>(response);
            return res;
        }

        public async Task<TransactionHistory> GetTransactionDetails(long orderId)
        {
            var response = await MakeGetRequest("v2/txn/transaction-history?orderid="+orderId.ToString()+"&token=" + Properties.Settings.Default.accessToken);
            Debug.WriteLine("Transaction Details:" + response);
            TransactionHistory bills = new JavaScriptSerializer().Deserialize<TransactionHistory>(response);
            return bills;
        }

        public async Task<ResponseModel> UpdateEzetapPayment(string json)
        {
            var response = await MakePostRequest("v2/payment/ezytap-desktop-payment-info?token=" + Properties.Settings.Default.accessToken, json);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<ResponseModel> GenerateWalletOTP(string json)
        {
            var response = await MakePostRequest("v2/payment/wallet-otp?token=" + Properties.Settings.Default.accessToken, json);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<ResponseModel> WalletPayment(long orderId, string walletName, string otp, string mobile, string email)
        {
            var json = new JavaScriptSerializer().Serialize(new { order_id = orderId, wallet = walletName, otp = otp, mobile = mobile, email = email });
            Debug.WriteLine("WalletPayment Params:" + json);
            var response = await MakePostRequest("v2/payment/wallet-pay?token=" + Properties.Settings.Default.accessToken, json);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<ResponseModel> ResendUPIPayment(string orderId, string mobile, string vpa)
        {
            var json = new JavaScriptSerializer().Serialize(new { order_id = orderId, mobile = mobile, vpa = vpa, upi = 1 });
            var response = await MakePostRequest("v2/customer/resend-bill?token=" + Properties.Settings.Default.accessToken, json);
            Debug.WriteLine("Params:" + json + " Resend UPI Bill:" + response);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<ResponseModel> UPIPayment(string billId, string mobile, double amount, string billNo, string name, string email, string vpa)
        {
            var json = new JavaScriptSerializer().Serialize(new { bill_id = billId, mobile = mobile, merchant_id = Global.currentUser.merchant_id, grand_total = amount.ToString("0.00"), bill_no = billNo, name = name, email = email, upi = 1, vpa = vpa });
            var response = await MakePostRequest("v2/customer/create-bill?token=" + Properties.Settings.Default.accessToken, json);
            Debug.WriteLine("Create UPI Bill:" + response);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<ResponseModel> authenticateUser(double amount, string txnId, string password)
        {
            var json = new JavaScriptSerializer().Serialize(new { mobile = Global.currentUser.mobile, passcode = password, txnid = txnId, amount = amount });
            var response = await MakePostRequest("v2/auth/auth-user?token=" + Properties.Settings.Default.accessToken, json);
            Debug.WriteLine("Check User:" + response);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<ResponseModel> refundTransaction(double amount, string txnId, string password)
        {
            var json = new JavaScriptSerializer().Serialize(new { mobile = Global.currentUser.mobile, passcode = password, txnid = txnId, amount = amount});
            var response = await MakePostRequest("v2/auth/auth-user?token=" + Properties.Settings.Default.accessToken, json);
            Debug.WriteLine("Check User:" + response);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            if(model.status == 1)
            {
                var jsonRefund = new JavaScriptSerializer().Serialize(new { txnid = txnId, amount = amount, merchant_id = Global.currentUser.merchant_id });
                var refundResponse = await MakePostRequest("v2/payment/refund-transaction?token=" + Properties.Settings.Default.accessToken, json);
                Debug.WriteLine("Refund:" + refundResponse);
                ResponseModel refundModel = new JavaScriptSerializer().Deserialize<ResponseModel>(refundResponse);
                return refundModel;
            }
            else
            {
                return model;
            }
        }

        public async Task<ResponseModel> PayByCashSync(long orderId)
        {
            var json = new JavaScriptSerializer().Serialize(new { bill_id = orderId, timestamp = Utility.getCurrentDate("yyyy-MM-dd HH:mm:ss"), merchant_id = Global.currentUser.merchant_id });
            Debug.WriteLine("Cash JSON: " + json);
            var response = await MakePostRequest("v2/payment/pay-by-cash-sync?token=" + Properties.Settings.Default.accessToken, json);
            Debug.WriteLine("PayByCashSync:" + response);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async Task<ResponseModel> ResendLinkPayment(long orderId, string mobile = "")
        {
            var json = new JavaScriptSerializer().Serialize(new { order_id = orderId, link = 1, mobile = mobile});
            var response = await MakePostRequest("v2/customer/resend-bill?token=" + Properties.Settings.Default.accessToken, json);
            Debug.WriteLine("Params:" + json + "ResendLinkPayment:" + response);
            ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(response);
            return model;
        }

        public async void Logout()
        {
            var response = await MakeGetRequest("v2/auth/logout?token=" + Properties.Settings.Default.accessToken);
            Debug.WriteLine("Logout:" + response);
        }

        /*||***********************************************************
         *|| Make POST request or call server POST API
         *||***********************************************************
         *|| @param: method - Method for calling api.
         *|| @param: json - JSON data for sending. 
         *||***********************************************************
         */
        public async Task<string> MakePostRequest(string method, string json)
        {
            if(Global.isInternetConnected() == false)
            {
                throw new Exception("No internet connection...");
            }

            HttpClient httpClient = new HttpClient();
            if(Global.isApplicationLive == true)
            {
                httpClient.BaseAddress = new Uri(baseURL_LIVE);
            }
            else
            {
                httpClient.BaseAddress = new Uri(baseURL_TEST);
            }
            
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.PostAsync(method, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            httpClient = null;
            return await response.Content.ReadAsStringAsync();
        }

        /*||***********************************************************
         *|| Make Get request or call server GET API
         *||***********************************************************
         *|| @param: method - Method for calling api.
         *||***********************************************************
         */
        private async Task<string> MakeGetRequest(string method)
        {
            if (Global.isInternetConnected() == false)
            {
                throw new Exception("No internet connection...");
            }

            HttpClient httpClient = new HttpClient();
            if (Global.isApplicationLive == true)
            {
                httpClient.BaseAddress = new Uri(baseURL_LIVE);
            }
            else
            {
                httpClient.BaseAddress = new Uri(baseURL_TEST);
            }

            var response = await httpClient.GetAsync(method);
            response.EnsureSuccessStatusCode();
            httpClient = null;
            return await response.Content.ReadAsStringAsync();
        }

    }
}
