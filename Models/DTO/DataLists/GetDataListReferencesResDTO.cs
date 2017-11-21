using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.DTO.DataLists
{
    public class GetDataListReferencesResDTO
    {
        public List<GetDataListReferencesResItemDTO> Items { set; get; }
    }

    public class GetDataListReferencesResItemDTO
    {
        public Guid Id { set; get; }

        public string Name { set; get; }
    }
}
