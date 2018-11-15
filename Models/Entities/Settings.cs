using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class Settings:Entity
    {
        [Display(Name="Template used in the case home")]
        public virtual ReportTemplate ProjectViewTemplate { set; get; }

        public Guid? ProjectViewTemplateId { set; get; }
    }
}
