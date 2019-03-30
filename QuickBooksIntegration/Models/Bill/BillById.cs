using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class BillByIdResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class BillByIdResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class BillByIdResItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class BillByIdResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class BillByIdResItemBasedExpenseLineDetail
    {
        public string BillableStatus { get; set; }
        public BillByIdResItemRef ItemRef { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Qty { get; set; }
        public BillByIdResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class BillByIdResLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string DetailType { get; set; }
        public BillByIdResItemBasedExpenseLineDetail ItemBasedExpenseLineDetail { get; set; }
    }

    public class BillByIdResVendorRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class BillByIdResAPAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class BillByIdResBill
    {
        public string DueDate { get; set; }
        public decimal Balance { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public BillByIdResMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public BillByIdResCurrencyRef CurrencyRef { get; set; }
        public List<BillByIdResLine> Line { get; set; }
        public BillByIdResVendorRef VendorRef { get; set; }
        public BillByIdResAPAccountRef APAccountRef { get; set; }
        public decimal TotalAmt { get; set; }
    }

    public class BillByIdRes
    {
        public BillByIdResBill Bill { get; set; }
        public DateTime time { get; set; }
    }
}