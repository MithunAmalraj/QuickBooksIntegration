using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllCustomesResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class ReadAllCustomesResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllCustomesResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllCustomesResPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class ReadAllCustomesResPrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class ReadAllCustomesResShipAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class ReadAllCustomesResMobile
    {
        public string FreeFormNumber { get; set; }
    }

    public class ReadAllCustomesResFax
    {
        public string FreeFormNumber { get; set; }
    }

    public class ReadAllCustomesResWebAddr
    {
        public string URI { get; set; }
    }

    public class ReadAllCustomesResCustomer
    {
        public bool Taxable { get; set; }
        public ReadAllCustomesResBillAddr BillAddr { get; set; }
        public bool Job { get; set; }
        public bool BillWithParent { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithJobs { get; set; }
        public ReadAllCustomesResCurrencyRef CurrencyRef { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public ReadAllCustomesResMetaData MetaData { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullyQualifiedName { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public ReadAllCustomesResPrimaryPhone PrimaryPhone { get; set; }
        public ReadAllCustomesResPrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public ReadAllCustomesResShipAddr ShipAddr { get; set; }
        public ReadAllCustomesResMobile Mobile { get; set; }
        public ReadAllCustomesResFax Fax { get; set; }
        public ReadAllCustomesResWebAddr WebAddr { get; set; }
    }

    public class ReadAllCustomesResQueryResponse
    {
        public List<ReadAllCustomesResCustomer> Customer { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class ReadAllCustomesRes
    {
        public ReadAllCustomesResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }


    public class CustomersSummary
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}