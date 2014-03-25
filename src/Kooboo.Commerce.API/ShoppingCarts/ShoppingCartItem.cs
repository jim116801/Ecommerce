﻿using Kooboo.Commerce.API.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.API.ShoppingCarts
{
    /// <summary>
    /// shopping cart item
    /// </summary>
    public class ShoppingCartItem
    {
        /// <summary>
        /// shopping cart item id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// shopping cart
        /// </summary>
        public ShoppingCart ShoppingCart { get; set; }
        /// <summary>
        /// product price
        /// </summary>
        public ProductPrice ProductPrice { get; set; }
        /// <summary>
        /// quantity
        /// </summary>
        public int Quantity { get; set; }
    }
}
