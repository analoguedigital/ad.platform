using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class FormTemplate : Entity
    {
        [Required]
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public Guid? ProjectId { set; get; }
        public virtual Project Project { set; get; }

        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        public double Version { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { set; get; }

        [Display(Name ="Created by")]
        public OrgUser CreatedBy { set; get; }
        public Guid CreatedById { set; get; }

        [Required]
        [Display(Name = "Published?")]
        [UIHint("YesNo")]
        public bool IsPublished { set; get; }

        [NotMapped]
        public bool IsLastVersion { set; get; }

        public string Colour { set; get; }

        public Guid? CalendarDateMetricId { set; get; }
        public virtual Metric CalendarDateMetric { set; get; }

        public Guid? TimelineBarMetricId { set; get; }
        public virtual Metric TimelineBarMetric { set; get; }

        [Required]
        public Guid? FormTemplateCategoryId { set; get; }
        public virtual FormTemplateCategory FormTemplateCategory { set; get; }

        public virtual ICollection<MetricGroup> MetricGroups { get; set; }

        public FormTemplate()
        {
            Version = 1.0;
            IsPublished = false;
            MetricGroups = new List<MetricGroup>();
        }

        public MetricGroup AddGroup(MetricGroup group)
        {
            group.Order = GetMaxGroupOrder() + 1;
            group.FormTemplate = this;
            MetricGroups.Add(group);
            return group;

        }

        public int GetMaxGroupOrder()
        {
            if (MetricGroups == null || !MetricGroups.Any())
                return 0;

            return MetricGroups.Max(g => g.Order);
        }

        public int GetMaxPageNumber()
        {
            if (MetricGroups == null || !MetricGroups.Any())
                return 0;

            return MetricGroups.Max(g => g.Page);
        }

        public void Publish()
        {
            IsPublished = true;
        }

        public FormTemplate Clone()
        {
            var clone = new FormTemplate
            {
                Code = Code,
                Colour = Colour,
                Description = Description,
                FormTemplateCategoryId = FormTemplateCategoryId,
                IsPublished = false,
                OrganisationId = OrganisationId,
                ProjectId = ProjectId,
                Title = Title,
                Version = Version
            };

            foreach (var metricGroup in MetricGroups)
                clone.MetricGroups.Add(metricGroup.Clone(clone));

            return clone;
        }
    }
}
