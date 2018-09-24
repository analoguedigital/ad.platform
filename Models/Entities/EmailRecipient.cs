using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class EmailRecipient : Entity
    {
        [Required]
        [Index("IX_User_Recipient", 1, IsUnique = true)]
        public Guid OrgUserId { get; set; }
        public virtual OrgUser OrgUser { get; set; }

        public bool Feedbacks { get; set; }

        public bool NewMobileUsers { get; set; }

        public bool OrgRequests { get; set; }

        public bool OrgConnectionRequests { get; set; }
    }
}
