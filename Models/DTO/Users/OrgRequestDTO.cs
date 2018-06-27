using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class OrgRequestDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ContactName { get; set; }

        public string Email { get; set; }

        public string TelNumber { get; set; }

        public string Postcode { get; set; }

        public OrgUserDTO OrgUser { get; set; }
    }
}
