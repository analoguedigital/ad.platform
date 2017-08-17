using System.Collections.Generic;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class CheckboxFilter : MetricFilter
    {
        public List<MetricFilterOption> DataList { get; set; }

        public CheckboxFilter()
        {
            this.DataList = new List<MetricFilterOption>();
        }
    }
}
