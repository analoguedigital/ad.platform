using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using static LightMethods.Survey.Models.DAL.AssignmentsRepository;

namespace WebApi.Controllers
{
    public class ProjectsController : BaseApiController
    {
        // GET api/<controller>
        [ResponseType(typeof(IEnumerable<ProjectDTO>))]
        public IHttpActionResult Get()
        {
            var projects = UnitOfWork.ProjectsRepository.GetProjects(CurrentUser).OrderByDescending(p => p.DateCreated);
            var result = projects.ToList().Select(p => Mapper.Map<ProjectDTO>(p)).ToList();

            return Ok(result);
        }

        // GET api/<controller>/5
        [ResponseType(typeof(ProjectDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<ProjectDTO>(new Project()));

            var project = UnitOfWork.ProjectsRepository.GetProjects(CurrentUser)
                .Where(p => p.Id == id)
                .ToList()
                .Select(p => Mapper.Map<ProjectDTO>(p))
                .SingleOrDefault();

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [Route("api/projects/{id:guid}/assignments")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult GetAssignments(Guid id)
        {
            var project = UnitOfWork.ProjectsRepository.FindIncluding(id, p => p.Assignments);
            if (project == null)
                return NotFound();

            return Ok(project.Assignments.Select(a => Mapper.Map<ProjectAssignmentDTO>(a)));
        }

        [HttpPost]
        [Route("api/projects/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult AddAssignments(Guid id, Guid userId, string accessLevel)
        {
            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            if (CurrentOrganisationId != project.OrganisationId || orgUser.OrganisationId != project.OrganisationId)
                return NotFound();

            if (string.IsNullOrEmpty(accessLevel))
                return BadRequest();

            var result = this.UnitOfWork.AssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: true);

            if (result == AssignAccessLevelResult.NotFound)
                return NotFound();

            if (result == AssignAccessLevelResult.BadRequest)
                return BadRequest();

            return Ok();
        }

        [HttpDelete]
        [Route("api/projects/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult DeleteAssignments(Guid id, Guid userId, string accessLevel)
        {
            var project = UnitOfWork.ProjectsRepository.FindIncluding(id, p => p.Assignments);
            if (project == null)
                return NotFound();

            if (CurrentOrganisationId != project.OrganisationId)
                return NotFound();

            var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == userId);
            if (assignment == null)
                return NotFound();

            if (string.IsNullOrEmpty(accessLevel))
                return BadRequest();

            var result = this.UnitOfWork.AssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: false);
            if (result == AssignAccessLevelResult.NotFound)
                return NotFound();
            if (result == AssignAccessLevelResult.BadRequest)
                return BadRequest();

            return Ok();
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]ProjectDTO value)
        {
            var project = Mapper.Map<Project>(value);
            project.OrganisationId = CurrentOrgUser.OrganisationId.Value;
            UnitOfWork.ProjectsRepository.InsertOrUpdate(project);
            UnitOfWork.Save();

            return Ok();
        }

        // PUT api/<controller>/5
        public void Put(Guid id, [FromBody]ProjectDTO value)
        {

            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return;

            Mapper.Map(value, project);

            project.OrganisationId = CurrentOrgUser.OrganisationId.Value;
            UnitOfWork.ProjectsRepository.InsertOrUpdate(project);
            UnitOfWork.Save();
        }

        // DELETE api/<controller>/5
        public void Delete(Guid id)
        {
            UnitOfWork.ProjectsRepository.Delete(id);
            UnitOfWork.Save();
        }
    }
}