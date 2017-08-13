using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class SliderFilter : MetricFilter
    {
        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int DefaultValue { get; set; }

        public List<MetricFilterDataItem> DataList { get; set; }

        public int SelectedValue { get; set; }

        public SliderFilter()
        {
            this.DataList = new List<MetricFilterDataItem>();
        }
    }
}
