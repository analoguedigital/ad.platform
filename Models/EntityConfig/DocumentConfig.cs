using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class DocumentConfig : EntityTypeConfiguration<Document>
    {
        public DocumentConfig()
        {
            this.HasRequired<File>(d => d.File)
                .WithMany()
                .HasForeignKey(d => d.FileId)
                .WillCascadeOnDelete();
        }
    }
}
