using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class TextFilter : MetricFilter
    {
        public int MaxLength { get; set; }

        public int NumberOfLines { get; set; }

        public string SelectedValue { get; set; }
    }
}
