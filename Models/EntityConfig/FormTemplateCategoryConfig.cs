using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
