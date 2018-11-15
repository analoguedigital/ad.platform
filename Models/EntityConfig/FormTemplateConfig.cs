using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class FormTemplateConfig : EntityTypeConfiguration<FormTemplate>
    {
        public FormTemplateConfig()
        {
            this.HasMany<MetricGroup>(t => t.MetricGroups)
                .WithRequired(g => g.FormTemplate)
                .HasForeignKey(g => g.FormTemplateId)
                .WillCascadeOnDelete(false);

            this.HasRequired<OrgUser>(f => f.CreatedBy)
                .WithMany()
                .HasForeignKey(f => f.CreatedById)
                .WillCascadeOnDelete(false);
        }
    }
}
