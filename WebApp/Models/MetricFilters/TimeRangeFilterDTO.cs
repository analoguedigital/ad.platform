using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class TimeRangeFilterDTO : MetricFilterDTO
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}