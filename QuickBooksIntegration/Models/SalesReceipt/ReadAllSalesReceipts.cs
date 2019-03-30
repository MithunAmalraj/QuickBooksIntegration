using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllSalesReceiptResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllSalesReceiptResCustomField
    {
        public string DefinitionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class ReadAllSalesReceiptResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllSalesReceiptResLinkedTxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class ReadAllSalesReceiptResItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllSalesReceiptResTaxCodeRef
    {
        public string value { get; set; }
    }

    public class ReadAllSalesReceiptResSalesItemLineDetail
    {
        public ReadAllSalesReceiptResItemRef ItemRef { get; set; }
        public int UnitPrice { get; set; }
        public double Qty { get; set; }
        public ReadAllSalesReceiptResTaxCodeRef TaxCodeRef { get; set; }
    }

    public class ReadAllSalesReceiptResSubTotalLineDetail
    {
    }

    public class ReadAllSalesReceiptResDiscountAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllSalesReceiptResDiscountLineDetail
    {
        public bool PercentBased { get; set; }
        public int DiscountPercent { get; set; }
        public ReadAllSalesReceiptResDiscountAccountRef DiscountAccountRef { get; set; }
    }

    public class Line
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public ReadAllSalesReceiptResSalesItemLineDetail SalesItemLineDetail { get; set; }
        public ReadAllSalesReceiptResSubTotalLineDetail SubTotalLineDetail { get; set; }
        public ReadAllSalesReceiptResDiscountLineDetail DiscountLineDetail { get; set; }
    }

    public class ReadAllSalesReceiptResTxnTaxDetail
    {
        public int TotalTax { get; set; }
    }

    public class ReadAllSalesReceiptResCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllSalesReceiptResCustomerMemo
    {
        public string value { get; set; }
    }

    public class ReadAllSalesReceiptResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Line4 { get; set; }
    }

    public class ReadAllSalesReceiptResBillEmail
    {
        public string Address { get; set; }
    }

    public class ReadAllSalesReceiptResDepositToAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllSalesReceiptResShipAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Country { get; set; }
    }

    public class ReadAllSalesReceiptResPaymentMethodRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllSalesReceiptResSalesReceipt
    {
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public ReadAllSalesReceiptResMetaData MetaData { get; set; }
        public List<ReadAllSalesReceiptResCustomField> CustomField { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public ReadAllSalesReceiptResCurrencyRef CurrencyRef { get; set; }
        public List<ReadAllSalesReceiptResLinkedTxn> LinkedTxn { get; set; }
        public List<Line> Line { get; set; }
        public ReadAllSalesReceiptResTxnTaxDetail TxnTaxDetail { get; set; }
        public ReadAllSalesReceiptResCustomerRef CustomerRef { get; set; }
        public ReadAllSalesReceiptResCustomerMemo CustomerMemo { get; set; }
        public ReadAllSalesReceiptResBillAddr BillAddr { get; set; }
        public double TotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public string EmailStatus { get; set; }
        public ReadAllSalesReceiptResBillEmail BillEmail { get; set; }
        public int Balance { get; set; }
        public ReadAllSalesReceiptResDepositToAccountRef DepositToAccountRef { get; set; }
        public ReadAllSalesReceiptResShipAddr ShipAddr { get; set; }
        public ReadAllSalesReceiptResPaymentMethodRef PaymentMethodRef { get; set; }
        public string PaymentRefNum { get; set; }
    }

    public class ReadAllSalesReceiptResQueryResponse
    {
        public List<ReadAllSalesReceiptResSalesReceipt> SalesReceipt { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class ReadAllSalesReceiptRes
    {
        public ReadAllSalesReceiptResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }

    public class SalesReceiptsSummary
    {
        public string id { get; set; }
        public string txndate { get; set; }
    }
}