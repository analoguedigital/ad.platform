using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.DTO;

namespace LightMethods.Survey.Models.DAL
{
    public class CommentariesRepository : Repository<Commentary>
    {
        public CommentariesRepository(UnitOfWork uow)
            : base(uow)
        {

        }
        
        public void PrepareDocumentsDTO(Commentary commentary)
        {
            commentary.DocumentsDTO = commentary.Documents.ToDocumentDTO();
        }

        public void PrepareDocuments(Commentary commentary)
        {

            if (commentary.DocumentsDTO == null)
                commentary.DocumentsDTO = new List<DocumentDTO>();

            foreach (var item in commentary.DocumentsDTO)
            {
                if (item.Id == Guid.Empty)
                {
                    if (item.File != null)
                    {
                        byte[] content = new byte[item.File.ContentLength];
                        item.File.InputStream.Read(content, 0, item.File.ContentLength);
                        var docFile = new File() { Content = content };
                        Context.Files.Add(docFile);

                        var newDoc = new CommentaryDocument()
                        {
                            File = docFile,
                            FileName = item.File.FileName,
                            Title = item.Title
                        };

                        commentary.Documents.Add(newDoc);
                    }
                }
                else
                {
                    commentary.Documents.Single(d => d.Id == item.Id).Title = item.Title;
                }

            }

            var itemsToDelete = commentary.Documents
                .Where(d => !commentary.DocumentsDTO.Select(dt => dt.Id).Contains(d.Id)).ToList();

            using (var uow = new UnitOfWork(Context))
            {
                uow.DocumentsRepository.Delete(itemsToDelete);
            }
        }
    }
}
