using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class RateMetricConfig : EntityTypeConfiguration<RateMetric>
    {
        public RateMetricConfig()
        {
            this.HasOptional<DataList>(m => m.DataList)
                .WithMany()
                .HasForeignKey(m => m.DataListId)
                .WillCascadeOnDelete(false);
        }
    }
}
