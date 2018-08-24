using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.MetricFilters;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using static LightMethods.Survey.Models.DAL.ThreadAssignmentsRepository;

namespace WebApi.Controllers
{
    public class FormTemplatesController : BaseApiController
    {
        private FormTemplatesService FormTemplatesService { get; set; }

        public FormTemplatesController()
        {
            this.FormTemplatesService = new FormTemplatesService(this.UnitOfWork, this.CurrentOrgUser, this.CurrentUser);
        }

        // GET api/formTemplates
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<FormTemplateDTO>))]
        public IHttpActionResult Get(FormTemplateDiscriminators discriminator, Guid? projectId = null)
        {
            var response = this.FormTemplatesService.Get(projectId, discriminator);
            return Ok(response);
        }

        [ResponseType(typeof(IEnumerable<FormTemplateDTO>))]
        [Route("api/formtemplates/shared/{projectId:guid}")]
        public IHttpActionResult GetShared(Guid projectId)
        {
            if (this.CurrentUser is SuperUser)
                return Ok();
            
            var threadAssignments = UnitOfWork.ThreadAssignmentsRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == this.CurrentOrgUser.Id && x.FormTemplate.ProjectId == projectId)
                .ToList();

            var threads = threadAssignments.Select(x => x.FormTemplate)
                .Where(x => x.Discriminator == FormTemplateDiscriminators.RegularThread && x.CreatedById != this.CurrentOrgUser.Id)
                .ToList();

            var result = new List<FormTemplateDTO>();

            foreach(var template in threads)
            {
                var dto = Mapper.Map<FormTemplateDTO>(template);
                var assignment = this.UnitOfWork.FormTemplatesRepository.GetUserAssignment(template, this.CurrentOrgUser.Id);
                Mapper.Map(assignment, dto);

                result.Add(dto);
            }

            return Ok(result);
        }

        // GET api/formTemplates/{id}
        [DeflateCompression]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Get(Guid id)
        {
            var result = this.FormTemplatesService.Get(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // GET api/formTemplates/{id}/filters
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<MetricFilter>))]
        [Route("api/formtemplates/{id:Guid}/filters")]
        public IHttpActionResult GetFilters(Guid id)
        {
            var template = this.UnitOfWork.FormTemplatesRepository.Find(id);
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
            var template = this.UnitOfWork.FormTemplatesRepository.Find(id);
            if (template == null)
                return NotFound();

            var assignments = template.Assignments.Select(a => Mapper.Map<ThreadAssignmentDTO>(a)).ToList();
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

            var response = this.FormTemplatesService.Create(formTemplate);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return Content(System.Net.HttpStatusCode.BadGateway, response);

                return BadRequest(response.Message);
            }

            return Ok(response.ReturnValue);
        }

        // PUT api/formTemplates/{id}
        [DeflateCompression]
        [ResponseType(typeof(FormTemplateDTO))]
        public HttpResponseMessage Put(Guid id, FormTemplateDTO value)
        {
            var response = this.FormTemplatesService.Update(id, value);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);

                if (response.ValidationErrors.Any())
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, response);

                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, response.Message);
            }

            var retVal = Mapper.Map<FormTemplateDTO>(response.ReturnValue);
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, retVal);
        }

        // PUT api/formTemplates/{id}/details
        [HttpPut]
        [DeflateCompression]
        [ResponseType(typeof(FormTemplateDTO))]
        [Route("api/formtemplates/{id:Guid}/details")]
        public IHttpActionResult EditBasicDetails(Guid id, EditBasicDetailsReqDTO value)
        {
            var response = this.FormTemplatesService.UpdateBasicDetails(id, value);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return Content(System.Net.HttpStatusCode.BadRequest, response);

                return BadRequest(response.Message);
            }

            return Ok(response.ReturnValue);
        }

        // DEL api/formTemplates/{id}
        public IHttpActionResult Delete(Guid id)
        {
            var response = this.FormTemplatesService.Delete(id);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                return BadRequest(response.Message);
            }

            return Ok();
        }

        // POST api/formTemplates/{id}/clone
        [HttpPost]
        [DeflateCompression]
        [Route("api/formtemplates/{id:Guid}/clone")]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Clone(Guid id, CloneReqDTO request)
        {
            var response = this.FormTemplatesService.Clone(id, request);
            return Ok(response.ReturnValue);
        }

        // PUT api/formTemplates/{id}/publish
        [HttpPut]
        [Route("api/formtemplates/{id:Guid}/publish")]
        public IHttpActionResult Publish(Guid id)
        {
            var response = this.FormTemplatesService.Publish(id);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return Content(System.Net.HttpStatusCode.BadRequest, response);

                return BadRequest(response.Message);
            }

            return Ok();
        }

        // DEL api/formTemplates/{id}/publish
        [HttpDelete]
        [Route("api/formtemplates/{id:Guid}/publish")]
        public IHttpActionResult UndoPublish(Guid id)
        {
            var response = this.FormTemplatesService.Unpublish(id);
            if (!response.Success)
            {
                if (response.Message.ToLower() == "not found")
                    return NotFound();

                if (response.ValidationErrors.Any())
                    return Content(System.Net.HttpStatusCode.BadGateway, response);

                return BadRequest(response.Message);
            }

            return Ok();
        }

        // POST api/formTemplates/{id}/assign/{userId}/{accessLevel}
        [HttpPost]
        [Route("api/formtemplates/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ThreadAssignmentDTO>))]
        public IHttpActionResult AddAssignments(Guid id, Guid userId, ThreadAccessLevels accessLevel)
        {
            var thread = UnitOfWork.FormTemplatesRepository.Find(id);
            if (thread == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            var result = this.UnitOfWork.ThreadAssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: true);

            return Ok(Mapper.Map<ThreadAssignmentDTO>(result));
        }

        // DEL api/formTemplates/{id}/assign/{userId}/{accessLevel}
        [HttpDelete]
        [Route("api/formtemplates/{id:guid}/assign/{userId:guid}/{accessLevel}")]
        [ResponseType(typeof(IEnumerable<ThreadAssignmentDTO>))]
        public IHttpActionResult DeleteAssignments(Guid id, Guid userId, ThreadAccessLevels accessLevel)
        {
            var thread = UnitOfWork.FormTemplatesRepository.FindIncluding(id, t => t.Assignments);
            if (thread == null)
                return NotFound();

            var assignment = thread.Assignments.SingleOrDefault(a => a.OrgUserId == userId);
            if (assignment == null)
                return NotFound();

            var result = this.UnitOfWork.ThreadAssignmentsRepository.AssignAccessLevel(id, userId, accessLevel, grant: false);
            if (result != null)
                return Ok(Mapper.Map<ThreadAssignmentDTO>(result));

            return Ok(new ThreadAssignmentDTO());
        }

    }
}