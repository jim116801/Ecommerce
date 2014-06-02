﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Events.Categories
{
    [Event(Order = 200)]
    public class CategoryUpdated : DomainEvent, ICategoryEvent
    {
        public int CategoryId { get; set; }
    }
}