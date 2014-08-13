﻿using System;
using System.Linq;
using System.Web.Mvc;
using Kooboo.Commerce.Customers;
using Kooboo.Commerce.Customers.Services;
using Kooboo.Commerce.Data;
using Kooboo.Commerce.Orders.Services;
using Kooboo.Commerce.Web.Areas.Commerce.Models.Customers;
using Kooboo.Commerce.Countries.Services;
using Kooboo.Commerce.Web.Framework.Mvc;
using Kooboo.Commerce.Web.Framework.UI.Topbar;
using Kooboo.Commerce.Web.Areas.Commerce.Topbar;
using Kooboo.Commerce.Web.Framework.UI.Tabs.Queries;
using Kooboo.Commerce.Web.Areas.Commerce.Models.TabQueries;
using Kooboo.Commerce.Web.Areas.Commerce.Tabs.Queries.Customers;
using Kooboo.Commerce.Web.Areas.Commerce.Tabs.Queries.Customers.Default;
using Kooboo.Commerce.Web.Framework.Mvc.ModelBinding;

namespace Kooboo.Commerce.Web.Areas.Commerce.Controllers
{
    public class CustomerController : CommerceController
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly ICountryService _countryService;

        public CustomerController(ICustomerService customerService, ICountryService countryService, IOrderService orderService)
        {
            _customerService = customerService;
            _countryService = countryService;
            _orderService = orderService;
        }

        public ActionResult Index()
        {
            var model = this.CreateTabQueryModel("Customers", new DefaultCustomersQuery());
            return View(model);
        }

        public ActionResult Create()
        {
            return View("Edit");
        }

        public ActionResult Edit(int id)
        {
            var customer = _customerService.GetById(id);

            ViewBag.ToolbarCommands = TopbarCommands.GetCommands(ControllerContext, customer, CurrentInstance);

            return View("Edit");
        }

        [HttpGet]
        public ActionResult Get(int? id = null)
        {
            var obj = id == null ? null : _customerService.GetById(id.Value);
            if (obj == null)
            {
                obj = new Customer();
            }
            return JsonNet(obj);
        }

        [HttpPost, Transactional]
        public ActionResult Save(Customer obj)
        {
            try
            {
                if (obj.Id > 0)
                {
                    _customerService.Update(obj);
                }
                else
                {
                    _customerService.Create(obj);
                }

                return this.JsonNet(new { status = 0, message = "customer succssfully saved." });
            }
            catch (Exception ex)
            {
                return this.JsonNet(new { status = 1, message = ex.Message });
            }
        }

        [HttpPost, HandleAjaxFormError]
        public ActionResult Delete(CustomerModel[] model)
        {
            foreach (var obj in model)
            {
                var customer = _customerService.GetById(obj.Id);
                _customerService.Delete(customer);
            }

            return AjaxForm().ReloadPage();
        }

        [HttpGet]
        public ActionResult GetCountries()
        {

            var objs = _countryService.Query();
            return JsonNet(objs);
        }

        [HttpGet]
        public ActionResult GetOrders(int customerId, int page = 1, int pageSize = 50)
        {
            var objs = _orderService.Query()
                                    .Where(o => o.CustomerId == customerId)
                                    .OrderByDescending(o => o.Id)
                                    .Paginate(page - 1, pageSize)
                                    .ToPagedList();
            return JsonNet(objs);
        }
    }
}