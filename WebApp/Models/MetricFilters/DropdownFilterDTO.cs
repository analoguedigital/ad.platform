using LightMethods.Survey.Models.MetricFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class DropdownFilterDTO : MetricFilterDTO
    {
        public List<MetricFilterDataItem> DataList { get; set; }

        public int? SelectedValue { get; set; }

        public DropdownFilterDTO()
        {
            this.DataList = new List<MetricFilterDataItem>();
        }
    }
}