using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class EmailRecipientDTO
    {
        public Guid OrgUserId { get; set; }

        public string OrgUserName { get; set; }

        public string Email { get; set; }

        public bool IsRootUser { get; set; }

        public bool Feedbacks { get; set; }

        public bool NewMobileUsers { get; set; }

        public bool OrgRequests { get; set; }

        public bool OrgConnectionRequests { get; set; }
    }
}
