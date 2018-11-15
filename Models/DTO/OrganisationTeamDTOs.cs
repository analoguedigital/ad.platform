using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class OrgTeamAssignmentDTO
    {
        public List<OrgTeamUserFlat> Users { get; set; }
    }

    public class OrgTeamUserFlat
    {
        public Guid OrgUserId { get; set; }

        public bool IsManager { get; set; }
    }

    public class OrgTeamAssignmentsDTO
    {
        public List<Guid> Projects { get; set; }

        public List<Guid> OrgUsers { get; set; }
    }
}
