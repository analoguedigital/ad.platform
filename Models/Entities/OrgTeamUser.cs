using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class OrgTeamUser : Entity
    {
        [Required]
        public Guid OrganisationTeamId { get; set; }
        public virtual OrganisationTeam OrganisationTeam { get; set; }

        [Required]
        public Guid OrgUserId { get; set; }
        public virtual OrgUser OrgUser { get; set; }

        public bool IsManager { get; set; }
    }
}
