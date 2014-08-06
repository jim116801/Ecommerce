﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kooboo.Commerce.CMSIntegration.DataSources.Generic
{
    public class ParsedGenericCommerceDataSourceSettings
    {
        public List<ParsedFilter> Filters { get; set; }

        public List<string> Includes { get; set; }

        public string SortField { get; set; }

        public SortDirection SortDirection { get; set; }

        public TakeOperation TakeOperation { get; set; }

        public int? Top { get; set; }

        public bool EnablePaging { get; set; }

        public int? PageSize { get; set; }

        public int? PageNumber { get; set; }

        public ParsedGenericCommerceDataSourceSettings()
        {
            Filters = new List<ParsedFilter>();
            Includes = new List<string>();
        }
    }
}