﻿using Kooboo.Commerce.Orders;
using Kooboo.Commerce.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Events.Orders
{
    [Event(Order = 100)]
    public class OrderCreated : DomainEvent, IOrderEvent
    {
        [Reference(typeof(Order), Prefix = "")]
        public int OrderId { get; set; }

        protected OrderCreated() { }

        public OrderCreated(Order order)
        {
            OrderId = order.Id;
        }
    }
}
