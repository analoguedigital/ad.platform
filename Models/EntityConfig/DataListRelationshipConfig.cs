using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
