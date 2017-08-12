using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class NumericRangeFilter : MetricFilter
    {
        public int? MinValue { get; set; }

        public int? MaxValue { get; set; }
    }
}
