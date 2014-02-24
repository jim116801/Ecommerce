﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Products.Services {

    public interface IProductTypeService {

        ProductType GetById(int id);

        IEnumerable<ProductType> GetAllProductTypes();

        IPagedList<T> GetAllProductTypes<T>(int? pageIndex, int? pageSize, Func<ProductType, T> func);

        void Create(ProductType type);

        void Update(ProductType type);

        void Update(ProductType oldType, ProductType newType);

        void Delete(ProductType type);

        void Enable(ProductType type);

        void Disable(ProductType type);
    }
}