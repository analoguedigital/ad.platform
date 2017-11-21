using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.Entities
{
    public interface IEnumEntity
    {
        string Name{ get; set; }

        string SystemName{ get; set; }

        int Order{ get; set; }

        Guid Id{ get; set; }

    }
}
