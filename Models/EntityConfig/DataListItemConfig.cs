using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
