using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class DataListRelationshipConfig : EntityTypeConfiguration<DataListRelationship>
    {
        public DataListRelationshipConfig()
        {
            this.HasRequired<DataList>(d => d.Owner)
                .WithMany(d => d.NotOrderedRelationships)
                .HasForeignKey(r => r.OwnerId)
                .WillCascadeOnDelete(false);

            this.HasRequired<DataList>(d => d.DataList)
                .WithMany()
                .HasForeignKey(r => r.DataListId)
                .WillCascadeOnDelete(false);
        }
    }
}
