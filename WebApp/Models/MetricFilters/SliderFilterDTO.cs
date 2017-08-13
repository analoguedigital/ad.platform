using LightMethods.Survey.Models.MetricFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class SliderFilterDTO : MetricFilterDTO
    {
        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int DefaultValue { get; set; }

        public List<MetricFilterDataItem> DataList { get; set; }

        public int SelectedValue { get; set; }

        public SliderFilterDTO()
        {
            this.DataList = new List<MetricFilterDataItem>();
        }
    }
}