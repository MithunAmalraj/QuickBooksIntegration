using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models.Invoice
{
    public class InvoiceVoidReq
    {
        public string Id { get; set; }
        public string SyncToken { get; set; }
    }
}