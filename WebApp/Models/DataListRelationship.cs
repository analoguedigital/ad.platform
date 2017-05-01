using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class DataListRelationshipDTO
    {
        public Guid Id { set; get; }

        public Guid OwnerId { set; get; }

        public Guid DataListId { set; get; }

        public DataListDTO DataList { set; get; }

        public string Name { set; get; }

        public int Order { set; get; }
    }
}