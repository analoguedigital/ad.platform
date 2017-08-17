using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class RangeFilterValueDTO : FilterValueDTO
    {
        public Object FromValue { get; set; }

        public Object ToValue { get; set; }
    }
}