using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
