using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.MetricFilters;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Results;
using WebApi.Services;
using static LightMethods.Survey.Models.DAL.ThreadAssignmentsRepository;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator,Organisation user")]
    public class FormTemplatesController : BaseApiController
    {

        #region Properties

        private const string CACHE_KEY = "FORM_TEMPLATES";
        private const string ADMIN_KEY = "ADMIN";
        private const string ORG_ADMIN_KEY = "ORG_ADMIN";

        private FormTemplatesService FormTemplatesService { get; set; }

        #endregion Properties

        #region C-tor

        public FormTemplatesController()
        {
            FormTemplatesService = new FormTemplatesService(UnitOfWork, CurrentOrgUser, CurrentUser);
        }

        #endregion C-tor

        #region CRUD

        // GET api/formTemplates/{discriminator}/{projectId?}
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<FormTemplateDTO>))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user,Restricted user")]
        public async Task<IHttpActionResult> Get(FormTemplateDiscriminators discriminator, Guid? projectId = null)
        {
            if (projectId.HasValue && projectId == Guid.Empty)
                return BadRequest("project id is empty");

            if (CurrentUser is OrgUser)
            {
                var cacheKey = string.Empty;
                var orgAdminRole = "Organisation administrator";

                var isOrgAdmin = await ServiceContext.UserManager.IsInRoleAsync(CurrentOrgUser.Id, orgAdminRole);
                if (isOrgAdmin)
                {
                    cacheKey = projectId.HasValue ?
                        $"{CACHE_KEY}_{ORG_ADMIN_KEY}_{discriminator}_{projectId}_{CurrentOrgUser.Id}" :
                        $"{CACHE_KEY}_{ORG_ADMIN_KEY}_{discriminator}_{CurrentOrgUser.Id}";
                }
                else
                {
                    cacheKey = projectId.HasValue ?
                        $"{CACHE_KEY}_{discriminator}_{projectId}_{CurrentOrgUser.Id}" :
                        $"{CACHE_KEY}_{discriminator}_{CurrentOrgUser.Id}";
                }

                var cacheEntry = MemoryCacher.GetValue(cacheKey);
                if (cacheEntry == null)
                {
                    var templates = FormTemplatesService.Get(projectId, discriminator).ToList();
                    MemoryCacher.Add(cacheKey, templates, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(templates);
                }
                else
                {
                    var result = (List<FormTemplateDTO>)cacheEntry;
                    return new CachedResult<List<FormTemplateDTO>>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser
            var _cacheKey = projectId.HasValue ?
                $"{CACHE_KEY}_{ADMIN_KEY}_{discriminator}_{projectId}" :
                $"{CACHE_KEY}_{ADMIN_KEY}_{discriminator}";

            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);
            if (_cacheEntry == null)
            {
                var templates = FormTemplatesService.Get(projectId, discriminator).ToList();
                MemoryCacher.Add(_cacheKey, templates, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(templates);
            }
            else
            {
                var response = (List<FormTemplateDTO>)_cacheEntry;
                return new CachedResult<List<FormTemplateDTO>>(response, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/formTemplates/shared/{projectId}
        [ResponseType(typeof(IEnumerable<FormTemplateDTO>))]
        [Route("api/formtemplates/shared/{projectId:guid}")]
        [OverrideAuthorization]
        [Authorize(Roles = "Organisation user,Restricted user")]
        public IHttpActionResult GetShared(Guid projectId)
        {
            if (projectId == Guid.Empty)
                return BadRequest("project id is empty");

            var cacheKey = $"{CACHE_KEY}_SHARED_{projectId}_{CurrentOrgUser.Id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var threadAssignments = UnitOfWork.ThreadAssignmentsRepository
                    .AllAsNoTracking
                    .Where(x => x.OrgUserId == CurrentOrgUser.Id && x.FormTemplate.ProjectId == projectId)
                    .ToList();

                var threads = threadAssignments.Select(x => x.FormTemplate)
                    .Where(x => x.Discriminator == FormTemplateDiscriminators.RegularThread && x.CreatedById != CurrentOrgUser.Id)
                    .ToList();

                var result = new List<FormTemplateDTO>();

                foreach (var template in threads)
                {
                    var dto = Mapper.Map<FormTemplateDTO>(template);
                    var assignment = UnitOfWork.FormTemplatesRepository.GetUserAssignment(template, CurrentOrgUser.Id);
                    Mapper.Map(assignment, dto);

                    result.Add(dto);
                }

                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (List<FormTemplateDTO>)cacheEntry;
                return new CachedResult<List<FormTemplateDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/formTemplates/{id}
        [DeflateCompression]
        [ResponseType(typeof(FormTemplateDTO))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user,Restricted user")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            // not necessary to validate empty IDs,
            // because the FormTemplateService method
            // returns an empty FormTemplateDTO.
            //if (id == Guid.Empty)
            //    return BadRequest("id is empty");

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
                    var template = FormTemplatesService.Get(id);
                    if (template == null)
                        return NotFound();

                    MemoryCacher.Add(cacheKey, template, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(template);
                }
                else
                {
                    var result = (FormTemplateDTO)cacheEntry;
                    return new CachedResult<FormTemplateDTO>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser
            var _cacheKey = $"{CACHE_KEY}_{ADMIN_KEY}_{id}";
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var template = FormTemplatesService.Get(id);
                if (template == null)
                    return NotFound();

                MemoryCacher.Add(_cacheKey, template, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(template);
            }
            else
            {
                var response = (FormTemplateDTO)_cacheEntry;
                return new CachedResult<FormTemplateDTO>(response, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/formTemplates/{id}/filters
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<MetricFilter>))]
        [Route("api/formtemplates/{id:Guid}/filters")]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user,Restricted user")]
        public IHttpActionResult GetFilters(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var template = UnitOfWork.FormTemplatesRepository.Find(id);
            if (template == null)
                return NotFound();

            var metricFilters = template.GetMetricFilters();
            return Ok(metricFilters);
        }

        // GET api/formTemplates/{id}/assignments
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<ThreadAssignmentDTO>))]
        [Route("api/formtemplates/{id:guid}/assignments")]
        public IHttpActionResult GetAssignments(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var template = UnitOfWork.FormTemplatesRepository.Find(id);
            if (template == null)
                return NotFound();

            var assignments = template.Assignments
                .Select(a => Mapper.Map<ThreadAssignmentDTO>(a))
                .ToList();

            return Ok(assignments);
        }

        // POST api/formTemplates
        [HttpPost]
        public IHttpActionResult Post(FormTemplateDTO value)
        {
            var formTemplate = Mapper.Map<FormTemplate>(value);

            ModelState.Clear();
            Validate(formTemplate);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            formTemplate.OrganisationId = Guid.Parse(value.Organisation.Id);
            formTemplate.ProjectId = value.Project.Id;
            formTemplate.Discriminator = FormTemplateDiscriminators.RegularThread;

            var response = FormTemplatesService.Create(formTemplate);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return Content(HttpStatusCode.BadRequest, response);

                return BadRequest(response.Message);
            }

            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok(response.ReturnValue);
        }

        // PUT api/formTemplates/{id}
        [DeflateCompression]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Put(Guid id, FormTemplateDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var response = FormTemplatesService.Update(id, value);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return BadRequest(response.Message);

                return BadRequest(response.Message);
            }

            var retVal = Mapper.Map<FormTemplateDTO>(response.ReturnValue);
            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok(retVal);
        }

        // DEL api/formTemplates/{id}
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var response = FormTemplatesService.Delete(id);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                return BadRequest(response.Message);
            }

            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok();
        }

        // DEL api/formTemplates/force-delete/{id}
        [HttpPost]
        [Route("api/formtemplates/{id:guid}/force-delete")]
        public IHttpActionResult ForceDelete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var response = FormTemplatesService.ForceDelete(id);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                return BadRequest(response.Message);
            }

            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok();
        }

        // PUT api/formTemplates/{id}/details
        [HttpPut]
        [DeflateCompression]
        [ResponseType(typeof(FormTemplateDTO))]
        [Route("api/formtemplates/{id:Guid}/details")]
        public IHttpActionResult EditBasicDetails(Guid id, EditBasicDetailsReqDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var response = FormTemplatesService.UpdateBasicDetails(id, value);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return Content(System.Net.HttpStatusCode.BadRequest, response);

                return BadRequest(response.Message);
            }

            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok(response.ReturnValue);
        }

        // POST api/formTemplates/{id}/clone
        [HttpPost]
        [DeflateCompression]
        [Route("api/formtemplates/{id:Guid}/clone")]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Clone(Guid id, CloneReqDTO request)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var response = FormTemplatesService.Clone(id, request);
            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok(response.ReturnValue);
        }

        #endregion CRUD

        #region Publish/Unpublish

        // PUT api/formTemplates/{id}/publish
        [HttpPut]
        [Route("api/formtemplates/{id:Guid}/publish")]
        public IHttpActionResult Publish(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var response = FormTemplatesService.Publish(id);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return Content(System.Net.HttpStatusCode.BadRequest, response);

                return BadRequest(response.Message);
            }

            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok();
        }

        // DEL api/formTemplates/{id}/publish
        [HttpDelete]
        [Route("api/formtemplates/{id:Guid}/publish")]
        public IHttpActionResult UndoPublish(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var response = FormTemplatesService.Unpublish(id);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return Content(System.Net.HttpStatusCode.BadGateway, response);

                return BadRequest(response.Message);
            }

            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok();
        }

        #endregion Publish/Unpublish

        #region Thread Assignments

        // POST api/formTemplates/{id}/assign/{userId}/{accessLevel}
        [HttpPost]
        [Route("api/formtemplates/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ThreadAssignmentDTO>))]
        public IHttpActionResult AddAssignments(Guid id, Guid userId, ThreadAccessLevels accessLevel)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var thread = UnitOfWork.FormTemplatesRepository.Find(id);
            if (thread == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            var result = UnitOfWork.ThreadAssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: true);

            return Ok(Mapper.Map<ThreadAssignmentDTO>(result));
        }

        // DEL api/formTemplates/{id}/assign/{userId}/{accessLevel}
        [HttpDelete]
        [Route("api/formtemplates/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ThreadAssignmentDTO>))]
        public IHttpActionResult DeleteAssignments(Guid id, Guid userId, ThreadAccessLevels accessLevel)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var thread = UnitOfWork.FormTemplatesRepository.FindIncluding(id, t => t.Assignments);
            if (thread == null)
                return NotFound();

            var assignment = thread.Assignments.SingleOrDefault(a => a.OrgUserId == userId);
            if (assignment == null)
                return NotFound();

            var result = UnitOfWork.ThreadAssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: false);
            if (result != null)
                return Ok(Mapper.Map<ThreadAssignmentDTO>(result));

            return Ok(new ThreadAssignmentDTO());
        }

        #endregion Thread Assignments

    }
}