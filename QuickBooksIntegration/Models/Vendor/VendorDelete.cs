using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class VendorDeleteReqBillAddr
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

    public class VendorDeleteReqCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class VendorDeleteReqPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class VendorDeleteReqMobile
    {
        public string FreeFormNumber { get; set; }
    }

    public class VendorDeleteReqPrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class VendorDeleteReqWebAddr
    {
        public string URI { get; set; }
    }

    public class VendorDeleteReq
    {
        public VendorDeleteReqBillAddr BillAddr { get; set; }
        public string TaxIdentifier { get; set; }
        public int Balance { get; set; }
        public string AcctNum { get; set; }
        public bool Vendor1099 { get; set; }
        public VendorDeleteReqCurrencyRef CurrencyRef { get; set; }
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
        public VendorDeleteReqPrimaryPhone PrimaryPhone { get; set; }
        public VendorDeleteReqMobile Mobile { get; set; }
        public VendorDeleteReqPrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public VendorDeleteReqWebAddr WebAddr { get; set; }
    }
}