﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.Commerce.Data;
using Kooboo.Commerce.Events;
using Kooboo.Commerce.Events.Brands;

namespace Kooboo.Commerce.Brands.Services
{
    [Dependency(typeof(IBrandService))]
    public class BrandService : IBrandService
    {
        private readonly IRepository<Brand> _brandRepository;

        public BrandService(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public Brand GetById(int id)
        {
            var query = Query();
            return query.Where(o => o.Id == id).FirstOrDefault();
        }

        public IQueryable<Brand> Query()
        {
            return _brandRepository.Query();
        }

        public void Create(Brand brand)
        {
            _brandRepository.Insert(brand);
            Event.Raise(new BrandCreated(brand));
        }

        public void Update(Brand brand)
        {
            _brandRepository.Update(brand);
            Event.Raise(new BrandUpdated(brand));
        }

        public void Delete(Brand brand)
        {
            _brandRepository.Delete(brand);
            Event.Raise(new BrandDeleted(brand));
        }
    }
}