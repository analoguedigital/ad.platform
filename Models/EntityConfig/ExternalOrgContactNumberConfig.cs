using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class ExternalOrgContactNumberConfig : EntityTypeConfiguration<ExternalOrgContactNumber>
    {
        public ExternalOrgContactNumberConfig()
        {
            this.HasRequired<ExternalOrganisation>(a => a.Organisation)
                .WithMany(a => a.ContactNumbers)
                .HasForeignKey(a => a.OrganisationId)
                .WillCascadeOnDelete(true);
        }
    }
}
