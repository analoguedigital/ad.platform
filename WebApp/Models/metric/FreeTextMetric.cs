using System;

namespace WebApi.Models
{
    public class FreeTextMetricDTO : MetricDTO
    {
        public int NumberOfLine { set; get; }

        public int MinLength { set; get; }

        public int MaxLength { set; get; }
    }
}
