using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class ReportTemplateCategoryConfig : EntityTypeConfiguration<ReportTemplateCategory>
    {
        public ReportTemplateCategoryConfig()
        {
            this.HasRequired<Organisation>(f => f.Organisation)
                .WithMany(o => o.ReportTemplateCategories)
                .HasForeignKey(f => f.OrganisationId)
                .WillCascadeOnDelete(false);
        }
    }
}
