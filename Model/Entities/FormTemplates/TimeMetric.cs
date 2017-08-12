using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AppHelper;
using System.ComponentModel.DataAnnotations.Schema;
using LightMethods.Survey.Models.MetricFilters;

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
                MetricId = this.Id,
                ShortTitle = this.ShortTitle,
                Description = this.Description,
                Type = MetricFilterTypes.TimeRange.ToString()
            };
        }

    }
}
