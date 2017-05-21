using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AppHelper;
using System.ComponentModel.DataAnnotations.Schema;

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

            if (this.DataListId == null)
            {
                // numeric range
                if (this.DefaultValue < this.MinValue || this.DefaultValue > this.MaxValue)
                    yield return new ValidationResult("Default value must fall within the min/max range.");
            }
            else
            {
                // ad-hoc list
                if(!this.DataList.Items.Select(i=>i.Value).Contains(DefaultValue))
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
    }
}
