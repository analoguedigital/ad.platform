using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.FilterValues
{
    public class RangeFilterValue : FilterValue
    {
        public Object FromValue { get; set; }

        public Object ToValue { get; set; }
    }
}
