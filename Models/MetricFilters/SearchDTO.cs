using LightMethods.Survey.Models.FilterValues;
using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.MetricFilters
{
    public class SearchDTO
    {
        public Guid ProjectId { get; set; }

        public List<Guid> FormTemplateIds { get; set; }

        public string Term { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<FilterValue> FilterValues { get; set; }

        public SearchDTO()
        {
            this.FilterValues = new List<FilterValue>();
        }
    }
}
