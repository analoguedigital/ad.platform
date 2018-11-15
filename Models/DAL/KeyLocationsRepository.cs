using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class KeyLocationsRepository : Repository<KeyLocation>
    {
        public KeyLocationsRepository(UnitOfWork uow) : base(uow) { }
    }
}
