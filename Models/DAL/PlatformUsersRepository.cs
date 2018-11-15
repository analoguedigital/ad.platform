using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class PlatformUsersRepository : Repository<PlatformUser>
    {
        public PlatformUsersRepository(UnitOfWork uow) : base(uow) { }
    }
}
