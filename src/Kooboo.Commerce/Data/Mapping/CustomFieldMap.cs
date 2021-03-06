﻿using Kooboo.Commerce.Products;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Data.Mapping
{
    class CustomFieldMap : EntityTypeConfiguration<CustomFieldDefinition>
    {
        public CustomFieldMap()
        {
            this.HasMany(o => o.ValidationRules).WithRequired().WillCascadeOnDelete(true);
        }
    }
}
