using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class FormValueDTO
    {
        public Guid Id { set; get; }

        public Guid FilledFormId { set; get; }

        public Guid MetricId { set; get; }

        public int? RowNumber { set; get; }

        public Guid? RowDataListItemId { set; get; }

        public string TextValue { set; get; }

        public bool? BoolValue { set; get; }

        public double? NumericValue { set; get; }

        public DateTime? DateValue { set; get; }

        public DateTime? TimeValue { set; get; }

        public Guid? GuidValue { set; get; }

        public IEnumerable<AttachmentDTO> Attachments { set; get; }
       
    }
}
