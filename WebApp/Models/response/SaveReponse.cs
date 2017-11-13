using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class SaveReponse
    {
        public bool HasError { get; set; }
        public Dictionary<string, string> Errors { set; get; } = new Dictionary<string, string>();
    }
}