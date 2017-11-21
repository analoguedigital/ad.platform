using LightMethods.Survey.Models.FilterValues;
using LightMethods.Survey.Models.MetricFilters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

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

        public override MetricFilter GetMetricFilter()
        {
            return new NumericRangeFilter
            {
                ShortTitle = this.ShortTitle,
                MinValue = this.MinVal,
                MaxValue = this.MaxVal,
                Type = MetricFilterTypes.NumericRange.ToString()
            };
        }

        public override Expression<Func<FilledForm, bool>> GetFilterExpression(FilterValue filter)
        {
            var rangeValue = filter as RangeFilterValue;

            int? fromValue = null;
            int? toValue = null;

            if (!string.IsNullOrEmpty(rangeValue.FromValue))
                fromValue = Convert.ToInt32(rangeValue.FromValue);

            if (!string.IsNullOrEmpty(rangeValue.ToValue))
                toValue = Convert.ToInt32(rangeValue.ToValue);

            Expression<Func<FilledForm, bool>> result = null;

            if (fromValue.HasValue && !toValue.HasValue)
                // we have a start value
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && v.NumericValue >= fromValue.Value);
            else if (!fromValue.HasValue && toValue.HasValue)
                // we have a end value
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && v.NumericValue <= toValue);
            else if (fromValue.HasValue && toValue.HasValue)
                // we have a numeric range
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && v.NumericValue >= fromValue && v.NumericValue <= toValue);

            return result;
        }
    }
}
