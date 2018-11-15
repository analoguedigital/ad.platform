using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class ContactNumber : Entity
    {
        public virtual ContactNumberType Type { set; get; }

        public Guid TypeId { set; get; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(20)]
        public string Number { set; get; }

        [MaxLength(50)]
        public string Note { set; get; }
    }

    public class AdultContactNumber : ContactNumber
    {

        public Guid AdultId { set; get; }

        public virtual Adult Adult { set; get; }
    }

    public class ExternalOrgContactNumber : ContactNumber
    {
        public Guid OrganisationId { set; get; }

        public virtual ExternalOrganisation Organisation { set; get; }
    }

}
