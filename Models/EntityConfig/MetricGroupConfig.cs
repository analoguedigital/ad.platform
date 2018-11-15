using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class MetricGroupConfig : EntityTypeConfiguration<MetricGroup>
    {
        public MetricGroupConfig()
        {
            this.HasMany<Metric>(t => t.Metrics)
                .WithRequired(g => g.MetricGroup)
                .HasForeignKey(g => g.MetricGroupId)
                .WillCascadeOnDelete(false);
        }
    }
}
