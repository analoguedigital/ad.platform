using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class OrganisationTeam : Entity
    {
        [Required]
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Team Name")]
        public string Name { get; set; }

        [StringLength(150)]
        [Display(Name = "Team Description")]
        public string Description { get; set; }

        [StringLength(7)]
        public string Colour { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<OrgUser> Managers { get; set; }

        public virtual ICollection<OrgUser> Members { get; set; }
    }
}
