﻿using Kooboo.Commerce.Categories;
using Kooboo.Commerce.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Events.Categories
{
    [Event(Order = 100)]
    public class CategoryCreated : DomainEvent, ICategoryEvent
    {
        [Reference(typeof(Category))]
        public int CategoryId { get; set; }

        protected CategoryCreated() { }

        public CategoryCreated(Category category)
        {
            CategoryId = category.Id;
        }
    }
}
