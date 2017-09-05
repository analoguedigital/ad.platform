using LightMethods.Survey.Models.FilterValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class Search
    {
        public Guid ProjectId { get; set; }

        public Guid FormTemplateId { get; set; }

        public string Term { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<FilterValue> FilterValues { get; set; }

        public Search()
        {
            this.FilterValues = new List<FilterValue>();
        }
    }

    public class SummarySearchDTO
    {
        public Guid ProjectId { get; set; }

        public List<Guid> FormTemplateIds { get; set; }

        public string Term { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<FilterValue> FilterValues { get; set; }

        public SummarySearchDTO()
        {
            this.FormTemplateIds = new List<Guid>();
            this.FilterValues = new List<FilterValue>();
        }
    }
}
