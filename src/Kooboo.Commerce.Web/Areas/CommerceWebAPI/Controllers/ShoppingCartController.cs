﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kooboo.CMS.Membership.Models;
using Kooboo.Commerce.API.ShoppingCarts;
using Kooboo.Commerce.API;
using Kooboo.Commerce.API.HAL;
using Kooboo.Commerce.API.Locations;

namespace Kooboo.Commerce.Web.Areas.CommerceWebAPI.Controllers
{
    public class ShoppingCartController : CommerceAPIControllerQueryBase<ShoppingCart>
    {
        /// <summary>
        /// build the commerce query filters from query string.
        /// </summary>
        /// <returns>commerce query</returns>
        [HalParameterProvider()]
        [HalParameter(Name = "sessionId", ParameterType = typeof(string))]
        [HalParameter(Name = "accountId", ParameterType = typeof(string))]
        protected override ICommerceQuery<ShoppingCart> BuildQueryFromQueryStrings()
        {
            var qs = Request.RequestUri.ParseQueryString();
            var query = Commerce().ShoppingCarts.Query();
            if (!string.IsNullOrEmpty(qs["sessionId"]))
                query = query.BySessionId(qs["sessionId"]);
            if (!string.IsNullOrEmpty(qs["accountId"]))
                query = query.ByAccountId(qs["accountId"]);

            return BuildLoadWithFromQueryStrings(query, qs);
        }

        [HttpPost]
        [Resource("apply_coupon")]
        public bool ApplyCoupon(int cartId, string coupon)
        {
            return Commerce().ShoppingCarts.ApplyCoupon(cartId, coupon);
        }

        [HttpPost]
        [Resource("change_shipping_address")]
        public int ChangeShippingAddress(int cartId, Address address)
        {
            Commerce().ShoppingCarts.ChangeShippingAddress(cartId, address);
            return address.Id;
        }

        [HttpPost]
        [Resource("change_billing_address")]
        public int ChangeBillingAddress(int cartId, Address address)
        {
            Commerce().ShoppingCarts.ChangeBillingAddress(cartId, address);
            return address.Id;
        }

        /// <summary>
        /// add item to shopping cart
        /// </summary>
        /// <param name="cartId">cart id</param>
        /// <param name="item">shopping cart item</param>
        /// <returns>true if successfully, else false</returns>
        [HttpPost]
        [Resource("add_cart_item")]
        public bool AddCartItem(int cartId, [FromBody]ShoppingCartItem item)
        {
            return Commerce().ShoppingCarts.AddCartItem(cartId, item);
        }

        /// <summary>
        /// update shopping cart item
        /// </summary>
        /// <param name="cartId">cart id</param>
        /// <param name="item">shopping cart item</param>
        /// <returns>true if successfully, else false</returns>
        [HttpPost]
        [Resource("update_cart_item")]
        public bool UpdateCartItem(int cartId, [FromBody]ShoppingCartItem item)
        {
            return Commerce().ShoppingCarts.UpdateCartItem(cartId, item);
        }
        /// <summary>
        /// remove shopping cart item
        /// </summary>
        /// <param name="cartId">cart id</param>
        /// <param name="item">shopping cart item</param>
        /// <returns>true if successfully, else false</returns>
        [HttpDelete]
        [Resource("remove_cart_item")]
        public bool RemoveCartItem(int cartId, int cartItemId)
        {
            return Commerce().ShoppingCarts.RemoveCartItem(cartId, cartItemId);
        }

        /// <summary>
        /// add the specified product to current user's shopping cart
        /// add up the amount if the product already in the shopping item
        /// </summary>
        /// <param name="sessionId">current session id</param>
        /// <param name="accountId">current user's account id</param>
        /// <param name="productPriceId">specified product price</param>
        /// <param name="quantity">quantity</param>
        /// <returns>true if successfully, else false</returns>
        [HttpPost]
        [Resource("add_to_cart")]
        public bool AddToCart(string sessionId, string accountId, int productPriceId, int quantity)
        {
            return Commerce().ShoppingCarts.AddToCart(sessionId, accountId, productPriceId, quantity);
        }

        /// <summary>
        /// update the specified product's quantity
        /// update the amount if the product already in the shopping item
        /// </summary>
        /// <param name="sessionId">current session id</param>
        /// <param name="accountId">current user's account id</param>
        /// <param name="productPriceId">specified product price</param>
        /// <param name="quantity">quantity</param>
        /// <returns>true if successfully, else false</returns>
        [HttpPost]
        [Resource("update_cart")]
        public bool UpdateCart(string sessionId, string accountId, int productPriceId, int quantity)
        {
            return Commerce().ShoppingCarts.UpdateCart(sessionId, accountId, productPriceId, quantity);
        }

        /// <summary>
        /// fill with customer info by current user's account
        /// </summary>
        /// <param name="sessionId">current session id</param>
        /// <param name="user">current user's info</param>
        /// <returns>true if successfully, else false</returns>
        [HttpPost]
        [Resource("fill_cart_customer")]
        public bool FillCustomerByAccount(string sessionId, [FromBody]Kooboo.CMS.Membership.Models.MembershipUser user)
        {
            return Commerce().ShoppingCarts.FillCustomerByAccount(sessionId, user);
        }

        /// <summary>
        /// expire the shopping cart, so that user can create another new shopping cart by current session id
        /// </summary>
        /// <param name="shoppingCartId">shopping cart id</param>
        /// <returns>true if successfully, else false</returns>
        [HttpPost]
        [Resource("expire_cart")]
        public bool ExpireShppingCart(int shoppingCartId)
        {
            return Commerce().ShoppingCarts.ExpireShppingCart(shoppingCartId);
        }

    }
}
