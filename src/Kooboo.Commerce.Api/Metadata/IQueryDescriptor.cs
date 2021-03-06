﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Api.Metadata
{
    public interface IQueryDescriptor
    {
        IEnumerable<FilterDescription> Filters { get; }

        IEnumerable<string> OptionalIncludeFields { get; }

        IEnumerable<string> DefaultIncludedFields { get; }

        IEnumerable<string> SortFields { get; }
    }
}
