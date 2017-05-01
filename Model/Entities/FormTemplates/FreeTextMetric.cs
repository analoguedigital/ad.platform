using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AppHelper;

namespace LightMethods.Survey.Models.Entities
{
    public class FreeTextMetric : Metric
    {
        [Display(Name = "Number of lines")]
        public int NumberOfLine { set; get; }

        [Display(Name = "Maximum answer length")]
        public int MaxLength { set; get; }

        [Display(Name = "Minimum answer length")]
        public int MinLength { set; get; }

        public FreeTextMetric()
        {
            MaxLength = 1000;
            MinLength = 0;
            NumberOfLine = 3;
        }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            if (Mandatory && (value.TextValue == null || value.TextValue.IsEmpty()))
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));

            if (value.TextValue != null && value.TextValue.Length > this.MaxLength)
                yield return new ValidationResult("Length of {0} should be less than {1}.".FormatWith(ShortTitle, MaxLength.ToString()));

            if (value.TextValue != null && value.TextValue.Length < this.MinLength)
                yield return new ValidationResult("Length of {0} should be more than {1}.".FormatWith(ShortTitle, MinLength.ToString()));
        }

        public override string GetStringValue(FormValue value)
        {
            return value.TextValue;
        }

        public override Metric Clone(FormTemplate template, MetricGroup metricGroup)
        {
            var clone = BaseClone<FreeTextMetric>(template, metricGroup);
            clone.MaxLength = MaxLength;
            clone.NumberOfLine = NumberOfLine;
            clone.MinLength = MinLength;
            return clone;
        }
    }
}

