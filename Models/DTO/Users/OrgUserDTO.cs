using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using static LightMethods.Survey.Models.Entities.User;

namespace LightMethods.Survey.Models.DTO
{
    public class OrgUserDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public bool IsActive { get; set; }

        public Guid OrganisationId { get; set; }

        public OrganisationDTO Organisation { get; set; }

        public bool IsRootUser { set; get; }

        public Guid? CurrentProjectId { get; set; }

        public ProjectDTO CurrentProject { get; set; }

        public OrgUserTypeDTO Type { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool IsWebUser { get; set; }

        public bool IsMobileUser { get; set; }

        public List<ProjectAssignmentDTO> Assignments { get; set; }

        public GenderType Gender { get; set; }

        public DateTime? Birthdate { set; get; }

        public string Address { set; get; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public AccountType AccountType { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public byte SecurityQuestion { get; set; }

        public string SecurityAnswer { get; set; }

        public OrgUserDTO()
        {
            this.Assignments = new List<ProjectAssignmentDTO>();
        }
    }
}
