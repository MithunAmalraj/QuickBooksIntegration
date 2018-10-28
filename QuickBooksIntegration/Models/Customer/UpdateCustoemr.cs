using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdateCustoemrReqBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
    }

    public class UpdateCustoemrReqCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateCustoemrReqPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class UpdateCustoemrReqPrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class UpdateCustoemrReqDefaultTaxCodeRef
    {
        public string value { get; set; }
    }

    public class UpdateCustoemrReq
    {
        public bool Taxable { get; set; }
        public UpdateCustoemrReqBillAddr BillAddr { get; set; }
        public string Notes { get; set; }
        public bool Job { get; set; }
        public bool BillWithParent { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithJobs { get; set; }
        public UpdateCustoemrReqCurrencyRef CurrencyRef { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public string FullyQualifiedName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public UpdateCustoemrReqPrimaryPhone PrimaryPhone { get; set; }
        public UpdateCustoemrReqPrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public UpdateCustoemrReqDefaultTaxCodeRef DefaultTaxCodeRef { get; set; }
    }
}