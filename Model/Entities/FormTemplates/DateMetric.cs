﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AppHelper;
using System.ComponentModel.DataAnnotations.Schema;

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

        public DateMetric()
        {
        }

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
            return BaseClone<DateMetric>(template, metricGroup);
        }

        public override FilterMetadata GetFilterMetadata()
        {
            return new DateMetricMetadata
            {
                MetricId = this.Id,
                ShortTitle = this.ShortTitle,
                SectionTitle = this.SectionTitle,
                Description = this.Description,
                InputType = this.HasTimeValue ? FilterInputType.DateTime.ToString() : FilterInputType.Date.ToString()
            };
        }
    }
}
