using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class OrganisationWorkerConfig : EntityTypeConfiguration<OrganisationWorker>
    {
        public OrganisationWorkerConfig()
        {
            this.ToTable("OrganisationWorkers");
            
            this.HasRequired<ExternalOrganisation>(w => w.Organisation)
                .WithMany(o => o.Workers)
                .HasForeignKey(w => w.OrganisationId)
                .WillCascadeOnDelete(true);
        }
    }
}
