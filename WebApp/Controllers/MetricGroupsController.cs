using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class MetricGroupsController : BaseApiController
    {
        // GET: api/MetricGroups
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<MetricGroupDTO>))]
        [Route("api/formtemplates/{formTemplateId}/metricGroups")]
        public IHttpActionResult Get(Guid formTemplateId)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            return Ok(surveyProvider.GetFormTemplate(formTemplateId)
                .MetricGroups
                .Select(m => Mapper.Map<MetricGroupDTO>(m)));
        }

        // GET: api/MetricGroups/5
        [DeflateCompression]
        [ResponseType(typeof(MetricGroupDTO))]
        [Route("api/formtemplates/{formTemplateId}/metricGroups/{id}")]
        public IHttpActionResult Get(Guid formTemplateId, Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<MetricGroupDTO>(new MetricGroup() { FormTemplateId = formTemplateId }));

            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            var group = surveyProvider.GetFormTemplate(formTemplateId)
                    .MetricGroups
                    .Where(g => g.Id == id)
                    .Select(g => Mapper.Map<MetricGroupDTO>(g))
                    .SingleOrDefault();

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        // POST: api/MetricGroups
        [Route("api/formtemplates/{formTemplateId}/metricGroups")]
        public IHttpActionResult Post(Guid formTemplateId, MetricGroupDTO metricGroupDto)
        {
            var formTemplate = GetFormTemplate(formTemplateId);

            if (formTemplate == null)
                return NotFound();

            var group = Mapper.Map<MetricGroup>(metricGroupDto);
            group.FormTemplateId = formTemplateId;

            UnitOfWork.MetricGroupsRepository.InsertOrUpdate(group);
            UnitOfWork.MetricGroupsRepository.Save();

            return Ok();
        }

        // PUT: api/MetricGroups/5
        [Route("api/formtemplates/{formTemplateId}/metricGroups/{id}")]
        public IHttpActionResult Put(Guid formTemplateId, Guid id, [FromBody]MetricGroupDTO metricGroupDto)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            var group = surveyProvider.GetFormTemplate(formTemplateId)
                    .MetricGroups
                    .Where(g => g.Id == id)
                    .SingleOrDefault();

            if (group == null)
                NotFound();

            Mapper.Map(metricGroupDto, group);
            UnitOfWork.MetricGroupsRepository.Save();

            return Ok();
        }

        // DELETE: api/MetricGroups/5
        [Route("api/formtemplates/{formTemplateId}/metricGroups/{id}")]
        public IHttpActionResult Delete(Guid formTemplateId, Guid id)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            var group = surveyProvider.GetFormTemplate(formTemplateId)
                    .MetricGroups
                    .Where(g => g.Id == id)
                    .SingleOrDefault();

            if (group == null)
                NotFound();

            UnitOfWork.MetricGroupsRepository.Delete(group);
            UnitOfWork.Save();

            return Ok();
        }

        private FormTemplate GetFormTemplate(Guid id)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            return surveyProvider
                .GetAllFormTemplates()
                .SingleOrDefault(f => f.Id == id);
        }
    }
}
