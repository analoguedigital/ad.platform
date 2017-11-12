using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LightMethods.Survey.Models.Entities;
using Newtonsoft.Json;

namespace WebApi.Models
{
    public class OrgUserDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public bool IsActive { get; set; }

        public Guid OrganisationId { get; set; }

        public bool IsRootUser { set; get; }

        public OrgUserTypeDTO Type { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool IsWebUser { get; set; }

        public bool IsMobileUser { get; set; }

    }
}