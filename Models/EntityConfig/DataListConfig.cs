using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class DataListConfig : EntityTypeConfiguration<DataList>
    {
        public DataListConfig()
        {
            this.HasRequired<Organisation>(d => d.Organisation)
                .WithMany(o => o.DataLists)
                .HasForeignKey(d => d.OrganisationId)
                .WillCascadeOnDelete(true);
        }
    }
}
