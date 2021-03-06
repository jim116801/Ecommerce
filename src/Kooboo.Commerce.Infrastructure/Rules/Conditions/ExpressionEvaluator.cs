﻿using Kooboo.Commerce.Rules.Conditions.Expressions;
using Kooboo.Commerce.Rules.Conditions.Operators;
using Kooboo.Commerce.Rules.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kooboo.Commerce.Rules.Conditions
{
    /// <summary>
    /// Represents a evaluator to evaluate the result of a condition expression.
    /// </summary>
    class ExpressionEvaluator : ExpressionVisitor
    {
        private object _dataContext;
        private Stack<bool> _results = new Stack<bool>();
        private List<RuleParameter> _availableParameters;
        private RuleParameterProviderCollection _parameterProviders;
        private ComparisonOperatorCollection _comparisonOperators;

        public ExpressionEvaluator(
            RuleParameterProviderCollection parameterProviders, ComparisonOperatorCollection comparisonOperators)
        {
            Require.NotNull(parameterProviders, "parameterProviders");
            Require.NotNull(comparisonOperators, "comparisonOperators");

            _parameterProviders = parameterProviders;
            _comparisonOperators = comparisonOperators;
        }

        /// <summary>
        /// Evalute the value of the specified condition expression.
        /// </summary>
        /// <param name="expression">The condition expression to evaluate.</param>
        /// <param name="dataContext">The context object.</param>
        /// <returns>True if the the condition passed, false otherwise.</returns>
        public bool Evaluate(Expression expression, object dataContext)
        {
            Require.NotNull(expression, "expression");
            Require.NotNull(dataContext, "dataContext");

            _dataContext = dataContext;

            var dataContextType = dataContext.GetType();
            _availableParameters = _parameterProviders.SelectMany(x => x.GetParameters(dataContextType))
                                                      .DistinctBy(x => x.Name)
                                                      .ToList();

            Visit(expression);

            Debug.Assert(_results.Count == 1);

            return _results.Pop();
        }

        protected override void Visit(ComparisonExpression exp)
        {
            var paramName = exp.Param.ParamName;
            var param = _availableParameters.FirstOrDefault(x => x.Name == paramName);
            if (param == null)
                throw new UnrecognizedParameterException("Unrecognized parameter \"" + paramName + "\" or it's not accessable in currect context.");

            var @operator = _comparisonOperators.Find(exp.Operator);
            if (@operator == null)
                throw new UnrecognizedComparisonOperatorException("Unrecognized comparison operator \"" + exp.Operator + "\".");

            var result = false;

            var paramValue = param.ResolveValue(_dataContext);
            if (paramValue != null)
            {
                var paramType = paramValue.GetType();
                object conditionValue = null;

                if (paramType.IsEnum)
                {
                    conditionValue = Enum.Parse(paramType, exp.Value.Value);
                }
                else
                {
                    conditionValue = Convert.ChangeType(exp.Value.Value, paramType);
                }

                result = @operator.Apply(param, paramValue, conditionValue);
            }

            _results.Push(result);
        }

        protected override void Visit(LogicalBindaryExpression exp)
        {
            Visit(exp.Left);

            // Check if we can get result by only checking left expression.
            // If yes, then the result in the stack is alreay the result.
            var leftResult = _results.Peek();

            // A and B, if A is false, the result will always be false, no need to check B
            if (exp.Operator == LogicalOperator.AND && leftResult == false)
            {
                return;
            }
            // A or B, if A is true, the result will always be true, no need to check B
            if (exp.Operator == LogicalOperator.OR && leftResult == true)
            {
                return;
            }

            // Pop the result of the left expression
            _results.Pop();

            // If we cannot get result by checking left expression, 
            // then check right expression.
            // The final result is same as the result of the right expression.
            // For example,
            // A and B, if A is true, then final result = B
            // A or B, if A is false, then final result = B
            Visit(exp.Right);
        }
    }
}
