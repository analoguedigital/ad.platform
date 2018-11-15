using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class AdultConfig : EntityTypeConfiguration<Adult>
    {
        public AdultConfig()
        {
            this.HasRequired<Project>(r => r.Project)
                .WithMany()
                .HasForeignKey(r => r.ProjectId)
                .WillCascadeOnDelete(false);
        }
    }
}
