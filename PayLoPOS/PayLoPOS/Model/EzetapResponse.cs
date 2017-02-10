using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLoPOS.Model
{
    class EzetapResponse
    {
        public Boolean success { get; set; }
        public double amount { get; set; }
        public string authCode { get; set; }
        public string cardTxnTypeDesc { get; set; }
        public string cardType { get; set; }
        public string chargeSlipDate { get; set; }
        public string currencyCode { get; set; }
        public string customerEmail { get; set; }
        public string customerMobile { get; set; }
        public string customerName { get; set; }
        public string customerReceiptUrl { get; set; }
        public string formattedPan { get; set; }
        public string invoiceNumber { get; set; }
        public string nameOnCard { get; set; }
        public string orderNumber { get; set; }
        public string payerName { get; set; }
        public string paymentCardBrand { get; set; }
        public string paymentCardType { get; set; }
        public string paymentMode { get; set; }
        public string pgInvoiceNumber { get; set; }
        public string receiptUrl { get; set; }
        public string txnId { get; set; }
        public string rrNumber { get; set; }
    }
}
