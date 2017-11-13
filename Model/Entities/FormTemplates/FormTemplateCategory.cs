using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class FormTemplateCategory : Entity
    {
        public Guid OrganisationId { set; get; }
        public Organisation Organisation { set; get; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public virtual ICollection<FormTemplate> FormTemplates { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
