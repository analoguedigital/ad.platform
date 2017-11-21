using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class AdultContactNumberConfig : EntityTypeConfiguration<AdultContactNumber>
    {
        public AdultContactNumberConfig()
        {
            this.HasRequired<Adult>(a => a.Adult)
                .WithMany(a => a.ContactNumbers)
                .HasForeignKey(a => a.AdultId)
                .WillCascadeOnDelete(true);
        }
    }
}
