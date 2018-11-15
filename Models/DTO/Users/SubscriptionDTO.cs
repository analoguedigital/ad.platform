using LightMethods.Survey.Models.Enums;
using System;

namespace LightMethods.Survey.Models.DTO
{
    public class SubscriptionDTO
    {
        public Guid Id { get; set; }

        public UserSubscriptionType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Note { get; set; }

        // not necessary to include the payment record, i think.
        public Guid? PaymentRecordId { get; set; }

        public PaymentRecordDTO PaymentRecord { get; set; }

        public bool IsActive { get; set; }

        public Guid OrgUserId { get; set; }

        public OrgUserDTO OrgUser { get; set; }

        public Guid? OrganisationId { get; set; }

        public OrganisationDTO Organisation { get; set; }

        public Guid? SubscriptionPlanId { get; set; }

        public SubscriptionPlanDTO SubscriptionPlan { get; set; }
    }

    public class LatestSubscriptionDTO
    {
        public DateTime? Date { get; set; }
    }
}
