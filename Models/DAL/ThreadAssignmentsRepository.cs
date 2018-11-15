using LightMethods.Survey.Models.Entities;
using System;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class ThreadAssignmentsRepository : Repository<ThreadAssignment>
    {
        public ThreadAssignmentsRepository(UnitOfWork uow) : base(uow) { }

        public enum ThreadAccessLevels
        {
            AllowView,
            AllowAdd,
            AllowEdit,
            AllowDelete
        }

        private ThreadAssignment FlagAccessLevel(ThreadAssignment assignment, ThreadAccessLevels accessLevel, bool grant)
        {
            switch (accessLevel)
            {
                case ThreadAccessLevels.AllowView:
                    {
                        assignment.CanView = grant;
                        break;
                    }
                case ThreadAccessLevels.AllowAdd:
                    {
                        assignment.CanAdd = grant;
                        if (grant) assignment.CanView = true;
                        break;
                    }
                case ThreadAccessLevels.AllowEdit:
                    {
                        assignment.CanEdit = grant;
                        if (grant) assignment.CanView = true;
                        break;
                    }
                case ThreadAccessLevels.AllowDelete:
                    {
                        assignment.CanDelete = grant;
                        if (grant) assignment.CanView = true;
                        break;
                    }
                default:
                    break;
            }

            return assignment;
        }

        public ThreadAssignment AssignAccessLevel(Guid formTemplateId, Guid userId, ThreadAccessLevels accessLevel, bool grant)
        {
            var thread = this.CurrentUOW.FormTemplatesRepository.Find(formTemplateId);
            var assignment = thread.Assignments.SingleOrDefault(a => a.OrgUserId == userId);

            if (assignment != null)
            {
                assignment = FlagAccessLevel(assignment, accessLevel, grant);
                this.CurrentUOW.ThreadAssignmentsRepository.InsertOrUpdate(assignment);
            }
            else
            {
                assignment = new ThreadAssignment() { FormTemplateId = formTemplateId, OrgUserId = userId };
                assignment = FlagAccessLevel(assignment, accessLevel, grant);
                this.CurrentUOW.ThreadAssignmentsRepository.InsertOrUpdate(assignment);
            }

            this.CurrentUOW.Save();

            return thread.Assignments.SingleOrDefault(a => a.OrgUserId == userId);
        }

        public override void InsertOrUpdate(ThreadAssignment entity)
        {
            if (entity.CanAdd || entity.CanEdit || entity.CanDelete || entity.CanView)
                base.InsertOrUpdate(entity);
            else
            {
                var entry = this.Context.Entry(entity);
                if (entry.State == System.Data.Entity.EntityState.Modified || entry.State == System.Data.Entity.EntityState.Detached)
                    this.Delete(entity);
            }
        }
    }
}
