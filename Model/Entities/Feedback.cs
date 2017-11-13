using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class Feedback : Entity
    {
        public virtual OrgUser AddedBy { get; set; }
        public Guid AddedById { get; set; }

        public virtual Organisation Organisation { get; set; }
        public Guid OrganisationId { get; set; }

        public DateTime AddedAt { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.AddedById == Guid.Empty)
                yield return new ValidationResult("AddedById is empty.");

            if (this.OrganisationId == Guid.Empty)
                yield return new ValidationResult("OrganisationId is empty.");

            if (string.IsNullOrEmpty(this.Comment))
                yield return new ValidationResult("Comment is required.");
        }
    }
}
