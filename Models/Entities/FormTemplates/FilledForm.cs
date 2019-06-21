using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public class FilledForm : Entity, IValidatableObject
    {
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

        public bool? IsRead { get; set; }

        public virtual ICollection<FilledFormLocation> Locations { set; get; }

        public virtual ICollection<FormValue> FormValues { set; get; }

        [StringLength(50)]
        public string SerialReferences { get; set; }

        [NotMapped]
        private string _description = string.Empty;

        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(this._description))
                    return this._description;

                if (this.FormTemplate != null)
                {
                    var descriptionMetrics = this.FormTemplate.GetDescriptionMetrics();
                    if (descriptionMetrics.Any())
                    {
                        var descFormat = this.FormTemplate.DescriptionFormat;

                        foreach (var metric in descriptionMetrics)
                        {
                            var formValue = this.FormValues.Where(fm => fm.MetricId == metric.Id).FirstOrDefault();
                            if (formValue != null)
                            {
                                var src = "{{" + metric.ShortTitle.ToLower() + "}}";
                                descFormat = descFormat.Replace(src, formValue.ToString());
                            }
                        }

                        this._description = descFormat;

                        return this._description;
                    }
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public DateTime Date
        {
            get
            {
                if (this.FormTemplate != null && this.FormTemplate.CalendarDateMetricId.HasValue)
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
                                    return formValue.DateValue.Value;
                            }
                        }
                    }
                }

                return this.SurveyDate;
            }
        }

        public FilledForm()
        {
            SurveyDate = DateTimeService.UtcNow;
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
