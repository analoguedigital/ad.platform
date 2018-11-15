using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class ExternalOrganisation :Entity
    {
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Display(Name="Email address")]
        public string Email { get; set; }

        [Display(Name = "Start date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End date")]
        public DateTime? EndDate { get; set; }

        public Guid AddressId { get; set; }

        public Address Address { get; set; }

        public virtual ICollection<ExternalOrgContactNumber> ContactNumbers { get; set; }

        public virtual ICollection<OrganisationWorker> Workers { get; set; }
    }
}
