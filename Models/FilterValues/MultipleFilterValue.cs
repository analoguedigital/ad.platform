using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.FilterValues
{
    public class MultipleFilterValue : FilterValue
    {
        public List<string> Values { get; set; }

        public MultipleFilterValue()
        {
            this.Values = new List<string>();
        }
    }
}
