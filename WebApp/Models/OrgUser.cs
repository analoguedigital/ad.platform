using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LightMethods.Survey.Models.Entities;
using Newtonsoft.Json;
using static LightMethods.Survey.Models.Entities.User;

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

        public List<ProjectAssignmentDTO> Assignments { get; set; }

        public GenderType Gender { get; set; }

        public DateTime? Birthdate { set; get; }

        public string Address { set; get; }

        public OrgUserDTO()
        {
            this.Assignments = new List<ProjectAssignmentDTO>();
        }

    }
}