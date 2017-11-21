using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class AdultConfig : EntityTypeConfiguration<Adult>
    {
        public AdultConfig()
        {
            this.HasRequired<Project>(r => r.Project)
                .WithMany()
                .HasForeignKey(r => r.ProjectId)
                .WillCascadeOnDelete(false);
        }
    }
}
