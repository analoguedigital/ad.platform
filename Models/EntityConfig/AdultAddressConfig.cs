using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class AdultAddressConfig : EntityTypeConfiguration<AdultAddress>
    {
        public AdultAddressConfig()
        {
            this.ToTable("AdultAddresses");

            this.HasRequired<Adult>(a => a.Adult)
                .WithMany(a => a.Addresses)
                .HasForeignKey(a => a.AdultId)
                .WillCascadeOnDelete(true);
        }
    }
}
