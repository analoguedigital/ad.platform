using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class SeverityLevelRepository : Repository<SeverityLevel>
    {
        public SeverityLevelRepository(UnitOfWork uow) : base(uow) { }
    }
}
