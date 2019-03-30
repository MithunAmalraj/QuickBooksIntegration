using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllBillResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllBillResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillResItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class ReadAllBillResItemBasedExpenseLineDetail
    {
        public string BillableStatus { get; set; }
        public ReadAllBillResItemRef ItemRef { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Qty { get; set; }
        public ReadAllBillResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class ReadAllBillResCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillResAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillResTaxCodeRef2
    {
        public string value { get; set; }
    }

    public class ReadAllBillResAccountBasedExpenseLineDetail
    {
        public ReadAllBillResCustomerRef CustomerRef { get; set; }
        public ReadAllBillResAccountRef AccountRef { get; set; }
        public string BillableStatus { get; set; }
        public ReadAllBillResTaxCodeRef2 TaxCodeRef { get; set; }
    }

    public class ReadAllBillResLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public ReadAllBillResItemBasedExpenseLineDetail ItemBasedExpenseLineDetail { get; set; }
        public ReadAllBillResAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class ReadAllBillResVendorRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillResAPAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillResSalesTermRef
    {
        public string value { get; set; }
    }

    public class ReadAllBillResLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class ReadAllBillResBill
    {
        public string DueDate { get; set; }
        public decimal Balance { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public ReadAllBillResMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public ReadAllBillResCurrencyRef CurrencyRef { get; set; }
        public List<ReadAllBillResLine> Line { get; set; }
        public ReadAllBillResVendorRef VendorRef { get; set; }
        public ReadAllBillResAPAccountRef APAccountRef { get; set; }
        public double TotalAmt { get; set; }
        public ReadAllBillResSalesTermRef SalesTermRef { get; set; }
        public List<ReadAllBillResLinkedTxn> LinkedTxn { get; set; }
    }

    public class ReadAllBillResQueryResponse
    {
        public List<ReadAllBillResBill> Bill { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
        public int totalCount { get; set; }
    }

    public class ReadAllBillRes
    {
        public ReadAllBillResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }
    public class BillsSummary
    {
        public string id { get; set; }
        public string txndate { get; set; }
    }
}