using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public enum FilterInputType
    {
        Date,
        Time,
        DateTime,
        Numeric,
        Text,
        MultipleChoice,
        Dichotomous,
        Rate
    }

    public class FilterMetadata
    {
        public Guid MetricId { get; set; }

        public string ShortTitle { get; set; }

        public string Description { get; set; }

        public string SectionTitle { get; set; }

        public string InputType { get; set; }
    }
}
