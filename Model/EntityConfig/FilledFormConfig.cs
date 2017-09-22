using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
