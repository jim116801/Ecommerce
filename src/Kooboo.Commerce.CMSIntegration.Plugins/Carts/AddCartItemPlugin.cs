﻿using Kooboo.CMS.Sites.Models;
using Kooboo.Commerce.CMSIntegration.Plugins.Carts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kooboo.CMS.Sites.Membership;

namespace Kooboo.Commerce.CMSIntegration.Plugins.Carts
{
    public class AddCartItemPlugin : SubmissionPluginBase<AddCartItemModel>
    {
        protected override SubmissionExecuteResult Execute(AddCartItemModel model)
        {
            var cartId = HttpContext.CurrentCartId();
            var itemId = Api.ShoppingCarts.AddItem(cartId, model.ProductVariantId, model.Quantity);

            return new SubmissionExecuteResult
            {
                RedirectUrl = ResolveUrl(model.SuccessUrl, ControllerContext),
                Data = new AddCartItemResult
                {
                    ItemId = itemId
                }
            };
        }
    }
}
