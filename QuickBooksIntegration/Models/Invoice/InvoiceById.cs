using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class InvoiceByIdResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class InvoiceByIdResCustomField
    {
        public string DefinitionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class InvoiceByIdResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class InvoiceByIdResLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class InvoiceByIdResItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class InvoiceByIdResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class InvoiceByIdResSalesItemLineDetail
    {
        public InvoiceByIdResItemRef ItemRef { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Qty { get; set; }
        public InvoiceByIdResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class InvoiceByIdResSubTotalLineDetail
    {
    }

    public class InvoiceByIdResLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string DetailType { get; set; }
        public InvoiceByIdResSalesItemLineDetail SalesItemLineDetail { get; set; }
        public InvoiceByIdResSubTotalLineDetail SubTotalLineDetail { get; set; }
    }

    public class InvoiceByIdResTxnTaxCodeRef
    {
        public string value { get; set; }
    }

    public class InvoiceByIdResTaxRateRef
    {
        public string value { get; set; }
    }

    public class InvoiceByIdResTaxLineDetail
    {
        public InvoiceByIdResTaxRateRef TaxRateRef { get; set; }
        public bool PercentBased { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal NetAmountTaxable { get; set; }
    }

    public class InvoiceByIdResTaxLine
    {
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public InvoiceByIdResTaxLineDetail TaxLineDetail { get; set; }
    }

    public class InvoiceByIdResTxnTaxDetail
    {
        public InvoiceByIdResTxnTaxCodeRef TxnTaxCodeRef { get; set; }
        public double TotalTax { get; set; }
        public List<InvoiceByIdResTaxLine> TaxLine { get; set; }
    }

    public class InvoiceByIdResCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class InvoiceByIdResCustomerMemo
    {
        public string value { get; set; }
    }

    public class InvoiceByIdResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class InvoiceByIdResSalesTermRef
    {
        public string value { get; set; }
    }

    public class InvoiceByIdResBillEmail
    {
        public string Address { get; set; }
    }

    public class InvoiceByIdResInvoice
    {
        public decimal Deposit { get; set; }
        public bool AllowIPNPayment { get; set; }
        public bool AllowOnlinePayment { get; set; }
        public bool AllowOnlineCreditCardPayment { get; set; }
        public bool AllowOnlineACHPayment { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public InvoiceByIdResMetaData MetaData { get; set; }
        public List<InvoiceByIdResCustomField> CustomField { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public InvoiceByIdResCurrencyRef CurrencyRef { get; set; }
        public List<InvoiceByIdResLinkedTxn> LinkedTxn { get; set; }
        public List<InvoiceByIdResLine> Line { get; set; }
        public InvoiceByIdResTxnTaxDetail TxnTaxDetail { get; set; }
        public InvoiceByIdResCustomerRef CustomerRef { get; set; }
        public InvoiceByIdResCustomerMemo CustomerMemo { get; set; }
        public InvoiceByIdResBillAddr BillAddr { get; set; }
        public InvoiceByIdResSalesTermRef SalesTermRef { get; set; }
        public string DueDate { get; set; }
        public double TotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public string EmailStatus { get; set; }
        public InvoiceByIdResBillEmail BillEmail { get; set; }
        public double Balance { get; set; }
    }

    public class InvoiceByIdRes
    {
        public InvoiceByIdResInvoice Invoice { get; set; }
        public DateTime time { get; set; }
    }
}