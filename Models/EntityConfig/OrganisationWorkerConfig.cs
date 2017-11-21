using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
