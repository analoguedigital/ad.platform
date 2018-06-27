using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class OrgRequest : Entity
    {
        [Required]
        [StringLength(30)]
        [Display(Name = "Organisation Name")]
        public string Name { get; set; }

        [StringLength(150)]
        public string Address { get; set; }

        [StringLength(30)]
        public string ContactName { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(30)]
        [Display(Name = "Telephone Number")]
        public string TelNumber { get; set; }

        [StringLength(8)]
        public string Postcode { get; set; }

        [Required]
        public Guid OrgUserId { get; set; }

        public virtual OrgUser OrgUser { get; set; }
    }
}
