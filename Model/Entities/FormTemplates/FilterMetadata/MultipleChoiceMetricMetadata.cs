using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class MultipleChoiceMetricMetadata : FilterMetadata
    {
        public Guid DataListId { get; set; }
        public DataList DataList { get; set; }
        public MultipleChoiceViewType ViewType { get; set; }
    }
}
