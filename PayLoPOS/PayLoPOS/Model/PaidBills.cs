using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayLoPOS.Model
{
    

    class PaymentMode
    {
        public string id { get; set; }
        public string pay_method { get; set; }
    }

    class PaidBill
    {
        public double amount { get; set; }
        public long bill_id { get; set; }
        public string bill_no { get; set; }
        public string created_at { get; set; }
        public string created_by { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public long orderid { get; set; }
        public string payment_mode { get; set; }
        public string pay_method { get; set; }
        public string pg_message { get; set; }
        public string pg_txnid { get; set; }
        public string refund_status { get; set; }
        public int status_code { get; set; }
        public string txnid { get; set; }
        public string txn_status { get; set; }
        public double refunded_amount { get; set; }
        public int is_refund_allowed { get; set; }
        public double refundable_amount { get; set; }
        public string gateway { get; set; }

    }

    class PaidBillsData
    {
        public List<PaymentMode> pmode { get; set; }
        public List<PaidBill> txns { get; set; }
        public double today_sale { get; set; } = 0;
        public string msg { get; set; }

        public PaidBillsData(Dictionary<string, object> param)
        {
            if (Utility.isNotNull(param, "today_sale"))
            {
                today_sale = Convert.ToDouble(param["today_sale"]);
            }

            msg = (string)param["msg"];

            var json = new JavaScriptSerializer().Serialize(param["pmode"]);
            pmode = new JavaScriptSerializer().Deserialize<List<PaymentMode>>(json);

            var json1 = new JavaScriptSerializer().Serialize(param["txns"]);
            txns = new JavaScriptSerializer().Deserialize<List<PaidBill>>(json1);
        }
    }

    class PaidBillResponse
    {
        
        public int status { get; set; }
        public PaidBillsData data { get; set; }

        public PaidBillResponse(Dictionary<string, object> param)
        {
            status = (int)param["status"];

            if (Utility.isNotNull(param, "data"))
            {
                data = new PaidBillsData((Dictionary<string, object>)param["data"]);
            }
        }
    }
}
