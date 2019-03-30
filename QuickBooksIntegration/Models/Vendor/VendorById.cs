using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class VendorByIdResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class VendorByIdResTermRef
    {
        public string value { get; set; }
    }

    public class VendorByIdResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class VendorByIdResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class VendorByIdResPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class VendorByIdResVendor
    {
        public VendorByIdResBillAddr BillAddr { get; set; }
        public VendorByIdResTermRef TermRef { get; set; }
        public int Balance { get; set; }
        public bool Vendor1099 { get; set; }
        public VendorByIdResCurrencyRef CurrencyRef { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public VendorByIdResMetaData MetaData { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public VendorByIdResPrimaryPhone PrimaryPhone { get; set; }
    }

    public class VendorByIdRes
    {
        public VendorByIdResVendor Vendor { get; set; }
        public DateTime time { get; set; }
    }
}