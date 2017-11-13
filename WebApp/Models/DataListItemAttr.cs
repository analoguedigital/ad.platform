using System;

namespace WebApi.Models
{
    public class DataListItemAttrDTO
    {
        public Guid Id { set; get; }

        public Guid RelationshipId { set; get; }

        public Guid ValueId { set; get; }
    }
}
