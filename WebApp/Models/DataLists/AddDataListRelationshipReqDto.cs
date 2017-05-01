using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class AddDataListRelationshipReqDto
    {
        public Guid DataListId { set; get; }
        public string Name { set; get; }
    }
}