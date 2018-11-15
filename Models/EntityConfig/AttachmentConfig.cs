using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

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
