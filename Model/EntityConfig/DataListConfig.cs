using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
