using System;

namespace LightMethods.Survey.Models.DTO
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public OrganisationDTO Organisation { get; set; }

        public string Notes { get; set; }

        public bool AllowView { get; set; }

        public bool AllowAdd { get; set; }

        public bool AllowEdit { get; set; }

        public bool AllowDelete { get; set; }

        public bool AllowExportPdf { get; set; }
        
        public bool AllowExportZip { get; set; }

        public UserDTO CreatedBy { get; set; }

        public DateTime? LastEntry { get; set; }

        public int AssignmentsCount { get; set; }

        public int TeamsCount { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as ProjectDTO;

            if (item == null)
                return false;

            return this.Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
