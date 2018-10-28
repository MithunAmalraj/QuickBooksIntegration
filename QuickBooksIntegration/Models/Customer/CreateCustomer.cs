using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreateCustomerReqBillAddr
    {
        public string Line1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
    }

    public class CreateCustomerReqPrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class CreateCustomerReqPrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class CreateCustomerReq
    {
        public CreateCustomerReqBillAddr BillAddr { get; set; }
        public string Notes { get; set; }
        public string DisplayName { get; set; }
        public CreateCustomerReqPrimaryPhone PrimaryPhone { get; set; }
        public CreateCustomerReqPrimaryEmailAddr PrimaryEmailAddr { get; set; }
    }

    public class CreateCustomerResBillAddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class CreateCustomerResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateCustomerResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class CreateCustomerResDefaultTaxCodeRef
    {
        public string value { get; set; }
    }

    public class CreateCustomerResCustomer
    {
        public bool Taxable { get; set; }
        public CreateCustomerResBillAddr BillAddr { get; set; }
        public string Notes { get; set; }
        public bool Job { get; set; }
        public bool BillWithParent { get; set; }
        public int Balance { get; set; }
        public int BalanceWithJobs { get; set; }
        public CreateCustomerResCurrencyRef CurrencyRef { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public CreateCustomerResMetaData MetaData { get; set; }
        public string FullyQualifiedName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public CreateCustomerResDefaultTaxCodeRef DefaultTaxCodeRef { get; set; }
    }
    public class CreateCustomerRes
    {
        public CreateCustomerResCustomer Customer { get; set; }
        public DateTime time { get; set; }
    }
}