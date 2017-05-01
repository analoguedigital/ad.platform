using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LightMethods.Survey.Models.DAL;
using System.ComponentModel.DataAnnotations.Schema;

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

        public virtual ICollection<FilledFormLocation> Locations { set; get; }

        public virtual ICollection<FormValue> FormValues { set; get; }

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
