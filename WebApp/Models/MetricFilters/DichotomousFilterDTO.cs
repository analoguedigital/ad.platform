using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class DichotomousFilterDTO : MetricFilterDTO
    {
        public string YesText { get; set; }

        public string NoText { get; set; }

        public bool SelectedValue { get; set; }

        public DichotomousFilterDTO()
        {
            this.YesText = "Yes";
            this.NoText = "No";
        }
    }
}