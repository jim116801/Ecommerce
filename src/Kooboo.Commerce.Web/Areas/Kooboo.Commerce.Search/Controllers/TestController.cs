﻿using Kooboo.Commerce.Products;
using Kooboo.Commerce.Search.Models;
using Kooboo.Commerce.Search.Facets;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kooboo.Commerce.Search.Controllers
{
    public class TestController : Controller
    {
        public void Index()
        {
            var indexer = IndexStores.Get<ProductModel>("Vitaminstore", CultureInfo.InvariantCulture);

            var results = indexer.Query().ToFacets(new Facet[] {
                new Facet { Field = "Brand" },
                new Facet { Field = "Price", Ranges = new List<FacetRange>
                {
                    FacetRange.Parse("[0 TO 5000}", "[0 TO 5000}"),
                    FacetRange.Parse("[5000 TO 10000}", "[5000 TO 10000}"),
                    FacetRange.Parse("[10000 TO NULL]", "[10000 TO NULL]")
                }}
            });

            foreach (var result in results)
            {
                Response.Write("<h5>" + result.Name+ "</h5>");
                foreach (var value in result.Values)
                {
                    Response.Write(value.Term + "(" + value.Hits + "), ");
                }
            }
        }
    }
}
