using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class OrganisationInvitationConfig : EntityTypeConfiguration<OrganisationInvitation>
    {
        public OrganisationInvitationConfig()
        {
            this.Property(x => x.Token)
                .HasMaxLength(10)
                .IsFixedLength()
                .IsRequired();

            this.HasRequired(x => x.Organisation)
                .WithMany(x => x.Invitations)
                .HasForeignKey(x => x.OrganisationId)
                .WillCascadeOnDelete();
        }
    }
}
