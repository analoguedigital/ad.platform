using LightMethods.Survey.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public static class EntityExtensions
    {
        public static IEnumerable<DocumentDTO> ToDocumentDTO<T>(this ICollection<T> documents) where T:Document
        {
            return documents.Select(d => new DocumentDTO()
            {
                Title = d.Title,
                FileName = d.FileName,
                Id = d.Id
            });
        }
    }
}
