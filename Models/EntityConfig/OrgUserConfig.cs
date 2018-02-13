using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class OrgUserConfig : EntityTypeConfiguration<OrgUser>
    {
        public OrgUserConfig()
        {
            this.HasRequired<OrgUserType>(u => u.Type)
                .WithMany()
                .WillCascadeOnDelete(false);

            this.HasRequired<Organisation>(u => u.Organisation)
                .WithMany(o => o.OrgUsers)
                .HasForeignKey(u => u.OrganisationId)
                .WillCascadeOnDelete();

            this.HasOptional<Project>(u => u.CurrentProject)
                .WithMany()
                .HasForeignKey(u => u.CurrentProjectId)
                .WillCascadeOnDelete(false);

            this.HasMany(u => u.Teams)
                .WithMany()
                .Map(x =>
                {
                    x.MapLeftKey("OrganisationTeamId");
                    x.MapRightKey("OrgUserId");
                    x.ToTable("OrganisationTeamUsers");
                });
        }
    }
}
