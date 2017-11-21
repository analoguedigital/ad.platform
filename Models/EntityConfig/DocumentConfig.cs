using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
