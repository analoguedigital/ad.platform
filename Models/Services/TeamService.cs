using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.Services
{
    public class TeamService
    {

        private UnitOfWork UnitOfWork { get; set; }

        public TeamService(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public List<OrganisationTeamDTO> Get(Guid? organisationId = null)
        {
            IQueryable<OrganisationTeam> dataSource =
                UnitOfWork.OrganisationTeamsRepository
                    .AllAsNoTracking
                    .OrderBy(x => x.Organisation.Name)
                    .ThenBy(x => x.Name);

            if (organisationId.HasValue)
                dataSource = dataSource.Where(x => x.OrganisationId == organisationId.Value);

            var result = dataSource
                .ToList()
                .Select(x => Mapper.Map<OrganisationTeamDTO>(x))
                .ToList();

            return result;
        }

        public List<OrganisationTeamDTO> GetUserTeams(Guid userId)
        {
            var teams = UnitOfWork.OrgTeamUsersRepository
                .AllAsNoTracking
                .Where(u => u.OrgUserId == userId)
                .Select(t => t.OrganisationTeam)
                .ToList()
                .Select(x => Mapper.Map<OrganisationTeamDTO>(x))
                .ToList();

            return teams;
        }

        public List<OrgUserDTO> GetAssignableUsers(OrganisationTeam team)
        {
            var users = UnitOfWork.OrgUsersRepository
                .AllIncluding(u => u.Type)
                .Where(u => u.OrganisationId == team.OrganisationId)
                .OrderBy(u => u.Surname)
                .ThenBy(u => u.FirstName)
                .ToList()
                .Select(u => Mapper.Map<OrgUserDTO>(u))
                .ToList();

            var result = users
                .Where(x => !team.Users.Any(u => u.OrgUserId == x.Id))
                .ToList();

            return result;
        }

        public List<ProjectDTO> GetTeamProjects(OrganisationTeam team)
        {
            var projects = new List<ProjectDTO>();

            foreach (var user in team.Users)
            {
                var userProjects = user.OrgUser.Assignments
                    .Where(a => a.Project.OrganisationId == team.OrganisationId)
                    .Select(x => Mapper.Map<ProjectDTO>(x.Project))
                    .ToList();

                projects.AddRange(userProjects);
            }

            return projects.Distinct().ToList();
        }

        public List<ProjectAssignmentDTO> GetAssignments(OrgTeamAssignmentsDTO model)
        {
            var result = new List<ProjectAssignmentDTO>();

            foreach (var projectId in model.Projects)
            {
                var assignments = UnitOfWork.AssignmentsRepository
                    .AllAsNoTracking
                    .Where(a => a.ProjectId == projectId)
                    .ToList()
                    .Where(a => model.OrgUsers.Any(u => a.OrgUserId == u))
                    .Select(a => Mapper.Map<ProjectAssignmentDTO>(a))
                    .ToList();

                result.AddRange(assignments);
            }

            return result;
        }

    }
}
