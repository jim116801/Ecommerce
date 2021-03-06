﻿using Kooboo.CMS.Common.Runtime;
using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.Commerce.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;

namespace Kooboo.Commerce.Web.Framework.Http
{
    public class TransactionalAttribute : ActionFilterAttribute
    {
        private ICommerceDbTransaction _transaction;

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            var instance = CommerceInstance.Current;
            if (instance == null)
                throw new InvalidOperationException(typeof(TransactionalAttribute).Name + " can only be applied to an action within commerce instance context.");

            _transaction = instance.Database.BeginTransaction();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                if (actionExecutedContext.Exception == null && !_transaction.IsCommitted)
                {
                    _transaction.Commit();
                }
            }
            finally
            {
                // reset fields to null in case the filter instance is cached by asp.net mvc
                _transaction.Dispose();
                _transaction = null;
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
