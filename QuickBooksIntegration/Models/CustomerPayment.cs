using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CustomerPaymentListCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CustomerPaymentListDepositToAccountRef
    {
        public string value { get; set; }
    }

    public class CustomerPaymentListMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class CustomerPaymentListCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CustomerPaymentListLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class CustomerPaymentListValue
    {
        public string Name { get; set; }
        [JsonProperty("Value")]
        public string Values { get; set; }
    }

    public class CustomerPaymentListAny
    {
        public string name { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public CustomerPaymentListValue value { get; set; }
        public bool nil { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class CustomerPaymentListLineEx
    {
        public List<CustomerPaymentListAny> any { get; set; }
    }

    public class CustomerPaymentListLine
    {
        public double Amount { get; set; }
        public List<CustomerPaymentListLinkedTxn> LinkedTxn { get; set; }
        public CustomerPaymentListLineEx LineEx { get; set; }
    }

    public class CustomerPaymentListPayment
    {
        public CustomerPaymentListCustomerRef CustomerRef { get; set; }
        public CustomerPaymentListDepositToAccountRef DepositToAccountRef { get; set; }
        public double TotalAmt { get; set; }
        public int UnappliedAmt { get; set; }
        public bool ProcessPayment { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public CustomerPaymentListMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public CustomerPaymentListCurrencyRef CurrencyRef { get; set; }
        public List<CustomerPaymentListLine> Line { get; set; }
    }

    public class CustomerPaymentListQueryResponse
    {
        public List<CustomerPaymentListPayment> Payment { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class CustomerPaymentListPaymentsRootObject
    {
        public CustomerPaymentListQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }

    public class CustomerPaymentListSummary
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string TransactionDate { get; set; }
        public string Amount { get; set; }
    }
}