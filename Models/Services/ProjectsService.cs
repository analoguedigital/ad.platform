using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.Services
{
    public class ProjectsService
    {

        #region Properties

        private UnitOfWork UnitOfWork { get; set; }

        #endregion Properties

        #region C-tor

        public ProjectsService(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        #endregion C-tor

        #region Methods

        public List<ProjectDTO> Get(User user, Guid? organisationId)
        {
            var projects = new List<Project>();
            var result = new List<ProjectDTO>();

            // get the list of projects
            if (user is OrgUser)
            {
                projects = UnitOfWork.ProjectsRepository
                      .GetProjects(user)
                      .OrderByDescending(p => p.DateCreated)
                      .ToList();
            }
            else if (user is SuperUser)
            {
                if (organisationId.HasValue)
                {
                    projects = UnitOfWork.ProjectsRepository
                        .AllAsNoTracking
                        .Where(x => x.OrganisationId == organisationId.Value)
                        .OrderByDescending(x => x.DateCreated)
                        .ToList();
                }
                else
                {
                    projects = UnitOfWork.ProjectsRepository
                        .AllAsNoTracking
                        .OrderByDescending(x => x.DateCreated)
                        .ToList();
                }
            }

            foreach (var project in projects)
            {
                var dto = Mapper.Map<ProjectDTO>(project);
                dto.AssignmentsCount = project.Assignments.Count;

                // map the user assignment
                dto = MapUserAssignment(dto, user, project);

                // get the last entry
                var lastEntry = GetLastEntry(project.Id);
                if (lastEntry != null)
                    dto.LastEntry = lastEntry.SurveyDate;

                // get project teams
                dto.TeamsCount = GetTeamsCount(project);

                result.Add(dto);
            }

            return result;
        }

        public ProjectDTO Get(User user, Guid id)
        {
            var project = UnitOfWork.ProjectsRepository.GetProjects(user)
                .Where(p => p.Id == id)
                .SingleOrDefault();

            if (project == null) return null;

            var result = Mapper.Map<ProjectDTO>(project);
            result.AssignmentsCount = project.Assignments.Count;

            // map user assignment
            result = MapUserAssignment(result, user, project);

            // get last entry date
            var lastEntry = GetLastEntry(id);
            if (lastEntry != null)
                result.LastEntry = lastEntry.SurveyDate;

            // get teams count
            result.TeamsCount = GetTeamsCount(project);

            return result;
        }

        public ProjectDTO GetDirect(User user, Guid id)
        {
            var project = UnitOfWork.ProjectsRepository
                .AllIncluding(x => x.Assignments)
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (project == null) return null;

            var result = Mapper.Map<ProjectDTO>(project);
            result.AssignmentsCount = project.Assignments.Count;

            // map user assignment
            result = MapUserAssignment(result, user, project);

            // get last entry date
            var lastEntry = GetLastEntry(id);
            if (lastEntry != null)
                result.LastEntry = lastEntry.SurveyDate;

            // get teams count
            result.TeamsCount = GetTeamsCount(project);

            return result;
        }

        public List<ProjectDTO> GetSharedProjects(OrgUser user)
        {
            var threadAssignments = UnitOfWork.ThreadAssignmentsRepository
                .AllAsNoTracking
                .Where(x => x.OrgUserId == user.Id && x.FormTemplate.Discriminator == FormTemplateDiscriminators.RegularThread && x.FormTemplate.CreatedById != user.Id)
                .ToList();

            var projects = threadAssignments
                .Select(x => x.FormTemplate.Project)
                .ToList()
                .Distinct()
                .ToList();

            var result = new List<ProjectDTO>();

            foreach (var project in projects)
            {
                var dto = Mapper.Map<ProjectDTO>(project);
                dto.AssignmentsCount = project.Assignments.Count();

                dto = MapUserAssignment(dto, user, project);

                // get last entry
                var lastEntry = GetLastEntry(project.Id);
                if (lastEntry != null)
                    dto.LastEntry = lastEntry.SurveyDate;

                // get teams count
                dto.TeamsCount = GetTeamsCount(project);

                result.Add(dto);
            }

            return result;
        }

        public List<OrganisationTeamDTO> GetTeams(Project project)
        {
            var projectTeams = UnitOfWork.OrganisationTeamsRepository
                .AllAsNoTracking
                .Where(x => x.OrganisationId == project.OrganisationId);

            var teams = new List<OrganisationTeamDTO>();

            foreach (var team in projectTeams)
            {
                foreach (var user in team.Users)
                {
                    if (user.OrgUser.Assignments.Any(a => a.ProjectId == project.Id))
                        teams.Add(Mapper.Map<OrganisationTeamDTO>(team));
                }
            }
            
            return teams.Distinct().ToList();
        }

        #endregion Methods

        #region Helpers

        private ProjectDTO MapUserAssignment(ProjectDTO value, User user, Project project)
        {
            if (user is OrgUser)
            {
                var assignment = UnitOfWork.ProjectsRepository.GetUserAssignment(project, user.Id);
                Mapper.Map(assignment, value);
            }
            else if (user is SuperUser)
            {
                value.AllowView = true;
                value.AllowAdd = true;
                value.AllowEdit = true;
                value.AllowDelete = true;
                value.AllowExportPdf = true;
                value.AllowExportZip = true;
            }

            return value;
        }

        private FilledForm GetLastEntry(Guid projectId)
        {
            var lastEntry = UnitOfWork.FilledFormsRepository
                .AllAsNoTracking
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.SurveyDate)
                .Take(1)
                .FirstOrDefault();

            return lastEntry;
        }

        private int GetTeamsCount(Project project)
        {
            var relatedTeams = new List<OrganisationTeamDTO>();

            var teams = UnitOfWork.OrganisationTeamsRepository
                .AllAsNoTracking
                .Where(x => x.OrganisationId == project.OrganisationId);

            foreach (var team in teams)
            {
                foreach (var teamUser in team.Users)
                {
                    if (teamUser.OrgUser.Assignments.Any(a => a.ProjectId == project.Id))
                        relatedTeams.Add(Mapper.Map<OrganisationTeamDTO>(team));
                }
            }

            return relatedTeams.Distinct().Count();
        }

        #endregion Helpers

    }
}
