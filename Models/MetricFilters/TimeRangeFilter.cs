using System;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class TimeRangeFilter : MetricFilter
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
