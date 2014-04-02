﻿using Kooboo.Commerce.API;
using Kooboo.Commerce.API.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kooboo.Commerce.Web.Areas.CommerceWebAPI.Controllers
{
    public class PaymentMethodController : CommerceAPIControllerQueryBase<PaymentMethod>
    {
        protected override ICommerceQuery<PaymentMethod> BuildQueryFromQueryStrings()
        {
            var qs = Request.RequestUri.ParseQueryString();
            var query = Commerce().PaymentMethods.Query();

            if (!string.IsNullOrEmpty(qs["id"]))
            {
                query = query.ById(Convert.ToInt32(qs["id"]));
            }

            return query;
        }
    }
}