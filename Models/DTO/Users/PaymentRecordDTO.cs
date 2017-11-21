using System;

namespace LightMethods.Survey.Models.DTO
{
    public class PaymentRecordDTO
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string Reference { get; set; }

        public string Note { get; set; }

        public PromotionCodeDTO PromotionCode { get; set; }

        public Guid OrgUserId { get; set; }
    }
}
