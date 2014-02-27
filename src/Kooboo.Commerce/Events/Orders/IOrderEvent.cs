﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Commerce.Events;
using Kooboo.Commerce.Orders;
using Kooboo.Commerce.Activities;
using System.ComponentModel;

namespace Kooboo.Commerce.Events.Orders
{
    [ActivityVisible("Order Events")]
    public interface IOrderEvent : IEvent
    {
        Order Order { get; }
    }
}
