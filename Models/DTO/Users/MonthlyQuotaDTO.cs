namespace LightMethods.Survey.Models.DTO
{
    public class MonthlyQuotaDTO
    {
        public int? Quota { get; set; }

        public int Used { get; set; }

        public int? MaxDiskSpace { get; set; }

        public int UsedDiskSpace { get; set; }
    }
}
