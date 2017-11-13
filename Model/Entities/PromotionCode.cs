using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class PromotionCode : Entity
    {
        public string Title { get; set; }

        [Index("IX_PromotionCode_Code", 1, IsUnique = true)]
        public string Code { get; set; }

        public decimal Amount { get; set; }

        public bool IsRedeemed { get; set; }

        public Guid? PaymentRecordId { get; set; }

        public virtual PaymentRecord PaymentRecord { get; set; }

        public Guid OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
