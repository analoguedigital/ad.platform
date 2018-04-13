using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class SubscriptionPlanDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Length { get; set; }

        public bool IsLimited { get; set; }

        public int? MonthlyQuota { get; set; }

        public bool PdfExport { get; set; }

        public bool ZipExport { get; set; }
    }
}
