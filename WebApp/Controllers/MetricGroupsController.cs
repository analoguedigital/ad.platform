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
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator")]
    public class MetricGroupsController : BaseApiController
    {

        private const string CACHE_KEY = "METRIC_GROUPS";

        // GET: api/formTemplates/{formTemplateId}/metricGroups
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<MetricGroupDTO>))]
        [Route("api/formtemplates/{formTemplateId}/metricGroups")]
        public IHttpActionResult Get(Guid formTemplateId)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            var cacheKey = $"{CACHE_KEY}_{formTemplateId}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, onlyPublished: false);
                var metricGroups = surveyProvider.GetFormTemplate(formTemplateId)
                    .MetricGroups
                    .Select(m => Mapper.Map<MetricGroupDTO>(m))
                    .ToList();

                MemoryCacher.Add(cacheKey, metricGroups, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(metricGroups);
            }
            else
            {
                var result = (List<MetricGroupDTO>)cacheEntry;
                return new CachedResult<List<MetricGroupDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET: api/api/formTemplates/{formTemplateId}/metricGroups/{id}
        [DeflateCompression]
        [ResponseType(typeof(MetricGroupDTO))]
        [Route("api/formtemplates/{formTemplateId}/metricGroups/{id}")]
        public IHttpActionResult Get(Guid formTemplateId, Guid id)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            if (id == Guid.Empty)
                return Ok(Mapper.Map<MetricGroupDTO>(new MetricGroup() { FormTemplateId = formTemplateId }));

            var cacheKey = $"{CACHE_KEY}_{formTemplateId}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, onlyPublished: false);
                var group = surveyProvider.GetFormTemplate(formTemplateId)
                        .MetricGroups
                        .Where(g => g.Id == id)
                        .Select(g => Mapper.Map<MetricGroupDTO>(g))
                        .SingleOrDefault();

                if (group == null)
                    return NotFound();

                MemoryCacher.Add(cacheKey, group, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(group);
            }
            else
            {
                var result = (MetricGroupDTO)cacheEntry;
                return new CachedResult<MetricGroupDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/formTemplates/{formTemplateId}/metricGroups
        [Route("api/formtemplates/{formTemplateId}/metricGroups")]
        public IHttpActionResult Post(Guid formTemplateId, MetricGroupDTO metricGroupDto)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            var formTemplate = GetFormTemplate(formTemplateId);
            if (formTemplate == null)
                return NotFound();

            var group = Mapper.Map<MetricGroup>(metricGroupDto);
            group.FormTemplateId = formTemplateId;

            try
            {
                UnitOfWork.MetricGroupsRepository.InsertOrUpdate(group);
                UnitOfWork.MetricGroupsRepository.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        // PUT: api/formTemplates/{formTemplateId}/metricGroups/{id}
        [Route("api/formtemplates/{formTemplateId}/metricGroups/{id}")]
        public IHttpActionResult Put(Guid formTemplateId, Guid id, [FromBody]MetricGroupDTO metricGroupDto)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            if (id == Guid.Empty)
                return BadRequest("metric group id is empty");

            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, onlyPublished: false);

            var group = surveyProvider.GetFormTemplate(formTemplateId)
                    .MetricGroups
                    .Where(g => g.Id == id)
                    .SingleOrDefault();

            if (group == null)
                return NotFound();

            try
            {
                Mapper.Map(metricGroupDto, group);
                UnitOfWork.MetricGroupsRepository.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/formTemplates/{formTemplateId}/metricGroups/{id}
        [Route("api/formtemplates/{formTemplateId}/metricGroups/{id}")]
        public IHttpActionResult Delete(Guid formTemplateId, Guid id)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            if (id == Guid.Empty)
                return BadRequest("metric group id is empty");

            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

            var group = surveyProvider.GetFormTemplate(formTemplateId)
                    .MetricGroups
                    .Where(g => g.Id == id)
                    .SingleOrDefault();

            if (group == null)
                return NotFound();

            try
            {
                UnitOfWork.MetricGroupsRepository.Delete(group);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #region Helpers

        private FormTemplate GetFormTemplate(Guid id)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, onlyPublished: false);

            return surveyProvider
                .GetAllFormTemplates()
                .SingleOrDefault(f => f.Id == id);
        }

        #endregion

    }
}
