using LightMethods.Survey.Models.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class PaymentRecordConfig : EntityTypeConfiguration<PaymentRecord>
    {
        public PaymentRecordConfig()
        {
            this.Property(x => x.Reference).HasMaxLength(50);

            this.HasRequired(x => x.OrgUser)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.OrgUserId)
                .WillCascadeOnDelete();

            this.HasMany(x => x.Subscriptions)
                .WithOptional(x => x.PaymentRecord)
                .HasForeignKey(x => x.PaymentRecordId)
                .WillCascadeOnDelete();

            this.HasOptional(x => x.Voucher)
                .WithOptionalDependent(x => x.PaymentRecord)
                .WillCascadeOnDelete(false);
        }
    }
}
