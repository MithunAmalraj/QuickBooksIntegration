using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreatePaymentReqCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreatePaymentReqLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class CreatePaymentReqLine
    {
        public double Amount { get; set; }
        public List<CreatePaymentReqLinkedTxn> LinkedTxn { get; set; }
    }

    public class CreatePaymentReq
    {
        public CreatePaymentReqCustomerRef CustomerRef { get; set; }
        public double TotalAmt { get; set; }
        public List<CreatePaymentReqLine> Line { get; set; }
    }
}