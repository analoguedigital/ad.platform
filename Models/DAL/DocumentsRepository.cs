using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class DocumentsRepository : Repository<Document>
    {
        public DocumentsRepository(UnitOfWork uow) : base(uow) { }
    }
}
