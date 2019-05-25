using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Data.Entity;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class FormTemplatesRepository : Repository<FormTemplate>
    {
        public FormTemplatesRepository(UnitOfWork uow) : base(uow) { }

        //Getting a form template with preloaded metrics
        public FormTemplate GetNotTrackedFullTemplate(Guid id)
        {
            return Context.FormTemplates
                .Where(t => t.Id == id)
                .Include(t => t.MetricGroups.Select(g => g.Metrics))
                .AsNoTracking()
                .First();
        }

        //Getting a form template with preloaded metrics
        public FormTemplate GetFullTemplate(Guid id)
        {
            return Context.FormTemplates
                .Where(t => t.Id == id)
                .Include(t => t.MetricGroups.Select(g => g.Metrics))
                .First();
        }

        public FormTemplate Clone(FormTemplate template, Guid creatorId, string newTitle, string newColour, Guid? newProjectId)
        {
            var clone = template.Clone();
            clone.Title = newTitle;
            clone.Colour = newColour;
            clone.ProjectId = newProjectId;
            //clone.CreatedById = user.Id;
            clone.CreatedById = creatorId;
            clone.IsPublished = true;

            using (var tran = Context.Database.BeginTransaction())
            {
                InsertOrUpdate(clone);
                Save();

                if (template.CalendarDateMetricId != null)
                {
                    clone.CalendarDateMetricId = clone.MetricGroups.SelectMany(g => g.Metrics)
                        .Single(m => m.ShortTitle == template.CalendarDateMetric.ShortTitle).Id;
                }

                if (template.TimelineBarMetricId != null)
                {
                    clone.TimelineBarMetricId = clone.MetricGroups.SelectMany(g => g.Metrics)
                        .Single(m => m.ShortTitle == template.TimelineBarMetric.ShortTitle).Id;
                }

                InsertOrUpdate(clone);
                Save();
                tran.Commit();
            }

            return clone;
        }

        public FormTemplate CreateAdviceThread(FormTemplate template, Guid userId, string title, string colour, Guid projectId)
        {
            var clone = template.Clone();
            clone.Title = title;
            clone.Colour = colour;
            clone.ProjectId = projectId;
            clone.CreatedById = userId;
            clone.IsPublished = true;
            clone.Discriminator = FormTemplateDiscriminators.AdviceThread;

            using (var tran = Context.Database.BeginTransaction())
            {
                InsertOrUpdate(clone);
                Save();

                if (template.CalendarDateMetricId != null)
                {
                    clone.CalendarDateMetricId = clone.MetricGroups.SelectMany(g => g.Metrics)
                        .Single(m => m.ShortTitle == template.CalendarDateMetric.ShortTitle).Id;
                }

                if (template.TimelineBarMetricId != null)
                {
                    clone.TimelineBarMetricId = clone.MetricGroups.SelectMany(g => g.Metrics)
                        .Single(m => m.ShortTitle == template.TimelineBarMetric.ShortTitle).Id;
                }

                InsertOrUpdate(clone);
                Save();
                tran.Commit();
            }

            return clone;
        }

        public ThreadAssignmentDTO GetUserAssignment(FormTemplate template, Guid userId)
        {
            var result = new ThreadAssignmentDTO();

            if (this.CurrentUOW.UserManager.RolesContainsAny(userId, Role.ORG_ADMINSTRATOR))
            {
                result.CanView = true;
                result.CanAdd = true;
                result.CanEdit = true;
                result.CanDelete = true;
            }
            else
            {
                //var assignment = template.Assignments.Where(a => a.OrgUserId == userId).SingleOrDefault();
                var assignment = base.CurrentUOW.ThreadAssignmentsRepository.AllAsNoTracking
                    .Where(x => x.FormTemplateId == template.Id && x.OrgUserId == userId)
                    .SingleOrDefault();

                if (assignment != null)
                {
                    result.CanView = assignment.CanView;
                    result.CanAdd = assignment.CanAdd;
                    result.CanEdit = assignment.CanEdit;
                    result.CanDelete = assignment.CanDelete;
                }
                else
                {
                    // user doesn't have a thread assignment on this Form Template
                    result.CanView = null;
                    result.CanAdd = null;
                    result.CanEdit = null;
                    result.CanDelete = null;
                }
            }

            return result;
        }
    }
}
