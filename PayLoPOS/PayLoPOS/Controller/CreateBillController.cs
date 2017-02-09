using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayLoPOS.Model;
using System.Diagnostics;

namespace PayLoPOS.Controller
{
    class CreateBillController
    {
        public static async Task<string> Cash(string jsonParams)
        {
            var response = await new RestClient().MakePostRequest("v2/payment/bill-with-cash", jsonParams);
            Debug.WriteLine("Cash Payment: "+response);
            return response;
        }
    }
}
