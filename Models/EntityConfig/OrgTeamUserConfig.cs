using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class OrgTeamUserConfig : EntityTypeConfiguration<OrgTeamUser>
    {
        public OrgTeamUserConfig()
        {
            this.HasRequired<OrganisationTeam>(u => u.OrganisationTeam)
                .WithMany(o => o.Users)
                .HasForeignKey(p => p.OrganisationTeamId)
                .WillCascadeOnDelete(false);

            this.HasRequired<OrgUser>(u => u.OrgUser)
                .WithMany()
                .HasForeignKey(x => x.OrgUserId)
                .WillCascadeOnDelete(false);
        }
    }
}
