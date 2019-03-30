using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdatePaymentReqCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdatePaymentReqDepositToAccountRef
    {
        public string value { get; set; }
    }

    public class UpdatePaymentReqCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdatePaymentReqLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class UpdatePaymentReqValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class UpdatePaymentReqAny
    {
        public string name { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public UpdatePaymentReqValue value { get; set; }
        public bool nil { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class UpdatePaymentReqLineEx
    {
        public List<UpdatePaymentReqAny> any { get; set; }
    }

    public class UpdatePaymentReqLine
    {
        public decimal Amount { get; set; }
        public List<UpdatePaymentReqLinkedTxn> LinkedTxn { get; set; }
        public UpdatePaymentReqLineEx LineEx { get; set; }
    }

    public class UpdatePaymentReq
    {
        public UpdatePaymentReqCustomerRef CustomerRef { get; set; }
        public UpdatePaymentReq DepositToAccountRef { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal UnappliedAmt { get; set; }
        public bool ProcessPayment { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public string TxnDate { get; set; }
        public UpdatePaymentReqCurrencyRef CurrencyRef { get; set; }
        public decimal ExchangeRate { get; set; }
        public List<UpdatePaymentReqLine> Line { get; set; }
    }
}