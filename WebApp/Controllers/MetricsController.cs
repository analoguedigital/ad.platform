using AutoMapper;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class MetricsController : BaseApiController
    {

        [HttpGet]
        [ResponseType(typeof(MetricDTO))]
        [Route("api/metrics/{metricType}")]
        public IHttpActionResult Get(string metricType)
        {

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

            return Ok(Mapper.Map(metric, metric.GetType(), typeof(MetricDTO)));
        }


        // GET api/<controller>/5
        [DeflateCompression]
        [ResponseType(typeof(MetricDTO))]
        [Route("api/formtemplates/{formTemplateId:Guid}/metrics/{id:Guid}")]
        public IHttpActionResult Get(Guid formTemplateId, Guid id)
        {
            var metric = GetMetric(formTemplateId, id);

            if (metric == null)
                return NotFound();

            return Ok(Mapper.Map(metric, metric.GetType(), typeof(MetricDTO)));

        }

        // POST api/<controller>
        [Route("api/formtemplates/{formTemplateId:Guid}/metrics")]
        public IHttpActionResult Post(Guid formTemplateId, MetricDTO metricDto)
        {

            var formTemplate = GetFormTemplate(formTemplateId);

            if (formTemplate == null)
                return NotFound();

            var metric = Mapper.Map<Metric>(metricDto);
            metric.FormTemplateId = formTemplateId;

            UnitOfWork.MetricsRepository.InsertOrUpdate(metric);
            UnitOfWork.MetricsRepository.Save();

            return Ok();
        }

        // PUT api/<controller>/5
        [Route("api/formtemplates/{formTemplateId:guid}/metrics/{id:guid}")]
        public IHttpActionResult Put(Guid formTemplateId, Guid id, [FromBody]MetricDTO metricDto)
        {
            var metric = GetMetric(formTemplateId, id);

            if (metric == null)
                NotFound();

            Mapper.Map(metricDto, metric);
            UnitOfWork.MetricsRepository.Save();

            return Ok();
        }

        // DELETE api/<controller>/5
        [Route("api/formtemplates/{formTemplateId:guid}/metrics/{id:guid}")]
        public IHttpActionResult Delete(Guid formTemplateId, Guid id)
        {

            var metric = GetMetric(formTemplateId, id);

            if (metric == null)
                NotFound();

            UnitOfWork.MetricsRepository.Delete(metric);
            UnitOfWork.MetricsRepository.Save();

            return Ok();
        }

        private FormTemplate GetFormTemplate(Guid id)
        {
            var surveyProvider = new SurveyProvider(CurrentOrgUser, UnitOfWork, false);

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
    }
}