using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class OrgConnectionRequestsRepository : Repository<OrgConnectionRequest>
    {
        public OrgConnectionRequestsRepository(UnitOfWork uow) : base(uow) { }
    }
}
