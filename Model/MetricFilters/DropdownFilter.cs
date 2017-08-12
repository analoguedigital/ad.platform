using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class DropdownFilter : MetricFilter
    {
        public List<MetricFilterDataItem> DataList { get; set; }

        public int SelectedValue { get; set; }

        public DropdownFilter()
        {
            this.DataList = new List<MetricFilterDataItem>();
        }
    }
}
