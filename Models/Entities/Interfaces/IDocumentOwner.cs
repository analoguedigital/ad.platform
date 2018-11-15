using LightMethods.Survey.Models.DTO;
using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.Entities
{
    public interface IDocumentOwner
    {
        Guid Id { get; }

        ICollection<Document> Documents { get; }

        IEnumerable<DocumentDTO> DocumentsDTO { get; }
    }
}
