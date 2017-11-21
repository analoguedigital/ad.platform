using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
