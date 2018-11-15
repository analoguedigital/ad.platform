using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class CommentaryDocumentConfig : EntityTypeConfiguration<CommentaryDocument>
    {
        public CommentaryDocumentConfig()
        {
            this.ToTable("CommentaryDocuments");

            this.HasRequired<Commentary>(d => d.Commentary)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CommentaryId)
                .WillCascadeOnDelete();
        }
    }
}
