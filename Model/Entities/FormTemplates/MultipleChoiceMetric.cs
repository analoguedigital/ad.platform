using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AppHelper;
using System.ComponentModel.DataAnnotations.Schema;

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

        public override FilterMetadata GetFilterMetadata()
        {
            return new MultipleChoiceMetricMetadata
            {
                MetricId = this.Id,
                ShortTitle = this.ShortTitle,
                SectionTitle = this.SectionTitle,
                Description = this.Description,
                InputType = FilterInputType.MultipleChoice.ToString(),
                DataListId = this.DataListId,
                ViewType = this.ViewType
            };
        }
    }
}
