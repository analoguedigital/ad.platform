using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class RateMetricMetadata : FilterMetadata
    {
        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int DefaultValue { get; set; }

        public Guid? DataListId { get; set; }

        public DataList DataList { get; set; }
    }
}
