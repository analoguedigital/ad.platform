using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.FilterValues
{
    public class RangeFilterValue : FilterValue
    {
        public string FromValue { get; set; }

        public string ToValue { get; set; }
    }
}
