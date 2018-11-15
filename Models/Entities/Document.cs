using System;

namespace LightMethods.Survey.Models.Entities
{
    public class Document : Entity
    {
        public string Title { set; get; }

        public string FileName { set; get; }

        public string FileExt { set; get; }

        public virtual File File { set; get; }

        public Guid FileId { set; get; }
    }

    public class CommentaryDocument : Document
    {
        public Guid CommentaryId { set; get; }
        
        public virtual Commentary Commentary { set; get; }
    }

    //public class ReportPDFDocument : Document
    //{
    //    public Guid ReportId { set; get; }
    //    public virtual Report Report { set; get; }
    //}
}

