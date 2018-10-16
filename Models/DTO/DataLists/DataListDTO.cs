using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.DTO
{
    public class DataListDTO
    {
        public Guid Id { set; get; }

        public string Name { set; get; }

        public OrganisationDTO Organisation { get; set; }

        public ICollection<DataListItemDTO> Items { set; get; } = new List<DataListItemDTO>();

        public ICollection<DataListRelationshipDTO> Relationships { set; get; }
    }
}
