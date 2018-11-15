using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class OrganisationTeamConfig : EntityTypeConfiguration<OrganisationTeam>
    {
        public OrganisationTeamConfig()
        {
            this.HasRequired<Organisation>(u => u.Organisation)
                .WithMany(o => o.Teams)
                .HasForeignKey(p => p.OrganisationId)
                .WillCascadeOnDelete(false);
        }
    }
}
