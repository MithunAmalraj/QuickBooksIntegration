using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreateSalesReceiptReqItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateSalesReceiptReqTaxCodeRef
    {
        public string value { get; set; }
    }

    public class CreateSalesReceiptReqSalesItemLineDetail
    {
        public CreateSalesReceiptReqItemRef ItemRef { get; set; }
        public int UnitPrice { get; set; }
        public int Qty { get; set; }
        public CreateSalesReceiptReqTaxCodeRef TaxCodeRef { get; set; }
    }

    public class CreateSalesReceiptReqLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public CreateSalesReceiptReqSalesItemLineDetail SalesItemLineDetail { get; set; }
    }

    public class CreateSalesReceiptReq
    {
        public List<CreateSalesReceiptReqLine> Line { get; set; }
    }
}