using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Helpers;

namespace WebApi.Models.MetricFilters
{
    [JsonConverter(typeof(FilterValueConverter))]
    public class FilterValueDTO
    {
        private class FilterValueConverter : JsonCreationConverter<FilterValueDTO>
        {
            protected override FilterValueDTO Create(Type objectType, JObject jObject)
            {
                var valueType = jObject.Value<string>("type");

                if (valueType == "single") return new SingleFilterValueDTO();
                else if (valueType == "range") return new RangeFilterValueDTO();
                else if (valueType == "multiple") return new MultipleFilterValueDTO();
                else return new FilterValueDTO();
            }
        }

        public string ShortTitle { get; set; }

        public string Type { get; set; }
    }
}