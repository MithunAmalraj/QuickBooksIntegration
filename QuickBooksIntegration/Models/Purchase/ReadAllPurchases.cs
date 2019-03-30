using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllPurchaseResAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllPurchaseResValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ReadAllPurchaseResAny
    {
        public string name { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public ReadAllPurchaseResValue value { get; set; }
        public bool nil { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class ReadAllPurchaseResPurchaseEx
    {
        public List<ReadAllPurchaseResAny> any { get; set; }
    }

    public class ReadAllPurchaseResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllPurchaseResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllPurchaseResAccountRef2
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllPurchaseResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class ReadAllPurchaseResAccountBasedExpenseLineDetail
    {
        public ReadAllPurchaseResAccountRef2 AccountRef { get; set; }
        public string BillableStatus { get; set; }
        public ReadAllPurchaseResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class ReadAllPurchaseResLine
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public ReadAllPurchaseResAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class EntityRef
    {
        public string value { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class ReadAllPurchaseResPurchase
    {
        public ReadAllPurchaseResAccountRef AccountRef { get; set; }
        public string PaymentType { get; set; }
        public bool Credit { get; set; }
        public double TotalAmt { get; set; }
        public ReadAllPurchaseResPurchaseEx PurchaseEx { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public ReadAllPurchaseResMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public ReadAllPurchaseResCurrencyRef CurrencyRef { get; set; }
        public List<ReadAllPurchaseResLine> Line { get; set; }
        public EntityRef EntityRef { get; set; }
        public string PrivateNote { get; set; }
    }

    public class ReadAllPurchaseResQueryResponse
    {
        public List<ReadAllPurchaseResPurchase> Purchase { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class ReadAllPurchaseRes
    {
        public ReadAllPurchaseResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }
    public class PurchasesSummary
    {
        public string id { get; set; }
        public string txndate { get; set; }
    }
}