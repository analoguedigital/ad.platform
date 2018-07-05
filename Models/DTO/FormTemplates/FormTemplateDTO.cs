using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.DTO
{
    public class FormTemplateDTO
    {
        public Guid Id { set; get; }

        public FormTemplateCategoryDTO FormTemplateCategory { set; get; }

        public Guid? ProjectId { set; get; }

        public ProjectDTO Project { get; set; }

        public OrganisationDTO Organisation { get; set; }

        public Guid CreatedById { set; get; }

        public string ProjectName { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public double Version { get; set; }

        public string Description { set; get; }

        public bool IsPublished { set; get; }

        public string Colour { set; get; }

        public string DescriptionFormat { get; set; }

        public Guid? CalendarDateMetricId { set; get; }

        public Guid? TimelineBarMetricId { set; get; }

        public FormTemplateDiscriminators Discriminator { get; set; }

        public ICollection<MetricGroupDTO> MetricGroups { set; get; }

        public bool? CanView { get; set; }

        public bool? CanAdd { get; set; }

        public bool? CanEdit { get; set; }

        public bool? CanDelete { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as FormTemplateDTO;

            if (item == null)
                return false;

            return this.Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
