using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LightMethods.Survey.Models.DAL;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class Metric : Entity, IArchivable
    {

        public DateTime? DateArchived { set; get; }

        [Index]
        public Guid FormTemplateId { set; get; }
        public virtual FormTemplate FormTemplate { set; get; }

        [Display(Name = "Short title")]
        [MaxLength(50)]
        [Required]
        public string ShortTitle { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Index]
        public Guid MetricGroupId { set; get; }
        public virtual MetricGroup MetricGroup { set; get; }

        [UIHint("YesNo")]
        public bool Mandatory { set; get; }

        [StringLength(100)]
        [Display(Name = "Section title")]
        public string SectionTitle { set; get; }


        public int Order { set; get; }

        public Metric()
        {
            Order = 1000;
            Mandatory = false;
        }

        public bool MustBeArchived(SurveyContext context)
        {
            return context.FormValues.Any(v => v.MetricId == this.Id);
        }

        public void Render() { }

        public virtual IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void MoveUp()
        {
            var otherMetrics = MetricGroup.Metrics.Where(g => g.Order < Order).ToList();
            if (!otherMetrics.Any())
                return;

            var MaxSmaller = otherMetrics.Max(g => g.Order);
            if (MaxSmaller > 0)
            {

                var smaller = otherMetrics.Where(g => g.Order == MaxSmaller).First();
                smaller.Order = Order;
                Order = MaxSmaller;
            }
        }

        public void MoveDown()
        {
            var otherMetrics = MetricGroup.Metrics.Where(g => g.Order > Order).ToList();
            if (!otherMetrics.Any())
                return;

            var MinBigger = otherMetrics.Min(g => g.Order);
            if (MinBigger > 0)
            {

                var bigger = otherMetrics.Where(g => g.Order == MinBigger).First();
                bigger.Order = Order;
                Order = MinBigger;
            }
        }

        public virtual string GetStringValue(FormValue value)
        {
            return string.Empty;
        }

        protected T BaseClone<T>(FormTemplate cloneTemplate, MetricGroup cloneMetricGroup) where T : Metric, new()
        {
            var clone = new T()
            {
                FormTemplate = cloneMetricGroup.FormTemplate,
                Description = Description,
                Mandatory = Mandatory,
                Order = Order,
                SectionTitle = SectionTitle,
                ShortTitle = ShortTitle,
                DateArchived = DateArchived,
            };

            return clone;
        }

        public virtual Metric Clone(FormTemplate template, MetricGroup metricGroup)
        {
            throw new InvalidOperationException();
        }

        public virtual FilterMetadata GetFilterMetadata()
        {
            return new FilterMetadata();
        }
    }
}
