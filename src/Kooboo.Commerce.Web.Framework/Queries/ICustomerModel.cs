﻿using Kooboo.Commerce.Web.Framework.Queries.Grid;
using Kooboo.Web.Mvc.Grid2.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Web.Framework.Queries
{
    [Grid(Checkable = true, IdProperty = "Id", GridItemType = typeof(CustomerGridItem))]
    public interface ICustomerModel
    {
        int Id { get; set; }
    }
}
