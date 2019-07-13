using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.DTO.FormTemplates;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Results;
using WebApi.Services;
using static LightMethods.Survey.Models.DAL.AssignmentsRepository;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator,Organisation administrator")]
    public class ProjectsController : BaseApiController
    {

        #region Properties

        private const string CACHE_KEY = "PROJECTS";
        private const string ADMIN_KEY = "ADMIN";
        private const string ORG_ADMIN_KEY = "ORG_ADMIN";

        private ProjectsService ProjectService { get; set; }

        #endregion Properties

        public ProjectsController()
        {
            ProjectService = new ProjectsService(UnitOfWork);
        }

        #region CRUD

        // GET api/projects
        [ResponseType(typeof(IEnumerable<ProjectDTO>))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation user,Restricted user")]
        public async Task<IHttpActionResult> Get(Guid? organisationId = null)
        {
            if (CurrentUser is OrgUser)
            {
                var isOrgAdmin = await ServiceContext.UserManager.IsInRoleAsync(CurrentOrgUser.Id, Role.ORG_ADMINSTRATOR);

                var _cacheKey = isOrgAdmin ?
                    $"{CACHE_KEY}_{ORG_ADMIN_KEY}_{CurrentOrgUser.OrganisationId}" :
                    $"{CACHE_KEY}_{CurrentOrgUser.OrganisationId}_{CurrentOrgUser.Id}";

                var _cacheEntry = MemoryCacher.GetValue(_cacheKey);
                if (_cacheEntry == null)
                {
                    var values = ProjectService.Get(CurrentOrgUser, organisationId: null);
                    MemoryCacher.Add(_cacheKey, values, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(values);
                }
                else
                {
                    var values = (List<ProjectDTO>)_cacheEntry;
                    return new CachedResult<List<ProjectDTO>>(values, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if CurrentUser is SuperUser
            var cacheKey = organisationId.HasValue ?
                $"{CACHE_KEY}_{ADMIN_KEY}_{organisationId.Value}" :
                $"{CACHE_KEY}_{ADMIN_KEY}";

            var cacheEntry = MemoryCacher.GetValue(cacheKey);
            if (cacheEntry == null)
            {
                //var values = ProjectService.Get(CurrentUser, organisationId);
                var values = ProjectService.GetProjectsForAdmin(organisationId);
                MemoryCacher.Add(cacheKey, values, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(values);
            }
            else
            {
                var values = (List<ProjectFlatDTO>)cacheEntry;
                return new CachedResult<List<ProjectFlatDTO>>(values, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/projects/{id}
        [DeflateCompression]
        [ResponseType(typeof(ProjectDTO))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user,Restricted user")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<ProjectDTO>(new Project()));

            if (CurrentUser is OrgUser)
            {
                var orgAdminRole = "Organisation administrator";
                var isOrgAdmin = await ServiceContext.UserManager.IsInRoleAsync(CurrentOrgUser.Id, orgAdminRole);

                var cacheKey = isOrgAdmin ?
                    $"{CACHE_KEY}_{ORG_ADMIN_KEY}_{id}_{CurrentOrgUser.Id}" :
                    $"{CACHE_KEY}_{id}_{CurrentOrgUser.Id}";

                var cacheEntry = MemoryCacher.GetValue(cacheKey);
                if (cacheEntry == null)
                {
                    var project = ProjectService.Get(CurrentUser, id);
                    if (project == null)
                        return NotFound();

                    MemoryCacher.Add(cacheKey, project, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(project);
                }
                else
                {
                    var result = (ProjectDTO)cacheEntry;
                    return new CachedResult<ProjectDTO>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser
            var _cacheKey = $"{CACHE_KEY}_{ADMIN_KEY}_{id}";
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var project = ProjectService.Get(CurrentUser, id);
                if (project == null)
                    return NotFound();

                MemoryCacher.Add(_cacheKey, project, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(project);
            }
            else
            {
                var response = (ProjectDTO)_cacheEntry;
                return new CachedResult<ProjectDTO>(response, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/projects/shared
        [ResponseType(typeof(IEnumerable<ProjectDTO>))]
        [Route("api/projects/shared")]
        [OverrideAuthorization]
        [Authorize(Roles = "Organisation user,Restricted user")]
        public IHttpActionResult GetSharedProjects()
        {
            var cacheKey = $"{CACHE_KEY}_SHARED_{CurrentOrgUser.Id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var result = ProjectService.GetSharedProjects(CurrentOrgUser);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (List<ProjectDTO>)cacheEntry;
                return new CachedResult<List<ProjectDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        [ResponseType(typeof(ProjectDTO))]
        [Route("api/projects/direct/{id:guid}")]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation user,Restricted user")]
        public IHttpActionResult GetDirect(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var result = ProjectService.GetDirect(CurrentUser, id);
                if (result == null)
                    return NotFound();

                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (ProjectDTO)cacheEntry;
                return new CachedResult<ProjectDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/projects/{id}/teams
        [DeflateCompression]
        [Route("api/projects/{id:guid}/teams")]
        [ResponseType(typeof(IEnumerable<OrganisationTeamDTO>))]
        public IHttpActionResult GetTeams(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var cacheKey = $"{CACHE_KEY}_TEAMS_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var project = UnitOfWork.ProjectsRepository.Find(id);
                if (project == null)
                    return NotFound();

                var projectTeams = ProjectService.GetTeams(project);

                return Ok(projectTeams);
            }
            else
            {
                var result = (List<OrganisationTeamDTO>)cacheEntry;
                return new CachedResult<List<OrganisationTeamDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/projects
        public IHttpActionResult Post([FromBody]ProjectDTO value)
        {
            var project = Mapper.Map<Project>(value);
            project.Organisation = null;

            if (CurrentUser is OrgUser)
                project.OrganisationId = CurrentOrgUser.OrganisationId.Value;
            else if (CurrentUser is SuperUser)
            {
                if (value.Organisation == null)
                    return BadRequest("Organisation is required");

                project.OrganisationId = Guid.Parse(value.Organisation.Id);
            }

            project.CreatedById = CurrentUser.Id;
            project.IsAggregate = true; // projects created by admins, are aggregate cases (shared, groups)

            try
            {
                UnitOfWork.ProjectsRepository.InsertOrUpdate(project);

                // no need to create an assignment for the OrgAdmin.
                // OrgAdmins have access to all projects under their organization.
                //if (CurrentUser is OrgUser)
                //{
                //    // when OrgAdmins create new projects,
                //    // assign them with full access.
                //    var orgAdminAssignment = new Assignment
                //    {
                //        OrgUserId = CurrentUser.Id,
                //        ProjectId = project.Id,
                //        CanView = true,
                //        CanAdd = true,
                //        CanEdit = true,
                //        CanDelete = true,
                //        CanExportPdf = true,
                //        CanExportZip = true
                //    };

                //    UnitOfWork.AssignmentsRepository.InsertOrUpdate(orgAdminAssignment);
                //}

                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/projects/{id}
        public IHttpActionResult Put(Guid id, [FromBody]ProjectDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            project.Number = value.Number;
            project.Name = value.Name;
            project.StartDate = value.StartDate;
            project.EndDate = value.EndDate;
            project.Notes = value.Notes;

            if (CurrentUser is SuperUser)
            {
                if (value.Organisation == null)
                    return BadRequest("Organisation is required");

                project.OrganisationId = Guid.Parse(value.Organisation.Id);
            }

            try
            {
                UnitOfWork.ProjectsRepository.InsertOrUpdate(project);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/projects/{id}
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            try
            {
                UnitOfWork.ProjectsRepository.Delete(id);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (DbUpdateException dbEx)
            {
                return InternalServerError(dbEx);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion CRUD

        #region Assignments

        // POST api/projects/{id}/assign/{userId}/{accessLevel}}
        [HttpPost]
        [Route("api/projects/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult AddAssignments(Guid id, Guid userId, AccessLevels accessLevel)
        {
            if (id == Guid.Empty)
                return BadRequest("project id is empty");

            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            // reject client to client assignments
            bool isCaseOwnerAClient = project.CreatedBy is OrgUser caseOwner && caseOwner.AccountType == AccountType.MobileAccount;
            if (orgUser.AccountType == AccountType.MobileAccount && !project.IsAggregate && isCaseOwnerAClient)
            {
                return BadRequest("Client to client assignments are not allowed");
            }

            var assignment = UnitOfWork.AssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: true);
            var result = Mapper.Map<ProjectAssignmentDTO>(assignment);

            MemoryCacher.DeleteStartingWith("ORG_TEAMS_ASSIGNMENTS");
            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok(result);
        }

        // DELETE api/projects/{id}/assign/{userId}/{accessLevel}
        [HttpDelete]
        [Route("api/projects/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult DeleteAssignments(Guid id, Guid userId, AccessLevels accessLevel)
        {
            if (id == Guid.Empty)
                return BadRequest("project id is empty");

            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var project = UnitOfWork.ProjectsRepository.FindIncluding(id, p => p.Assignments);
            if (project == null)
                return NotFound();

            var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == userId);
            if (assignment == null)
                return NotFound();

            var updatedAssignment = UnitOfWork.AssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: false);
            if (updatedAssignment != null)
                return Ok(Mapper.Map<ProjectAssignmentDTO>(updatedAssignment));

            MemoryCacher.DeleteStartingWith("ORG_TEAMS_ASSIGNMENTS");
            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok(new ProjectAssignmentDTO());
        }

        // GET api/projects/{id}/assignments
        [DeflateCompression]
        [Route("api/projects/{id:guid}/assignments")]
        [ResponseType(typeof(IEnumerable<ProjectAssignmentDTO>))]
        public IHttpActionResult GetAssignments(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("project id is empty");

            var project = UnitOfWork.ProjectsRepository.FindIncluding(id, p => p.Assignments);
            if (project == null)
                return NotFound();

            var assignments = project.Assignments;

            if (CurrentUser is OrgUser)
            {
                // if an OrgAdmin is requesting this,
                // filter the assignments to the current organization.
                assignments = assignments
                    .Where(x => x.OrgUser.Organisation.Id == CurrentOrgUser.Organisation.Id)
                    .ToList();
            }

            var result = assignments
                .Select(a => Mapper.Map<ProjectAssignmentDTO>(a))
                .ToList();

            foreach (var item in result)
            {
                if (project.CreatedBy != null && project.CreatedBy.Id == item.OrgUserId)
                    item.IsOwner = true;
            }

            return Ok(result);
        }

        #endregion Assignments

        #region Advice and Record Threads

        // POST api/projects/{id}/create-advice-thread
        [HttpPost]
        [Route("api/projects/{id:guid}/create-advice-thread")]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult CreateAdviceThread(Guid id, [FromBody]CreateAdviceThreadDTO model)
        {
            if (id == Guid.Empty)
                return BadRequest("project id is empty");

            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            //var templateId = Guid.Parse("74EADB8F-7434-49C0-AD5A-854B0E77BCBD");  // first form
            var templateId = Guid.Parse("780f4d2d-524f-4714-a5b2-a43c8eaff3c3");    // advice template
            var template = UnitOfWork.FormTemplatesRepository.Find(templateId);

            //var random = new Random();
            //var title = $"{model.Title} {random.Next(1, 10000)}";
            //var color = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"

            var adviceThread = UnitOfWork.FormTemplatesRepository.CreateAdviceThread(template, CurrentUser.Id, model.Title, model.Colour, id, project.OrganisationId);
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
            }

            try
            {
                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith("FORM_TEMPLATES");
                return Ok(returnValue);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/projects/{id:guid}/create-record-thread")]
        public IHttpActionResult CreateRecordThread(Guid id, [FromBody]CreateRecordThreadDTO model)
        {
            if (id == Guid.Empty)
                return BadRequest("project id is empty");

            var project = UnitOfWork.ProjectsRepository.Find(id);
            if (project == null)
                return NotFound();

            var templateId = Guid.Parse("74EADB8F-7434-49C0-AD5A-854B0E77BCBD");    // seed template
            var template = UnitOfWork.FormTemplatesRepository.Find(templateId);

            //var formTemplateService = new FormTemplatesService(UnitOfWork, CurrentOrgUser, CurrentUser);
            //var result = formTemplateService.Clone(templateId, new CloneReqDTO
            //{
            //    Code = model.Code,
            //    Title = model.Title,
            //    Description = model.Description,
            //    Colour = model.Colour,
            //    ProjectId = id
            //});

            try
            {
                var newThread = UnitOfWork.FormTemplatesRepository.CreateRegularThread(template, CurrentUser.Id, model.Title, model.Description, model.Colour, id, project.OrganisationId);
                var returnValue = Mapper.Map<FormTemplateDTO>(newThread);

                MemoryCacher.DeleteStartingWith("FORM_TEMPLATES");
                return Ok(returnValue);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion Advice and Record Threads

        [Route("api/projects/user/{orgUserId:guid}")]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator,Organisation team manager")]
        public IHttpActionResult GetProjectByUserId(Guid orgUserId)
        {
            if (orgUserId == Guid.Empty)
                return BadRequest();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(orgUserId);
            if (orgUser == null)
                return NotFound();

            ProjectDTO result = new ProjectDTO();

            if (orgUser.CurrentProjectId.HasValue)
                result = ProjectService.GetDirect(orgUser, orgUser.CurrentProjectId.Value);
            else
            {
                if (orgUser.Assignments.Count == 1)
                    result = ProjectService.GetDirect(orgUser, orgUser.Assignments.First().ProjectId);
            }

            return Ok(result);
        }

    }


}