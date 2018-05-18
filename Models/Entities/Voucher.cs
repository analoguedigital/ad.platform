using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class Voucher : Entity
    {
        public string Title { get; set; }

        [Index("IX_Voucher_Code", 1, IsUnique = true)]
        public string Code { get; set; }

        public int Period { get; set; }

        public bool IsRedeemed { get; set; }

        public Guid? PaymentRecordId { get; set; }

        public virtual PaymentRecord PaymentRecord { get; set; }

        public Guid OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
