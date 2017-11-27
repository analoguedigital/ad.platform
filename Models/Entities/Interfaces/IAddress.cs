using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.Entities
{
    public interface IAddress : IEntity
    {

        string AddressLine1 { get; set; }

        string AddressLine2{ get; set; }

        string AddressLine3{ get; set; }

        string Town{ get; set; }

        string County{ get; set; }

        string Country{ get; set; }

        string Postcode{ get; set; }

    }
}
