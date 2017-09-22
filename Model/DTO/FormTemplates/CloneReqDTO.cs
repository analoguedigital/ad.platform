using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class CloneReqDTO
    {
        public string Title { get; set; }
        public string Colour { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
