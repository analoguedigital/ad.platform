using System.Collections.Generic;

namespace LightMethods.Survey.Models.Entities
{
    public interface IContactNumberOwner : IEntity
    {
        ICollection<ContactNumber> ContactNumbers { get; set; }
    }
}
