using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class DichotomousFilter : MetricFilter
    {
        public string YesText { get; set; }

        public string NoText { get; set; }

        public bool SelectedValue { get; set; }

        public DichotomousFilter()
        {
            this.YesText = "Yes";
            this.NoText = "No";
        }
    }
}
