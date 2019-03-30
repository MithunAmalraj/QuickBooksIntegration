using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdateInvoiceReqCustomField
    {
        public string DefinitionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class UpdateInvoiceReqCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateInvoiceReqItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateInvoiceReqTaxCodeRef
    {
        public string value { get; set; }
    }

    public class UpdateInvoiceReqSalesItemLineDetail
    {
        public UpdateInvoiceReqItemRef ItemRef { get; set; }
        public UpdateInvoiceReqTaxCodeRef TaxCodeRef { get; set; }
    }

    public class UpdateInvoiceReqSubTotalLineDetail
    {
    }

    public class UpdateInvoiceReqLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public int Amount { get; set; }
        public string DetailType { get; set; }
        public UpdateInvoiceReqSalesItemLineDetail SalesItemLineDetail { get; set; }
        public UpdateInvoiceReqSubTotalLineDetail SubTotalLineDetail { get; set; }
    }

    public class UpdateInvoiceReqTxnTaxDetail
    {
        public int TotalTax { get; set; }
    }

    public class UpdateInvoiceReqCustomerRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateInvoiceReqBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class UpdateInvoiceReqShipAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class UpdateInvoiceReq
    {
        public int Deposit { get; set; }
        public bool AllowIPNPayment { get; set; }
        public bool AllowOnlinePayment { get; set; }
        public bool AllowOnlineCreditCardPayment { get; set; }
        public bool AllowOnlineACHPayment { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public List<UpdateInvoiceReqCustomField> CustomField { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public UpdateInvoiceReqCurrencyRef CurrencyRef { get; set; }
        public int ExchangeRate { get; set; }
        public List<object> LinkedTxn { get; set; }
        public List<UpdateInvoiceReqLine> Line { get; set; }
        public UpdateInvoiceReqTxnTaxDetail TxnTaxDetail { get; set; }
        public UpdateInvoiceReqCustomerRef CustomerRef { get; set; }
        public UpdateInvoiceReqBillAddr BillAddr { get; set; }
        public UpdateInvoiceReqShipAddr ShipAddr { get; set; }
        public string DueDate { get; set; }
        public int TotalAmt { get; set; }
        public int HomeTotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public string EmailStatus { get; set; }
        public int Balance { get; set; }
    }
}