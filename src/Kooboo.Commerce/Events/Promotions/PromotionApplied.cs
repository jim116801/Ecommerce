﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Events.Promotions
{
    [Event(Order = 600)]
    public class PromotionApplied : BusinessEvent, IPromotionEvent
    {
        public int PromotionId { get; set; }
    }
}
