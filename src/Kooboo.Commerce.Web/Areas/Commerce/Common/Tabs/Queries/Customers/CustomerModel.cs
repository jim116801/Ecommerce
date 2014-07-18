﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kooboo.Commerce.Customers;
using Kooboo.Commerce.Web.Grid2;
using Kooboo.Web.Mvc.Grid2.Design;
using Kooboo.Commerce.Web.Framework.UI.Tabs.Queries;

namespace Kooboo.Commerce.Web.Areas.Commerce.Models.Customers
{
    public class CustomerModel : ICustomerModel
    {
        [GridColumn]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [GridColumn(GridItemColumnType = typeof(LinkedGridItemColumn))]
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [GridColumn]
        public string Email { get; set; }

        public CustomerModel() { }

        public CustomerModel(Customer customer)
        {
            Id = customer.Id;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Email = customer.Email;
        }
    }
}