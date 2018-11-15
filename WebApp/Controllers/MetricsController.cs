using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator")]
    public class MetricsController : BaseApiController
    {

        private const string CACHE_KEY = "METRICS";

        // GET api/metrics/{metricType}
        [HttpGet]
        [ResponseType(typeof(MetricDTO))]
        [Route("api/metrics/{metricType}")]
        public IHttpActionResult Get(string metricType)
        {
            if (string.IsNullOrEmpty(metricType))
                return BadRequest();

            metricType = metricType.ToLower();
            Metric metric = null;

            if (metricType == "numericmetric") metric = new NumericMetric();
            else if (metricType == "freetextmetric") metric = new FreeTextMetric();
            else if (metricType == "ratemetric") metric = new RateMetric();
            else if (metricType == "datemetric") metric = new DateMetric();
            else if (metricType == "timemetric") metric = new TimeMetric();
            else if (metricType == "multiplechoicemetric") metric = new MultipleChoiceMetric();
            else if (metricType == "dichotomousmetric") metric = new DichotomousMetric();
            else if (metricType == "attachmentmetric") metric = new AttachmentMetric();

            if (metric == null)
                return NotFound();

            var result = Mapper.Map(metric, metric.GetType(), typeof(MetricDTO));

            return Ok(result);
        }

        // GET api/formTemplates/{formTemplateId}/metrics/{id}
        [DeflateCompression]
        [ResponseType(typeof(MetricDTO))]
        [Route("api/formtemplates/{formTemplateId:Guid}/metrics/{id:Guid}")]
        public IHttpActionResult Get(Guid formTemplateId, Guid id)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            if (id == Guid.Empty)
                return BadRequest("metric id is empty");

            var cacheKey = $"{CACHE_KEY}_{formTemplateId}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var metric = GetMetric(formTemplateId, id);
                if (metric == null)
                    return NotFound();

                var result = Mapper.Map(metric, metric.GetType(), typeof(MetricDTO));
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                return new CachedResult<object>(cacheEntry, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/formTemplates/{formTemplateId}/metrics
        [Route("api/formtemplates/{formTemplateId:Guid}/metrics")]
        public IHttpActionResult Post(Guid formTemplateId, MetricDTO metricDto)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            var formTemplate = GetFormTemplate(formTemplateId);
            if (formTemplate == null)
                return NotFound();

            var metric = Mapper.Map<Metric>(metricDto);
            metric.FormTemplateId = formTemplateId;

            try
            {
                UnitOfWork.MetricsRepository.InsertOrUpdate(metric);
                UnitOfWork.MetricsRepository.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/formTemplates/{formTemplateId}/metrics/{id}
        [Route("api/formtemplates/{formTemplateId:guid}/metrics/{id:guid}")]
        public IHttpActionResult Put(Guid formTemplateId, Guid id, [FromBody]MetricDTO metricDto)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            if (id == Guid.Empty)
                return BadRequest("metric id is empty");

            var metric = GetMetric(formTemplateId, id);
            if (metric == null)
                NotFound();

            try
            {
                Mapper.Map(metricDto, metric);
                UnitOfWork.MetricsRepository.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/formTemplates/{formTemplateId}/metrics/{id}
        [Route("api/formtemplates/{formTemplateId:guid}/metrics/{id:guid}")]
        public IHttpActionResult Delete(Guid formTemplateId, Guid id)
        {
            if (formTemplateId == Guid.Empty)
                return BadRequest("form template id is empty");

            if (id == Guid.Empty)
                return BadRequest("metric id is empty");

            var metric = GetMetric(formTemplateId, id);
            if (metric == null)
                NotFound();

            try
            {
                UnitOfWork.MetricsRepository.Delete(metric);
                UnitOfWork.MetricsRepository.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #region helpers

        private FormTemplate GetFormTemplate(Guid id)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, onlyPublished: false);

            return surveyProvider
                .GetAllFormTemplates()
                .SingleOrDefault(f => f.Id == id);
        }

        private Metric GetMetric(Guid formTemplateId, Guid id)
        {
            var formTemplate = GetFormTemplate(formTemplateId);
            if (formTemplate == null)
                return null;

            return UnitOfWork.MetricsRepository.Find(id);
        }

        #endregion

    }
}