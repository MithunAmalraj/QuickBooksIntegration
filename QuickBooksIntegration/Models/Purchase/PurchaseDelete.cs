﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegration.Models
{
    public class PurchaseDeleteReq
    {
        public string Id { get; set; }
        public string SyncToken { get; set; }
    }
}