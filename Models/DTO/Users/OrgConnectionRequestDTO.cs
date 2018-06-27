using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class OrgConnectionRequestDTO
    {
        public Guid Id { get; set; }

        public OrgUserDTO OrgUser { get; set; }

        public OrganisationDTO Organisation { get; set; }

        public bool IsApproved { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}
