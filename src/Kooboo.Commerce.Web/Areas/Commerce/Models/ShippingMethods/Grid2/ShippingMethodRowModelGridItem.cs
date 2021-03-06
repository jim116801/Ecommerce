﻿using Kooboo.Web.Mvc.Grid2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kooboo.Commerce.Web.Areas.Commerce.Models.ShippingMethods.Grid2
{
    public class ShippingMethodRowModelGridItem : GridItem
    {
        public ShippingMethodRowModelGridItem(IGridModel gridModel, object dataItem, int dataIndex)
            : base(gridModel, dataItem, dataIndex)
        {
        }

        public override IHtmlString RenderItemContainerAtts()
        {
            var model = (ShippingMethodRowModel)DataItem;
            return MvcHtmlString.Create(String.Format("class='{0}'", model.IsEnabled ? "enabled" : "disabled"));
        }
    }
}