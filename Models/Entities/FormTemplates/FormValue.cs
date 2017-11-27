using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class FormValue : Entity, IValidatableObject
    {
        [Index]
        public Guid? FilledFormId { set; get; }
        public virtual FilledForm FilledForm { set; get; }

        public Guid? MetricId { set; get; }
        public virtual Metric Metric { set; get; }

        public int? RowNumber { set; get; }

        public virtual DataListItem RowDataListItem { set; get; }
        public Guid? RowDataListItemId { set; get; }

        public string TextValue { set; get; }

        [UIHint("YesNo")]
        public bool? BoolValue { set; get; }
        public double? NumericValue { set; get; }
        public DateTime? DateValue { set; get; }
        public Guid? GuidValue { set; get; }
        public TimeSpan? TimeValue { set; get; }
        public virtual ICollection<Attachment> Attachments { set; get; }

        public FormValue()
        {
            Attachments = new List<Attachment>();
        }

        public static bool operator ==(FormValue value, Object obj)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(value, obj))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)value == null) || (obj == null))
            {
                return false;
            }

            if (obj is FormValue) return value.Equals((FormValue)obj);

            if (value.Metric is FreeTextMetric)
                return value.TextValue == obj.ToString();

            if (value.Metric is NumericMetric)
            {
                if (obj is double)
                    return value.NumericValue == (double)obj;

                double objVal = 0;
                if (double.TryParse(obj.ToString(), out objVal))
                {
                    return value.NumericValue == objVal;
                }

                return false;
            }

            if (value.Metric is DateMetric)
            {
                if (obj is DateTime)
                    return value.DateValue == (DateTime)obj;

                DateTime objVal = DateTime.MinValue;
                if (DateTime.TryParse(obj.ToString(), out objVal))
                {
                    return value.DateValue?.Date == objVal.Date;
                }

                return false;
            }

            if (value.Metric is DichotomousMetric)
            {
                if (obj is bool)
                    return value.BoolValue == (bool)obj;

                bool objVal = false;
                if (bool.TryParse(obj.ToString(), out objVal))
                {
                    return value.BoolValue == objVal;
                }

                return false;

            }

            if (value.Metric is RateMetric)
            {
                double objVal = 0;
                if (double.TryParse(obj.ToString(), out objVal))
                {
                    return value.NumericValue == objVal;
                }

                return false;
            }

            return false;
        }

        public static bool operator !=(FormValue value, Object obj)
        {
            return !(value == obj);
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Metric != null)
                return Metric.ValidateValue(this);

            return new List<ValidationResult>();
        }

        public override string ToString()
        {
            return Metric.GetStringValue(this);
        }
    }
}
