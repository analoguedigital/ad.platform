using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class SubscriptionPlan : Entity
    {
        [Required]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Length { get; set; }

        public bool IsLimited { get; set; }

        public int? MonthlyQuota { get; set; }

        public bool PdfExport { get; set; }

        public bool ZipExport { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}
