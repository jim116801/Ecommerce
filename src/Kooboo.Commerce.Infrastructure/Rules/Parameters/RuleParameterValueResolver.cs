﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Rules.Parameters
{
    /// <summary>
    /// Defines methods to resolve parameter value from the context object.
    /// </summary>
    public abstract class RuleParameterValueResolver
    {
        /// <summary>
        /// Resolve the parameter value of the specified parameter from the context object.
        /// </summary>
        /// <param name="param">The parameter to resolve value.</param>
        /// <param name="dataContext">The context object.</param>
        /// <returns>The value of the parameter.</returns>
        public abstract object ResolveValue(RuleParameter param, object dataContext);

        public static RuleParameterValueResolver FromDelegate(Func<RuleParameter, object, object> resolver)
        {
            return new FuncParameterValueResolver(resolver);
        }

        public static RuleParameterValueResolver Dumb()
        {
            return DumbParameterValueResolver.Instance;
        }
    }

    /// <summary>
    /// Represents a parameter resolve directly returning the data context as the value of the parameter.
    /// It can be used as the Null-Object pattern to eliminate the boring null checks.
    /// </summary>
    class DumbParameterValueResolver : RuleParameterValueResolver
    {
        public static readonly DumbParameterValueResolver Instance = new DumbParameterValueResolver();

        public override object ResolveValue(RuleParameter param, object dataContext)
        {
            return dataContext;
        }
    }

    class FuncParameterValueResolver : RuleParameterValueResolver
    {
        private Func<RuleParameter, object, object> _resolveValue;

        public FuncParameterValueResolver(Func<RuleParameter, object, object> resolveValue)
        {
            _resolveValue = resolveValue;
        }

        public override object ResolveValue(RuleParameter param, object dataContext)
        {
            return _resolveValue(param, dataContext);
        }
    }
}
