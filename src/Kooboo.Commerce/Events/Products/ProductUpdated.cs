﻿using Kooboo.Commerce.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Events.Products
{
    [Event(Order = 200)]
    public class ProductUpdated : DomainEvent, IProductEvent
    {
        public int ProductId { get; set; }

        protected ProductUpdated() { }

        public ProductUpdated(Product product)
        {
            ProductId = product.Id;
        }
    }
}