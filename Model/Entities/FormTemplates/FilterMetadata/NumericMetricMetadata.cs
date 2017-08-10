using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class NumericMetricMetadata : FilterMetadata
    {
        public int? MinVal { get; set; }
        public int? MaxVal { get; set; }
    }
}
