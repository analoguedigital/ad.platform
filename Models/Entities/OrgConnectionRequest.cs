using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class OrgConnectionRequest : Entity
    {
        [Required]
        public Guid OrgUserId { get; set; }

        public virtual OrgUser OrgUser { get; set; }

        [Required]
        public Guid OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public bool IsApproved { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}
