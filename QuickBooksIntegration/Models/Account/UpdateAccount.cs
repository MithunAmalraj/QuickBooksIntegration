using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class UpdateAccountReqCurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class UpdateAccountReq
    {
        public string Name { get; set; }
        public bool SubAccount { get; set; }
        public string FullyQualifiedName { get; set; }
        public bool Active { get; set; }
        public string Classification { get; set; }
        public string AccountType { get; set; }
        public string AccountSubType { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal CurrentBalanceWithSubAccounts { get; set; }
        public UpdateAccountReqCurrencyRef CurrencyRef { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
    }
}