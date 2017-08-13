using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class DateRangeFilterDTO : MetricFilterDTO
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool CanSelectTime { get; set; }
    }
}