using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllPaymentResCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllPaymentResDepositToAccountRef
    {
        public string value { get; set; }
    }

    public class ReadAllPaymentResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllPaymentResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllPaymentResLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class ReadAllPaymentResValue
    {
        public string Name { get; set; }

        public string value { get; set; }
    }

    public class ReadAllPamentAny
    {
        public string name { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public ReadAllPaymentResValue value { get; set; }
        public bool nil { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class ReadAllPaymentResLineEx
    {
        public List<ReadAllPamentAny> any { get; set; }
    }

    public class ReadAllPaymentResLine
    {
        public double Amount { get; set; }
        public List<ReadAllPaymentResLinkedTxn> LinkedTxn { get; set; }
        public ReadAllPaymentResLineEx LineEx { get; set; }
    }

    public class ReadAllPaymentResLinkedTxn2
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class ReadAllPaymentResPaymentMethodRef
    {
        public string value { get; set; }
    }

    public class ReadAllPaymentResPayment
    {
        public ReadAllPaymentResCustomerRef CustomerRef { get; set; }
        public ReadAllPaymentResDepositToAccountRef DepositToAccountRef { get; set; }
        public double TotalAmt { get; set; }
        public decimal UnappliedAmt { get; set; }
        public bool ProcessPayment { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public ReadAllPaymentResMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public ReadAllPaymentResCurrencyRef CurrencyRef { get; set; }
        public List<ReadAllPaymentResLine> Line { get; set; }
        public List<ReadAllPaymentResLinkedTxn2> LinkedTxn { get; set; }
        public ReadAllPaymentResPaymentMethodRef PaymentMethodRef { get; set; }
    }

    public class ReadAllPaymentResQueryResponse
    {
        public List<ReadAllPaymentResPayment> Payment { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class ReadAllPaymentRes
    {
        public ReadAllPaymentResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }
    public class ReadAllPaymentSummary
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string TransactionDate { get; set; }
        public string Amount { get; set; }
    }
}