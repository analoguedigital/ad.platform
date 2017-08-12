using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public enum MetricFilterTypes
    {
        Text,
        Dropdown,
        Checkbox,
        Dichotomous,
        Slider,
        DateRange,
        NumericRange,
        TimeRange
    }

    public class MetricFilter
    {
        public Guid MetricId { get; set; }

        public string ShortTitle { get; set; }

        public string Description { get; set; }

        public string FilterType { get; set; }
    }

    public class MetricFilterDataItem
    {
        public string Text { get; set; }

        public int Value { get; set; }
    }
}
