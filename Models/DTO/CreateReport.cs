using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LightMethods.Survey.Models.Services;

namespace LightMethods.Survey.Models.DTO
{
    public class CreateReport
    {
        [Required]
        public Guid TemplateId { set; get; }

        [Required]
        [Display(Name="Start date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [UIHint("HtmlText")]
        [Display(Name = "Introduction")]
        [DataType(DataType.MultilineText)]
        public string Introduction { get; set; }

        [UIHint("HtmlText")]
        [Display(Name = "Conclusion")]
        [DataType(DataType.MultilineText)]
        public string Conclusion { get; set; }

        public CreateReport()
        {
            StartDate = DateTimeService.UtcNow.AddMonths(-1);
            EndDate = DateTimeService.UtcNow;
        }
    }
}
