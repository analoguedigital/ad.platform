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
    public class DichotomousMetric : Metric
    {

        /// <summary>
        /// Just for UI usage
        /// </summary>
        [NotMapped]
        public bool? Value { set; get; }

        public DichotomousMetric()
        {
        }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            if (Mandatory && value.BoolValue == null)
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));
        }

        public override string GetStringValue(FormValue value)
        {
            if (value.BoolValue.HasValue)
                return value.BoolValue.Value ? "Yes" : "No";

            return string.Empty;
        }

        public override Metric Clone(FormTemplate template, MetricGroup metricGroup)
        {
            return BaseClone<DichotomousMetric>(template, metricGroup);
        }

        public override MetricFilter GetMetricFilter()
        {
            var filter = new CheckboxFilter
            {
                ShortTitle = this.ShortTitle,
                Type = MetricFilterTypes.Checkbox.ToString()
            };

            filter.DataList.Add(new MetricFilterOption { Text = "Yes", Value = 1 });
            filter.DataList.Add(new MetricFilterOption { Text = "No", Value = 0 });

            return filter;
        }

        public override Expression<Func<FilledForm, bool>> GetFilterExpression(FilterValue filter)
        {
            var multipleFilterValue = filter as MultipleFilterValue;
            var values = new List<bool?>();

            foreach (var item in multipleFilterValue.Values)
            {
                var value = Convert.ToInt32(item);
                values.Add(Convert.ToBoolean(value));
            }

            Expression<Func<FilledForm, bool>> result = f => f.FormValues.Any(v => v.MetricId == this.Id && values.Contains(v.BoolValue));

            return result;
        }

    }
}
