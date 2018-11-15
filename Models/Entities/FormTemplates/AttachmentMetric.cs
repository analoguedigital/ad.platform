using LightMethods.Survey.Models.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public class AttachmentMetric : Metric
    {
        [Display(Name = "Allow multiple file")]
        public bool AllowMultipleFiles { set; get; }

        public virtual ICollection<AttachmentType> AllowedAttachmentTypes { set; get; }

        public AttachmentMetric()
        {
            AllowedAttachmentTypes = new List<AttachmentType>();
        }

        public override IEnumerable<ValidationResult> ValidateValue(FormValue value)
        {
            var attachments = new List<Attachment>();

            using (var content = new SurveyContext())
            {
                attachments = content.Attachments.AsNoTracking().Where(a => a.FormValueId == value.Id).ToList();
            }

            if (Mandatory && !attachments.Any())
                yield return new ValidationResult("{0} is required.".FormatWith(ShortTitle));

            //foreach(var attachment in attachments)
            //    if(attachment.FileSize > attachment.Type.MaxFileSize * 1024)
            //        yield return new ValidationResult("Maximum file size for {0} attachments should be less than {1} KB.".FormatWith(attachment.Type.Name, attachment.Type.MaxFileSize.ToString()));
        }

        public override string GetStringValue(FormValue value)
        {
            return value.TextValue;
        }

        public override Metric Clone(FormTemplate template, MetricGroup metricGroup)
        {
            var clone = BaseClone<AttachmentMetric>(template, metricGroup);
            clone.AllowMultipleFiles = AllowMultipleFiles;
            clone.AllowedAttachmentTypes = new List<AttachmentType>(AllowedAttachmentTypes);

            return clone;
        }

    }
}
