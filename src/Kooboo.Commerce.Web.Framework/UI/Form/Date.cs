﻿using Kooboo.CMS.Common.Runtime.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kooboo.Commerce.Web.Framework.UI.Form
{
    [Dependency(typeof(IFormControl), Key = "Date")]
    public class Date : InputControl
    {
        protected override string Type
        {
            get { return "text"; }
        }

        public override bool IsValuesPredefined
        {
            get { return false; }
        }

        public override bool IsSelectionList
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "Date"; }
        }

        protected override void BuildControl(System.Web.Mvc.TagBuilder builder, Products.CustomFieldDefinition field, string value, object htmlAttributes, System.Web.Mvc.ViewContext viewContext)
        {
            builder.MergeAttribute("data-toggle", "datepicker");
            base.BuildControl(builder, field, value, htmlAttributes, viewContext);
        }
    }
}