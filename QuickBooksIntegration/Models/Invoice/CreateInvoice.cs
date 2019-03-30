using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreateInvoiceReqItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateInvoiceReqSalesItemLineDetail
    {
        public CreateInvoiceReqItemRef ItemRef { get; set; }
    }

    public class CreateInvoiceReqLine
    {
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public CreateInvoiceReqSalesItemLineDetail SalesItemLineDetail { get; set; }
    }

    public class CreateInvoiceReqCustomerRef
    {
        public string value { get; set; }
    }

    public class CreateInvoiceReq
    {
        public List<CreateInvoiceReqLine> Line { get; set; }
        public CreateInvoiceReqCustomerRef CustomerRef { get; set; }
    }
    public class CreateInvoiceRes
    {

    }
}