using System;

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

        public int? MonthlyDiskSpace { get; set; }

        public bool PdfExport { get; set; }

        public bool ZipExport { get; set; }
    }
}
