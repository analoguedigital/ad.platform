using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MBS
{
    public class TargetDTO
    {
        public Guid SurveyId { set; get; }
        public DateTime? WeekTargetDate { set; get; }
        public string Description { set; get; }
        public string HowAchieved { set; get; }
        public string SupervisoryTargets { set; get; }
        public IList<AchievementDTO>Achievements {set;get;}
        public string HowOthersHelp { set; get; }

        public static TargetDTO From(FilledForm form)
        {
            var result = new TargetDTO();

            result.SurveyId = form.Id;
            result.WeekTargetDate = form.FormValues.Where(fv => fv.MetricId == Guid.Parse("4119B913-6937-4D82-9F83-F98385D642A3")).FirstOrDefault()?.DateValue;
            result.Description = form.FormValues.Where(fv => fv.MetricId == Guid.Parse("E4AA77F2-3F07-4AAE-90AD-B2D3183A87E7")).FirstOrDefault()?.ToString();
            result.HowAchieved = form.FormValues.Where(fv => fv.MetricId == Guid.Parse("FF24A578-08FD-4A5F-AEDE-1C6152848663")).FirstOrDefault()?.ToString();
            result.SupervisoryTargets = form.FormValues.Where(fv => fv.MetricId == Guid.Parse("B55853DB-723D-4F93-8054-1F78EB8CBFE2")).FirstOrDefault()?.ToString();
            result.HowOthersHelp = form.FormValues.Where(fv => fv.MetricId == Guid.Parse("E4180993-FBC6-40DB-A4D4-B7C91FFA6897")).FirstOrDefault()?.ToString();

            return result;
        }
    }
}
