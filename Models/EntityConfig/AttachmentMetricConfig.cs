using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class AttachmentMetricConfig : EntityTypeConfiguration<AttachmentMetric>
    {
        public AttachmentMetricConfig()
        {
            this.HasMany<AttachmentType>(m => m.AllowedAttachmentTypes)
                .WithMany()
                .Map(x =>
                {
                    x.MapLeftKey("AttachmentMetricId");
                    x.MapRightKey("AttachmentTypeId");
                    x.ToTable("AttachmentMetricAllowedTypes");
                });
        }
    }
}
