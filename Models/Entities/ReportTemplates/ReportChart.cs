using System.Collections.Generic;

namespace LightMethods.Survey.Models.Entities
{
    public class ReportChart:ReportItem
    {
        public virtual ICollection<ChartSerie> Series { get; set; }
    }
}
