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

        #region Properties

        public OrgUser User { set; get; }

        private UnitOfWork UOW { set; get; }

        private bool OnlyPublished { set; get; }

        private ICollection<OrgUserType> UserTypesWithFullAccess { set; get; }

        private ICollection<OrgUserType> UserTypesWithLimitedAccess { set; get; }

        #endregion Properties

        #region C-tor

        public SurveyProvider(OrgUser user, UnitOfWork uow, bool onlyPublished = true)
        {
            this.User = user;
            this.UOW = uow;
            this.OnlyPublished = onlyPublished;
            this.UserTypesWithFullAccess = new List<OrgUserType>(new[] { OrgUserTypesRepository.Administrator, OrgUserTypesRepository.Manager });
            this.UserTypesWithLimitedAccess = new List<OrgUserType>(new[] { OrgUserTypesRepository.TeamUser, OrgUserTypesRepository.ExternalUser });
        }

        #endregion C-tor

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

        public IEnumerable<FormTemplate> GetAllProjectTemplates(Guid? projectId, FormTemplateDiscriminators discriminator)
        {
            var templates = Enumerable.Empty<FormTemplate>().AsQueryable();

            // uncomment the following lines and replace it with the code-block below it,
            // if you'd like to include SHARED FORMS in the result.

            //if (this.User != null)
            //{
            //    templates = OnlyCanBeAccessedByUser(UOW.FormTemplatesRepository
            //        .AllIncludingNoTracking(f => f.Project, t => t.MetricGroups.Select(g => g.Metrics)));

            //    var assignments = UOW.AssignmentsRepository.AllAsNoTracking;
            //    if (projectId == null)
            //        assignments = assignments.Where(a => a.OrgUserId == User.Id);
            //    else
            //        assignments = assignments.Where(a => a.ProjectId == projectId && a.OrgUserId == User.Id);

            //    templates = templates.Where(t => assignments.Any(a => a.ProjectId == t.ProjectId || t.ProjectId == null));
            //}
            //else
            //    templates = UOW.FormTemplatesRepository
            //        .AllIncludingNoTracking(f => f.Project, t => t.MetricGroups.Select(g => g.Metrics))
            //        .Where(t => projectId == null || t.ProjectId == null || t.ProjectId == projectId);

            // exclude Shared Forms from the result.
            if (this.User != null)
            {
                templates = OnlyCanBeAccessedByUser(UOW.FormTemplatesRepository
                    .AllIncludingNoTracking(f => f.Project, t => t.MetricGroups.Select(g => g.Metrics)));

                var assignments = UOW.AssignmentsRepository.AllAsNoTracking;
                if (projectId == null)
                    assignments = assignments.Where(a => a.OrgUserId == User.Id);
                else
                    assignments = assignments.Where(a => a.ProjectId == projectId && a.OrgUserId == User.Id);

                templates = templates.Where(t => assignments.Any(a => a.ProjectId == t.ProjectId));
            }
            else
            {
                templates = UOW.FormTemplatesRepository
                    .AllIncludingNoTracking(f => f.Project, t => t.MetricGroups.Select(g => g.Metrics))
                    .Where(t => projectId == null || t.ProjectId == projectId);
            }
            
            var result = templates.Where(t => t.Discriminator == discriminator).ToList();

            return PostLoadFilters(result);
        }

        private IQueryable<FormTemplate> OnlyCanBeAccessedByUser(IQueryable<FormTemplate> datasource)
        {
            if (OnlyPublished)
                datasource = datasource.Where(t => t.IsPublished);

            if (this.User != null)
            {
                if (UserTypesWithFullAccess.Contains(User.Type))
                {
                    var threads = datasource.Where(t => t.OrganisationId == User.OrganisationId);
                    var assignedThreads = datasource.Where(t => t.Project.Assignments.Select(a => a.OrgUserId).Contains(User.Id));
                    var looseThreads = datasource.Where(t => t.Assignments.Any(a => a.OrgUserId == User.Id));

                    var res = new List<FormTemplate>();
                    res.AddRange(threads.ToList());
                    res.AddRange(assignedThreads.ToList());
                    res.AddRange(looseThreads.ToList());

                    return res.Distinct().AsQueryable();
                }

                if (UserTypesWithLimitedAccess.Contains(User.Type))
                {
                    var threads = datasource.Where(t => t.ProjectId == null || !t.Project.Archived && t.Project.Assignments.Select(a => a.OrgUserId).Contains(User.Id));
                    var looseThreads = datasource.Where(t => t.Assignments.Any(a => a.OrgUserId == User.Id));

                    var res = new List<FormTemplate>();
                    res.AddRange(threads.ToList());
                    res.AddRange(looseThreads.ToList());

                    return res.Distinct().AsQueryable();
                }
            }

            return datasource;
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



