using System;

namespace WebApi.Models
{
    public class RateMetricDTO : MetricDTO
    {
        public int MinValue { get; set; }

        public int MaxValue { get; set; }
    }
}
