using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.DTO;

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
