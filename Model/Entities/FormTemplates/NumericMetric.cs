using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AppHelper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class NumericMetric : Metric, IMeasureableMetric
    {
        [Display(Name = "Min value")]
        public int? MinVal { get; set; }

        [Display(Name = "Max value")]
        public int? MaxVal { get; set; }

        /// <summary>
        /// Just for UI usage
        /// </summary>
        [NotMapped]
        public double? Value { set; get; }

        public NumericMetric()
        {

        }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            if (Mandatory && value.NumericValue == null)
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));

            if (MinVal.HasValue && value.NumericValue.HasValue && MinVal.Value > value.NumericValue.Value)
                yield return new ValidationResult("{0} should be bigger than {1}.".FormatWith(ShortTitle, MinVal.Value.ToString()));

            if (MaxVal.HasValue && value.NumericValue.HasValue && MaxVal.Value < value.NumericValue.Value)
                yield return new ValidationResult("{0} should be bigger than {1}.".FormatWith(ShortTitle, MaxVal.Value.ToString()));
        }

        public override string GetStringValue(FormValue value)
        {
            if (value.NumericValue.HasValue)
                return value.NumericValue.ToString();

            return string.Empty;
        }

        public override Metric Clone(FormTemplate template, MetricGroup metricGroup) 
        {
            var clone = BaseClone<NumericMetric>(template, metricGroup);
            clone.MaxVal = MaxVal;
            return clone;
        }
    }
}
