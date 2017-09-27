using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class AssignmentsRepository : Repository<Assignment>
    {
        public AssignmentsRepository(UnitOfWork uow) : base(uow) { }

        public enum AccessLevels
        {
            AllowView,
            AllowAdd,
            AllowEdit,
            AllowDelete
        }

        private Assignment FlagAccessLevel(Assignment assignment, AccessLevels accessLevel, bool grant)
        {
            switch (accessLevel)
            {
                case AccessLevels.AllowView:
                    {
                        assignment.CanView = grant;
                        break;
                    }
                case AccessLevels.AllowAdd:
                    {
                        assignment.CanAdd = grant;
                        if (grant) assignment.CanView = true;
                        break;
                    }
                case AccessLevels.AllowEdit:
                    {
                        assignment.CanEdit = grant;
                        if (grant) assignment.CanView = true;
                        break;
                    }
                case AccessLevels.AllowDelete:
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

        public void AssignAccessLevel(Guid projectId, Guid userId, AccessLevels accessLevel, bool grant)
        {
            var project = this.CurrentUOW.ProjectsRepository.Find(projectId);
            var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == userId);
            if (assignment != null)
            {
                assignment = FlagAccessLevel(assignment, accessLevel, grant);
                this.CurrentUOW.AssignmentsRepository.InsertOrUpdate(assignment);
            }
            else
            {
                var entity = new Assignment() { ProjectId = projectId, OrgUserId = userId };
                entity = FlagAccessLevel(entity, accessLevel, grant);
                this.CurrentUOW.AssignmentsRepository.InsertOrUpdate(entity);
            }

            this.CurrentUOW.Save();
        }

        public override void InsertOrUpdate(Assignment entity)
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
