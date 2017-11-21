using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class AttachmentConfig : EntityTypeConfiguration<Attachment>
    {
        public AttachmentConfig()
        {
            this.HasRequired<FormValue>(a => a.FormValue)
                .WithMany(a => a.Attachments)
                .WillCascadeOnDelete(false);
        }
    }
}
