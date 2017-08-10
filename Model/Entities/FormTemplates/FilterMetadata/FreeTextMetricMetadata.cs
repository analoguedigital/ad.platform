﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class FreeTextMetricMetadata : FilterMetadata
    {
        public int NumberOfLines { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }
    }
}
