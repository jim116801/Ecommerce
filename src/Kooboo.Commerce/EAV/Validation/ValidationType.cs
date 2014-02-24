﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.EAV.Validation {

    public enum ValidationType {
        Required = 0,
        Unique = 1,
        StringLength = 2,
        Range = 3,
        Regex = 4
    }
}
