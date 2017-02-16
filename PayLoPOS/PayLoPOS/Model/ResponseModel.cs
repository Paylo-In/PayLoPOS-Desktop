using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLoPOS.Model
{
    class ResponseModelData
    {
        public string msg { get; set; }
        public long bill_id { get; set; }
        public long bill { get; set; }
        public string errorCode { get; set; }
        public long order_id { get; set; }
        public string order_code { get; set; }
        public string bill_no { get; set; }
    }

    class ResponseModel
    {
        public int status { get; set; }
        public ResponseModelData data { get; set; }
    }

    //{"data":{"bill":1,"errorCode":"","order_id":3126,"order_code":"60X01-15a3621aed3-2c446"},"status":1}
}
