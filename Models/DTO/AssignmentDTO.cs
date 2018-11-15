namespace LightMethods.Survey.Models.DTO
{
    public class AssignmentDTO
    {
        public bool AllowView { get; set; }

        public bool AllowAdd { get; set; }

        public bool AllowEdit { get; set; }

        public bool AllowDelete { get; set; }

        public bool AllowExportPdf { get; set; }

        public bool AllowExportZip { get; set; }
    }
}
