using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class ThreadAssignment : Entity
    {
        [Index("IX_User_Thread", 2, IsUnique = true)]
        public Guid OrgUserId { get; set; }
        public virtual OrgUser OrgUser { get; set; }

        [Index("IX_User_Thread", 1, IsUnique = true)]
        public Guid FormTemplateId { get; set; }
        public virtual FormTemplate FormTemplate { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanView { get; set; }
    }
}
