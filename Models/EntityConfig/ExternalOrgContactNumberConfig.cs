using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

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
