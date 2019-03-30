using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models.SalesReceipt
{
    public class SalesReceiptVoidReq
    {
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
    }
}