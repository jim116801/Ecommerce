﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kooboo.Commerce.Products;
using Kooboo.Commerce.Data;

namespace Kooboo.Commerce.Products
{
    public class ProductCustomField : IOrphanable, ICustomField
    {
        [Key]
        protected int Id { get; set; }

        [StringLength(50)]
        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        [Column]
        protected int? ProductId { get; set; }

        public ProductCustomField() { }

        public ProductCustomField(string fieldName, string fieldValue)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
        }

        bool IOrphanable.IsOrphan()
        {
            return ProductId == null;
        }

        public override string ToString()
        {
            return FieldName + " = " + FieldValue;
        }
    }
}
