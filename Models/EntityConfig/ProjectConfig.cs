using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class ProjectConfig : EntityTypeConfiguration<Project>
    {
        public ProjectConfig()
        {
            this.HasRequired<Organisation>(u => u.Organisation)
                .WithMany(o => o.Projects)
                .HasForeignKey(p => p.OrganisationId)
                .WillCascadeOnDelete(false);
        }
    }
}
