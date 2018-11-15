using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class OrganisationConfig : EntityTypeConfiguration<Organisation>
    {
        public OrganisationConfig()
        {
            this.HasMany(x => x.Vouchers)
                .WithRequired(x => x.Organisation)
                .HasForeignKey(x => x.OrganisationId)
                .WillCascadeOnDelete();
        }
    }
}
