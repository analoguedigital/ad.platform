using System;
using System.Collections.Generic;
using WebApi.Models.MetricFilters;

namespace WebApi.Models.FilledForms
{
    public class SearchDTO
    {
        public Guid ProjectId { get; set; }

        public Guid FormTemplateId { get; set; }

        public string Term { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<FilterValueDTO> FilterValues { get; set; }
    }
}