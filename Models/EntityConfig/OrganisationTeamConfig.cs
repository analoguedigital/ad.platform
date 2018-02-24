using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
