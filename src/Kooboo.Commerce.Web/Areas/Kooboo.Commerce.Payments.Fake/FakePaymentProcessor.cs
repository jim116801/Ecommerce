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

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest request)
        {
            var gatewayUrl = UrlUtility.Combine(request.CommerceBaseUrl,
                Strings.AreaName + "/Home/Gateway?commerceName=" + request.Payment.Metadata.CommerceName
                + "&paymentId=" + request.Payment.Id
                + "&currency=" + request.CurrencyCode
                + "&commerceReturnUrl=" + HttpUtility.UrlEncode(request.ReturnUrl));
            return ProcessPaymentResult.Pending(new RedirectResult(gatewayUrl), Guid.NewGuid().ToString("N"));
        }

        public IEnumerable<PaymentMethodType> SupportedPaymentTypes
        {
            get
            {
                yield return PaymentMethodType.CreditCard;
                yield return PaymentMethodType.DirectDebit;
                yield return PaymentMethodType.ExternalPayment;
            }
        }

        public bool SupportMultiplePaymentMethods
        {
            get { return false; }
        }

        public IEnumerable<SupportedPaymentMethod> GetSupportedPaymentMethods(PaymentMethodType paymentType)
        {
            throw new NotSupportedException();
        }
    }
}