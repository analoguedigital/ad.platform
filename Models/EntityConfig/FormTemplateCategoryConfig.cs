using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class FormTemplateCategoryConfig : EntityTypeConfiguration<FormTemplateCategory>
    {
        public FormTemplateCategoryConfig()
        {
            this.HasRequired<Organisation>(f => f.Organisation)
                .WithMany(o => o.FormTemplateCategories)
                .HasForeignKey(f => f.OrganisationId)
                .WillCascadeOnDelete(false);
        }
    }
}
