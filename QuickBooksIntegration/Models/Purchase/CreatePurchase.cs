using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreatePurchaseReqAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreatePurchaseReqAccountRef2
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class CreatePurchaseReqAccountBasedExpenseLineDetail
    {
        public CreatePurchaseReqAccountRef2 AccountRef { get; set; }
    }

    public class CreatePurchaseReqLine
    {
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public CreatePurchaseReqAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class CreatePurchaseReq
    {
        public CreatePurchaseReqAccountRef AccountRef { get; set; }
        public string PaymentType { get; set; }
        public List<CreatePurchaseReqLine> Line { get; set; }
    }
}