using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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
        public string externalRefNumber2 { get; set; }
        public string merchantCode { get; set; }
        public string merchantName { get; set; }
        public  string tid { get; set; }
        public string mid { get; set; }
        public string deviceSerial { get; set; }

        public string getPayLoJSON()
        {
            string json = new JavaScriptSerializer().Serialize(
                new { error = "", status = (success == true)?"success":"failed", result  = new {
                    references = new { reference2 = orderNumber, reference1 = externalRefNumber2 },
                    receipt = new { receiptUrl = receiptUrl, receiptDate = chargeSlipDate },
                    customer = new { email  = customerEmail, mobileNo  = customerMobile, name  = customerName},
                    merchant = new { merchantCode = merchantCode, merchantName = merchantName },
                    txn = new { amount = amount, authCode = authCode, paymentMode = paymentMode, currencyCode = currencyCode, emiId = "", tid = tid, mid = mid, txnId = txnId, deviceSerial = deviceSerial, txnDate = chargeSlipDate },
                    cardDetails = new { cardBrand = paymentCardBrand, maskedCardNo = formattedPan }
                } }
                );
            return json;
        }
    }
}
