using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace LightMethods.Survey.Models.FilterValues
{
    [JsonConverter(typeof(FilterValueConverter))]
    public class FilterValue
    {
        private class FilterValueConverter : JsonCreationConverter<FilterValue>
        {
            protected override FilterValue Create(Type objectType, JObject jObject)
            {
                var valueType = jObject.Value<string>("type");

                if (valueType == "single") return new SingleFilterValue();
                else if (valueType == "range") return new RangeFilterValue();
                else if (valueType == "multiple") return new MultipleFilterValue();
                else return new FilterValue();
            }
        }

        public Guid Id { get; set; }

        public string ShortTitle { get; set; }

        public string Type { get; set; }
    }
}
