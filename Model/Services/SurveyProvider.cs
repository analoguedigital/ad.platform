using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services
{
    public class SurveyProvider
    {
        public OrgUser User { set; get; }
        private UnitOfWork UOW { set; get; }
        private bool OnlyPublished { set; get; }
        private ICollection<OrgUserType> UserTypesWithFullAccess { set; get; }
        private ICollection<OrgUserType> UserTypesWithLimitedAccess { set; get; }

        public SurveyProvider(OrgUser user, UnitOfWork uow, bool onlyPublished = true)
        {
            this.User = user;
            this.UOW = uow;
            this.OnlyPublished = onlyPublished;
            this.UserTypesWithFullAccess = new List<OrgUserType>(new[] { OrgUserTypesRepository.Administrator, OrgUserTypesRepository.Manager });
            this.UserTypesWithLimitedAccess = new List<OrgUserType>(new[] { OrgUserTypesRepository.TeamUser, OrgUserTypesRepository.ExternalUser });
        }

        public IEnumerable<FormTemplate> GetAllFormTemplates()
        {
            var result = OnlyCanBeAccessedByUser(UOW.FormTemplatesRepository.AllIncluding(f => f.Project));
            return result.ToList();
        }

        public FormTemplate GetFormTemplate(Guid id)
        {
            var result = OnlyCanBeAccessedByUser(UOW.FormTemplatesRepository.AllIncluding(f => f.Project)).Where(f => f.Id == id);
            return result.SingleOrDefault();
        }

        public FormTemplate GetFormTemplateWithMetrics(Guid id)
        {
            var result = OnlyCanBeAccessedByUser(UOW.FormTemplatesRepository.AllIncludingNoTracking(f => f.Project, t => t.MetricGroups.Select(g => g.Metrics))).Where(f => f.Id == id);
            return result.SingleOrDefault();
        }

        public IEnumerable<FormTemplate> GetAllFormTemplatesWithMetrics()
        {
            var result = OnlyCanBeAccessedByUser(UOW.FormTemplatesRepository.AllIncludingNoTracking(f => f.Project, t => t.MetricGroups.Select(g => g.Metrics)));
            return PostLoadFilters(result.ToList());
        }

        public IEnumerable<FormTemplate> GetAllProjectTemplates(Guid? projectId)
        {
            var templates = OnlyCanBeAccessedByUser(UOW.FormTemplatesRepository.AllIncludingNoTracking(f => f.Project, t => t.MetricGroups.Select(g => g.Metrics)));

            templates = templates.Where(t => projectId == null || t.ProjectId == null || t.ProjectId == projectId);

            var assignments = UOW.AssignmentsRepository.AllAsNoTracking;
            if (projectId == null)
                assignments = assignments.Where(a => a.OrgUserId == User.Id);
            else
                assignments = assignments.Where(a => a.ProjectId == projectId && a.OrgUserId == User.Id);

            templates = templates.Where(t => assignments.Any(a => a.ProjectId == t.ProjectId || t.ProjectId == null));

            return PostLoadFilters(templates.ToList());
        }

        private IQueryable<FormTemplate> OnlyCanBeAccessedByUser(IQueryable<FormTemplate> datasource)
        {
            if (OnlyPublished)
                datasource = datasource.Where(t => t.IsPublished);

            if (UserTypesWithFullAccess.Contains(User.Type))
                return datasource.Where(t => t.OrganisationId == User.OrganisationId);

            if (UserTypesWithLimitedAccess.Contains(User.Type))
                return datasource.Where(t => t.ProjectId == null || !t.Project.Archived && t.Project.Assignments.Select(a => a.OrgUserId).Contains(User.Id));

            return null;
        }

        private IEnumerable<FormTemplate> PostLoadFilters(IEnumerable<FormTemplate> templates)
        {
            foreach (var template in templates)
            {
                template.MetricGroups = template.MetricGroups.OrderBy(g => g.PageOrder).ToList();

                foreach (var group in template.MetricGroups)
                {
                    group.Metrics = group.Metrics.Where(m => !m.IsArchived()).OrderBy(m => m.Order).ToList();
                }

                yield return template;
            }
        }
    }
}



