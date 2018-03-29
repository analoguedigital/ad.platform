using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class ThreadAssignmentConfig : EntityTypeConfiguration<ThreadAssignment>
    {
        public ThreadAssignmentConfig()
        {
            this.HasRequired<OrgUser>(u => u.OrgUser)
                .WithMany(o => o.ThreadAssignments)
                .HasForeignKey(u => u.OrgUserId)
                .WillCascadeOnDelete();

            this.HasRequired<FormTemplate>(u => u.FormTemplate)
                .WithMany(o => o.Assignments)
                .HasForeignKey(u => u.FormTemplateId)
                .WillCascadeOnDelete();
        }
    }
}
