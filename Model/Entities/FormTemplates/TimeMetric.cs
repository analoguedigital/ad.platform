using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AppHelper;
using System.ComponentModel.DataAnnotations.Schema;

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

        public override FilterMetadata GetFilterMetadata()
        {
            return new TimeMetricMetadata
            {
                MetricId = this.Id,
                ShortTitle = this.ShortTitle,
                SectionTitle = this.SectionTitle,
                Description = this.Description,
                InputType = FilterInputType.Time.ToString()
            };
        }
    }
}
