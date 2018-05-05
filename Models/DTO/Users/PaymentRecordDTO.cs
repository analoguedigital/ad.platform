using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.DTO
{
    public class PaymentRecordDTO
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string Reference { get; set; }

        public string Note { get; set; }

        public VoucherDTO Voucher { get; set; }

        public Guid OrgUserId { get; set; }

        // not necessary to include the payment record, i think.
        // since SubscriptionDTO includes a reference to a PaymentRecordDTO,
        // defining this list property causes a JSON formatter exception.
        //public List<SubscriptionDTO> Subscriptions { get; set; }

        //public PaymentRecordDTO()
        //{
        //    this.Subscriptions = new List<SubscriptionDTO>();
        //}
    }
}
