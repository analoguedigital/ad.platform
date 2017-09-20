using AutoMapper;
using AutoMapper.QueryableExtensions;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Models;
using Newtonsoft.Json;
using LightMethods.Survey.Models.MetricFilters;

namespace WebApi.Controllers
{
    public class FormTemplatesController : BaseApiController
    {
        // GET api/<controller>
        [ResponseType(typeof(IEnumerable<FormTemplateDTO>))]
        public IHttpActionResult Get(Guid? projectId = null)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);
            var templates = surveyProvider.GetAllFormTemplates()
                .Where(t => t.ProjectId == null || t.ProjectId == projectId);
            var result = templates.Select(t => Mapper.Map<FormTemplateDTO>(t));

            return Ok(result);
        }

        // GET api/<controller>/5
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<FormTemplateDTO>(new FormTemplate() { }));

            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            var form = surveyProvider.GetAllFormTemplatesWithMetrics()
                .Where(f => f.Id == id)
                .Select(f => Mapper.Map<FormTemplateDTO>(f))
                .SingleOrDefault();

            if (form == null)
                return NotFound();

            return Ok(form);
        }

        [ResponseType(typeof(IEnumerable<MetricFilter>))]
        [Route("api/formtemplates/{id:Guid}/filters")]
        public IHttpActionResult GetFilters(Guid id)
        {
            var template = this.UnitOfWork.FormTemplatesRepository.Find(id);
            if (template != null)
            {
                var metricFilters = template.GetMetricFilters();
                return Ok(metricFilters);
            }

            return BadRequest("FormTemplate not found!");
        }

        // POST api/<controller>
        public IHttpActionResult Post(FormTemplateDTO value)
        {
            var formTemplate = Mapper.Map<FormTemplate>(value);

            // Prevent reinserting the existing formTemplateCategory
            formTemplate.FormTemplateCategory = null;
            formTemplate.CreatedById = CurrentUser.Id;
            formTemplate.OrganisationId = CurrentOrganisationId.Value;
            //formTemplate.ProjectId = UnitOfWork.ProjectsRepository.All.Where(p => p.OrganisationId == CurrentOrgUser.OrganisationId).FirstOrDefault().Id;

            ModelState.Clear();
            Validate(formTemplate);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            UnitOfWork.FormTemplatesRepository.InsertOrUpdate(formTemplate);
            UnitOfWork.Save();

            return Ok();
        }

        // PUT api/<controller>/5
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Put(Guid id, FormTemplateDTO value)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            var form = surveyProvider.GetAllFormTemplatesWithMetrics()
                .Where(f => f.Id == value.Id)
                .SingleOrDefault();

            if (form == null)
                return NotFound();

            Mapper.Map(value, form);

            UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);
            var groupOrder = 1;
            foreach (var valueGroup in value.MetricGroups)
            {
                var group = form.MetricGroups.SingleOrDefault(g => g.Id == valueGroup.Id);
                if (valueGroup.isDeleted && group != null)
                {
                    if (valueGroup.Metrics.Any(m => !m.isDeleted))
                    {
                        ModelState.AddModelError(group.Id.ToString(), $"Group {group.Title} is not empty!");
                        return BadRequest(ModelState);
                    }
                }

                group = valueGroup.Map(group, UnitOfWork, CurrentOrganisation);
                group.FormTemplateId = form.Id;
                group.Order = groupOrder++;

                var metricOrder = 1;
                foreach (var valueMetric in valueGroup.Metrics)
                {
                    var metric = group.Metrics.Where(m => m.Id == valueMetric.Id)
                        // .Select(m => Mapper.Map(valueMetric, m))
                        .SingleOrDefault();

                    if (metric == null && valueMetric.isDeleted)
                        continue;

                    metric = valueMetric.Map(metric, UnitOfWork, CurrentOrganisation);

                    if (valueMetric.isDeleted) // Delete
                    {
                        UnitOfWork.MetricsRepository.Delete(metric);
                    }
                    else
                    {   // Insert or update
                        metric.FormTemplateId = form.Id;
                        metric.MetricGroup = group;
                        metric.Order = metricOrder++;

                        // Validate metric
                        var validationResult = metric.Validate();
                        if (validationResult.Any())
                        {
                            validationResult.ToList().ForEach(res => ModelState.AddModelError(metric.Id.ToString(), res.ErrorMessage));
                            return BadRequest(ModelState);
                        }

                        UnitOfWork.MetricsRepository.InsertOrUpdate(metric);
                    }
                }

                if (valueGroup.isDeleted && group != null)
                {
                    UnitOfWork.MetricGroupsRepository.Delete(group);
                }
                else
                {
                    UnitOfWork.MetricGroupsRepository.InsertOrUpdate(group);
                }
            }

            try
            {
                ModelState.Clear();

                var result = form.Validate(new ValidationContext(form));
                if (result.Any())
                {
                    result.ToList().ForEach(res => ModelState.AddModelError(id.ToString(), res.ErrorMessage));
                    return BadRequest(ModelState);
                }

                UnitOfWork.Save();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                        ModelState.AddModelError(
                            (entityError.Entry.Entity as Entity)?.Id.ToString() ?? string.Empty,
                            validationError.ErrorMessage
                            );
                }
                return BadRequest(ModelState);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ModelState.AddModelError(form.Id.ToString(), ex);
                return BadRequest(ModelState);
            }

            return Ok(Mapper.Map<FormTemplateDTO>(form));
        }

        [HttpPut]
        [ResponseType(typeof(FormTemplateDTO))]
        [Route("api/formtemplates/{id:Guid}/details")]
        public IHttpActionResult EditBasicDetails(Guid id, EditBasicDetailsRequest value)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            var form = surveyProvider.GetAllFormTemplatesWithMetrics()
                .Where(f => f.Id == id)
                .SingleOrDefault();

            if (form == null)
                return NotFound();

            Mapper.Map(value, form);
            UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);

            try
            {
                ModelState.Clear();
                UnitOfWork.Save();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                        ModelState.AddModelError(
                            (entityError.Entry.Entity as Entity)?.Id.ToString() ?? string.Empty,
                            validationError.ErrorMessage
                            );
                }
                return BadRequest(ModelState);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ModelState.AddModelError(form.Id.ToString(), ex);
                return BadRequest(ModelState);
            }

            return Ok(Mapper.Map<FormTemplateDTO>(form));
        }


        // DELETE api/<controller>/5
        public IHttpActionResult Delete(Guid id)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            var form = surveyProvider.GetAllFormTemplatesWithMetrics()
                .Where(f => f.Id == id)
                .SingleOrDefault();

            if (form == null)
                return NotFound();

            UnitOfWork.FormTemplatesRepository.Delete(form);
            UnitOfWork.Save();

            return Ok();
        }

        [HttpPost]
        [Route("api/formtemplates/{id:Guid}/clone")]
        [ResponseType(typeof(FormTemplateDTO))]
        public IHttpActionResult Clone(Guid id, CloneRequest request)
        {
            var template = UnitOfWork.FormTemplatesRepository.Find(id);
            var clone = UnitOfWork.FormTemplatesRepository.Clone(template, CurrentUser as OrgUser, request.Title, request.Colour, request.ProjectId);
            return Ok(Mapper.Map<FormTemplateDTO>(clone));
        }

        [HttpPut]
        [Route("api/formtemplates/{id:Guid}/publish")]
        public IHttpActionResult Publish(Guid id)
        {
            var template = UnitOfWork.FormTemplatesRepository.Find(id);
            if (template == null)
                return NotFound();

            var result = template.Publish();
            if (result.Any())
            {
                result.ToList().ForEach(res => ModelState.AddModelError(id.ToString(), res.ErrorMessage));
                return BadRequest(ModelState);
            }

            UnitOfWork.FormTemplatesRepository.InsertOrUpdate(template);
            UnitOfWork.Save();
            return Ok();
        }

        [HttpDelete]
        [Route("api/formtemplates/{id:Guid}/publish")]
        public IHttpActionResult UndoPublish(Guid id)
        {
            var template = UnitOfWork.FormTemplatesRepository.Find(id);
            if (template == null)
                return NotFound();

            template.IsPublished = false;
            UnitOfWork.FormTemplatesRepository.InsertOrUpdate(template);
            UnitOfWork.Save();
            return Ok();
        }

        public class CloneRequest
        {
            public string Title { get; set; }
            public string Colour { get; set; }
            public Guid? ProjectId { get; set; }
        }

        public class EditBasicDetailsRequest
        {
            public string Code { get; set; }
            public string Title { get; set; }
            public double Version { get; set; }
            public string Description { set; get; }
            public string Colour { get; set; }
        }
    }
}