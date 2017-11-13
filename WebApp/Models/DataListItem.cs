using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public class DataListItemDTO
    {
        public Guid Id { set; get; }

        public string Text { set; get; }

        public string Description { set; get; }

        public IEnumerable<DataListItemAttrDTO> Attributes { set; get; }
        
        public int Value { set; get; }

        public int Order { set; get; }

        public bool IsDeleted { set; get;}
        
    }
}
