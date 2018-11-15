using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

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
