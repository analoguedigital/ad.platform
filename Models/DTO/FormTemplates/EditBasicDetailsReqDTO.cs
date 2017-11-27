using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class EditBasicDetailsReqDTO
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public double Version { get; set; }
        public string Description { set; get; }
        public string Colour { get; set; }
    }
}
