using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LightMethods.Survey.Models.DAL;
using AppHelper;

namespace LightMethods.Survey.Models.Entities
{
    public class OrgUser : User
    {
        [Display(Name = "First names")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Display(Name = "Surname")]
        [MaxLength(30)]
        public string Surname { get; set; }

        public Guid? OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public Guid TypeId { get; set; }
        public virtual OrgUserType Type { get; set; }

        [ScaffoldColumn(false)]
        [ReadOnly(true)]
        public bool IsRootUser { set; get; }

        public Guid? CurrentProjectId { set; get; }
        public virtual Project CurrentProject { set; get; }

        public virtual ICollection<Assignment> Assignments { get; set; }

        public bool IsWebUser { get; set; }

        public bool IsMobileUser { get; set; }

        public OrgUser()
        {
            IsRootUser = false;
        }

        public string Status
        {
            get
            {
                return IsActive ? "Active" : "Deactivated";
            }
        }

        public void SetCurrentProject(Guid projectId)
        {
            if (Type == OrgUserTypesRepository.TeamUser || Type == OrgUserTypesRepository.ExternalUser)
                if (!Assignments.Any(a => a.ProjectId == projectId))
                    throw new ValidationException("The user is not allowed to access this project's data!");

            CurrentProjectId = projectId;
        }

        public override string ToString()
        {
            if (FirstName.HasValue() && Surname.HasValue())
                return "{0} {1}".FormatWith(FirstName, Surname);

            return base.ToString();
        }
     
    }
}
