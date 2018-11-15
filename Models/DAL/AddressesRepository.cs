using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class AddressesRepository : Repository<Address>
    {
        public AddressesRepository(UnitOfWork uow) : base(uow) { }
    }
}
