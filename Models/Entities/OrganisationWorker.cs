using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class OrganisationWorker :Adult
    {
        public Guid OrganisationId { get; set; }
        public virtual ExternalOrganisation Organisation { get; set; }

        [Display(Name="Summary of role")]
        [DataType(DataType.MultilineText)]
        public string RoleDescription { set; get; }

        [Display(Name="Start date")]
        public DateTime? StartDate { set; get; }

        [Display(Name = "End date")]
        public DateTime? EndDate { set; get; }
    }
}
