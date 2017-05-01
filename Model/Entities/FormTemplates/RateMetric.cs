using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AppHelper;

namespace LightMethods.Survey.Models.Entities
{
    public class RateMetric : Metric, IMeasureableMetric
    {
        [Required]
        [Display(Name = "Min value")]
        public int MinValue { get; set; }

        [Required]
        [Display(Name = "Max value")]
        public int MaxValue { get; set; }

        public RateMetric()
        {
            MinValue = 1;
        }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            if (Mandatory && value.NumericValue == null)
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));
        }

        public override string GetStringValue(FormValue value)
        {
            if (value.NumericValue.HasValue)
                return value.NumericValue.ToString();

            return string.Empty;
        }

        public override Metric Clone(FormTemplate template, MetricGroup metricGroup)
        {
            var clone = BaseClone<RateMetric>(template, metricGroup);
            clone.MaxValue = MaxValue;
            clone.MinValue = MinValue;
            return clone;
        }
    }
}
