using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class PaymentRecord : Entity
    {
        public DateTime Date { get; set; }
        
        public decimal Amount { get; set; }

        public string Reference { get; set; }

        public string Note { get; set; }

        public virtual PromotionCode PromotionCode { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }

        public Guid OrgUserId { get; set; }

        public virtual OrgUser OrgUser { get; set; }
    }
}
