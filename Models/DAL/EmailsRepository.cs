using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class EmailsRepository : Repository<Email>
    {
        public EmailsRepository(UnitOfWork uow) : base(uow) { }
    }
}
