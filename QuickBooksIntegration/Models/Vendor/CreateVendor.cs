using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreateVendorReqBillAddr
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
    }

    public class CreateVendorReqPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class CreateVendorReqMobile
    {
        public string FreeFormNumber { get; set; }
    }

    public class CreateVendorReqPrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class CreateVendorReqWebAddr
    {
        public string URI { get; set; }
    }

    public class CreateVendorReq
    {
        public CreateVendorReqBillAddr BillAddr { get; set; }
        public string TaxIdentifier { get; set; }
        public string AcctNum { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public CreateVendorReqPrimaryPhone PrimaryPhone { get; set; }
        public CreateVendorReqMobile Mobile { get; set; }
        public CreateVendorReqPrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public CreateVendorReqWebAddr WebAddr { get; set; }
    }

    public class CreateVendorResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class CreateVendorResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateVendorResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class CreateVendorResVendor
    {
        public CreateVendorResBillAddr BillAddr { get; set; }
        public int Balance { get; set; }
        public bool Vendor1099 { get; set; }
        public CreateVendorResCurrencyRef CurrencyRef { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public CreateVendorResMetaData MetaData { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
    }

    public class CreateVendorRes
    {
        public CreateVendorResVendor Vendor { get; set; }
        public DateTime time { get; set; }
    }
}