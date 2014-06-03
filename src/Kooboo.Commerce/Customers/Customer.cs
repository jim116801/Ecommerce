﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Commerce.Locations;
using Kooboo.Commerce.Rules;
using Kooboo.Commerce.ComponentModel;
using Kooboo.Commerce.Events;
using Kooboo.Commerce.Events.Customers;

namespace Kooboo.Commerce.Customers
{
    public class Customer : INotifyCreated, INotifyUpdated, INotifyDeleted
    {
        public Customer()
        {
            Addresses = new List<Address>();
            CustomFields = new List<CustomerCustomField>();
        }

        [Param]
        public int Id { get; set; }

        public string AccountId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [Param]
        public string Email { get; set; }

        [Param]
        public Gender Gender { get; set; }

        public string Phone { get; set; }

        public int? CountryId { get; set; }

        public int? ShippingAddressId { get; set; }

        public int? BillingAddressId { get; set; }

        /// <summary>
        /// Redundant field for easy query only.  The detail address information should be in the Addresses field.
        /// </summary>
        public string City { get; set; }

        public virtual Country Country { get; set; }

        /// <summary>
        /// The list of addressed used by this user.
        /// </summary>
        public virtual List<Address> Addresses { get; set; }

        public virtual Address ShippingAddress 
        { 
            get
            {
                if(Addresses != null && ShippingAddressId.HasValue)
                {
                    return Addresses.FirstOrDefault(o => o.Id == ShippingAddressId.Value);
                }
                return null;
            }
        }

        public virtual Address BillingAddress
        {
            get
            {
                if (Addresses != null && BillingAddressId.HasValue)
                {
                    return Addresses.FirstOrDefault(o => o.Id == BillingAddressId.Value);
                }
                return null;
            }
        }

        public virtual CustomerLoyalty Loyalty { get; set; }
        public virtual ICollection<CustomerCustomField> CustomFields { get; set; }

        [Param]
        public string FullName
        {
            get { return string.Format("{0} {1} {2}", FirstName, MiddleName, LastName); }
        }

        void INotifyCreated.NotifyCreated()
        {
            Event.Raise(new CustomerCreated(this));
        }

        void INotifyUpdated.NotifyUpdated()
        {
            Event.Raise(new CustomerUpdated(this));
        }

        void INotifyDeleted.NotifyDeleted()
        {
            Event.Raise(new CustomerDeleted(this));
        }
    }
}