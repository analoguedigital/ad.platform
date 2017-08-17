using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class SingleFilterValueDTO : FilterValueDTO
    {
        public Object Value { get; set; }
    }
}