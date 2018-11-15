using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class OrgTeamUsersRepository : Repository<OrgTeamUser>
    {
        public OrgTeamUsersRepository(UnitOfWork uow) : base(uow) { }
    }
}
