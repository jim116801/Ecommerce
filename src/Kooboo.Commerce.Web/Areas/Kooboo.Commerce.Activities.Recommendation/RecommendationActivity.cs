﻿using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.Commerce.Events.Products;
using Kooboo.Commerce.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kooboo.Commerce.Activities.Recommendation
{
    [Dependency(typeof(IActivity), Key = "Kooboo.Commerce.Activities.Recommendation.RecommendationActivity")]
    public class RecommendationActivity : IActivity
    {
        public string Name
        {
            get
            {
                return Strings.ActivityName;
            }
        }

        public string DisplayName
        {
            get
            {
                return "Kooboo BigData Recommendation";
            }
        }

        public bool CanBindTo(Type eventType)
        {
            return typeof(GetRecommendations).IsAssignableFrom(eventType);
        }

        public ActivityResponse Execute(Events.IEvent evnt, ActivityBinding binding)
        {
            var queryEvent = evnt as GetRecommendations;
            queryEvent.Result = new List<Product>
            {
                new Product { Id = 1, Name = "Recommended product 1" },
                new Product { Id = 2, Name = "Recommended product 2" }
            };

            return ActivityResponse.Continue;
        }
    }
}