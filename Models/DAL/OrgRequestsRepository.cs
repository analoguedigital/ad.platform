using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class OrgRequestsRepository : Repository<OrgRequest>
    {
        public OrgRequestsRepository(UnitOfWork uow) : base(uow) { }
    }
}
