using LightMethods.Survey.Models.Enums;
using System;

namespace LightMethods.Survey.Models.Entities
{
    public class Subscription : Entity
    {
        public UserSubscriptionType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Note { get; set; }

        public Guid? PaymentRecordId { get; set; }

        public virtual PaymentRecord PaymentRecord { get; set; }

        public bool IsActive { get; set; }

        public Guid OrgUserId { get; set; }

        public virtual OrgUser OrgUser { get; set; }

        public Guid? OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public Guid? SubscriptionPlanId { get; set; }

        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    }
}
