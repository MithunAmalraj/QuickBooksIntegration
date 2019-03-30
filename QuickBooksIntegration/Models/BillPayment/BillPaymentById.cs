using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class BillPaymentByIdResVendorRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class BillPaymentByIdResCCAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class BillPaymentByIdResCreditCardPayment
    {
        public BillPaymentByIdResCCAccountRef CCAccountRef { get; set; }
    }

    public class BillPaymentByIdResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class BillPaymentByIdResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class BillPaymentByIdResLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class BillPaymentByIdResLine
    {
        public double Amount { get; set; }
        public List<BillPaymentByIdResLinkedTxn> LinkedTxn { get; set; }
    }

    public class BillPaymentByIdResBillPayment
    {
        public BillPaymentByIdResVendorRef VendorRef { get; set; }
        public string PayType { get; set; }
        public BillPaymentByIdResCreditCardPayment CreditCardPayment { get; set; }
        public double TotalAmt { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public BillPaymentByIdResMetaData MetaData { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public BillPaymentByIdResCurrencyRef CurrencyRef { get; set; }
        public List<BillPaymentByIdResLine> Line { get; set; }
    }

    public class BillPaymentByIdRes
    {
        public BillPaymentByIdResBillPayment BillPayment { get; set; }
        public DateTime time { get; set; }
    }
}