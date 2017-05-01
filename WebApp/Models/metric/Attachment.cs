using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class AttachmentDTO
    {
        public Guid Id { set; get; }

        public int FileSize { set; get; }

        public string FileName { set; get; }

        public string Url { set; get; }

        public string TypeString { set; get;}

        public bool isDeleted { set; get; }
    }
}