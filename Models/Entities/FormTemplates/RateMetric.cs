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
    public class RateMetric : Metric, IMeasureableMetric, IHasDataList, IHasAdHocDataList
    {
        [Required]
        [Display(Name = "Min value")]
        public int MinValue { get; set; }

        [Required]
        [Display(Name = "Max value")]
        public int MaxValue { get; set; }

        public int DefaultValue { get; set; }

        public virtual DataList DataList { set; get; }

        [Column("RateMetricDataListId")]
        public Guid? DataListId { set; get; }

        public RateMetric()
        {
            MinValue = 1;
        }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            if (Mandatory && value.NumericValue == null)
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate()
                .Concat(ValidateDataList())
                .Concat(ValidateDefaultValue());
        }

        public IEnumerable<ValidationResult> ValidateDataList()
        {
            if (this.DataListId != null)
            {
                var items = this.DataList.AllItems.Select(x => x.Value).ToList();
                if (!items.Any())
                    yield return new ValidationResult("Ad-hoc list is empty. Add list items first.");

                if (items.Count != items.Distinct().Count())
                    yield return new ValidationResult("Ad-hoc items cannot contain duplicate values.");
            }
        }

        public IEnumerable<ValidationResult> ValidateDefaultValue()
        {
            if (this.DataList != null && !this.DataList.Items.Any())
            {
                // numeric range
                if (this.DefaultValue < this.MinValue || this.DefaultValue > this.MaxValue)
                    yield return new ValidationResult("Default value must fall within the min/max range.");
            }
            else
            {
                // ad-hoc list
                if (this.DataList != null && !this.DataList.Items.Select(i => i.Value).Contains(DefaultValue))
                    yield return new ValidationResult("Default value must be set to one of possible values.");
            }
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
            clone.DefaultValue = DefaultValue;
            clone.DataList = DataList.IsAdHoc ? DataList.Clone() : null; // Create a new datalist only if it is an ad-hoc
            clone.DataListId = DataList.IsAdHoc ? null : DataListId;

            return clone;
        }

        public override MetricFilter GetMetricFilter()
        {
            if (this.DataList == null || !this.DataList.Items.Any())
            {
                // use the range filter for numeric sliders
                return new NumericRangeFilter
                {
                    ShortTitle = this.ShortTitle,
                    MinValue = this.MinValue,
                    MaxValue = this.MaxValue,
                    Type = MetricFilterTypes.NumericRange.ToString()
                };
            }
            else
            {
                // use the checkbox filter for data lists
                var filter = new CheckboxFilter
                {
                    ShortTitle = this.ShortTitle,
                    Type = MetricFilterTypes.Checkbox.ToString()
                };

                var items = new List<MetricFilterOption>();
                foreach (var item in this.DataList.AllItems)
                    items.Add(new MetricFilterOption { Text = item.Text, Value = item.Value });

                filter.DataList = items;

                return filter;
            }

        }

        public override Expression<Func<FilledForm, bool>> GetFilterExpression(FilterValue filter)
        {
            if (filter is RangeFilterValue)
            {
                var rangeFilterValue = filter as RangeFilterValue;

                int? fromValue = null;
                int? toValue = null;

                if (!string.IsNullOrEmpty(rangeFilterValue.FromValue))
                    fromValue = Convert.ToInt32(rangeFilterValue.FromValue);

                if (!string.IsNullOrEmpty(rangeFilterValue.ToValue))
                    toValue = Convert.ToInt32(rangeFilterValue.ToValue);

                Expression<Func<FilledForm, bool>> result = null;

                if (fromValue.HasValue && !toValue.HasValue)
                    result = f => f.FormValues.Any(v => v.MetricId == this.Id && v.NumericValue >= fromValue.Value);
                else if (!fromValue.HasValue && toValue.HasValue)
                    result = f => f.FormValues.Any(v => v.MetricId == this.Id && v.NumericValue <= toValue.Value);
                else if (fromValue.HasValue && toValue.HasValue)
                    result = f => f.FormValues.Any(v => v.MetricId == this.Id && v.NumericValue >= fromValue.Value && v.NumericValue <= toValue.Value);

                return result;
            }

            if (filter is MultipleFilterValue)
            {
                var multipleFilterValue = filter as MultipleFilterValue;
                var values = new List<double?>();
                foreach (var item in multipleFilterValue.Values)
                    values.Add(Convert.ToDouble(item));

                Expression<Func<FilledForm, bool>> result = f => f.FormValues.Any(v => v.MetricId == this.Id && values.Contains(v.NumericValue));

                return result;
            }

            throw new InvalidOperationException("FilterValue could not be cast! invalid value.");
        }
    }
}
