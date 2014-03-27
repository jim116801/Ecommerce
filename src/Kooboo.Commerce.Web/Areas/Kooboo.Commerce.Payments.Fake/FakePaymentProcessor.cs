﻿using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.Commerce.Web;
using Kooboo.Web.Url;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kooboo.Commerce.Payments.Fake
{
    [Dependency(typeof(IPaymentProcessor), Key = "Kooboo.Commerce.Payments.Fake.FakePaymentProcessor")]
    public class FakePaymentProcessor : IPaymentProcessor
    {
        public string Name
        {
            get
            {
                return "Fake";
            }
        }

        public Func<HttpContextBase> HttpContextAccessor = () => new HttpContextWrapper(HttpContext.Current);

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest request)
        {
            var redirectUrl = Strings.AreaName 
                + "/Home/Gateway?commerceName=" + request.Payment.Metadata.CommerceName
                + "&paymentId=" + request.Payment.Id
                + "&currency=" + request.CurrencyCode
                + "&commerceReturnUrl=" + HttpUtility.UrlEncode(request.ReturnUrl);

            redirectUrl = redirectUrl.ToFullUrl(HttpContextAccessor());

            return ProcessPaymentResult.Pending(redirectUrl, Guid.NewGuid().ToString("N"));
        }
    }
}