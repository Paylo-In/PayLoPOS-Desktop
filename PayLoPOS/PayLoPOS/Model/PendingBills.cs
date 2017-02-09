using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace PayLoPOS.Model
{
    class Bill
    {
        public long id { get; set; }
        public string bill_no { get; set; }
        public double grand_total { get; set; }
        public double sub_total { get; set; }
        public double service_tax { get; set; }
        public double service_charge { get; set; }
        public double vat { get; set; }
        public double local_tax { get; set; }
        public double round_off { get; set; }
        public string order_items { get; set; }
        public long order_id { get; set; }
        public string order_code { get; set; }
        public long merchant_user_id { get; set; }
        public string comment { get; set; }
        public string sent_at { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string created_by { get; set; }
        public string expire_at { get; set; }
        public string outlet_name { get; set; }
        public int bill_status { get; set; }
    }

    class BillData
    {
        public List<Bill> bills { get; set; }

    }

    class PendingBills 
    {
        public int status { get; set; } = 0;
        public double today_sale { set; get; } = 0;
        public BillData data { get; set; }

        public PendingBills(Dictionary<string, object> param)
        {
            if(Utility.isNotNull(param, "today_sale"))
            {
                today_sale = Convert.ToDouble(param["today_sale"]);
            }

            if (Utility.isNotNull(param, "status"))
            {
                status = (int)param["status"];
            }

            if (Utility.isNotNull(param, "data"))
            {
                var json = new JavaScriptSerializer().Serialize(param["data"]);
                data = new JavaScriptSerializer().Deserialize<BillData>(json);
            }
        }
    }
}
