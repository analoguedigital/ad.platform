using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class OrganisationConfig : EntityTypeConfiguration<Organisation>
    {
        public OrganisationConfig()
        {
            this.HasMany(x => x.PromotionCodes)
                .WithRequired(x => x.Organisation)
                .HasForeignKey(x => x.OrganisationId)
                .WillCascadeOnDelete();
        }
    }
}
