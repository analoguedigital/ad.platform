namespace LightMethods.Survey.Models.MetricFilters
{
    public class TextFilter : MetricFilter
    {
        public int MaxLength { get; set; }

        public int NumberOfLines { get; set; }
    }
}
