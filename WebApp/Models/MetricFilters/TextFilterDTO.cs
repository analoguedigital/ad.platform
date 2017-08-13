using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class TextFilterDTO : MetricFilterDTO
    {
        public int MaxLength { get; set; }

        public int NumberOfLines { get; set; }

        public string SelectedValue { get; set; }
    }
}