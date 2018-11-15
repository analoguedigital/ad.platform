using LightMethods.Survey.Models.Enums;
using System;

namespace LightMethods.Survey.Models.DTO
{
    public class SubscriptionEntryDTO
    {
        public Guid? Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? Price { get; set; }

        public string Note { get; set; }

        public string Reference { get; set; }

        public Guid? PaymentRecordId { get; set; }

        public UserSubscriptionType Type { get; set; }

        public bool IsActive { get; set; }

        public SubscriptionPlanDTO SubscriptionPlan { get; set; }

        public OrganisationDTO Organisation { get; set; }
    }
}
