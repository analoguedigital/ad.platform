using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class OrganisationTeamDTO
    {
        public string Id { get; set; }

        public OrganisationDTO Organisation { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Colour { get; set; }

        public bool IsActive { get; set; }

        public ICollection<OrgTeamUserDTO> Users { get; set; }
    }
}
