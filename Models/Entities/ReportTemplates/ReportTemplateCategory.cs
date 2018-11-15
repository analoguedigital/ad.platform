using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class ReportTemplateCategory : Entity
    {
        public Guid OrganisationId { set; get; }

        public Organisation Organisation { set; get; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public virtual ICollection<ReportTemplate> ReportTemplates { get; set; }

        public override string ToString()
        {
            return Title;
        }

    }
}
