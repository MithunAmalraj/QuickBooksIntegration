using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class SalesReceiptByIdResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class SalesReceiptByIdResCustomField
    {
        public string DefinitionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SalesReceiptByIdResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class SalesReceiptByIdResLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class SalesReceiptByIdResItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class SalesReceiptByIdResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class SalesReceiptByIdResSalesItemLineDetail
    {
        public SalesReceiptByIdResItemRef ItemRef { get; set; }
        public int UnitPrice { get; set; }
        public int Qty { get; set; }
        public SalesReceiptByIdResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class SalesReceiptByIdResSubTotalLineDetail
    {
    }

    public class SalesReceiptByIdResLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string DetailType { get; set; }
        public SalesReceiptByIdResSalesItemLineDetail SalesItemLineDetail { get; set; }
        public SalesReceiptByIdResSubTotalLineDetail SubTotalLineDetail { get; set; }
    }

    public class SalesReceiptByIdResTxnTaxDetail
    {
        public int TotalTax { get; set; }
    }

    public class SalesReceiptByIdResCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class SalesReceiptByIdResCustomerMemo
    {
        public string value { get; set; }
    }

    public class SalesReceiptByIdResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class SalesReceiptByIdResBillEmail
    {
        public string Address { get; set; }
    }

    public class SalesReceiptByIdResDepositToAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class SalesReceiptByIdResSalesReceipt
    {
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public SalesReceiptByIdResMetaData MetaData { get; set; }
        public List<SalesReceiptByIdResCustomField> CustomField { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public SalesReceiptByIdResCurrencyRef CurrencyRef { get; set; }
        public List<SalesReceiptByIdResLinkedTxn> LinkedTxn { get; set; }
        public List<Line> Line { get; set; }
        public SalesReceiptByIdResTxnTaxDetail TxnTaxDetail { get; set; }
        public SalesReceiptByIdResCustomerRef CustomerRef { get; set; }
        public SalesReceiptByIdResCustomerMemo CustomerMemo { get; set; }
        public SalesReceiptByIdResBillAddr BillAddr { get; set; }
        public int TotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public string EmailStatus { get; set; }
        public SalesReceiptByIdResBillEmail BillEmail { get; set; }
        public int Balance { get; set; }
        public SalesReceiptByIdResDepositToAccountRef DepositToAccountRef { get; set; }
    }

    public class SalesReceiptByIdRes
    {
        public SalesReceiptByIdResSalesReceipt SalesReceipt { get; set; }
        public DateTime time { get; set; }
    }
}