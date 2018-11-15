using System;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class DateRangeFilter : MetricFilter
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool CanSelectTime { get; set; }
    }
}
