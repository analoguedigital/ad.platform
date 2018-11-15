﻿using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class ProjectsRepository : Repository<Project>
    {
        internal ProjectsRepository(UnitOfWork uow) : base(uow) { }

        public IQueryable<Project> GetProjects(User user)
        {
            var projects = this.AllIncluding(p => p.Assignments);

            if (user is OrgUser)
            {
                projects = projects.Where(p => p.OrganisationId == ((OrgUser)user).OrganisationId);

                if (!CurrentUOW.UserManager.RolesContainsAny(user.Id, Role.ORG_PROJECT_MANAGMENT, Role.ORG_ADMINSTRATOR))
                    projects = projects.Where(p => p.Assignments.Any(a => a.OrgUserId == user.Id && a.CanView));

                var assignedProjects = this.AllIncluding(x => x.Assignments)
                        .Where(a => a.Assignments.Any(x => x.OrgUserId == user.Id && x.CanView));

                var result = new List<Project>();
                result.AddRange(projects.ToList());
                result.AddRange(assignedProjects.ToList());

                return result.Distinct().AsQueryable();
            }

            return projects;
        }

        public IQueryable<Project> GetMyFlaggedProjects(User user)
        {
            return GetProjects(user).Where(p => p.Flagged);
        }

        public AssignmentDTO GetUserAssignment(Project project, Guid userId)
        {
            var result = new AssignmentDTO();

            if (this.CurrentUOW.UserManager.RolesContainsAny(userId, Role.ORG_PROJECT_MANAGMENT, Role.ORG_ADMINSTRATOR))
            {
                result.AllowView = true;
                result.AllowAdd = true;
                result.AllowEdit = true;
                result.AllowDelete = true;
                result.AllowExportPdf = true;
                result.AllowExportZip = true;
            }
            else
            {
                var assignment = project.Assignments.Where(a => a.OrgUserId == userId).SingleOrDefault();
                if (assignment != null)
                {
                    result.AllowView = assignment.CanView;
                    result.AllowAdd = assignment.CanAdd;
                    result.AllowEdit = assignment.CanEdit;
                    result.AllowDelete = assignment.CanDelete;
                    result.AllowExportPdf = assignment.CanExportPdf;
                    result.AllowExportZip = assignment.CanExportZip;
                }
            }

            return result;
        }
    }
}
