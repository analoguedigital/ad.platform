using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ComponentModel.DataAnnotations;

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
