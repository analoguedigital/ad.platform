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
                var ticks = this.MaxValue - this.MinValue;
                if (ticks <= 5)
                {
                    var filter = new SliderFilter
                    {
                        MetricId = this.Id,
                        ShortTitle = this.ShortTitle,
                        Description = this.Description,
                        MinValue = this.MinValue,
                        MaxValue = this.MaxValue,
                        DefaultValue = this.DefaultValue,
                        Type = MetricFilterTypes.Slider.ToString()
                    };

                    return filter;
                }
                else
                {
                    var filter = new DropdownFilter
                    {
                        MetricId = this.Id,
                        ShortTitle = this.ShortTitle,
                        Description = this.Description,
                        Type = MetricFilterTypes.Dropdown.ToString()
                    };

                    for (var i = this.MinValue; i <= this.MaxValue; i++)
                        filter.DataList.Add(new MetricFilterDataItem { Text = i.ToString(), Value = i });

                    return filter;
                }
            }
            else
            {
                if (this.DataList.Items.Count() <= 5)
                {
                    var minVal = this.DataList.Items.Min(x => x.Value);
                    var maxVal = this.DataList.Items.Max(x => x.Value);

                    var filter = new SliderFilter
                    {
                        MetricId = this.Id,
                        ShortTitle = this.ShortTitle,
                        Description = this.Description,
                        MinValue = minVal,
                        MaxValue = maxVal,
                        DefaultValue = this.DefaultValue,
                        Type = MetricFilterTypes.Slider.ToString()
                    };

                    foreach (var item in this.DataList.Items)
                        filter.DataList.Add(new MetricFilterDataItem { Text = item.Text, Value = item.Value });

                    return filter;
                }
                else
                {
                    var filter = new DropdownFilter
                    {
                        MetricId = this.Id,
                        ShortTitle = this.ShortTitle,
                        Description = this.Description,
                        Type = MetricFilterTypes.Dropdown.ToString()
                    };

                    foreach (var item in this.DataList.Items)
                        filter.DataList.Add(new MetricFilterDataItem { Text = item.Text, Value = item.Value });

                    return filter;
                }
            }

        }
    }
}
