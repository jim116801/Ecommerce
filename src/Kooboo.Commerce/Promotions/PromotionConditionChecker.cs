﻿using Kooboo.Commerce.Customers;
using Kooboo.Commerce.Rules;
using Kooboo.Commerce.Rules.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Promotions
{
    /// <summary>
    /// A checker to check if a promotion can be used for the current context.
    /// </summary>
    public class PromotionConditionChecker
    {
        public CheckPromotionConditionResult CheckConditions(Promotion promotion, PromotionContext context)
        {
            if (String.IsNullOrWhiteSpace(promotion.ConditionsExpression))
            {
                return new CheckPromotionConditionResult
                {
                    Success = true,
                    MatchedItems = context.Items.ToList()
                };
            }

            var expression = Expression.Parse(promotion.ConditionsExpression);
            var ruleEngine = new RuleEngine();
            var result = new CheckPromotionConditionResult();

            foreach (var item in context.Items)
            {
                var contextModel = new PromotionConditionContextModel
                {
                    Item = item,
                    Customer = context.Customer
                };

                if (ruleEngine.CheckCondition(expression, contextModel))
                {
                    result.Success = true;
                    result.MatchedItems.Add(item);
                }
            }

            return result;
        }
    }
}