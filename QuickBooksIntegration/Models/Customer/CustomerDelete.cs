using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class CustomerDeleteReq
    {
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public bool Active { get; set; }
    }
}