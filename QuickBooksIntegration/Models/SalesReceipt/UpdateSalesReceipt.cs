using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdateSalesReceiptReqCustomField
    {
        public string DefinitionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class UpdateSalesReceiptReqCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateSalesReceiptReqItemRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateSalesReceiptReqTaxCodeRef
    {
        public string value { get; set; }
    }

    public class UpdateSalesReceiptReqSalesItemLineDetail
    {
        public UpdateSalesReceiptReqItemRef ItemRef { get; set; }
        public int UnitPrice { get; set; }
        public int Qty { get; set; }
        public UpdateSalesReceiptReqTaxCodeRef TaxCodeRef { get; set; }
    }

    public class UpdateSalesReceiptReqSubTotalLineDetail
    {
    }

    public class UpdateSalesReceiptReqLine
    {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string DetailType { get; set; }
        public UpdateSalesReceiptReqSalesItemLineDetail SalesItemLineDetail { get; set; }
        public UpdateSalesReceiptReqSubTotalLineDetail SubTotalLineDetail { get; set; }
    }

    public class UpdateSalesReceiptReqTxnTaxDetail
    {
        public int TotalTax { get; set; }
    }

    public class UpdateSalesReceiptReqDepositToAccountRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateSalesReceiptReq
    {
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public List<UpdateSalesReceiptReqCustomField> CustomField { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public UpdateSalesReceiptReqCurrencyRef CurrencyRef { get; set; }
        public int ExchangeRate { get; set; }
        public List<Line> Line { get; set; }
        public UpdateSalesReceiptReqTxnTaxDetail TxnTaxDetail { get; set; }
        public int TotalAmt { get; set; }
        public int HomeTotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public string EmailStatus { get; set; }
        public int Balance { get; set; }
        public UpdateSalesReceiptReqDepositToAccountRef DepositToAccountRef { get; set; }
    }
}