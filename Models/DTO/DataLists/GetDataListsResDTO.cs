using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.DTO.DataLists
{
    public class GetDataListsResDTO
    {
        public List<GetDataListsResItemDTO> Items { set; get; }
    }

    public class GetDataListsResItemDTO
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
    }
}
