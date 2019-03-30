using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreateBillReqAccountRef
    {
        public string value { get; set; }
    }

    public class CreateBillReqAccountBasedExpenseLineDetail
    {
        public CreateBillReqAccountRef AccountRef { get; set; }
    }

    public class CreateBillReqLine
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public CreateBillReqAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class CreateBillReqVendorRef
    {
        public string value { get; set; }
    }

    public class CreateBillReq
    {
        public List<CreateBillReqLine> Line { get; set; }
        public CreateBillReqVendorRef VendorRef { get; set; }
    }


    public class CreateBillResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class CreateBillResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateBillResAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateBillResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class CreateBillResAccountBasedExpenseLineDetail
    {
        public CreateBillResAccountRef AccountRef { get; set; }
        public string BillableStatus { get; set; }
        public CreateBillResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class CreateBillResLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public int Amount { get; set; }
        public string DetailType { get; set; }
        public CreateBillResAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class CreateBillResVendorRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateBillResAPAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateBillResBill
    {
        public string DueDate { get; set; }
        public int Balance { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public CreateBillResMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public CreateBillResCurrencyRef CurrencyRef { get; set; }
        public List<CreateBillResLine> Line { get; set; }
        public CreateBillResVendorRef VendorRef { get; set; }
        public CreateBillResAPAccountRef APAccountRef { get; set; }
        public int TotalAmt { get; set; }
    }

    public class CreateBillRes
    {
        public CreateBillResBill Bill { get; set; }
        public DateTime time { get; set; }
    }
}