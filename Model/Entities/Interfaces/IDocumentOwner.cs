using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.DTO;

namespace LightMethods.Survey.Models.Entities
{
    public interface IDocumentOwner
    {
        Guid Id { get; }
        ICollection<Document> Documents { get; }
        IEnumerable<DocumentDTO> DocumentsDTO { get; }
    }
}
