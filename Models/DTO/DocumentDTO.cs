using System;
using System.Web;

namespace LightMethods.Survey.Models.DTO
{
    public class DocumentDTO
    {
        public Guid Id { set; get; }

        public string Title { set; get; }

        public HttpPostedFileBase File { set; get; }

        public string FileName { set; get; }

        
    }
}
