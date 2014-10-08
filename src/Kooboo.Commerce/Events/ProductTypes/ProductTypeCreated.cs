﻿using Kooboo.Commerce.Products;
using Kooboo.Commerce.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Events.ProductTypes
{
    public class ProductTypeCreated : IProductTypeEvent
    {
        [Reference(typeof(ProductType))]
        public int ProductTypeId { get; set; }

        public ProductTypeCreated() { }

        public ProductTypeCreated(ProductType productType)
        {
            ProductTypeId = productType.Id;
        }
    }
}
