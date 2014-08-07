﻿using Kooboo.Commerce.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kooboo.CMS.Sites.Membership;
using System.Globalization;
using System.Reflection;
using Kooboo.Commerce.CMSIntegration.DataSources.Generic;
using System.Runtime.Serialization;

namespace Kooboo.Commerce.CMSIntegration.DataSources.Generic.ApiBased
{
    [DataContract]
    [KnownType(typeof(ApiBasedDataSource))]
    public abstract class ApiBasedDataSource : GenericCommerceDataSource
    {
        private ApiQueryDescriptor _descriptor;

        protected ApiQueryDescriptor Descriptor
        {
            get
            {
                if (_descriptor == null)
                {
                    _descriptor = ApiQueryDescriptor.GetDescriptor(QueryType);
                }
                return _descriptor;
            }
        }

        protected abstract Type QueryType { get; }

        protected abstract Type ItemType { get; }

        private List<FilterDefinition> _filters;

        public override IEnumerable<FilterDefinition> Filters
        {
            get
            {
                return InternalFilters;
            }
        }

        protected List<FilterDefinition> InternalFilters
        {
            get
            {
                if (_filters == null)
                {
                    _filters = Descriptor.Filters.Select(f => f.ToFilterDefinition()).ToList();
                }
                return _filters;
            }
        }

        private HashSet<string> _sortableFields;

        public override IEnumerable<string> SortableFields
        {
            get
            {
                return InternalSortableFields;
            }
        }

        protected HashSet<string> InternalSortableFields
        {
            get
            {
                if (_sortableFields == null)
                {
                    _sortableFields = new HashSet<string>();
                }
                return _sortableFields;
            }
        }

        private HashSet<string> _includablePaths;

        public override IEnumerable<string> IncludablePaths
        {
            get
            {
                return InternalIncludablePaths;
            }
        }

        protected HashSet<string> InternalIncludablePaths
        {
            get
            {
                if (_includablePaths == null)
                {
                    _includablePaths = new HashSet<string>(Descriptor.IncludablePaths);
                }
                return _includablePaths;
            }
        }

        protected abstract object GetQuery(ICommerceApi api);

        protected override object DoExecute(CommerceDataSourceContext context, ParsedGenericCommerceDataSourceSettings settings)
        {
            var user = new HttpContextWrapper(HttpContext.Current).Membership().GetMembershipUser();
            var accountId = user == null ? null : user.UUID;

            var apiContext = new ApiContext(context.Site.GetCommerceInstanceName(), CultureInfo.GetCultureInfo(context.Site.Culture), null, accountId);
            var api = ApiService.Get(context.Site.ApiType(), apiContext);

            var query = GetQuery(api);

            if (settings.Filters != null && settings.Filters.Count > 0)
            {
                ApplyFilters(query, settings.Filters, context);
            }
            if (settings.Includes != null && settings.Includes.Count > 0)
            {
                ApplyIncludes(query, settings.Includes, context);
            }

            // Executing result
            if (settings.TakeOperation == Kooboo.Commerce.CMSIntegration.DataSources.TakeOperation.List)
            {
                if (settings.EnablePaging)
                {
                    var pageSize = settings.PageSize.GetValueOrDefault(10);
                    var pageNumber = settings.PageNumber.GetValueOrDefault(1);

                    return CallMethod(query, "Pagination", pageNumber - 1, pageSize);
                }

                return CallMethod(query, "ToArray");
            }
            else
            {
                return CallMethod(query, "FirstOrDefault");
            }
        }

        public override IDictionary<string, object> GetDefinitions(CommerceDataSourceContext context)
        {
            return DataSourceDefinitionHelper.GetDefinitions(ItemType);
        }

        protected virtual void ApplyFilters(object query, List<ParsedFilter> filters, CommerceDataSourceContext context)
        {
            foreach (var filter in filters)
            {
                var definition = Descriptor.Filters.FirstOrDefault(f => f.Name == filter.Name);
                if (definition != null)
                {
                    var parameters = new object[definition.Parameters.Count];
                    for (var i = 0; i < definition.Parameters.Count; i++)
                    {
                        var param = definition.Parameters[i];
                        var paramValue = filter.ParameterValues[param.Name];
                        parameters[i] = filter.ParameterValues[param.Name];
                    }

                    CallMethod(query, definition.Method.Name, parameters);
                }
            }
        }

        protected object CallMethod(object obj, string method, params object[] parameters)
        {
            var methodInfo = obj.GetType().GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (methodInfo == null)
                throw new InvalidOperationException("Method '" + method + "' was not found.");

            return methodInfo.Invoke(obj, parameters);
        }

        protected virtual void ApplyIncludes(object query, IEnumerable<string> includes, CommerceDataSourceContext context)
        {
            var methods = query.GetType()
                               .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                               .Where(m => m.Name == "Include");

            MethodInfo includeMethod = null;

            foreach (var method in methods)
            {
                var args = method.GetParameters();
                if (args.Length == 1 && args[0].ParameterType == typeof(string))
                {
                    includeMethod = method;
                    break;
                }
            }

            if (includeMethod == null)
                throw new InvalidOperationException("Cannot find Include(string path) method in query api.");

            foreach (var path in includes)
            {
                includeMethod.Invoke(query, new object[] { path });
            }
        }
    }
}