using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdateVendorReqBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
    }

    public class UpdateVendorReqCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateVendorReqPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class UpdateVendorReqMobile
    {
        public string FreeFormNumber { get; set; }
    }

    public class UpdateVendorReqPrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class UpdateVendorReqWebAddr
    {
        public string URI { get; set; }
    }

    public class UpdateVendorReq
    {
        public UpdateVendorReqBillAddr BillAddr { get; set; }
        public string TaxIdentifier { get; set; }
        public decimal Balance { get; set; }
        public string AcctNum { get; set; }
        public bool Vendor1099 { get; set; }
        public UpdateVendorReqCurrencyRef CurrencyRef { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public UpdateVendorReqPrimaryPhone PrimaryPhone { get; set; }
        public UpdateVendorReqMobile Mobile { get; set; }
        public UpdateVendorReqPrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public UpdateVendorReqWebAddr WebAddr { get; set; }
    }
}