using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CustomerByIdResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
    }

    public class CustomerByIdResShipAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
    }

    public class CustomerByIdResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CustomerByIdResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class CustomerByIdResPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class CustomerByIdResPrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class CustomerByIdResDefaultTaxCodeRef
    {
        public string value { get; set; }
    }

    public class CustomerByIdResCustomer
    {
        public bool Taxable { get; set; }
        public CustomerByIdResBillAddr BillAddr { get; set; }
        public CustomerByIdResShipAddr ShipAddr { get; set; }
        public string Notes { get; set; }
        public bool Job { get; set; }
        public bool BillWithParent { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithJobs { get; set; }
        public CustomerByIdResCurrencyRef CurrencyRef { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public CustomerByIdResMetaData MetaData { get; set; }
        public string FullyQualifiedName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public CustomerByIdResPrimaryPhone PrimaryPhone { get; set; }
        public CustomerByIdResPrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public CustomerByIdResDefaultTaxCodeRef DefaultTaxCodeRef { get; set; }
    }

    public class CustomerByIdRes
    {
        public CustomerByIdResCustomer Customer { get; set; }
        public DateTime time { get; set; }
    }
}