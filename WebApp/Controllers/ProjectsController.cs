using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.DTO.FormTemplates;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using static LightMethods.Survey.Models.DAL.AssignmentsRepository;

namespace WebApi.Controllers
{
    //[Authorize(Roles = "System administrator,Organisation administrator")]
    public class ProjectsController : BaseApiController
    {
        // GET api/projects
        [ResponseType(typeof(IEnumerable<ProjectDTO>))]
        public IHttpActionResult Get(Guid? organisationId = null)
        {
            if (this.CurrentUser is PlatformUser)
                return Ok();

            var projects = new List<Project>();
            var result = new List<ProjectDTO>();

            if (this.CurrentOrgUser != null)
            {
                projects = UnitOfWork.ProjectsRepository
                    .GetProjects(CurrentUser)
                    .OrderByDescending(p => p.DateCreated)
                    .ToList();

                foreach (var project in projects)
                {
                    var dto = Mapper.Map<ProjectDTO>(project);
                    dto.AssignmentsCount = project.Assignments.Count();

                    var assignment = UnitOfWork.ProjectsRepository.GetUserAssignment(project, this.CurrentUser.Id);
                    Mapper.Map(assignment, dto);

                    // get last entry
                    var lastEntry = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                        .Where(x => x.ProjectId == project.Id)
                        .OrderByDescending(x => x.SurveyDate)
                        .Take(1)
                        .FirstOrDefault();

                    if (lastEntry != null)
                        dto.LastEntry = lastEntry.SurveyDate;

                    // get teams count
                    var teams = UnitOfWork.OrganisationTeamsRepository.AllAsNoTracking
                        .Where(x => x.OrganisationId == project.OrganisationId);

                    var relatedTeams = new List<OrganisationTeamDTO>();
                    foreach (var team in teams)
                    {
                        foreach (var user in team.Users)
                        {
                            if (user.OrgUser.Assignments.Any(a => a.ProjectId == project.Id))
                                relatedTeams.Add(Mapper.Map<OrganisationTeamDTO>(team));
                        }
                    }

                    dto.TeamsCount = relatedTeams.Distinct().Count();

                    result.Add(dto);
                }
            }
            else
            {
                if (organisationId.HasValue)
                {
                    projects = UnitOfWork.ProjectsRepository.AllAsNoTracking
                        .Where(x => x.OrganisationId == organisationId.Value)
                        .OrderByDescending(x => x.DateCreated)
                        .ToList();
                }
                else
                {
                    projects = UnitOfWork.ProjectsRepository.AllAsNoTracking
                        .OrderByDescending(x => x.DateCreated)
                        .ToList();
                }

                foreach (var project in projects)
                {
                    var dto = Mapper.Map<ProjectDTO>(project);
                    dto.AssignmentsCount = project.Assignments.Count();

                    dto.AllowView = true;
                    dto.AllowAdd = true;
                    dto.AllowEdit = true;
                    dto.AllowDelete = true;
                    dto.AllowExportPdf = true;
                    dto.AllowExportZip = true;

                    // get last entry
                    var lastEntry = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                        .Where(x => x.ProjectId == project.Id)
                        .OrderByDescending(x => x.SurveyDate)
                        .Take(1)
                        .FirstOrDefault();

                    if (lastEntry != null)
                        dto.LastEntry = lastEntry.SurveyDate;

                    // get teams count
                    var teams = UnitOfWork.OrganisationTeamsRepository.AllAsNoTracking
                        .Where(x => x.OrganisationId == project.OrganisationId);

                    var relatedTeams = new List<OrganisationTeamDTO>();
                    foreach (var team in teams)
                    {
                        foreach (var user in team.Users)
                        {
                            if (user.OrgUser.Assignments.Any(a => a.ProjectId == project.Id))
                                relatedTeams.Add(Mapper.Map<OrganisationTeamDTO>(team));
                        }
                    }

                    dto.TeamsCount = relatedTeams.Distinct().Count();

                    result.Add(dto);
                }
            }

            return Ok(result);
        }

        [ResponseType(typeof(IEnumerable<ProjectDTO>))]
        [Route("api/projects/shared")]
        public IHttpActionResult GetSharedProjects()
        {
            if (this.CurrentUser is SuperUser)
                return Ok();

            var threadAssignments = UnitOfWork.ThreadAssignmentsRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == this.CurrentOrgUser.Id && x.FormTemplate.Discriminator == FormTemplateDiscriminators.RegularThread && x.FormTemplate.CreatedById != this.CurrentOrgUser.Id)
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

                var assignment = UnitOfWork.ProjectsRepository.GetUserAssignment(project, this.CurrentUser.Id);
                Mapper.Map(assignment, dto);

                // get last entry
                var lastEntry = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                    .Where(x => x.ProjectId == project.Id)
                    .OrderByDescending(x => x.SurveyDate)
                    .Take(1)
                    .FirstOrDefault();

                if (lastEntry != null)
                    dto.LastEntry = lastEntry.SurveyDate;

                // get teams count
                var teams = UnitOfWork.OrganisationTeamsRepository.AllAsNoTracking
                    .Where(x => x.OrganisationId == project.OrganisationId);

                var relatedTeams = new List<OrganisationTeamDTO>();
                foreach (var team in teams)
                {
                    foreach (var user in team.Users)
                    {
                        if (user.OrgUser.Assignments.Any(a => a.ProjectId == project.Id))
                            relatedTeams.Add(Mapper.Map<OrganisationTeamDTO>(team));
                    }
                }

                dto.TeamsCount = relatedTeams.Distinct().Count();

                result.Add(dto);
            }

            return Ok(result);
        }

        [ResponseType(typeof(ProjectDTO))]
        [Route("api/projects/direct/{id:guid}")]
        public IHttpActionResult GetDirect(Guid id)
        {
            var project = UnitOfWork.ProjectsRepository
                .AllIncluding(x => x.Assignments)
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (project == null)
                return NotFound();

            var dto = Mapper.Map<ProjectDTO>(project);
            dto.AssignmentsCount = project.Assignments.Count();

            var assignment = UnitOfWork.ProjectsRepository.GetUserAssignment(project, this.CurrentUser.Id);
            Mapper.Map(assignment, dto);

            // get last entry
            var lastEntry = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                .Where(x => x.ProjectId == project.Id)
                .OrderByDescending(x => x.SurveyDate)
                .Take(1)
                .FirstOrDefault();

            if (lastEntry != null)
                dto.LastEntry = lastEntry.SurveyDate;

            // get teams count
            var teams = UnitOfWork.OrganisationTeamsRepository.AllAsNoTracking
                .Where(x => x.OrganisationId == project.OrganisationId);

            var relatedTeams = new List<OrganisationTeamDTO>();
            foreach (var team in teams)
                foreach (var user in team.Users)
                    if (user.OrgUser.Assignments.Any(a => a.ProjectId == project.Id))
                        relatedTeams.Add(Mapper.Map<OrganisationTeamDTO>(team));

            dto.TeamsCount = relatedTeams.Distinct().Count();

            return Ok(dto);
        }

        // GET api/projects/{id}
        [DeflateCompression]
        [ResponseType(typeof(ProjectDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<ProjectDTO>(new Project()));

            var project = UnitOfWork.ProjectsRepository.GetProjects(CurrentUser).Where(p => p.Id == id).SingleOrDefault();
            if (project == null)
                return NotFound();

            var result = Mapper.Map<ProjectDTO>(project);
            if (this.CurrentOrgUser != null)
            {
                var assignment = UnitOfWork.ProjectsRepository.GetUserAssignment(project, this.CurrentUser.Id);
                Mapper.Map(assignment, result);
            }
            else
            {
                result.AllowView = true;
                result.AllowAdd = true;
                result.AllowEdit = true;
                result.AllowDelete = true;
                result.AllowExportPdf = true;
                result.AllowExportZip = true;
            }

            // get last entry
            var lastEntry = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                .Where(x => x.ProjectId == project.Id)
                .OrderByDescending(x => x.SurveyDate)
                .Take(1)
                .FirstOrDefault();

            if (lastEntry != null)
                result.LastEntry = lastEntry.SurveyDate;

            return Ok(result);
        }

        // GET api/projects/{id}/assignments
        [DeflateCompression]
        [Route("api/projects/{id:guid}/assignments")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult GetAssignments(Guid id)
        {
            var project = UnitOfWork.ProjectsRepository.FindIncluding(id, p => p.Assignments);
            if (project == null)
                return NotFound();

            var result = project.Assignments.Select(a => Mapper.Map<ProjectAssignmentDTO>(a));

            return Ok(result);
        }

        // GET api/projects/{id}/teams
        [DeflateCompression]
        [Route("api/projects/{id:guid}/teams")]
        [ResponseType(typeof(IEnumerable<OrganisationTeamDTO>))]
        public IHttpActionResult GetTeams(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            var teams = UnitOfWork.OrganisationTeamsRepository.AllAsNoTracking
                .Where(x => x.OrganisationId == project.OrganisationId);

            var result = new List<OrganisationTeamDTO>();
            foreach (var team in teams)
            {
                foreach (var user in team.Users)
                {
                    if (user.OrgUser.Assignments.Any(a => a.ProjectId == project.Id))
                        result.Add(Mapper.Map<OrganisationTeamDTO>(team));
                }
            }

            return Ok(result.Distinct());
        }

        // POST api/projects/{id}/assign/{userId}/{accessLevel}}
        [HttpPost]
        [Route("api/projects/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult AddAssignments(Guid id, Guid userId, AccessLevels accessLevel)
        {
            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            var result = this.UnitOfWork.AssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: true);

            return Ok(Mapper.Map<ProjectAssignmentDTO>(result));
        }

        // DELETE api/projects/{id}/assign/{userId}/{accessLevel}
        [HttpDelete]
        [Route("api/projects/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult DeleteAssignments(Guid id, Guid userId, AccessLevels accessLevel)
        {
            var project = UnitOfWork.ProjectsRepository.FindIncluding(id, p => p.Assignments);
            if (project == null)
                return NotFound();

            var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == userId);
            if (assignment == null)
                return NotFound();

            var result = this.UnitOfWork.AssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: false);
            if (result != null)
                return Ok(Mapper.Map<ProjectAssignmentDTO>(result));

            return Ok(new ProjectAssignmentDTO());
        }

        // POST api/projects
        public IHttpActionResult Post([FromBody]ProjectDTO value)
        {
            var project = Mapper.Map<Project>(value);
            project.Organisation = null;

            if (this.CurrentOrgUser != null)
                project.OrganisationId = CurrentOrgUser.OrganisationId.Value;
            else
            {
                if (value.Organisation == null)
                    return BadRequest("Organisation is required");

                project.OrganisationId = Guid.Parse(value.Organisation.Id);
            }

            UnitOfWork.ProjectsRepository.InsertOrUpdate(project);
            UnitOfWork.Save();

            return Ok();
        }

        // PUT api/projects/{id}
        public IHttpActionResult Put(Guid id, [FromBody]ProjectDTO value)
        {
            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            //Mapper.Map(value, project);
            project.Number = value.Number;
            project.Name = value.Name;
            project.StartDate = value.StartDate;
            project.EndDate = value.EndDate;
            project.Notes = value.Notes;

            if (this.CurrentOrgUser == null)
            {
                if (value.Organisation == null)
                    return BadRequest("Organisation is required");

                project.OrganisationId = Guid.Parse(value.Organisation.Id);
            }

            UnitOfWork.ProjectsRepository.InsertOrUpdate(project);
            UnitOfWork.Save();

            return Ok();
        }

        // DELETE api/projects/{id}
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                UnitOfWork.ProjectsRepository.Delete(id);
                UnitOfWork.Save();

                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest("This case cannot be deleted!");
            }
        }

        // POST api/projects/{id}/create-advice-thread
        [HttpPost]
        [Route("api/projects/{id:guid}/create-advice-thread")]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult CreateAdviceThread(Guid id, [FromBody]CreateAdviceThreadDTO model)
        {
            if (id == Guid.Empty)
                return BadRequest("Project Id is empty");

            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            //var templateId = Guid.Parse("74EADB8F-7434-49C0-AD5A-854B0E77BCBD");  // first form
            var templateId = Guid.Parse("780f4d2d-524f-4714-a5b2-a43c8eaff3c3");    // advice template
            var template = UnitOfWork.FormTemplatesRepository.Find(templateId);

            //var random = new Random();
            //var title = $"{model.Title} {random.Next(1, 10000)}";
            //var color = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"

            var adviceThread = UnitOfWork.FormTemplatesRepository.CreateAdviceThread(template, this.CurrentUser.Id, model.Title, model.Colour, id);
            var returnValue = Mapper.Map<FormTemplateDTO>(adviceThread);

            // create a thread assignment for the project owner.
            if (project.CreatedById.HasValue)
            {
                var assignment = new ThreadAssignment
                {
                    CanView = true,
                    CanAdd = true,
                    CanEdit = false,
                    CanDelete = false,
                    FormTemplateId = adviceThread.Id,
                    OrgUserId = project.CreatedById.Value
                };

                UnitOfWork.ThreadAssignmentsRepository.InsertOrUpdate(assignment);
                UnitOfWork.Save();
            }

            return Ok(returnValue);
        }

    }

  
}