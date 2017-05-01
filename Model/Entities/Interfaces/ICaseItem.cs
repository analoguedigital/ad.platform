using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.Entities
{
    public interface ICaseItem
    {
        Guid ProjectId { get; set; }
        Project Project { get; set; }
    }
}
