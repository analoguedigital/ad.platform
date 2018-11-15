using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class OrgUsersRepository : Repository<OrgUser>
    {
        public OrgUsersRepository(UnitOfWork uow) : base(uow) { }
    }
}
