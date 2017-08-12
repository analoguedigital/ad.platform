using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class TimeRangeFilter : MetricFilter
    {
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
