using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class CheckboxFilter : MetricFilter
    {
        public List<MetricFilterDataItem> DataList { get; set; }

        public CheckboxFilter()
        {
            this.DataList = new List<MetricFilterDataItem>();
        }
    }
}
