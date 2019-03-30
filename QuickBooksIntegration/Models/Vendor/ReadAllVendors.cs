using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllVendorResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllVendorResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllVendorResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class ReadAllVendorResPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class ReadAllVendorResPrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class ReadAllVendorResWebAddr
    {
        public string URI { get; set; }
    }

    public class ReadAllVendorResMobile
    {
        public string FreeFormNumber { get; set; }
    }

    public class ReadAllVendorResFax
    {
        public string FreeFormNumber { get; set; }
    }

    public class ReadAllVendorResTermRef
    {
        public string value { get; set; }
    }

    public class ReadAllVendorResVendor
    {
        public double Balance { get; set; }
        public bool Vendor1099 { get; set; }
        public ReadAllVendorResCurrencyRef CurrencyRef { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public ReadAllVendorResMetaData MetaData { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public ReadAllVendorResBillAddr BillAddr { get; set; }
        public string AcctNum { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string CompanyName { get; set; }
        public ReadAllVendorResPrimaryPhone PrimaryPhone { get; set; }
        public ReadAllVendorResPrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public ReadAllVendorResWebAddr WebAddr { get; set; }
        public ReadAllVendorResMobile Mobile { get; set; }
        public ReadAllVendorResFax Fax { get; set; }
        public ReadAllVendorResTermRef TermRef { get; set; }
    }

    public class ReadAllVendorResQueryResponse
    {
        public List<ReadAllVendorResVendor> Vendor { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class ReadAllVendorRes
    {
        public ReadAllVendorResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }

    public class VendorsSummary
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}