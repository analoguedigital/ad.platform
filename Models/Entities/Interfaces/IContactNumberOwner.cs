using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.Entities
{
    public interface IContactNumberOwner : IEntity
    {
        ICollection<ContactNumber> ContactNumbers { get; set; }
    }
}
