using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.DAL;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LightMethods.Survey.Models.Entities
{
    public class TableColumn:Entity
    {
        [Required]
        [Display(Name="Header text")]
        public string HeaderText { set; get; }

        public virtual ReportTable Table { get; set; }
        public Guid TableId { get; set; }

        public virtual Metric Metric { set; get; }
        public Guid MetricId { set; get; }

   }



    
}
