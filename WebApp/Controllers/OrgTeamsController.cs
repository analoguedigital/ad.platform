using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    // TODO: Team Managers shouldn't have full access to teams under their organization.
    // they should have access only to the teams that have been assigned to!

    [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator,Organisation team manager")]
    public class OrgTeamsController : BaseApiController
    {

        private const string CACHE_KEY = "ORG_TEAMS";

        private TeamService TeamService { get; set; }

        public OrgTeamsController()
        {
            TeamService = new TeamService(UnitOfWork);
        }

        #region CRUD

        // GET api/orgTeams/{organisationId?}
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrganisationTeamDTO>))]
        public IHttpActionResult Get(Guid? organisationId = null)
        {
            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{CurrentOrgUser.OrganisationId}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var teams = TeamService.Get(CurrentOrgUser.OrganisationId);
                    MemoryCacher.Add(cacheKey, teams, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(teams);
                }
                else
                {
                    var result = (List<OrganisationTeamDTO>)cacheEntry;
                    return new CachedResult<List<OrganisationTeamDTO>>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser or PlatformUser
            var _cacheKey = organisationId.HasValue ? $"{CACHE_KEY}_{organisationId}" : CACHE_KEY;
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var teams = organisationId.HasValue ? TeamService.Get(organisationId) : TeamService.Get();
                MemoryCacher.Add(_cacheKey, teams, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(teams);
            }
            else
            {
                var retVal = (List<OrganisationTeamDTO>)_cacheEntry;
                return new CachedResult<List<OrganisationTeamDTO>>(retVal, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/orgTeams/{id}
        [DeflateCompression]
        [ResponseType(typeof(OrganisationTeamDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<OrganisationTeamDTO>(new OrganisationTeam()));

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var team = UnitOfWork.OrganisationTeamsRepository.Find(id);
                if (team == null)
                    return NotFound();

                var result = Mapper.Map<OrganisationTeamDTO>(team);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (OrganisationTeamDTO)cacheEntry;
                return new CachedResult<OrganisationTeamDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/orgTeams/getUserTeams/{userId}
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrganisationTeamDTO>))]
        [Route("api/orgteams/getuserteams/{userId:guid}")]
        public IHttpActionResult GetUserTeams(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            var cacheKey = $"{CACHE_KEY}_USER_{userId}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var userTeams = TeamService.GetUserTeams(userId);
                MemoryCacher.Add(cacheKey, userTeams, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(userTeams);
            }
            else
            {
                var result = (List<OrganisationTeamDTO>)cacheEntry;
                return new CachedResult<List<OrganisationTeamDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/orgTeams/{id}/assignableUsers
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrgUserDTO>))]
        [Route("api/orgteams/{id:guid}/assignableusers")]
        public IHttpActionResult GetAssignableUsers(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<OrgUserDTO>(new OrgUser()));

            var team = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (team == null)
                return NotFound();

            var cacheKey = $"{CACHE_KEY}_ASSIGNABLE_USERS_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var assignableUsers = TeamService.GetAssignableUsers(team);
                MemoryCacher.Add(cacheKey, assignableUsers, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(assignableUsers);
            }
            else
            {
                var result = (List<OrgUserDTO>)cacheEntry;
                return new CachedResult<List<OrgUserDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/orgTeams/{id}/projects
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<ProjectDTO>))]
        [Route("api/orgteams/{id:guid}/projects")]
        public IHttpActionResult GetProjects(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var team = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (team == null)
                return NotFound();

            var cacheKey = $"{CACHE_KEY}_PROJECTS_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var projects = TeamService.GetTeamProjects(team);
                MemoryCacher.Add(cacheKey, projects, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(projects);
            }
            else
            {
                var result = (List<ProjectDTO>)cacheEntry;
                return new CachedResult<List<ProjectDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/orgTeams/{id}/assignments
        [HttpPost]
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        [Route("api/orgteams/{id:guid}/assignments")]
        public IHttpActionResult GetAssignments(Guid id, OrgTeamAssignmentsDTO model)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var team = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (team == null)
                return NotFound();

            var cacheKey = $"{CACHE_KEY}_ASSIGNMENTS_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var assignments = TeamService.GetAssignments(model);
                MemoryCacher.Add(cacheKey, assignments, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(assignments);
            }
            else
            {
                var result = (List<ProjectAssignmentDTO>)cacheEntry;
                return new CachedResult<List<ProjectAssignmentDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/orgTeams
        public IHttpActionResult Post([FromBody]OrganisationTeamDTO value)
        {
            var orgTeam = new OrganisationTeam
            {
                Name = value.Name,
                Description = value.Description,
                Colour = value.Colour,
                IsActive = value.IsActive
            };

            if (CurrentUser is OrgUser)
                orgTeam.Organisation = CurrentOrgUser.Organisation;
            else
            {
                if (value.Organisation == null)
                    return BadRequest("organisation is required");

                orgTeam.OrganisationId = Guid.Parse(value.Organisation.Id);
            }

            try
            {
                UnitOfWork.OrganisationTeamsRepository.InsertOrUpdate(orgTeam);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/orgTeams/{id}
        public IHttpActionResult Put(Guid id, [FromBody]OrganisationTeamDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var team = UnitOfWork.OrganisationTeamsRepository.Find(Guid.Parse(value.Id));
            if (team == null)
                return NotFound();

            team.Name = value.Name;
            team.Description = value.Description;
            team.Colour = value.Colour;
            team.IsActive = value.IsActive;

            if (CurrentUser is OrgUser)
                team.Organisation = CurrentOrgUser.Organisation;
            else
            {
                if (value.Organisation == null)
                    return BadRequest("organisation is required");

                team.OrganisationId = Guid.Parse(value.Organisation.Id);
            }

            try
            {
                UnitOfWork.OrganisationTeamsRepository.InsertOrUpdate(team);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/orgTeams/{id}
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var team = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (team == null)
                return NotFound();

            try
            {
                UnitOfWork.OrganisationTeamsRepository.Delete(team);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest("active teams cannot be deleted");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/orgTeams/{id}/removeUser/{userId}
        [HttpDelete]
        [Route("api/orgteams/{id:guid}/removeuser/{userId:guid}")]
        public async Task<IHttpActionResult> RemoveUser(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var orgTeam = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (orgTeam == null)
                return NotFound();

            var teamUser = UnitOfWork.OrgTeamUsersRepository.Find(userId);
            if (teamUser == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(teamUser.OrgUserId);

            try
            {
                if (teamUser.IsManager)
                    await UnitOfWork.UserManager.RemoveFromRoleAsync(teamUser.OrgUserId, Role.ORG_TEAM_MANAGER);

                await UnitOfWork.UserManager.RemoveFromRoleAsync(teamUser.OrgUserId, Role.ORG_TEAM_USER);

                UnitOfWork.OrgTeamUsersRepository.Delete(userId);

                // notify the orgUser by email.
                NotifyUserAboutLeavingTeam(orgTeam, teamUser.OrgUser.Email);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST api/orgTeams/{id}/assign
        [HttpPost]
        [Route("api/orgteams/{id:guid}/assign/")]
        public async Task<IHttpActionResult> AddAssignments(Guid id, OrgTeamAssignmentDTO model)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var orgTeam = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (orgTeam == null)
                return NotFound();

            foreach (var assignment in model.Users)
            {
                var orgUser = UnitOfWork.OrgUsersRepository.Find(assignment.OrgUserId);
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

                        // notify the orgUser by email.
                        NotifyUserAboutJoiningTeam(orgTeam, orgUser.Email);
                    }
                }
            }

            try
            {
                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST api/orgTeams/{id}/updateStatus/{userId}/{flag}
        [HttpPost]
        [Route("api/orgteams/{id:guid}/updatestatus/{userId:guid}/{flag:bool}")]
        public async Task<IHttpActionResult> UpdateUserStatus(Guid id, Guid userId, bool flag)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var orgTeam = UnitOfWork.OrganisationTeamsRepository.Find(id);
            if (orgTeam == null)
                return NotFound();

            var teamUser = UnitOfWork.OrgTeamUsersRepository.Find(userId);
            if (teamUser == null)
                return NotFound();

            teamUser.IsManager = flag;

            if (flag)
                await UnitOfWork.UserManager.AddToRoleAsync(teamUser.OrgUserId, Role.ORG_TEAM_MANAGER);
            else
                await UnitOfWork.UserManager.RemoveFromRoleAsync(teamUser.OrgUserId, Role.ORG_TEAM_MANAGER);

            try
            {
                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion CRUD

        #region Helpers

        private void NotifyUserAboutJoiningTeam(OrganisationTeam team, string userEmail)
        {
            var content = @"<p>You have joined <strong>" + team.Name + @"</strong> from <strong>" + team.Organisation.Name + @"</strong>.</p>";

            var email = new Email
            {
                To = userEmail,
                Subject = $"Joined organization team - {team.Name}",
                Content = WebHelpers.GenerateEmailTemplate(content, "You have joined a team")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
        }

        private void NotifyUserAboutLeavingTeam(OrganisationTeam team, string userEmail)
        {
            var content = @"<p>You have left <strong>" + team.Name + @"</strong> from <strong>" + team.Organisation.Name + @"</strong>.</p>";

            var email = new Email
            {
                To = userEmail,
                Subject = $"Left organization team - {team.Name}",
                Content = WebHelpers.GenerateEmailTemplate(content, "You have left a team")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
        }

        #endregion Helpers

    }

}
