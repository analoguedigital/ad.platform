using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.Entities
{
    public class PaymentRecord : Entity
    {
        public DateTime Date { get; set; }
        
        public decimal Amount { get; set; }

        public string Reference { get; set; }

        public string Note { get; set; }

        public virtual Voucher Voucher { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }

        public Guid OrgUserId { get; set; }

        public virtual OrgUser OrgUser { get; set; }
    }
}
