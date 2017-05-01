using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.Entities
{
    public class Assignment : Entity
    {
        [Index("IX_User_Project", 2, IsUnique = true)]
        public Guid OrgUserId { get; set; }
        public virtual OrgUser OrgUser { get; set; }

        [Index("IX_User_Project", 1, IsUnique = true)]
        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}
