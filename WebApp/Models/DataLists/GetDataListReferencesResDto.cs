using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class GetDataListReferencesResDto
    {
        public List<GetDataListReferencesResItemDto> Items { set; get; }
    }

    public class GetDataListReferencesResItemDto
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
    }
}