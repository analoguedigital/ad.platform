using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

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
