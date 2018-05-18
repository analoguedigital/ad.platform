using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class ThreadAssignmentDTO
    {
        public Guid FormTemplateId { set; get; }

        public Guid OrgUserId { set; get; }

        public string OrgUserName { set; get; }

        public string Email { get; set; }

        public AccountType AccountType { get; set; }

        public bool IsRootUser { get; set; }

        public bool? CanView { get; set; }

        public bool? CanAdd { get; set; }

        public bool? CanEdit { get; set; }

        public bool? CanDelete { get; set; }
    }
}
