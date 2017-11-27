namespace LightMethods.Survey.Models.MetricFilters
{
    public class NumericRangeFilter : MetricFilter
    {
        public int? MinValue { get; set; }

        public int? MaxValue { get; set; }
    }
}
