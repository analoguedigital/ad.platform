using System;

namespace WebApi.Models
{
    public class NumericMetricDTO : MetricDTO
    {
        public int? MinVal { get; set; }

        public int? MaxVal { get; set; }
    }
}
