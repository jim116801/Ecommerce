﻿using Kooboo.CMS.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kooboo.CMS.Sites.Membership;

namespace Kooboo.Commerce.CMSIntegration.Plugins
{
    static class HttpContextExtensions
    {
        public static string CurrentSessionId(this HttpContextBase context)
        {
            return context.Session.SessionID;
        }

        public static string CurrentCustomerAccountId(this HttpContextBase context)
        {
            var member = context.Membership().GetMembershipUser();
            return member == null ? null : member.UUID;
        }

        public static int CurrentCartId(this HttpContextBase context)
        {
            var accountId = context.CurrentCustomerAccountId();
            if (!String.IsNullOrWhiteSpace(accountId))
            {
                return Site.Current.Commerce().ShoppingCarts.GetCartIdByAccountId(accountId);
            }

            return Site.Current.Commerce().ShoppingCarts.GetCartIdBySessionId(context.CurrentSessionId());
        }
    }
}
