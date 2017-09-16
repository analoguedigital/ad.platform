using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class ProjectAssignmentDTO
    {
        public Guid ProjectId { set; get; }

        public Guid OrgUserId { set; get; }

        public string OrgUserName { set; get; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanView { get; set; }
    }
}