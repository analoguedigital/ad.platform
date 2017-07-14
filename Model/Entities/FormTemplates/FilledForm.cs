using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LightMethods.Survey.Models.DAL;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace LightMethods.Survey.Models.Entities
{
    public class FilledForm : Entity, IValidatableObject
    {
        private Regex descriptionFormatPattern = new Regex(@"\{\{([^}]*)\}\}");

        [Required]
        [Index]
        public Guid FormTemplateId { get; set; }
        public virtual FormTemplate FormTemplate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Serial { set; get; }

        [Required]
        [Index]
        [Display(Name = "Survey Date")]
        public DateTime SurveyDate { set; get; }

        [Display(Name = "Filled by")]
        public virtual User FilledBy { set; get; }
        public Guid FilledById { set; get; }

        [Required]
        public Guid ProjectId { set; get; }
        public virtual Project Project { set; get; }

        public virtual ICollection<FilledFormLocation> Locations { set; get; }

        public virtual ICollection<FormValue> FormValues { set; get; }

        [NotMapped]
        private string _description = string.Empty;
        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(this._description))
                    return this._description;

                var format = this.FormTemplate.DescriptionFormat;
                if (string.IsNullOrEmpty(format))
                    return string.Empty;

                var matches = this.descriptionFormatPattern.Matches(format);
                var names = new List<string>();
                foreach (Match match in matches)
                    names.Add(match.Groups[1].Value);

                var foundMetrics = new List<Metric>();
                foreach (var metricGroup in this.FormTemplate.MetricGroups)
                {
                    foreach (var metric in metricGroup.Metrics)
                    {
                        if (names.Contains(metric.ShortTitle.ToLower()))
                            foundMetrics.Add(metric);
                    }
                }

                if (foundMetrics.Any())
                {
                    var values = new List<string>();
                    foreach (var metric in foundMetrics)
                    {
                        var formValue = this.FormValues.Where(fm => fm.MetricId == metric.Id).FirstOrDefault();
                        if (formValue != null)
                            values.Add(formValue.ToString());
                    }

                    this._description = string.Join(" - ", values);
                    return this._description;
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public bool DateHasTimeValue { get; set; }

        [NotMapped]
        public DateTime Date
        {
            get
            {
                if (this.FormTemplate.CalendarDateMetricId.HasValue)
                {
                    foreach (var metricGroup in this.FormTemplate.MetricGroups)
                    {
                        var metric = metricGroup.Metrics.Where(m => m.Id == this.FormTemplate.CalendarDateMetricId).FirstOrDefault();
                        if (metric != null)
                        {
                            var formValue = this.FormValues.Where(fv => fv.MetricId == metric.Id).FirstOrDefault();
                            if (formValue != null)
                            {
                                if (formValue.DateValue.HasValue)
                                {
                                    var dateMetric = metric as DateMetric;
                                    this.DateHasTimeValue = dateMetric.HasTimeValue.HasValue && dateMetric.HasTimeValue.Value;

                                    return formValue.DateValue.Value;
                                }
                            }
                        }
                    }
                }

                return this.SurveyDate;
            }
        }

        public FilledForm()
        {
            SurveyDate = DateTime.Now;
            FormValues = new List<FormValue>();
        }

        public bool HasValueFor(Metric metric)
        {
            return FormValues.Select(v => v.Metric).Contains(metric);
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (FormValues == null || !FormValues.Any())
                return new List<ValidationResult>();

            if (FormTemplate == null)
            {
                var context = new LightMethods.Survey.Models.DAL.SurveyContext();
                using (var uow = new UnitOfWork(context))
                {
                    FormTemplate = uow.FormTemplatesRepository.GetNotTrackedFullTemplate(FormTemplateId);
                }
            }

            foreach (var value in FormValues)
                if (value.MetricId != Guid.Empty && value.Metric == null)
                {
                    value.Metric = FormTemplate.MetricGroups.SelectMany(g => g.Metrics).Where(m => m.Id == value.MetricId).Single();
                }

            return FormValues.SelectMany(v => v.Validate(validationContext));

        }
    }
}
