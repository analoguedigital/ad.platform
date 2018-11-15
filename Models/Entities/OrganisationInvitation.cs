using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class OrganisationInvitation : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        [Index("IX_OrgInvitation_Token", 1, IsUnique = true)]
        public string Token { get; set; }

        public int Limit { get; set; }

        public int Used { get; set; }

        public bool IsActive { get; set; }

        public Guid OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
