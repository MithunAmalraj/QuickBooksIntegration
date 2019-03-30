using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class PaymentByIdResCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class PaymentByIdResDepositToAccountRef
    {
        public string value { get; set; }
    }

    public class PaymentByIdResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class PaymentByIdResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class PaymentByIdResLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class PaymentByIdResValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PaymentByIdResAny
    {
        public string name { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public PaymentByIdResValue value { get; set; }
        public bool nil { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class PaymentByIdResLineEx
    {
        public List<PaymentByIdResAny> any { get; set; }
    }

    public class PaymentByIdResLine
    {
        public decimal Amount { get; set; }
        public List<PaymentByIdResLinkedTxn> LinkedTxn { get; set; }
        public PaymentByIdResLineEx LineEx { get; set; }
    }

    public class PaymentByIdResPayment
    {
        public PaymentByIdResCustomerRef CustomerRef { get; set; }
        public PaymentByIdResDepositToAccountRef DepositToAccountRef { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal UnappliedAmt { get; set; }
        public bool ProcessPayment { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public PaymentByIdResMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public PaymentByIdResCurrencyRef CurrencyRef { get; set; }
        public List<PaymentByIdResLine> Line { get; set; }
    }

    public class PaymentByIdRes
    {
        public PaymentByIdResPayment Payment { get; set; }
        public DateTime time { get; set; }
    }
}