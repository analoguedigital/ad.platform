using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public class DataListDTO
    {
        public Guid Id { set; get; }

        public string Name { set; get; }

        public ICollection<DataListItemDTO> Items { set; get; } = new List<DataListItemDTO>();

        public ICollection<DataListRelationshipDTO> Relationships { set; get; }
    }
}
