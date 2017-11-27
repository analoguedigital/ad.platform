using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class AssignmentConfig : EntityTypeConfiguration<Assignment>
    {
        public AssignmentConfig()
        {
            this.HasRequired<OrgUser>(u => u.OrgUser)
                .WithMany(o => o.Assignments)
                .HasForeignKey(u => u.OrgUserId)
                .WillCascadeOnDelete();

            this.HasRequired<Project>(u => u.Project)
                .WithMany(o => o.Assignments)
                .HasForeignKey(u => u.ProjectId)
                .WillCascadeOnDelete();
        }
    }
}
