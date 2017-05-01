using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LightMethods.Survey.Models.Entities;
using Newtonsoft.Json;

namespace WebApi.Models
{
    public class OrgUserTypeDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}