﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.Models;
using Kooboo.CMS.Sites.Membership;
using Kooboo.CMS.Common;
using Kooboo.Commerce.Orders;
using Kooboo.Commerce.Payments;
using Kooboo.Commerce.Web.Mvc;

namespace Kooboo.Commerce.API.PlugIn
{
    public class SaveOrderPlugIn : ISubmissionPlugin
    {
        public Dictionary<string, object> Parameters
        {
            get { return null; }
        }

        public ActionResult Submit(Site site, ControllerContext controllerContext, SubmissionSetting submissionSetting)
        {
            JsonResultData resultData = new JsonResultData();

            try
            {
                var commerService = site.Commerce();
                string sessionId = controllerContext.HttpContext.Session.SessionID;
                var memberAuth = controllerContext.HttpContext.Membership();
                var member = memberAuth.GetMembershipUser();
                var order = commerService.Order.GetMyOrder(sessionId, member);

                if (order != null)
                {
                    var form = controllerContext.HttpContext.Request.Form;

                    order.Coupon = form["Coupon"];
                    //order.ChangeOrderStatus((OrderStatus)Enum.Parse(typeof(OrderStatus), form["OrderStatus"]));
                    //order.ForceChangePaymentStatus((PaymentStatus)Enum.Parse(typeof(PaymentStatus), form["PaymentStatus"]));
                    order.SubTotal = string.IsNullOrEmpty(form["SubTotal"]) ? 0 : Convert.ToDecimal(form["SubTotal"]);
                    order.Discount = string.IsNullOrEmpty(form["Discount"]) ? 0 : Convert.ToDecimal(form["Discount"]);
                    order.TotalTax = string.IsNullOrEmpty(form["TotalTax"]) ? 0 : Convert.ToDecimal(form["TotalTax"]);
                    order.TotalWeight = string.IsNullOrEmpty(form["TotalWeight"]) ? 0 : Convert.ToDecimal(form["TotalWeight"]);
                    order.ShippingCost = string.IsNullOrEmpty(form["ShippingCost"]) ? 0 : Convert.ToDecimal(form["ShippingCost"]);
                    if (!string.IsNullOrEmpty(form["PaymentMethodId"]))
                        order.PaymentMethodId = Convert.ToInt32(form["PaymentMethodId"]);
                    order.PaymentMethodCost = string.IsNullOrEmpty(form["PaymentMethodCost"]) ? 0 : Convert.ToDecimal(form["PaymentMethodCost"]);
                    order.Total = string.IsNullOrEmpty(form["Total"]) ? 0 : Convert.ToDecimal(form["Total"]);

                    order.ShippingAddress = FormHelper.BindToModel<OrderAddress>(form, "ShippingAddress");
                    if (order.ShippingAddress != null)
                    {
                        order.ShippingAddress.CustomerId = order.CustomerId;
                        order.ShippingAddress.Id = order.ShippingAddressId.HasValue ? order.ShippingAddressId.Value : 0;
                    }
                    order.BillingAddress = FormHelper.BindToModel<OrderAddress>(form, "BillingAddress");
                    if (order.BillingAddress != null)
                    {
                        order.BillingAddress.CustomerId = order.CustomerId;
                        order.BillingAddress.Id = order.BillingAddressId.HasValue ? order.BillingAddressId.Value : 0;
                    }

                    order.Remark = form["Remark"];

                    var orderItems = FormHelper.BindToModels<OrderItem>(form, "OrderItem_");
                    foreach(var item in orderItems)
                    {
                        var oldOrderItem = order.OrderItems.FirstOrDefault(o => o.Id == item.Id);
                        if(oldOrderItem != null)
                        {
                            if(item.Quantity <= 0)
                            {
                                order.OrderItems.Remove(oldOrderItem);
                            }
                            else
                            {
                                oldOrderItem.Quantity = item.Quantity;
                                oldOrderItem.SubTotal = oldOrderItem.UnitPrice * oldOrderItem.Quantity;
                                oldOrderItem.Total = oldOrderItem.SubTotal - oldOrderItem.Discount + oldOrderItem.TaxCost;
                            }
                        }
                    }

                    // don't recalculate, accept the user input values.
                    //if (order.OrderItems != null)
                    //{
                    //    decimal subtotal = 0.0m;
                    //    decimal discount = 0.0m;
                    //    decimal tax = 0m;
                    //    foreach (var item in order.OrderItems)
                    //    {
                    //        subtotal += item.SubTotal;
                    //        discount += item.Discount;
                    //        tax += item.TaxCost;
                    //    }

                    //    order.SubTotal = subtotal;
                    //    order.Discount = discount;
                    //    order.TotalTax = tax;
                    //    order.Total = order.SubTotal - order.Discount + order.TotalTax + order.ShippingCost + order.PaymentMethodCost;
                    //}


                    if (commerService.Order.SaveOrder(order))
                    {
                        resultData.Success = true;
                        resultData.AddMessage("Successfully save order.");
                    }
                    else
                    {
                        resultData.Success = false;
                        resultData.AddMessage("Order saved fail.");
                    }
                }
                else
                {
                    resultData.Success = false;
                    resultData.AddMessage("Cannot find your order.");
                }
            }
            catch (Exception ex)
            {
                resultData.Success = false;
                resultData.AddMessage(ex.Message);
            }
            return new JsonResult() { Data = resultData };
        }
    }
}