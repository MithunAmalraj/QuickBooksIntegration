using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllBillPaymentResVendorRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillPaymentResCCAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillPaymentResCreditCardPayment
    {
        public ReadAllBillPaymentResCCAccountRef CCAccountRef { get; set; }
    }

    public class ReadAllBillPaymentResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllBillPaymentResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillPaymentResLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class ReadAllBillPaymentResLine
    {
        public double Amount { get; set; }
        public List<ReadAllBillPaymentResLinkedTxn> LinkedTxn { get; set; }
    }

    public class ReadAllBillPaymentResBankAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllBillPaymentResCheckPayment
    {
        public ReadAllBillPaymentResBankAccountRef BankAccountRef { get; set; }
        public string PrintStatus { get; set; }
    }

    public class ReadAllBillPaymentResBillPayment
    {
        public ReadAllBillPaymentResVendorRef VendorRef { get; set; }
        public string PayType { get; set; }
        public ReadAllBillPaymentResCreditCardPayment CreditCardPayment { get; set; }
        public double TotalAmt { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public ReadAllBillPaymentResMetaData MetaData { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public ReadAllBillPaymentResCurrencyRef CurrencyRef { get; set; }
        public List<ReadAllBillPaymentResLine> Line { get; set; }
        public ReadAllBillPaymentResCheckPayment CheckPayment { get; set; }
    }

    public class ReadAllBillPaymentResQueryResponse
    {
        public List<ReadAllBillPaymentResBillPayment> BillPayment { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
        public int totalCount { get; set; }
    }

    public class ReadAllBillPaymentRes
    {
        public ReadAllBillPaymentResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }
    public class BillPaymentsSummary
    {
        public string id { get; set; }
        public string txndate { get; set; }
    }
}