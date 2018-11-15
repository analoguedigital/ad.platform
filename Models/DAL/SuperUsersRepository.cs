using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class SuperUsersRepository : Repository<SuperUser>
    {
        public SuperUsersRepository(UnitOfWork uow) : base(uow) { }
    }
}
