using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class NumericRangeFilterDTO : MetricFilterDTO
    {
        public int? MinValue { get; set; }

        public int? MaxValue { get; set; }

        public int? StartValue { get; set; }

        public int? EndValue { get; set; }
    }
}