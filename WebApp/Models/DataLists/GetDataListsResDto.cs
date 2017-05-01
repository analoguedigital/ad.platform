using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class GetDataListsResDto
    {
        public List<GetDataListsResItemDto> Items { set; get; }
    }

    public class GetDataListsResItemDto
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
    }
}