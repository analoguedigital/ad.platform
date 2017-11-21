using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
