using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class FilledFormConfig : EntityTypeConfiguration<FilledForm>
    {
        public FilledFormConfig()
        {
            this.HasMany<FormValue>(f => f.FormValues)
                .WithRequired(v => v.FilledForm)
                .HasForeignKey(v => v.FilledFormId)
                .WillCascadeOnDelete(true);

            this.HasMany<FilledFormLocation>(f => f.Locations)
                .WithRequired(v => v.FilledForm)
                .HasForeignKey(v => v.FilledFormId)
                .WillCascadeOnDelete(true);
        }
    }
}
