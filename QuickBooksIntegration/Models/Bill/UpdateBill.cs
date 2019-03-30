using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdateBillReqAccountRef
    {
        public string value { get; set; }
    }

    public class UpdateBillReqAccountBasedExpenseLineDetail
    {
        public UpdateBillReqAccountRef AccountRef { get; set; }
    }

    public class UpdateBillReqLine
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public UpdateBillReqAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class UpdateBillReqVendorRef
    {
        public string value { get; set; }
    }

    public class UpdateBillReq
    {
        public List<UpdateBillReqLine> Line { get; set; }
        public UpdateBillReq VendorRef { get; set; }
    }
}