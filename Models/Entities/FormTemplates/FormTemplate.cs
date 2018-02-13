using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using LightMethods.Survey.Models.MetricFilters;

namespace LightMethods.Survey.Models.Entities
{
    public class FormTemplate : Entity
    {
        private Regex _descriptionFormatRegex = new Regex(@"\{\{([^}]*)\}\}");
        private Regex _descriptionFormatValueRegex = new Regex("{{({?}?[^{}])*}}");

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

        [Display(Name = "Created by")]
        public OrgUser CreatedBy { set; get; }
        public Guid CreatedById { set; get; }

        [Required]
        [Display(Name = "Published?")]
        [UIHint("YesNo")]
        public bool IsPublished { set; get; }

        [NotMapped]
        public bool IsLastVersion { set; get; }

        public string Colour { set; get; }

        [Display(Name = "Desc. Format")]
        [MaxLength(150)]
        public string DescriptionFormat { get; set; }

        public Guid? CalendarDateMetricId { set; get; }
        public virtual Metric CalendarDateMetric { set; get; }

        public Guid? TimelineBarMetricId { set; get; }
        public virtual Metric TimelineBarMetric { set; get; }

        [Required]
        public Guid? FormTemplateCategoryId { set; get; }
        public virtual FormTemplateCategory FormTemplateCategory { set; get; }

        public virtual ICollection<MetricGroup> MetricGroups { get; set; }

        public virtual ICollection<ThreadAssignment> Assignments { get; set; }

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

        public IEnumerable<ValidationResult> Publish()
        {
            var result = this.ValidateDescriptionFormat();
            if (!result.Any())
                IsPublished = true;

            return result;
        }

        public FormTemplate Clone()
        {
            var clone = new FormTemplate
            {
                Code = Code,
                Colour = Colour,
                Description = Description,
                DescriptionFormat = DescriptionFormat,
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

        private IEnumerable<ValidationResult> ValidateDescriptionFormat()
        {
            var format = this.DescriptionFormat;
            if (!string.IsNullOrEmpty(format))
            {
                var matches = this._descriptionFormatRegex.Matches(format);
                var names = new List<string>();
                foreach (Match match in matches)
                    names.Add(match.Groups[1].Value);

                var foundMetrics = new List<Metric>();
                foreach (var metricGroup in this.MetricGroups)
                {
                    foreach (var metric in metricGroup.Metrics)
                    {
                        if (names.Contains(metric.ShortTitle.ToLower()))
                            foundMetrics.Add(metric);
                    }
                }

                if (names.Count != foundMetrics.Count)
                    yield return new ValidationResult("Description Format is not valid. Correct the format string and try again.");
            }
        }

        public List<MetricFilter> GetMetricFilters()
        {
            var filters = new List<MetricFilter>();

            foreach (var metricGroup in this.MetricGroups)
            {
                foreach (var metric in metricGroup.Metrics.Where(m =>!(m is AttachmentMetric) && !m.IsArchived()).OrderBy(m => m.Order))
                    filters.Add(metric.GetMetricFilter());
            }

            return filters;
        }

        public List<Metric> GetDescriptionMetrics()
        {
            if (string.IsNullOrEmpty(this.DescriptionFormat))
                return new List<Metric>();

            Regex formatPattern = new Regex(@"\{\{([^}]*)\}\}");
            var matches = formatPattern.Matches(this.DescriptionFormat);

            var names = new List<string>();
            foreach (Match match in matches)
                names.Add(match.Groups[1].Value);

            var foundMetrics = new List<Metric>();
            foreach (var metricGroup in this.MetricGroups)
            {
                foreach (var metric in metricGroup.Metrics)
                {
                    if (names.Contains(metric.ShortTitle.ToLower()))
                        foundMetrics.Add(metric);
                }
            }

            return foundMetrics;
        }

    }
}
