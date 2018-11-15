using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

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
