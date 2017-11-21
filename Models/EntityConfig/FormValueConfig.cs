using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class FormValueConfig : EntityTypeConfiguration<FormValue>
    {
        public FormValueConfig()
        {
            this.HasRequired<Metric>(v => v.Metric)
                .WithMany()
                .HasForeignKey(v => v.MetricId)
                .WillCascadeOnDelete(false);
        }
    }
}
