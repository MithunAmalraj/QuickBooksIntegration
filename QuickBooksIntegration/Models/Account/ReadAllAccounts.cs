using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class ReadAllAccountResCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ReadAllAccountResMetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class ReadAllAccountResAccount
    {
        public string Name { get; set; }
        public bool SubAccount { get; set; }
        public string FullyQualifiedName { get; set; }
        public bool Active { get; set; }
        public string Classification { get; set; }
        public string AccountType { get; set; }
        public string AccountSubType { get; set; }
        public double CurrentBalance { get; set; }
        public double CurrentBalanceWithSubAccounts { get; set; }
        public ReadAllAccountResCurrencyRef CurrencyRef { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public ReadAllAccountResMetaData MetaData { get; set; }
    }

    public class ReadAllAccountResQueryResponse
    {
        public List<ReadAllAccountResAccount> Account { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class ReadAllAccountRes
    {
        public ReadAllAccountResQueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }

    public class AccountsSummary
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}