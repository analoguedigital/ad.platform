using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class PaymentRecordConfig : EntityTypeConfiguration<PaymentRecord>
    {
        public PaymentRecordConfig()
        {
            this.Property(x => x.Reference)
                .HasMaxLength(50);

            this.Property(x => x.Note);

            this.HasRequired(x => x.OrgUser)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.OrgUserId)
                .WillCascadeOnDelete();

            this.HasMany(x => x.Subscriptions)
                .WithRequired(x => x.PaymentRecord)
                .HasForeignKey(x => x.PaymentRecordId)
                .WillCascadeOnDelete();

            this.HasOptional(x => x.PromotionCode)
                .WithOptionalDependent(x => x.PaymentRecord)
                .WillCascadeOnDelete(false);
        }
    }
}
