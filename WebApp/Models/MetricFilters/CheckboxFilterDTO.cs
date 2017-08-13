using LightMethods.Survey.Models.MetricFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class CheckboxFilterDTO : MetricFilterDTO
    {
        public List<MetricFilterDataItem> DataList { get; set; }

        public CheckboxFilterDTO()
        {
            this.DataList = new List<MetricFilterDataItem>();
        }
    }
}