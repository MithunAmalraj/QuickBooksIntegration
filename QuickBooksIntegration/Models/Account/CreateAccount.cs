using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CreateAccountReq
    {
        public string AccountType { get; set; }
        public string Name { get; set; }
    }
    public class CreateAccountResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CreateAccountResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class CreateAccountResAccount
    {
        public string Name { get; set; }
        public bool SubAccount { get; set; }
        public string FullyQualifiedName { get; set; }
        public bool Active { get; set; }
        public string Classification { get; set; }
        public string AccountType { get; set; }
        public string AccountSubType { get; set; }
        public int CurrentBalance { get; set; }
        public int CurrentBalanceWithSubAccounts { get; set; }
        public CreateAccountResCurrencyRef CurrencyRef { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public CreateAccountResMetaData MetaData { get; set; }
    }

    public class CreateAccountRes
    {
        public CreateAccountResAccount Account { get; set; }
        public DateTime time { get; set; }
    }
}