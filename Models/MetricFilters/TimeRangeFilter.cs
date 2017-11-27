using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class TimeRangeFilter : MetricFilter
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
