using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdateBillPaymentReqVendorRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateBillPaymentReqBankAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateBillPaymentReqCheckPayment
    {
        public UpdateBillPaymentReqBankAccountRef BankAccountRef { get; set; }
    }

    public class UpdateBillPaymentReqLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class UpdateBillPaymentReqLine
    {
        public double Amount { get; set; }
        public List<UpdateBillPaymentReqLinkedTxn> LinkedTxn { get; set; }
    }

    public class UpdateBillPaymentReq
    {
        public UpdateBillPaymentReqVendorRef VendorRef { get; set; }
        public string PayType { get; set; }
        public UpdateBillPaymentReqCheckPayment CheckPayment { get; set; }
        public double TotalAmt { get; set; }
        public string PrivateNote { get; set; }
        public List<UpdateBillPaymentReqLine> Line { get; set; }
    }
}