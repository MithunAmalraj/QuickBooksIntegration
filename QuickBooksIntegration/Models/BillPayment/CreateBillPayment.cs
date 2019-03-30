using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreateBillPaymentReqVendorRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateBillPaymentReqBankAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateBillPaymentReqCheckPayment
    {
        public CreateBillPaymentReqBankAccountRef BankAccountRef { get; set; }
    }

    public class CreateBillPaymentReqLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class CreateBillPaymentReqLine
    {
        public double Amount { get; set; }
        public List<CreateBillPaymentReqLinkedTxn> LinkedTxn { get; set; }
    }

    public class CreateBillPaymentReq
    {
        public CreateBillPaymentReqVendorRef VendorRef { get; set; }
        public string PayType { get; set; }
        public CreateBillPaymentReqCheckPayment CheckPayment { get; set; }
        public double TotalAmt { get; set; }
        public string PrivateNote { get; set; }
        public List<CreateBillPaymentReqLine> Line { get; set; }
    }
    public class CreateBillPaymentRes
    {

    }
}