using LightMethods.Survey.Models.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.DTO
{
    public class InitReport
    {
        [Required]
        public Guid TemplateId { set; get; }

        [Required]
        [Display(Name="Start date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        public InitReport()
        {
            StartDate = DateTimeService.UtcNow.AddMonths(-1);
            EndDate = DateTimeService.UtcNow;
        }
    }
}
