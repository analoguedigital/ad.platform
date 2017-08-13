using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Helpers;

namespace WebApi.Models.MetricFilters
{
    [JsonConverter(typeof(MetricFilterConverter))]
    public class MetricFilterDTO
    {
        private class MetricFilterConverter : JsonCreationConverter<MetricFilterDTO>
        {
            protected override MetricFilterDTO Create(Type objectType, JObject jObject)
            {
                var filterType = jObject.Value<string>("type");

                if (filterType == "Checkbox") return new CheckboxFilterDTO();
                if (filterType == "DateRange") return new DateRangeFilterDTO();
                if (filterType == "Dichotomous") return new DichotomousFilterDTO();
                if (filterType == "Dropdown") return new DropdownFilterDTO();
                if (filterType == "NumericRange") return new NumericRangeFilterDTO();
                if (filterType == "Slider") return new SliderFilterDTO();
                if (filterType == "Text") return new TextFilterDTO();
                if (filterType == "TimeRange") return new TimeRangeFilterDTO();
                else return new MetricFilterDTO();
            }

        }

        public Guid MetricId { get; set; }

        public string ShortTitle { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }
    }
}