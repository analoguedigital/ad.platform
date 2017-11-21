using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using WebApi.Helpers;

namespace LightMethods.Survey.Models.DTO
{
    [JsonConverter(typeof(MetricConverter))]
    public class MetricDTO
    {

        private class MetricConverter : JsonCreationConverter<MetricDTO>
        {
            protected override MetricDTO Create(Type objectType, JObject jObject)
            {
                var metricType = jObject.Value<string>("type");

                if (metricType == "NumericMetric") return new NumericMetricDTO();
                else if (metricType == "FreeTextMetric") return new FreeTextMetricDTO();
                else if (metricType == "RateMetric") return new RateMetricDTO();
                else if (metricType == "DateMetric") return new DateMetricDTO();
                else if (metricType == "TimeMetric") return new TimeMetricDTO();
                else if (metricType == "MultipleChoiceMetric") return new MultipleChoiceMetricDTO();
                else if (metricType == "DichotomousMetric") return new DichotomousMetricDTO();
                else if (metricType == "AttachmentMetric") return new AttachmentMetricDTO();
                else return new MetricDTO();
            }

        }

        public virtual Metric Map(Metric entity, UnitOfWork uow, Organisation org)
        {
            if (entity == null)
                entity = Mapper.Map<Metric>(this); // new metric
            else
                Mapper.Map(this, entity);

            return entity;
        }

        public Guid Id { set; get; }

        public string Type { get; set; }

        public string ShortTitle { get; set; }

        public string Description { get; set; }

        public Guid MetricGroupId { set; get; }

        public bool Mandatory { set; get; }

        public string SectionTitle { set; get; }

        public int Order { set; get; }

        public bool isDeleted { set; get; }
    }
}
