using System;

namespace LightMethods.Survey.Models.MetricFilters
{
    public enum MetricFilterTypes
    {
        Text,
        Checkbox,
        DateRange,
        NumericRange,
        TimeRange
    }

    public class MetricFilter
    {
        public string ShortTitle { get; set; }

        public string Type { get; set; }
    }

    public class MetricFilterOption
    {
        public Guid? Id { get; set; }

        public string Text { get; set; }

        public int Value { get; set; }

    }
}
