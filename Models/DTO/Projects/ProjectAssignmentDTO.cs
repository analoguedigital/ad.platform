using System;

namespace LightMethods.Survey.Models.DTO
{
    public class ProjectAssignmentDTO
    {
        public Guid ProjectId { set; get; }

        public Guid OrgUserId { set; get; }

        public string OrgUserName { set; get; }

        public bool IsRootUser { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanView { get; set; }

        public bool CanExportPdf { get; set; }

        public bool CanExportZip { get; set; }
    }
}
