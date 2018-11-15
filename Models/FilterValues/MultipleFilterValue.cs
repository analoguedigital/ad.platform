using System.Collections.Generic;

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
