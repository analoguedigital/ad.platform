using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
