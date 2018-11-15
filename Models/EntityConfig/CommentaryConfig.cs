using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class CommentaryConfig : EntityTypeConfiguration<Commentary>
    {
        public CommentaryConfig()
        {
            this.HasRequired<Project>(c => c.Project)
                .WithMany()
                .WillCascadeOnDelete();
        }
    }
}
