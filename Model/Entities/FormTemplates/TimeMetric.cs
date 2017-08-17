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
    public class TimeMetric : Metric
    {

        /// <summary>
        /// Just for UI usage
        /// </summary>
        [NotMapped]
        public TimeSpan? Value { set; get; }

        public TimeMetric()
        {
        }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            if (Mandatory && value.TimeValue == null)
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));
        }

        public override string GetStringValue(FormValue value)
        {
            if (value.TimeValue.HasValue)
                return value.TimeValue.Value.ToString(@"hh\:mm");

            return string.Empty;
        }

        public override Metric Clone(FormTemplate template, MetricGroup metricGroup)
        {
            return BaseClone<TimeMetric>(template, metricGroup);
        }

        public override MetricFilter GetMetricFilter()
        {
            return new TimeRangeFilter
            {
                ShortTitle = this.ShortTitle,
                Type = MetricFilterTypes.TimeRange.ToString()
            };
        }

        public override Expression<Func<FilledForm, bool>> GetFilterExpression(FilterValue filter)
        {
            var rangeFilterValue = filter as RangeFilterValue;
            var fromTime = rangeFilterValue.FromValue as DateTime?;
            var toTime = rangeFilterValue.ToValue as DateTime?;

            Expression<Func<FilledForm, bool>> result = null;

            if (fromTime.HasValue && !toTime.HasValue)
            {   // we have a start time
                var start = new TimeSpan(fromTime.Value.Hour, fromTime.Value.Minute, 0);
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && v.TimeValue >= start);
            }
            else if (!fromTime.HasValue && toTime.HasValue)
            {   // we have a end date
                var end = new TimeSpan(toTime.Value.Hour, toTime.Value.Minute, 0);
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && v.TimeValue <= end);
            }
            else if (fromTime.HasValue && toTime.HasValue)
            {   // we have a date range
                var start = new TimeSpan(fromTime.Value.Hour, fromTime.Value.Minute, 0);
                var end = new TimeSpan(toTime.Value.Hour, toTime.Value.Minute, 0);
                result = (FilledForm f) => f.FormValues.Any(v => v.MetricId == this.Id && v.TimeValue >= start && v.TimeValue <= end);
            }

            return result;
        }

    }
}
