using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdatePurchaseReqAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdatePurchaseReqValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class UpdatePurchaseReqAny
    {
        public string name { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public UpdatePurchaseReqValue value { get; set; }
        public bool nil { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class UpdatePurchaseReqPurchaseEx
    {
        public List<UpdatePurchaseReqAny> any { get; set; }
    }

    public class UpdatePurchaseReqMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class UpdatePurchaseReqCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdatePurchaseReqAccountRef2
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdatePurchaseReqTaxCodeRef
    {
        public string value { get; set; }
    }

    public class UpdatePurchaseReqAccountBasedExpenseLineDetail
    {
        public UpdatePurchaseReqAccountRef2 AccountRef { get; set; }
        public string BillableStatus { get; set; }
        public UpdatePurchaseReqTaxCodeRef TaxCodeRef { get; set; }
    }

    public class UpdatePurchaseReqLine
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string DetailType { get; set; }
        public UpdatePurchaseReqAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class UpdatePurchaseReq
    {
        public UpdatePurchaseReqAccountRef AccountRef { get; set; }
        public string PaymentType { get; set; }
        public bool Credit { get; set; }
        public int TotalAmt { get; set; }
        public UpdatePurchaseReqPurchaseEx PurchaseEx { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public UpdatePurchaseReqMetaData MetaData { get; set; }
        public List<object> CustomField { get; set; }
        public string TxnDate { get; set; }
        public UpdatePurchaseReqCurrencyRef CurrencyRef { get; set; }
        public int ExchangeRate { get; set; }
        public List<UpdatePurchaseReqLine> Line { get; set; }
    }
}