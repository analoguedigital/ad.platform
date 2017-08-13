using LightMethods.Survey.Models.MetricFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Models.MetricFilters;

namespace WebApi.Models.FilledForms
{
    public class SearchDTO
    {
        public Guid ProjectId { get; set; }

        public string Term { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<MetricFilterDTO> Filters { get; set; }
    }
}