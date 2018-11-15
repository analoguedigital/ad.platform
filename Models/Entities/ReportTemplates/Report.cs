using LightMethods.Survey.Models.DTO;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class Report: Entity
    {
        public Guid TemplateId { get; set; }

        public virtual ReportTemplate Template { set; get; }

        public virtual Project Project { get; set; }

        public Guid ProjectId { get; set; }

        [Display(Name="Start date")]
        public DateTime StartDate { set; get; }

        [Display(Name = "End date")]
        public DateTime EndDate { set; get; }

        [Display(Name = "Created by")]
        public User CreatedBy { set; get; }

        public Guid CreatedById { set; get; }

        [UIHint("RichText")]
        [DataType(DataType.MultilineText)]
        public string Introduction { get; set; }

        [UIHint("RichText")]
        [DataType(DataType.MultilineText)]
        public string Conclusion { get; set; }

        public Guid PdfFileId { set; get; }

        public virtual Document PdfFile { set; get; }

        [NotMapped]
        public DocumentDTO DocumentDTO { set; get; }
    }
}
