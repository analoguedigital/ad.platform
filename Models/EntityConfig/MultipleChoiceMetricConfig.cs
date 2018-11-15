using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class MultipleChoiceMetricConfig : EntityTypeConfiguration<MultipleChoiceMetric>
    {
        public MultipleChoiceMetricConfig()
        {
            this.HasRequired<DataList>(m => m.DataList)
                .WithMany()
                .HasForeignKey(m => m.DataListId)
                .WillCascadeOnDelete(false);
        }
    }
}
