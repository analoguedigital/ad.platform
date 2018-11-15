using LightMethods.Survey.Models.Enums;
using System;

namespace LightMethods.Survey.Models.DTO
{
    public class UpdateSubscriptionEntryDTO
    {
        public Guid RecordId { get; set; }

        public UserSubscriptionType Type { get; set; }
    }
}
