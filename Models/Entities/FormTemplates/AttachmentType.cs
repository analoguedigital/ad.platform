using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class AttachmentType : Entity
    {
        public string Name { set; get; }

        public int MaxFileSize { set; get; }

        public string AllowedExtensions { set; get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
