using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class AdultContactNumberConfig : EntityTypeConfiguration<AdultContactNumber>
    {
        public AdultContactNumberConfig()
        {
            this.HasRequired<Adult>(a => a.Adult)
                .WithMany(a => a.ContactNumbers)
                .HasForeignKey(a => a.AdultId)
                .WillCascadeOnDelete(true);
        }
    }
}
