namespace LightMethods.Survey.Models.DTO
{
    public class FreeTextMetricDTO : MetricDTO
    {
        public int NumberOfLine { set; get; }

        public int MinLength { set; get; }

        public int MaxLength { set; get; }
    }
}
