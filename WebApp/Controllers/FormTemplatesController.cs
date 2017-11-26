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

namespace WebApi.Controllers
{
    public class FormTemplatesController : BaseApiController
    {
        private FormTemplatesService FormTemplatesService { get; set; }

        public FormTemplatesController()
        {
            this.FormTemplatesService = new FormTemplatesService(this.UnitOfWork, this.CurrentOrgUser);
        }

        // GET api/<controller>
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<FormTemplateDTO>))]
        public IHttpActionResult Get(Guid? projectId = null)
        {
            var response = this.FormTemplatesService.Get(projectId);
            return Ok(response);
        }

        // GET api/<controller>/5
        [DeflateCompression]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Get(Guid id)
        {
            var result = this.FormTemplatesService.Get(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

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

        // POST api/<controller>
        public IHttpActionResult Post(FormTemplateDTO value)
        {
            var formTemplate = Mapper.Map<FormTemplate>(value);

            ModelState.Clear();
            Validate(formTemplate);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

        // PUT api/<controller>/5
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


        // DELETE api/<controller>/5
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

        [HttpPost]
        [DeflateCompression]
        [Route("api/formtemplates/{id:Guid}/clone")]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Clone(Guid id, CloneReqDTO request)
        {
            var response = this.FormTemplatesService.Clone(id, request);
            return Ok(response.ReturnValue);
        }

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

    }
}