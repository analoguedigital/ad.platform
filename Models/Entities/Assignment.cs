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

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanView { get; set; }

        public bool CanExportPdf { get; set; }

        public bool CanExportZip { get; set; }
    }
}
