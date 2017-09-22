using System;

namespace WebApi.Models.MBS
{
    public class AchievementSummaryDTO
    {
        public Guid ProjectId { set; get; }
        public Guid AchievementId { set; get; }
        public int NumberOfTargets { set; get; }
        public int NumberOfEvidenceItems { set; get; }
        public bool IsAchieved { set; get; }
        public string BackgroundColor { set; get; }
    }
}