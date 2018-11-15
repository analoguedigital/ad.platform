using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class DataListItemConfig : EntityTypeConfiguration<DataListItem>
    {
        public DataListItemConfig()
        {
            this.HasRequired<DataList>(d => d.DataList)
                .WithMany(d => d.AllItems)
                .HasForeignKey(d => d.DataListId)
                .WillCascadeOnDelete(true);
        }
    }
}
