﻿using Kooboo.CMS.Common.Runtime;
using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.Commerce.Accessories;
using Kooboo.Commerce.CMSIntegration.DataSources.Generic;
using Kooboo.Commerce.Data;
using Kooboo.Commerce.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Commerce.Api;
using Kooboo.Commerce.Api.Metadata;

namespace Kooboo.Commerce.CMSIntegration.DataSources.Accessories
{
    [DataContract]
    [KnownType(typeof(AccessoryDataSource))]
    public class AccessoryDataSource : GenericCommerceDataSource
    {
        public override string Name
        {
            get
            {
                return "Accessories";
            }
        }

        public override IEnumerable<FilterDescription> Filters
        {
            get
            {
                return new[] { new FilterDescription("ByProduct", new Int32ParameterDescription("ProductId", true)) };
            }
        }

        public override IEnumerable<string> SortFields
        {
            get
            {
                return null;
            }
        }

        public override IEnumerable<string> OptionalIncludeFields
        {
            get
            {
                return null;
            }
        }

        // Cannot inject IProductService instance to this class
        // because ICommerceSource will be resolved in CMS data source management page
        // where there's no a commerce instance context
        // while IProductService requires a commerce instance context.
        public Func<ProductService> ProductService = () => EngineContext.Current.Resolve<ProductService>();

        public Func<IProductAccessoryService> ProductAccessoryService = () => EngineContext.Current.Resolve<IProductAccessoryService>();

        private ICommerceInstanceManager _instanceManager;

        public AccessoryDataSource(ICommerceInstanceManager instanceManager)
        {
            _instanceManager = instanceManager;
        }

        protected override object DoExecute(CommerceDataSourceContext context, ParsedGenericCommerceDataSourceSettings settings)
        {
            var productIdFilter = settings.Filters.Find(f => f.Name == "ByProduct");
            if (productIdFilter == null)
            {
                return null;
            }

            var productId = productIdFilter.GetParameterValue("productId");
            if (productId == null)
            {
                return null;
            }

            var instanceName = context.Site.GetCommerceInstanceName();

            if (String.IsNullOrWhiteSpace(instanceName))
                throw new InvalidOperationException("Commerce instance name is not configured in CMS.");

            using (var instance = _instanceManager.GetInstance(instanceName))
            using (var scope = Scope.Begin(instance))
            {
                var accessories = ProductAccessoryService().GetAccessories((int)productId);
                if (!String.IsNullOrEmpty(settings.SortField) && settings.SortField == "Rank")
                {
                    if (settings.SortDirection == SortDirection.Asc)
                    {
                        accessories = accessories.OrderBy(r => r.Rank).ToList();
                    }
                    else
                    {
                        accessories = accessories.OrderByDescending(r => r.Rank).ToList();
                    }
                }

                if (settings.Top != null)
                {
                    accessories = accessories.Take(settings.Top.Value).ToList();
                }

                var accessoryIds = accessories.Select(x => x.ProductId).ToArray();

                var result = new List<Kooboo.Commerce.Api.Products.Product>();
                foreach (var id in accessoryIds)
                {
                    // TODO: should not use resolve, use ApiService instead
                    var model = EngineContext.Current.Resolve<Kooboo.Commerce.Api.Products.IProductApi>()
                                           .Query()
                                           .ById(id)
                                           .Include("PriceList")
                                           .Include("Images")
                                           .Include("Brand")
                                           .FirstOrDefault();
                    if (model != null)
                    {
                        result.Add(model);
                    }
                }

                return result;
            }
        }

        public override IDictionary<string, object> GetDefinitions(CommerceDataSourceContext context)
        {
            return DataSourceDefinitionHelper.GetDefinitions(typeof(Kooboo.Commerce.Api.Products.Product));
        }
    }
}
