﻿using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.Commerce.Events.Orders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Commerce.Rules.Activities.Simple
{
    public class SimpleActivity : Activity<IOrderEvent>
    {
        public override string Name
        {
            get
            {
                return "SimpleActivity";
            }
        }

        public override Type ConfigType
        {
            get
            {
                return typeof(SimpleActivityConfig);
            }
        }

        protected override void Execute(IOrderEvent @event, ActivityContext context)
        {
            var param = context.Config as SimpleActivityConfig;

            var orderId = @event.OrderId;

            Debug.WriteLine("[" + DateTime.Now + "] SimpleActivity: Execute for order #" + orderId + ", param1: " + param.Parameter1 + ", param2: " + param.Parameter2);
        }
    }
}
