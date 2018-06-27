using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class OrgConnectionRequestConfig : EntityTypeConfiguration<OrgConnectionRequest>
    {
        public OrgConnectionRequestConfig()
        {
            this.HasRequired(x => x.Organisation)
                .WithMany(x => x.ConnectionRequests)
                .HasForeignKey(x => x.OrganisationId)
                .WillCascadeOnDelete();
        }
    }
}
