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
    }

    class ResponseModel
    {
        public int status { get; set; }
        public ResponseModelData data { get; set; }
    }
}
