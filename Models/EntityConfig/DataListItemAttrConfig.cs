using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class DataListItemAttrConfig : EntityTypeConfiguration<DataListItemAttr>
    {
        public DataListItemAttrConfig()
        {
            this.HasRequired<DataListItem>(a => a.Owner)
                .WithMany(i => i.Attributes)
                .HasForeignKey(a => a.OwnerId)
                .WillCascadeOnDelete(true);

            this.HasRequired<DataListItem>(a => a.Value)
                .WithMany()
                .HasForeignKey(a => a.ValueId)
                .WillCascadeOnDelete(false);

            this.HasRequired<DataListRelationship>(a => a.Relationship)
                .WithMany()
                .HasForeignKey(a => a.RelationshipId)
                .WillCascadeOnDelete(true);
        }
    }
}
