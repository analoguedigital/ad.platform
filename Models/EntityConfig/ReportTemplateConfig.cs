using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class ReportTemplateConfig : EntityTypeConfiguration<ReportTemplate>
    {
        public ReportTemplateConfig()
        {
            this.HasRequired<Organisation>(f => f.Organisation)
                .WithMany(o => o.ReportTemplates)
                .HasForeignKey(f => f.OrganisationId)
                .WillCascadeOnDelete(false);
        }
    }
}
