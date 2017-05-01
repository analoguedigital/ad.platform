using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class FormTemplateDTO
    {

        public Guid Id { set; get; }

        public FormTemplateCategoryDTO FormTemplateCategory { set; get; }

        public Guid? ProjectId { set; get; }

        public Guid CreatedById { set; get; }

        public string ProjectName { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public double Version { get; set; }

        public string Description { set; get; }

        public bool IsPublished { set; get; }

        public string Colour { set; get; }

        public Guid? CalendarDateMetricId { set; get; }

        public Guid? TimelineBarMetricId { set; get; }

        public ICollection<MetricGroupDTO> MetricGroups { set; get; }

    }
}