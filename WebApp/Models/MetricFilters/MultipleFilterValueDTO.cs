using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.MetricFilters
{
    public class MultipleFilterValueDTO : FilterValueDTO
    {
        public List<Object> Values { get; set; }

        public MultipleFilterValueDTO()
        {
            this.Values = new List<object>();
        }
    }
}