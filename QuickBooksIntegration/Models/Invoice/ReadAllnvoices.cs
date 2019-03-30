using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllInvoiceResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllInvoiceResCustomField
    {
        public string DefinitionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string StringValue { get; set; }
    }

    public class ReadAllInvoiceResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllInvoiceResItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllInvoiceResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class ReadAllInvoiceResSalesItemLineDetail
    {
        public ReadAllInvoiceResItemRef ItemRef { get; set; }
        public double UnitPrice { get; set; }
        public double Qty { get; set; }
        public ReadAllInvoiceResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class ReadAllInvoiceResSubTotalLineDetail
    {
    }

    public class ReadAllInvoiceResLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public ReadAllInvoiceResSalesItemLineDetail SalesItemLineDetail { get; set; }
        public ReadAllInvoiceResSubTotalLineDetail SubTotalLineDetail { get; set; }
    }

    public class ReadAllInvoiceResTxnTaxCodeRef
    {
        public string value { get; set; }
    }

    public class ReadAllInvoiceResTaxRateRef
    {
        public string value { get; set; }
    }

    public class ReadAllInvoiceResTaxLineDetail
    {
        public ReadAllInvoiceResTaxRateRef TaxRateRef { get; set; }
        public bool PercentBased { get; set; }
        public decimal TaxPercent { get; set; }
        public double NetAmountTaxable { get; set; }
    }

    public class ReadAllInvoiceResTaxLine
    {
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public ReadAllInvoiceResTaxLineDetail TaxLineDetail { get; set; }
    }

    public class ReadAllInvoiceResTxnTaxDetail
    {
        public ReadAllInvoiceResTxnTaxCodeRef TxnTaxCodeRef { get; set; }
        public double TotalTax { get; set; }
        public List<ReadAllInvoiceResTaxLine> TaxLine { get; set; }
    }

    public class ReadAllInvoiceResCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllInvoiceResCustomerMemo
    {
        public string value { get; set; }
    }

    public class ReadAllInvoiceResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class ReadAllInvoiceResSalesTermRef
    {
        public string value { get; set; }
    }

    public class ReadAllInvoiceResBillEmail
    {
        public string Address { get; set; }
    }

    public class ReadAllInvoiceResShipAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class ReadAllInvoiceResInvoice
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
        public ReadAllInvoiceResMetaData MetaData { get; set; }
        public List<ReadAllInvoiceResCustomField> CustomField { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public ReadAllInvoiceResCurrencyRef CurrencyRef { get; set; }
        public List<object> LinkedTxn { get; set; }
        public List<ReadAllInvoiceResLine> Line { get; set; }
        public ReadAllInvoiceResTxnTaxDetail TxnTaxDetail { get; set; }
        public ReadAllInvoiceResCustomerRef CustomerRef { get; set; }
        public ReadAllInvoiceResCustomerMemo CustomerMemo { get; set; }
        public ReadAllInvoiceResBillAddr BillAddr { get; set; }
        public ReadAllInvoiceResSalesTermRef SalesTermRef { get; set; }
        public string DueDate { get; set; }
        public double TotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public string EmailStatus { get; set; }
        public ReadAllInvoiceResBillEmail BillEmail { get; set; }
        public double Balance { get; set; }
        public ReadAllInvoiceResShipAddr ShipAddr { get; set; }
    }

    public class ReadAllInvoiceResQueryResponse
    {
        public List<ReadAllInvoiceResInvoice> Invoice { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
        public int totalCount { get; set; }
    }

    public class ReadAllInvoiceRes
    {
        public ReadAllInvoiceResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }

    public class InvoicesSummary
    {
        public string id { get; set; }
        public string txndate { get; set; }
    }
}