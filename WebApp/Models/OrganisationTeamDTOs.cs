using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class OrgTeamAssignmentDTO
    {
        public List<OrgTeamUserDTO> Users { get; set; }
    }

    public class OrgTeamUserDTO
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