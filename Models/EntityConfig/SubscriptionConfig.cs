using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class SubscriptionConfig : EntityTypeConfiguration<Subscription>
    {
        public SubscriptionConfig()
        {
            this.HasRequired(x => x.OrgUser)
                .WithMany(x => x.Subscriptions)
                .HasForeignKey(x => x.OrgUserId)
                .WillCascadeOnDelete(false);

            this.HasOptional(x => x.PaymentRecord)
                .WithMany(x => x.Subscriptions)
                .HasForeignKey(x => x.PaymentRecordId)
                .WillCascadeOnDelete();

            this.HasOptional(x => x.SubscriptionPlan)
                .WithMany(x => x.Subscriptions)
                .HasForeignKey(x => x.SubscriptionPlanId)
                .WillCascadeOnDelete(false);
        }
    }
}
