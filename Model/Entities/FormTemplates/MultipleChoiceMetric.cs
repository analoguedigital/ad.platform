using LightMethods.Survey.Models.FilterValues;
using LightMethods.Survey.Models.MetricFilters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace LightMethods.Survey.Models.Entities
{
    public class MultipleChoiceMetric : Metric, IMeasureableMetric, IHasDataList, IHasAdHocDataList
    {

        public virtual DataList DataList { set; get; }
        public Guid DataListId { set; get; }

        [Required]
        [Display(Name = "Displayed as")]
        public MultipleChoiceViewType ViewType { set; get; }

        public MultipleChoiceMetric()
        {
        }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            if (Mandatory && ViewType != MultipleChoiceViewType.CheckBoxList && (value.GuidValue == null || value.GuidValue == Guid.Empty))
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));
        }

        public override string GetStringValue(FormValue value)
        {
            if (value.GuidValue.HasValue)
                return DataList.AllItems.Where(c => c.Id == value.GuidValue).Single().Text;

            return string.Empty;
        }

        public override Metric Clone(FormTemplate template, MetricGroup metricGroup)
        {
            var clone = BaseClone<MultipleChoiceMetric>(template, metricGroup);
            clone.DataList = DataList.IsAdHoc ? DataList.Clone() : (DataList)null; // Create a new datalist only if it is an ad-hoc
            clone.DataListId = DataList.IsAdHoc ? (Guid)Guid.Empty : DataListId;
            clone.ViewType = ViewType;
            return clone;
        }

        public override MetricFilter GetMetricFilter()
        {
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

        public override Expression<Func<FilledForm, bool>> GetFilterExpression(FilterValue filter)
        {
            var multipleFilterValue = filter as MultipleFilterValue;
            var filterValues = new List<int>();

            foreach (var item in multipleFilterValue.Values)
                filterValues.Add(Convert.ToInt32(item));

            var values = new List<Guid?>();
            foreach (var item in filterValues)
            {
                var dataListItem = this.DataList.Items.Where(x => x.Value == item).FirstOrDefault();
                values.Add(dataListItem.Id);
            }

            Expression<Func<FilledForm, bool>> result = f => f.FormValues.Any(v => v.MetricId == this.Id && values.Contains(v.GuidValue));

            return result;
        }
    }

}
