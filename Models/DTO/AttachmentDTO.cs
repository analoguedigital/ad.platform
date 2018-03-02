using System;

namespace LightMethods.Survey.Models.DTO
{
    public class AttachmentDTO
    {
        public Guid Id { set; get; }

        public int FileSize { set; get; }

        public string FileName { set; get; }

        public string TypeString { set; get; }

        public bool isDeleted { set; get; }

        public string OneTimeAccessId { get; set; }
    }
}
