using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator,Organisation team manager")]
    public class OrgTeamsController : BaseApiController
    {
        private OrganisationRepository Organisations { get { return UnitOfWork.OrganisationRepository; } }
        private OrganisationTeamsRepository Teams { get { return UnitOfWork.OrganisationTeamsRepository; } }
        private OrgTeamUsersRepository TeamUsers { get { return UnitOfWork.OrgTeamUsersRepository; } }
        private OrgUsersRepository OrgUsers { get { return UnitOfWork.OrgUsersRepository; } }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrganisationTeamDTO>))]
        public IHttpActionResult Get(Guid? organisationId = null)
        {
            var result = new List<OrganisationTeamDTO>();

            if (this.CurrentOrgUser != null)
            {
                var teams = Teams.AllAsNoTracking
                    .Where(x => x.OrganisationId == CurrentOrganisationId)
                    .OrderBy(x => x.Name)
                    .ToList()
                    .Select(x => Mapper.Map<OrganisationTeamDTO>(x)).ToList();
                result = teams;
            }
            else
            {
                if (organisationId.HasValue)
                {
                    var teams = Teams.AllAsNoTracking
                       .Where(x => x.OrganisationId == organisationId.Value)
                       .OrderBy(x => x.Name)
                       .ToList()
                       .Select(x => Mapper.Map<OrganisationTeamDTO>(x)).ToList();
                    result = teams;
                }
                else
                {
                    var teams = Teams.AllAsNoTracking
                        .OrderBy(x => x.Organisation.Name)
                        .ThenBy(x => x.Name)
                        .ToList()
                        .Select(x => Mapper.Map<OrganisationTeamDTO>(x)).ToList();
                    result = teams;
                }
            }

            return Ok(result);
        }

        [DeflateCompression]
        [ResponseType(typeof(OrganisationTeamDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<OrganisationTeamDTO>(new OrganisationTeam()));

            var team = Mapper.Map<OrganisationTeamDTO>(Teams.Find(id));

            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrganisationTeamDTO>))]
        [Route("api/orgteams/getuserteams/{userId:guid}")]
        public IHttpActionResult GetUserTeams(Guid userId)
        {
            if (userId == Guid.Empty)
                return NotFound();

            var teams = UnitOfWork.OrgTeamUsersRepository.AllAsNoTracking
                .Where(u => u.OrgUserId == userId)
                .Select(t => t.OrganisationTeam)
                .ToList()
                .Select(x => Mapper.Map<OrganisationTeamDTO>(x))
                .ToList();

            return Ok(teams);
        }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrgUserDTO>))]
        [Route("api/orgteams/{id:guid}/assignableusers")]
        public IHttpActionResult GetAssignableUsers(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<OrgUserDTO>(new OrgUser()));

            var team = this.Teams.Find(id);
            if (team == null)
                return NotFound();

            var users = this.OrgUsers.AllIncluding(u => u.Type)
                    .Where(u => u.OrganisationId == team.OrganisationId)
                    .OrderBy(u => u.Surname)
                    .ThenBy(u => u.FirstName)
                    .ToList()
                    .Select(u => Mapper.Map<OrgUserDTO>(u)).ToList();

            var result = users.Where(x => !team.Users.Any(u => u.OrgUserId == x.Id)).ToList();

            return Ok(result);
        }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<ProjectDTO>))]
        [Route("api/orgteams/{id:guid}/projects")]
        public IHttpActionResult GetProjects(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var orgTeam = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (orgTeam == null)
                return NotFound();

            var projects = new List<ProjectDTO>();

            foreach (var user in orgTeam.Users)
            {
                var userProjects = user.OrgUser.Assignments
                    .Where(a => a.Project.OrganisationId == orgTeam.OrganisationId)
                    .Select(x => Mapper.Map<ProjectDTO>(x.Project))
                    .ToList();
                projects.AddRange(userProjects);
            }

            return Ok(projects.Distinct());
        }

        [HttpPost]
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        [Route("api/orgteams/{id:guid}/assignments")]
        public IHttpActionResult GetAssignments(Guid id, OrgTeamAssignmentsDTO model)
        {
            if (id == Guid.Empty)
                return NotFound();

            var team = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (team == null)
                return NotFound();

            var result = new List<ProjectAssignmentDTO>();

            foreach (var projectId in model.Projects)
            {
                var assignments = UnitOfWork.AssignmentsRepository.AllAsNoTracking
                    .Where(a => a.ProjectId == projectId).ToList()
                    .Where(a => model.OrgUsers.Any(u => a.OrgUserId == u))
                    .Select(a => Mapper.Map<ProjectAssignmentDTO>(a))
                    .ToList();
                result.AddRange(assignments);
            }

            return Ok(result);
        }

        public IHttpActionResult Post([FromBody]OrganisationTeamDTO value)
        {
            var orgTeam = new OrganisationTeam();
            orgTeam.Name = value.Name;
            orgTeam.Description = value.Description;
            orgTeam.Colour = value.Colour;
            orgTeam.IsActive = value.IsActive;

            if (this.CurrentOrgUser != null)
                orgTeam.Organisation = this.CurrentOrganisation;
            else
            {
                if (value.Organisation == null)
                    return BadRequest("Organisation is required");

                orgTeam.OrganisationId = Guid.Parse(value.Organisation.Id);
            }

            UnitOfWork.OrganisationTeamsRepository.InsertOrUpdate(orgTeam);
            UnitOfWork.Save();

            return Ok();
        }

        public IHttpActionResult Put(Guid id, [FromBody]OrganisationTeamDTO value)
        {
            var team = Teams.Find(Guid.Parse(value.Id));
            if (team == null)
                return NotFound();

            team.Name = value.Name;
            team.Description = value.Description;
            team.Colour = value.Colour;
            team.IsActive = value.IsActive;

            if (this.CurrentOrgUser == null)
            {
                if (value.Organisation == null)
                    return BadRequest("Organisation is required");

                team.OrganisationId = Guid.Parse(value.Organisation.Id);
            }

            UnitOfWork.OrganisationTeamsRepository.InsertOrUpdate(team);
            UnitOfWork.Save();

            return Ok();
        }

        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            try
            {
                var team = this.Teams.Find(id);
                if (team == null)
                    return NotFound();

                Teams.Delete(team);
                UnitOfWork.Save();

                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest("This team cannot be deleted!");
            }
        }

        [HttpDelete]
        [Route("api/orgteams/{id:guid}/removeuser/{userId:guid}")]
        public async Task<IHttpActionResult> RemoveUser(Guid id, Guid userId)
        {
            var orgTeam = this.Teams.Find(id);
            if (orgTeam == null)
                return NotFound();

            var teamUser = this.TeamUsers.Find(userId);
            if (teamUser == null)
                return NotFound();

            if (teamUser.IsManager)
                await UnitOfWork.UserManager.RemoveFromRoleAsync(teamUser.OrgUserId, Role.ORG_TEAM_MANAGER);

            await UnitOfWork.UserManager.RemoveFromRoleAsync(teamUser.OrgUserId, Role.ORG_TEAM_USER);

            UnitOfWork.OrgTeamUsersRepository.Delete(userId);
            UnitOfWork.Save();

            return Ok();
        }

        [HttpPost]
        [Route("api/orgteams/{id:guid}/assign/")]
        public async Task<IHttpActionResult> AddAssignments(Guid id, OrgTeamAssignmentDTO model)
        {
            var orgTeam = this.Teams.Find(id);
            if (orgTeam == null)
                return NotFound();

            foreach (var assignment in model.Users)
            {
                var orgUser = this.OrgUsers.Find(assignment.OrgUserId);
                if (orgUser != null)
                {
                    if (orgUser.Organisation.Id == orgTeam.Organisation.Id)
                    {
                        orgTeam.Users.Add(new OrgTeamUser
                        {
                            OrgUserId = orgUser.Id,
                            OrganisationTeamId = orgTeam.Id,
                            IsManager = assignment.IsManager
                        });

                        await UnitOfWork.UserManager.AddToRoleAsync(orgUser.Id, Role.ORG_TEAM_USER);

                        if (assignment.IsManager)
                            await UnitOfWork.UserManager.AddToRoleAsync(orgUser.Id, Role.ORG_TEAM_MANAGER);
                    }
                }
            }

            UnitOfWork.Save();

            return Ok();
        }

        [HttpPost]
        [Route("api/orgteams/{id:guid}/updatestatus/{userId:guid}/{flag:bool}")]
        public async Task<IHttpActionResult> UpdateUserStatus(Guid id, Guid userId, bool flag)
        {
            var orgTeam = this.Teams.Find(id);
            if (orgTeam == null)
                return NotFound();

            var teamUser = this.TeamUsers.Find(userId);
            if (teamUser == null)
                return NotFound();

            teamUser.IsManager = flag;

            if (flag)
                await UnitOfWork.UserManager.AddToRoleAsync(teamUser.OrgUserId, Role.ORG_TEAM_MANAGER);
            else
                await UnitOfWork.UserManager.RemoveFromRoleAsync(teamUser.OrgUserId, Role.ORG_TEAM_MANAGER);

            UnitOfWork.Save();

            return Ok();
        }

    }

}
