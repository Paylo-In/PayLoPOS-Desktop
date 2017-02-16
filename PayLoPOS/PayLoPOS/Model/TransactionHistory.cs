using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLoPOS.Model
{
    class TransactionHistoryBill
    {
        public long bill_id { get; set; }
        public string txnid { get; set; }
        public string pg_txnid { get; set; }
        public string payment_mode { get; set; }
        public string pg_message { get; set; }
        public string gateway { get; set; }
        public string created_at { get; set; }
        public int status_code { get; set; }
        public string pay_method { get; set; }
        public string txn_status { get; set; }
        public string refund_status { get; set; }
    }

    class TransactionHistoryData
    {
        public List<TransactionHistoryBill> txns { get; set; }
        public string msg { get; set; }
    }

    class TransactionHistory
    {
        public int status { get; set; }
        public TransactionHistoryData data { get; set; }
    }
}
