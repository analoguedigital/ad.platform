using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Models.MBS;

namespace WebApi.Controllers.MBS
{
    public class MBSDataController : BaseApiController
    {
        private Guid TermsDataListId = Guid.Parse("43846f75-8e07-4924-8f41-8d392551625e");
        private Guid AchievementDataListId = Guid.Parse("d8a8a1f3-4764-457b-ad6c-5e4753d2eb20");
        private Guid EvidenceAchievementMetricId = Guid.Parse("319b1695-c408-4d4a-a4fe-9ff1575ad3cc");
        private Guid TargetIdMetricId = Guid.Parse("6099A26A-C228-4884-B798-7EFAB1D82CF7");
        private Guid TargetAchievementIdMetricId = Guid.Parse("35C4CA39-395C-4E52-9C39-DCF979CADC7B");
        private Guid TargetFormTemplateId = Guid.Parse("800f7da6-94f1-44ef-83c3-d9a0d120a987");
        private Guid TargetFormWeekDateMetricId = Guid.Parse("4119b913-6937-4d82-9f83-f98385d642a3");
        private Guid TargetIsAchievedMetricId = Guid.Parse("15789598-167b-4fc3-a56b-83d98a03ded3");
        private Guid AchievementLogFormTemplateId = Guid.Parse("93c6b21e-926a-4b43-8990-fb9179806105");
        private Guid AchievementLogAchiementMetricId = Guid.Parse("28efd328-6fb6-421c-9d6f-513a10e44768");
        private Guid AchievementLogTermMetricId = Guid.Parse("565554fc-4e40-419b-9183-5ebd5ce247ec");
        private Guid IsAchievedDataListItemId = Guid.Parse("084866CB-FC5E-4ED0-97B0-16737A38EA17");

        [HttpGet]
        [Route("api/mbs/Updates")]
        [ResponseType(typeof(IEnumerable<UpdateDTO>))]
        public IHttpActionResult GetUpdates()
        {
            if (CurrentOrgUser != null)
            {
                var aWeekAgo = DateTime.Today.AddDays(-7);
                var assignedProjects = CurrentOrgUser.Assignments.Select(a => a.ProjectId);

                var updates = UnitOfWork.FilledFormsRepository
                   .AllIncludingNoTracking(f => f.Project, f => f.FormTemplate)
                   .Where(s => s.Project.OrganisationId == CurrentOrganisationId)
                   .Where(s => CurrentOrgUser.TypeId == OrgUserTypesRepository.Administrator.Id || assignedProjects.Contains(s.ProjectId))
                   .Where(s => s.DateCreated > aWeekAgo || s.DateUpdated > aWeekAgo)
                   .OrderByDescending(s => s.DateUpdated)
                   .Take(7)
                   .ToList()
                   .Select(f => GetUpdate(f));

                return Ok(updates);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/mbs/students/{projectId}/targets")]
        [ResponseType(typeof(IEnumerable<TargetDTO>))]
        public IHttpActionResult GetTargets(Guid projectId)
        {
            var today = DateTime.Today;

            var targetsform = UnitOfWork.FilledFormsRepository
                .AllIncludingNoTracking(f => f.FormValues)
                .Where(t => t.ProjectId == projectId && t.FormTemplateId == TargetFormTemplateId)
                .ToList();
            var targets = targetsform.Where(t => t.FormValues.FirstOrDefault(v => v.MetricId == TargetFormWeekDateMetricId).DateValue.Value.Date <= today.Date)
                    .Where(f => f.FormValues.FirstOrDefault(v => v.MetricId == TargetIsAchievedMetricId).GuidValue != IsAchievedDataListItemId)
                    .Select(filledForm => TargetDTO.From(filledForm));

            return Ok(targets);
        }

        [HttpGet]
        [Route("api/mbs/students/{projectId}/targets")]
        [ResponseType(typeof(IEnumerable<TargetDTO>))]
        public IHttpActionResult GetTargets(Guid projectId, Guid achievement)
        {
            var achievements = UnitOfWork.DataListsRepository.FindIncluding(AchievementDataListId, d => d.AllItems);
            var targets = UnitOfWork.FormValuesRepository.AllIncludingNoTracking(fv => fv.FilledForm)
                   .Where(fv => fv.FilledForm.ProjectId == projectId && fv.MetricId == TargetAchievementIdMetricId)
                   .Where(fv => achievement == null || (fv.GuidValue == achievement && fv.BoolValue == true))
                   .Select(fv => fv.FilledForm)
                   .Distinct()
                   .ToList()
                   .Select(filledForm => TargetDTO.From(filledForm));

            return Ok(targets);
        }


        [HttpGet]
        [Route("api/mbs/students/{projectId}/evidences")]
        [ResponseType(typeof(IEnumerable<EvidenceDTO>))]
        public IHttpActionResult GetEvidences(Guid projectId, Guid achievement)
        {
            var targets = UnitOfWork.FormValuesRepository.AllIncludingNoTracking(fv => fv.FilledForm)
                    .Where(fv => fv.FilledForm.ProjectId == projectId && fv.MetricId == TargetAchievementIdMetricId)
                    .Where(fv => fv.GuidValue == achievement && fv.BoolValue == true)
                    .Select(fv => fv.FilledFormId)
                    .Distinct()
                    .Select(filledFormId => filledFormId.ToString());

            var evidences = UnitOfWork.FormValuesRepository.AllIncludingNoTracking(fv => fv.FilledForm)
                    .Where(fv => fv.FilledForm.ProjectId == projectId)
                    .Where(fv =>
                    (fv.MetricId == EvidenceAchievementMetricId && fv.GuidValue == achievement) ||
                    (fv.MetricId == TargetIdMetricId && targets.Contains(fv.TextValue)))
                    .Select(fv => fv.FilledForm)
                    .Distinct()
                    .ToList()
                    .Select(f => EvidenceDTO.From(f));

            return Ok(evidences);
        }


        [HttpGet]
        [Route("api/mbs/students/{projectId}/achievementSummary")]
        [ResponseType(typeof(IEnumerable<AchievementSummaryDTO>))]
        public IHttpActionResult GetStudentAchievementSummary(Guid projectId)
        {
            var result = new List<AchievementSummaryDTO>();
            var achievements = UnitOfWork.DataListsRepository.FindIncluding(AchievementDataListId, d => d.AllItems);
            var terms = UnitOfWork.DataListsRepository.FindIncluding(TermsDataListId, d => d.AllItems);

            foreach (var achievement in achievements.Items)
            {
                var targets = UnitOfWork.FormValuesRepository.AllIncludingNoTracking(fv => fv.FilledForm)
                    .Where(fv => fv.FilledForm.ProjectId == projectId && fv.MetricId == TargetAchievementIdMetricId)
                    .Where(fv => fv.GuidValue == achievement.Id && fv.BoolValue == true)
                    .Select(fv => fv.FilledFormId)
                    .Distinct()
                    .Select(filledFormId => filledFormId.ToString());

                var evidences = UnitOfWork.FormValuesRepository.AllIncludingNoTracking(fv => fv.FilledForm)
                        .Where(fv => fv.FilledForm.ProjectId == projectId)
                        .Where(fv =>
                        (fv.MetricId == EvidenceAchievementMetricId && fv.GuidValue == achievement.Id) ||
                        (fv.MetricId == TargetIdMetricId && targets.Contains(fv.TextValue)))
                        .Select(fv => fv.FilledFormId)
                        .Distinct();

                var achievementLogs = UnitOfWork.FilledFormsRepository.AllIncludingNoTracking(f => f.FormValues)
                    .Where(fv => fv.ProjectId == projectId)
                    .Where(f => f.FormValues.Any(fv => fv.MetricId == AchievementLogAchiementMetricId && fv.GuidValue == achievement.Id))
                     .Select(f => f.FormValues.FirstOrDefault(fv => fv.MetricId == AchievementLogTermMetricId))
                     .Select(f => f.GuidValue)
                     .ToList();

                var summary = new AchievementSummaryDTO()
                {
                    ProjectId = projectId,
                    AchievementId = achievement.Id,
                    NumberOfTargets = targets.Count(),
                    NumberOfEvidenceItems = evidences.Count(),
                    IsAchieved = achievementLogs.Any(),
                    BackgroundColor = GetTermColor(terms, achievementLogs.FirstOrDefault())
                };

                result.Add(summary);
            }

            return Ok(result);
        }

        private UpdateDTO GetUpdate(FilledForm survey)
        {
            var desc = string.Empty;

            if (survey.DateCreated == survey.DateUpdated)
                desc = $"{survey.Project.Name} has new {survey.FormTemplate.Title} on {survey.DateCreated.ToString("dd/MM/yyyy")} ";
            else
                desc = $"{survey.Project.Name} {survey.FormTemplate.Title} updated on {survey.DateUpdated.ToString("dd/MM/yyyy")} ";

            return new UpdateDTO()
            {
                Date = survey.DateUpdated,
                Description = desc
            };
        }

        private string GetTermColor(DataList terms, Guid? dataListItemId)
        {
            if (dataListItemId == null)
                return string.Empty;

            return terms.AllItems.SingleOrDefault(i => i.Id == dataListItemId).Value.ToString("X");
        }
    }
}
