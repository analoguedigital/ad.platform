using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class Guidance: Entity
    {
        public string Page { set; get; }
        
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public Guid UserTypeId { set; get; }
        [Display(Name="User type")]
        public virtual OrgUserType UserType { set; get; }
    }

}
