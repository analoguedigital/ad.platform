using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class OrgTeamUserDTO
    {
        public string Id { get; set; }

        //public OrganisationTeamDTO OrganisationTeam { get; set; }
        public string OrganisationTeamId { get; set; }

        public OrgUserDTO OrgUser { get; set; }

        public bool IsManager { get; set; }
    }
}
