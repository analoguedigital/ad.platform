using LightMethods.Survey.Models.FilterValues;
using LightMethods.Survey.Models.MetricFilters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace LightMethods.Survey.Models.Entities
{
    public class DateMetric : Metric
    {
        /// <summary>
        /// Just for UI usage
        /// </summary>
        [NotMapped]
        public DateTime? Value { set; get; }

        public bool HasTimeValue { get; set; }

        public DateMetric() { }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            if (Mandatory && value.DateValue == null)
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));
        }

        public override string GetStringValue(FormValue value)
        {
            if (value.DateValue.HasValue)
            {
                if (value.DateValue.Value.TimeOfDay == TimeSpan.Zero)
                    return value.DateValue.Value.ToString("dd/MM/yyyy");
                else
                    return value.DateValue.Value.ToString("dd/MM/yyyy hh:mm tt");
            }

            return string.Empty;
        }

        public override Metric Clone(FormTemplate template, MetricGroup metricGroup)
        {
            var clone = BaseClone<DateMetric>(template, metricGroup);
            clone.HasTimeValue = HasTimeValue;
            return clone;
        }

        public override MetricFilter GetMetricFilter()
        {
            return new DateRangeFilter
            {
                ShortTitle = this.ShortTitle,
                CanSelectTime = this.HasTimeValue,
                Type = MetricFilterTypes.DateRange.ToString()
            };
        }

        public override Expression<Func<FilledForm, bool>> GetFilterExpression(FilterValue filter)
        {
            var rangeValue = filter as RangeFilterValue;

            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (!string.IsNullOrEmpty(rangeValue.FromValue))
                fromDate = Convert.ToDateTime(rangeValue.FromValue);

            if (!string.IsNullOrEmpty(rangeValue.ToValue))
                toDate = Convert.ToDateTime(rangeValue.ToValue);

            Expression<Func<FilledForm, bool>> result = null;

            if (fromDate.HasValue && !toDate.HasValue)
                // we have a start date
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && DbFunctions.TruncateTime(v.DateValue) >= DbFunctions.TruncateTime(fromDate.Value));
            else if (!fromDate.HasValue && toDate.HasValue)
                // we have a end date
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && DbFunctions.TruncateTime(v.DateValue) <= DbFunctions.TruncateTime(toDate));
            else if (fromDate.HasValue && toDate.HasValue)
                // we have a date range
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && DbFunctions.TruncateTime(v.DateValue) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.DateValue) <= DbFunctions.TruncateTime(toDate));

            return result;
        }
    }
}
