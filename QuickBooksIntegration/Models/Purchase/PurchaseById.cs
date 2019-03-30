using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class PurchaseByIdResAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class PurchaseByIdResValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PurchaseByIdResAny
    {
        public string name { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public PurchaseByIdResValue value { get; set; }
        public bool nil { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class PurchaseByIdResPurchaseEx
    {
        public List<PurchaseByIdResAny> any { get; set; }
    }

    public class PurchaseByIdResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class PurchaseByIdResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class PurchaseByIdResAccountRef2
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class PurchaseByIdResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class PurchaseByIdResAccountBasedExpenseLineDetail
    {
        public PurchaseByIdResAccountRef2 AccountRef { get; set; }
        public string BillableStatus { get; set; }
        public PurchaseByIdResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class PurchaseByIdResLine
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string DetailType { get; set; }
        public PurchaseByIdResAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class PurchaseByIdResPurchase
    {
        public PurchaseByIdResAccountRef AccountRef { get; set; }
        public string PaymentType { get; set; }
        public bool Credit { get; set; }
        public int TotalAmt { get; set; }
        public PurchaseByIdResPurchaseEx PurchaseEx { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public PurchaseByIdResMetaData MetaData { get; set; }
        public List<object> CustomField { get; set; }
        public string TxnDate { get; set; }
        public PurchaseByIdResCurrencyRef CurrencyRef { get; set; }
        public List<PurchaseByIdResLine> Line { get; set; }
    }

    public class PurchaseByIdRes
    {
        public PurchaseByIdResPurchase Purchase { get; set; }
        public DateTime time { get; set; }
    }
}