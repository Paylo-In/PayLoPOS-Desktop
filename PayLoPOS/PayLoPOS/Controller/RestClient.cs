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

        static string baseURL = "http://merchant.sumit.ts.paylo.in";

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

        /*||***********************************************************
         *||  Get user logged in profile
         *||***********************************************************
         */
        public async Task<User> GetProfile()
        {
            var response = await MakeGetRequest("v2/profile/init?token=" + Properties.Settings.Default.accessToken);
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
            var bills = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(response);
            return new PaidBillResponse(bills);
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
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseURL);
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
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseURL);
            var response = await httpClient.GetAsync(method);
            response.EnsureSuccessStatusCode();
            httpClient = null;
            return await response.Content.ReadAsStringAsync();
        }
    }
}
