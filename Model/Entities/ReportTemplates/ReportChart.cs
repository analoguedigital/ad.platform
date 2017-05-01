using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.Entities
{
    public class ReportChart:ReportItem
    {
        public virtual ICollection<ChartSerie> Series { get; set; }
    }
}
