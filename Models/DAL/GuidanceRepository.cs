using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class GuidanceRepository : Repository<Guidance>
    {
        public GuidanceRepository(UnitOfWork uow) : base(uow) { }
    }
}
